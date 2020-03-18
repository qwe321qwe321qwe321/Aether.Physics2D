﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.Decomposition;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Dynamics
{
    /// <summary>
    /// An easy to use factory for creating bodies
    /// </summary>
    public partial class Body
    {
        /// <summary>
        /// Creates a fixture and attach it to this body.
        /// If the density is non-zero, this function automatically updates the mass of the body.
        /// Contacts are not created until the next time step.
        /// Warning: This method is locked during callbacks.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="userData">Application specific data</param>
        /// <returns></returns>
        public virtual Fixture CreateFixture(Shape shape)
        {
            Fixture fixture = new Fixture(shape);
            Add(fixture);
            return fixture;
        }

        public Fixture CreateEdge(Vector2 start, Vector2 end, float radius = Settings.PolygonRadius)
        {
            EdgeShape edgeShape = new EdgeShape(start, end);
            edgeShape.Radius = radius;
            return CreateFixture(edgeShape);
        }

        public Fixture CreateChainShape(Vertices vertices, float radius = Settings.PolygonRadius)
        {
            ChainShape shape = new ChainShape(vertices);
            shape.Radius = radius;
            return CreateFixture(shape);
        }

        public Fixture CreateLoopShape(Vertices vertices, float radius = Settings.PolygonRadius)
        {
            ChainShape shape = new ChainShape(vertices, true);
            shape.Radius = radius;
            return CreateFixture(shape);
        }

        public Fixture CreateRectangle(float width, float height, float density, Vector2 offset, float radius = Settings.PolygonRadius)
        {
            Vertices rectangleVertices = PolygonTools.CreateRectangle(width / 2, height / 2);
            rectangleVertices.Translate(ref offset);
            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
            rectangleShape.Radius = radius;
            return CreateFixture(rectangleShape);
        }

        public Fixture CreateCircle(float radius, float density)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");

            CircleShape circleShape = new CircleShape(radius, density);
            return CreateFixture(circleShape);
        }

        public Fixture CreateCircle(float radius, float density, Vector2 offset)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");

            CircleShape circleShape = new CircleShape(radius, density);
            circleShape.Position = offset;
            return CreateFixture(circleShape);
        }

        public Fixture CreatePolygon(Vertices vertices, float density, float radius = Settings.PolygonRadius)
        {
            if (vertices.Count <= 1)
                throw new ArgumentOutOfRangeException("vertices", "Too few points to be a polygon");

            PolygonShape polygon = new PolygonShape(vertices, density);
            polygon.Radius = radius;
            return CreateFixture(polygon);
        }

        public Fixture CreateEllipse(float xRadius, float yRadius, int edges, float density)
        {
            if (xRadius <= 0)
                throw new ArgumentOutOfRangeException("xRadius", "X-radius must be more than 0");

            if (yRadius <= 0)
                throw new ArgumentOutOfRangeException("yRadius", "Y-radius must be more than 0");

            Vertices ellipseVertices = PolygonTools.CreateEllipse(xRadius, yRadius, edges);
            PolygonShape polygonShape = new PolygonShape(ellipseVertices, density);
            return CreateFixture(polygonShape);
        }

        public List<Fixture> CreateCompoundPolygon(List<Vertices> list, float density, float radius = Settings.PolygonRadius)
        {
            List<Fixture> res = new List<Fixture>(list.Count);

            //Then we create several fixtures using the body
            foreach (Vertices vertices in list)
            {
                if (vertices.Count == 2)
                {
                    EdgeShape shape = new EdgeShape(vertices[0], vertices[1]);
                    shape.Radius = radius;
                    res.Add(CreateFixture(shape));
                }
                else
                {
                    PolygonShape shape = new PolygonShape(vertices, density);
                    shape.Radius = radius;
                    res.Add(CreateFixture(shape));
                }
            }

            return res;
        }

        public Fixture CreateLineArc(float radians, int sides, float radius, bool closed)
        {
            Vertices arc = PolygonTools.CreateArc(radians, sides, radius);
            arc.Rotate((MathHelper.Pi - radians) / 2);
            return closed ? CreateLoopShape(arc) : CreateChainShape(arc);
        }

        public List<Fixture> CreateSolidArc(float density, float radians, int sides, float radius)
        {
            Vertices arc = PolygonTools.CreateArc(radians, sides, radius);
            arc.Rotate((MathHelper.Pi - radians) / 2);

            //Close the arc
            arc.Add(arc[0]);

            List<Vertices> triangles = Triangulate.ConvexPartition(arc, TriangulationAlgorithm.Earclip);

            return CreateCompoundPolygon(triangles, density);
        }
    }
}