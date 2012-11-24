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
        SoundEffect soundEffect;
        SoundEffectInstance soundEffectInstance;
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();


        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            worldTransform = body.WorldTransform;

        }
        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            Console.WriteLine("Got here!");

            emitter.Position = body.Position / 5; ;
            listener.Position = Game1.Instance.Camera.pos / 5;
            soundEffectInstance.Apply3D(listener, emitter);
            soundEffectInstance.Play();
        }

        public void configureEvents()
        {
            body.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
        }

    }
}
