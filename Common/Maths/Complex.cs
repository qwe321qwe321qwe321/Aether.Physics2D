// Copyright (c) 2017 Kastellanos Nikolaos

using System;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Common.Maths
{
    /// <summary>
    /// Present 2d-rotation. You can imagine Real is Cos and Imaginary is Sin for the angle.
    /// </summary>
    public struct Complex
    {
        private static readonly Complex _one = new Complex(1, 0);
        private static readonly Complex _imaginaryOne = new Complex(0, 1);

        public float Real;
        public float Imaginary;

        public static Complex One { get { return _one; } }
        public static Complex ImaginaryOne { get { return _imaginaryOne; } }

        public float Phase
        {
            get { return (float)Math.Atan2(Imaginary, Real); }
            set 
            {
                if (value == 0)
                {
                    this = Complex.One;
                    return;
                }
                this.Real      = (float)Math.Cos(value);
                this.Imaginary = (float)Math.Sin(value);
            }
        }

        public float Magnitude
        {
            get { return (float)Math.Round(Math.Sqrt(MagnitudeSquared())); }
        }


        public Complex(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }
                
        public static Complex FromAngle(float angle)
        {
            if (angle == 0)
                return Complex.One;

            return new Complex(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));
        }        

        public void Conjugate()
        {
            Imaginary = -Imaginary;
        }
                
        public void Negate()
        {
            Real = -Real;
            Imaginary = -Imaginary;
        }

        public float MagnitudeSquared()
        {
            return (Real * Real) + (Imaginary * Imaginary);
        }

        public void Normalize()
        {
            var mag = Magnitude;
            Real = Real / mag;
            Imaginary = Imaginary / mag;            
        }

        public XNAVector2 ToXNAVector2()
        {
            return new XNAVector2(Real, Imaginary);
        }

        /// <summary>
        /// Get the x-axis
        /// </summary>
        public XNAVector2 GetXAxis() {
            return new XNAVector2(Real, Imaginary);
        }

        /// <summary>
        /// Get the y-axis
        /// </summary>
        public XNAVector2 GetYAxis() {
            return new XNAVector2(-Imaginary, Real);
        }

        public static Complex Multiply(ref Complex left, ref Complex right)
        {
            return new Complex( left.Real      * right.Real  - left.Imaginary * right.Imaginary,
                                left.Imaginary * right.Real  + left.Real      * right.Imaginary);
        }

        public static Complex Divide(ref Complex left, ref Complex right)
        {
            return new Complex( right.Real * left.Real + right.Imaginary * left.Imaginary,
                                right.Real * left.Imaginary - right.Imaginary * left.Real);
        }
        public static void Divide(ref Complex left, ref Complex right, out Complex result)
        {
            result = new Complex(right.Real * left.Real + right.Imaginary * left.Imaginary,
                                 right.Real * left.Imaginary - right.Imaginary * left.Real);
        }

        public static XNAVector2 Multiply(ref XNAVector2 left, ref Complex right)
        {
            return new XNAVector2(left.X * right.Real - left.Y * right.Imaginary,
                               left.Y * right.Real + left.X * right.Imaginary);
        }
        public static void Multiply(ref XNAVector2 left, ref Complex right, out XNAVector2 result)
        {
            result = new XNAVector2(left.X * right.Real - left.Y * right.Imaginary,
                                 left.Y * right.Real + left.X * right.Imaginary);
        }
        public static XNAVector2 Multiply(XNAVector2 left, ref Complex right)
        {
            return new XNAVector2(left.X * right.Real - left.Y * right.Imaginary,
                               left.Y * right.Real + left.X * right.Imaginary);
        }

        public static XNAVector2 Divide(ref XNAVector2 left, ref Complex right)
        {
            return new XNAVector2(left.X * right.Real + left.Y * right.Imaginary,
                               left.Y * right.Real - left.X * right.Imaginary);
        }

        public static XNAVector2 Divide(XNAVector2 left, ref Complex right)
        {
            return new XNAVector2(left.X * right.Real + left.Y * right.Imaginary,
                               left.Y * right.Real - left.X * right.Imaginary);
        }
        public static void Divide(XNAVector2 left, ref Complex right, out XNAVector2 result)
        {
            result = new XNAVector2(left.X * right.Real + left.Y * right.Imaginary,
                                 left.Y * right.Real - left.X * right.Imaginary);
        }
        
        public static Complex Conjugate(ref Complex value)
        {
            return new Complex(value.Real, -value.Imaginary);
        }

        public static Complex Negate(ref Complex value)
        {
            return new Complex(-value.Real, -value.Real);
        }

        public static Complex Normalize(ref Complex value)
        {
            var mag = value.Magnitude;
            return new Complex(value.Real / mag, -value.Imaginary / mag);
        }
        
        public override string ToString()
        {
            return String.Format("{{Real: {0} Imaginary: {1} Phase: {2} Magnitude: {3}}}", Real, Imaginary, Phase, Magnitude);
        }
    }
}
