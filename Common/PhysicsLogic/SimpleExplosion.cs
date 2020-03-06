/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;

namespace tainicom.Aether.Physics2D.Common.PhysicsLogic
{
    /// <summary>
    /// Creates a simple explosion that ignores other bodies hiding behind static bodies.
    /// </summary>
    public sealed class SimpleExplosion : PhysicsLogic
    {
        public SimpleExplosion(World world): base(world)
        {
            Power = 1; //linear
        }

        /// <summary>
        /// This is the power used in the power function. A value of 1 means the force
        /// applied to bodies in the explosion is linear. A value of 2 means it is exponential.
        /// </summary>
        public float Power { get; set; }

        /// <summary>
        /// Activate the explosion at the specified position.
        /// </summary>
        /// <param name="pos">The position (center) of the explosion.</param>
        /// <param name="radius">The radius of the explosion.</param>
        /// <param name="force">The force applied</param>
        /// <param name="maxForce">A maximum amount of force. When force gets over this value, it will be equal to maxForce</param>
        /// <returns>A list of bodies and the amount of force that was applied to them.</returns>
        public Dictionary<Body, XNAVector2> Activate(XNAVector2 pos, float radius, float force, float maxForce = float.MaxValue)
        {
            HashSet<Body> affectedBodies = new HashSet<Body>();

            AABB aabb;
            aabb.LowerBound = pos - new XNAVector2(radius);
            aabb.UpperBound = pos + new XNAVector2(radius);

            // Query the world for bodies within the radius.
            World.QueryAABB(fixture =>
            {
                if (XNAVector2.Distance(fixture.Body.Position, pos) <= radius)
                {
                    if (!affectedBodies.Contains(fixture.Body))
                        affectedBodies.Add(fixture.Body);
                }

                return true;
            }, ref aabb);

            return ApplyImpulse(pos, radius, force, maxForce, affectedBodies);
        }

        private Dictionary<Body, XNAVector2> ApplyImpulse(XNAVector2 pos, float radius, float force, float maxForce, HashSet<Body> overlappingBodies)
        {
            Dictionary<Body, XNAVector2> forces = new Dictionary<Body, XNAVector2>(overlappingBodies.Count);

            foreach (Body overlappingBody in overlappingBodies)
            {
                if (IsActiveOn(overlappingBody))
                {
                    float distance = XNAVector2.Distance(pos, overlappingBody.Position);
                    float forcePercent = GetPercent(distance, radius);

                    XNAVector2 forceXNAVector = pos - overlappingBody.Position;
                    forceXNAVector *= 1f / (float)Math.Sqrt(forceXNAVector.X * forceXNAVector.X + forceXNAVector.Y * forceXNAVector.Y);
                    forceXNAVector *= MathHelper.Min(force * forcePercent, maxForce);
                    forceXNAVector *= -1;

                    overlappingBody.ApplyLinearImpulse(forceXNAVector);
                    forces.Add(overlappingBody, forceXNAVector);
                }
            }

            return forces;
        }

        private float GetPercent(float distance, float radius)
        {
            //(1-(distance/radius))^power-1
            float percent = (float)Math.Pow(1 - ((distance - radius) / radius), Power) - 1;

            if (float.IsNaN(percent))
                return 0f;

            return MathHelper.Clamp(percent, 0f, 1f);
        }
    }
}