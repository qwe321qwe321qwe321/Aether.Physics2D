/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Fluids
{
    public class FluidParticle
    {
        public XNAVector2 Position;
        public XNAVector2 PreviousPosition;

        public XNAVector2 Velocity;
        public XNAVector2 Acceleration;

        internal FluidParticle(XNAVector2 position)
        {
            Neighbours = new List<FluidParticle>();
            IsActive = true;
            MoveTo(position);

            Damping = 0.0f;
            Mass = 1.0f;
        }

        public bool IsActive { get; set; }

        public List<FluidParticle> Neighbours { get; private set; }

        // For gameplay purposes
        public float Density { get; internal set; }
        public float Pressure { get; internal set; }

        // Other properties
        public int Index { get; internal set; }

        // Physics properties
        public float Damping { get; set; }
        public float Mass { get; set; }

        public void MoveTo(XNAVector2 p)
        {
            Position = p;
            PreviousPosition = p;

            Velocity = XNAVector2.Zero;
            Acceleration = XNAVector2.Zero;
        }

        public void ApplyForce(ref XNAVector2 force)
        {
            Acceleration += force * Mass;
        }

        public void ApplyImpulse(ref XNAVector2 impulse)
        {
            Velocity += impulse;
        }

        public void Update(float deltaTime)
        {
            Velocity += Acceleration * deltaTime;

            XNAVector2 delta = (1.0f - Damping) * Velocity * deltaTime;

            PreviousPosition = Position;
            Position += delta;

            Acceleration = XNAVector2.Zero;
        }

        public void UpdateVelocity(float deltaTime)
        {
            Velocity = (Position - PreviousPosition) / deltaTime;
        }
    }
}
