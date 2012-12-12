using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GamesAssignmentMars
{
    // Code taken from 2-XNA-Daleks-with-a-Camera demo provided during the module and adapted for this assignment
    class Lazer:GameEntity
    {
        public override void LoadContent()
        {
        }
        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            float width = Game1.Instance.Terrain.terrainWidth;//changed this to fit terrain, not the ground which has been deleted
            float height = Game1.Instance.Terrain.terrainHeight;//changed this to fit terrain, not the ground which has been deleted
            float speed = 50.0f;
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if ((pos.X < -(width / 2)) || (pos.X > width / 2) || (pos.Z < -(height / 2)) || (pos.Z > height / 2) || (pos.Y < 0) || (pos.Y > 100))
            {
                Alive = false;
            }
             pos += look * speed * timeDelta;
        }

        public override void Draw(GameTime gameTime)
        {
            Line.DrawLine(pos, pos + look * 10, Color.Green);
            Line.Draw();
        }
    }
}
