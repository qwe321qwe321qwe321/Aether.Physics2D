﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;

namespace tainicom.Aether.Physics2D.Collision
{
    public interface IBroadPhase
    {
        int ProxyCount { get; }
        void UpdatePairs(BroadphaseDelegate callback);

        bool TestOverlap(int proxyIdA, int proxyIdB);

        int AddProxy(ref AABB aabb);

        void RemoveProxy(int proxyId);

        void MoveProxy(int proxyId, ref AABB aabb, XNAVector2 displacement);

        void SetProxy(int proxyId, ref FixtureProxy proxy);

        FixtureProxy GetProxy(int proxyId);

        void TouchProxy(int proxyId);

        void GetFatAABB(int proxyId, out AABB aabb);

        void Query(Func<int, bool> callback, ref AABB aabb);

        void RayCast(Func<RayCastInput, int, float> callback, ref RayCastInput input);

        void ShiftOrigin(XNAVector2 newOrigin);
    }
}