using Microsoft.Xna.Framework;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using System;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework.Audio;
namespace GamesAssignmentMars
{
    public class  BepuEntity : GameEntity
    {
        public BEPUphysics.Entities.Entity body;


        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            worldTransform = body.WorldTransform;

        }

    }
}
