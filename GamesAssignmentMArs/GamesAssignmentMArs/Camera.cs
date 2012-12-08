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
        public bool isRoverCamera = false;

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
            pos = new Vector3(0.0f+248, 30.0f+34, 50.0f-250);
            look = new Vector3(0.0f, 0.0f, -1.0f);
        }

        public override void Update(GameTime gameTime)
        {

            float timeDelta = (float)(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            if (!isRoverCamera)
            {
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
                    walk(timeDelta * 10.0f);
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
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    Game1.Instance.rover.baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                    Matrix T = Matrix.CreateRotationY(-0.01f);
                    Game1.Instance.rover.laserLook = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    look = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    pos = Game1.Instance.rover.cameraLaser.body.Position + (Game1.Instance.rover.laserLook * 3);
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    Game1.Instance.rover.baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.01f;
                    Matrix T = Matrix.CreateRotationY(0.01f);
                    Game1.Instance.rover.laserLook = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    look = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    pos = Game1.Instance.rover.cameraLaser.body.Position + (Game1.Instance.rover.laserLook * 3);
                }

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    Game1.Instance.rover.tester.Motor.Settings.Servo.Goal += 1 * 0.01f;
                    Matrix T = Matrix.CreateFromAxisAngle(right, 0.01f);//rotating around the right vector.
                    Game1.Instance.rover.laserLook = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    look = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    pos = Game1.Instance.rover.cameraLaser.body.Position + (Game1.Instance.rover.laserLook * 3);
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    Game1.Instance.rover.tester.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                    Matrix T = Matrix.CreateFromAxisAngle(right,-0.01f);//rotating around the right vector.
                    Game1.Instance.rover.laserLook = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    look = Vector3.Transform(Game1.Instance.rover.laserLook, T);
                    pos = Game1.Instance.rover.cameraLaser.body.Position + (Game1.Instance.rover.laserLook * 3);
                }
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
