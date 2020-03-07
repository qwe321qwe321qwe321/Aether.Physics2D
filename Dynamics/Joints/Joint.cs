/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Dynamics.Joints
{
    public enum JointType
    {
        Unknown,
        Revolute,
        Prismatic,
        Distance,
        Pulley,
        //Mouse, <- We have fixed mouse
        Gear,
        Wheel,
        Weld,
        Friction,
        Rope,
        Motor,

        //FPE note: From here on and down, it is only FPE joints
        Angle,
        FixedMouse,
        FixedRevolute,
        FixedDistance,
        FixedLine,
        FixedPrismatic,
        FixedAngle,
        FixedFriction,
    }

    public enum LimitState
    {
        Inactive,
        AtLower,
        AtUpper,
        Equal,
    }

    /// <summary>
    /// A joint edge is used to connect bodies and joints together
    /// in a joint graph where each body is a node and each joint
    /// is an edge. A joint edge belongs to a doubly linked list
    /// maintained in each attached body. Each joint has two joint
    /// nodes, one for each attached body.
    /// </summary>
    public sealed class JointEdge
    {
        /// <summary>
        /// The joint.
        /// </summary>
        public Joint Joint;

        /// <summary>
        /// The next joint edge in the body's joint list.
        /// </summary>
        public JointEdge Next;

        /// <summary>
        /// Provides quick access to the other body attached.
        /// </summary>
        public Body Other;

        /// <summary>
        /// The previous joint edge in the body's joint list.
        /// </summary>
        public JointEdge Prev;
    }

    public abstract class Joint
    {
        private float _breakForce;
        private float _breakForceSquared;
        private float _breakTorque;

        /// <summary>
        /// Indicate if this join is enabled or not. Disabling a joint
        /// means it is still in the simulation, but inactive.
        /// </summary>
        public bool Enabled = true;

        internal JointEdge EdgeA = new JointEdge();
        internal JointEdge EdgeB = new JointEdge();
        internal bool IslandFlag;

        protected Joint()
        {
            BreakForce = float.MaxValue;
            BreakTorque = float.MaxValue;

            //Connected bodies should not collide by default
            CollideConnected = false;
        }

        protected Joint(Body bodyA, Body bodyB) : this()
        {
            //Can't connect a joint to the same body twice.
            Debug.Assert(bodyA != bodyB);

            BodyA = bodyA;
            BodyB = bodyB;
        }

        /// <summary>
        /// Constructor for fixed joint
        /// </summary>
        protected Joint(Body body) : this()
        {
            BodyA = body;
        }

        /// <summary>
        /// Gets or sets the type of the joint.
        /// </summary>
        /// <value>The type of the joint.</value>
        public JointType JointType { get; protected set; }

        /// <summary>
        /// Get the first body attached to this joint.
        /// </summary>
        public Body BodyA { get; internal set; }

        /// <summary>
        /// Get the second body attached to this joint.
        /// </summary>
        public Body BodyB { get; internal set; }

        /// <summary>
        /// Get the anchor point on bodyA in world coordinates.
        /// On some joints, this value indicate the anchor point within the world.
        /// </summary>
        public abstract XNAVector2 WorldAnchorA { get; set; }

        /// <summary>
        /// Get the anchor point on bodyB in world coordinates.
        /// On some joints, this value indicate the anchor point within the world.
        /// </summary>
        public abstract XNAVector2 WorldAnchorB { get; set; }

        /// <summary>
        /// Set the user data pointer.
        /// </summary>
        /// <value>The data.</value>
        public object Tag;

        /// <summary>
        /// Set this flag to true if the attached bodies should collide.
        /// </summary>
        public bool CollideConnected { get; set; }

        /// <summary>
        /// The Breakforce simply indicates the maximum Value the JointError can be before it breaks.
        /// The default value is float.MaxValue, which means it never breaks.
        /// </summary>
        public float BreakForce 
        {
            get { return _breakForce; }
            set {
                _breakForce = value;
                _breakForceSquared = _breakForce * _breakForce;
            }
        }

        /// <summary>
        /// The Breaktorque simply indicates the maximum torque the JointError can be before it breaks.
        /// The default value is float.MaxValue, which means it never breaks.
        /// </summary>
        public float BreakTorque 
        {
            get { return _breakTorque; }
            set 
            {
                _breakTorque = value;
            }
        }

        /// <summary>
        /// The event handler
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="reactForce"></param>
        /// <param name="reactTorque"></param>
        /// <param name="isBrokenByTorque"></param>
        public delegate void OnBrokenHandler(Joint joint, float reactForce, float reactTorque, bool isBrokenByTorque);
        /// <summary>
        /// Fires when the joint is broken.
        /// </summary>
        public event OnBrokenHandler OnBrokenEvent;

        public delegate void OnDestroyHandler(Joint joint);
        /// <summary>
        /// Fires when the joint is broken by World.Remove().
        /// the property Enabled will be False when it fired.
        /// </summary>
        public event OnDestroyHandler OnDestroyEvent;

        /// <summary>
        /// Destroy joint by World.Remove().
        /// </summary>
        internal void OnDestroy() {
            Enabled = false;
            OnDestroyEvent?.Invoke(this);
        }

        /// <summary>
        /// Get the reaction force on body at the joint anchor in Newtons.
        /// </summary>
        /// <param name="invDt">The inverse delta time.</param>
        public abstract XNAVector2 GetReactionForce(float invDt);

        /// <summary>
        /// Get the reaction torque on the body at the joint anchor in N*m.
        /// </summary>
        /// <param name="invDt">The inverse delta time.</param>
        public abstract float GetReactionTorque(float invDt);

        protected void WakeBodies()
        {
            if (BodyA != null)
                BodyA.Awake = true;

            if (BodyB != null)
                BodyB.Awake = true;
        }

        /// <summary>
        /// Return true if the joint is a fixed type.
        /// </summary>
        public bool IsFixedType()
        {
            return JointType == JointType.FixedRevolute ||
                   JointType == JointType.FixedDistance ||
                   JointType == JointType.FixedPrismatic ||
                   JointType == JointType.FixedLine ||
                   JointType == JointType.FixedMouse ||
                   JointType == JointType.FixedAngle ||
                   JointType == JointType.FixedFriction;
        }

        internal abstract void InitVelocityConstraints(ref SolverData data);

        internal void Validate(float invDt)
        {
            if (!Enabled)
                return;

            float jointFroceSquared = GetReactionForce(invDt).LengthSquared();
            float jointTorque = GetReactionTorque(invDt);

            if (Math.Abs(jointFroceSquared) <= _breakForceSquared &&
                Math.Abs(jointTorque) <= _breakTorque) {
                return;
            }

            Enabled = false;

            bool isBrokenByTorque = Math.Abs(jointTorque) > _breakTorque;
            OnBrokenEvent?.Invoke(this, (float)Math.Sqrt(jointFroceSquared), jointTorque, isBrokenByTorque);
        }

        internal abstract void SolveVelocityConstraints(ref SolverData data);

        /// <summary>
        /// Solves the position constraints.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>returns true if the position errors are within tolerance.</returns>
        internal abstract bool SolvePositionConstraints(ref SolverData data);
    }
}