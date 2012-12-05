using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GamesAssignmentMars
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : GameEntity
    {

        public Matrix projection;
        public Matrix view;
        private KeyboardState keyboardState;
        private MouseState mouseState;

        public override void Draw(GameTime gameTime)
        {
            // Do nothing
        }

        public override void LoadContent()
        {
        }
        public override void UnloadContent()
        {
        }

        public Camera()
        {
            pos = new Vector3(0.0f+248, 30.0f+35, 50.0f-250);
            look = new Vector3(0.0f, 0.0f, -1.0f);
        }

        public override void Update(GameTime gameTime)
        {

            float timeDelta = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;

            int deltaX = mouseX - midX;
            int deltaY = mouseY - midY;

            yaw(-(float)deltaX / 100.0f);
            pitch(-(float)deltaY / 100.0f);
            Mouse.SetPosition(midX, midY);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector3 newTargetPos = pos + (look * 50.0f);
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Vector3 newTargetPos = pos;

            }

            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                timeDelta *= 20.0f;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                walk(timeDelta*100.0f);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                walk(-timeDelta);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                strafe(-timeDelta);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                strafe(timeDelta);
            }

            view = Matrix.CreateLookAt(pos, pos + look, up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Game1.Instance.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);

        }

        public Matrix getProjection()
        {
            return projection;
        }

        public Matrix getView()
        {
            return view;
        }
    }
}
