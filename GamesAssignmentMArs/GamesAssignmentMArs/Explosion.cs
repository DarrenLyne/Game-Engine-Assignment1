using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics;
using Microsoft.Xna.Framework;
using BEPUphysics.Collidables.MobileCollidables;

namespace GamesAssignmentMars
{
    class Explosion
    {
        //code taken from bepu physics demo explosion.cs and adapted for this project, not edited
        private readonly List<BroadPhaseEntry> affectedEntries = new List<BroadPhaseEntry>();

        public Explosion(Vector3 pos, float explosionMagnitude, float maxDist, Space containingSpace)
        {
            Position = pos;
            Magnitude = explosionMagnitude;
            MaxDistance = maxDist;
            Space = containingSpace;
        }

        public Vector3 Position { get; set; }
        public float Magnitude { get; set; }
        public float MaxDistance { get; set; }
        public Space Space { get; set; }

        public void Explode()
        {
            Space.BroadPhase.QueryAccelerator.GetEntries(new BoundingSphere(Position, MaxDistance), affectedEntries);

            foreach (BroadPhaseEntry entry in affectedEntries)
            {
                var entityCollision = entry as EntityCollidable;
                if (entityCollision != null)
                {
                    var e = entityCollision.Entity;
                    if (e.IsDynamic)
                    {
                        Vector3 offset = e.Position - Position;
                        float distanceSquared = offset.LengthSquared();
                        if (distanceSquared > Toolbox.Epsilon)
                        {
                            var distance = (float)Math.Sqrt(distanceSquared);
                            e.LinearMomentum += (offset * (Magnitude / (distanceSquared * distance)));
                        }
                        else
                        {
                            e.LinearMomentum += (new Vector3(0, Magnitude, 0));
                        }
                    }
                }
            }

            affectedEntries.Clear();
        }
    }
}
