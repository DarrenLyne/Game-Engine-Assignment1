using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GamesAssignmentMars
{
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
           // float width = Game1
           // float height = Game1.Instance.Ground.height;
            float speed = 50.0f;
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
           // if ((pos.X < -(width / 2)) || (pos.X > width / 2) || (pos.Z < -(height / 2)) || (pos.Z > height / 2) || (pos.Y < 0) || (pos.Y > 100))
           // {
           //     Alive = false;
            //}
             pos += look * speed * timeDelta;
        }

        public override void Draw(GameTime gameTime)
        {
            Line.DrawLine(pos, pos + look * 10, Color.Green);
            Line.Draw();
        }
    }
}
