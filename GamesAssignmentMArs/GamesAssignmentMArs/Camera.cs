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
            pos = new Vector3(248, 64, -200);
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
                    if (Game1.Instance.Rover.IfMaxMovementAngleUp())
                        RotateAroundXAxis(-0.01f);
                }

                if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (Game1.Instance.Rover.IfMaxMovementAngleDown())
                        RotateAroundXAxis(0.01f);
                }

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    RotateAroundYAxis(0.01f);
                }

                if (keyboardState.IsKeyDown(Keys.D))
                {
                    RotateAroundYAxis(-0.01f);
                }
            }

            view = Matrix.CreateLookAt(pos, pos + look, up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Game1.Instance.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);

        }

        public void RotateAroundXAxis(float angle)
        {
            Game1.Instance.Rover.SetCameraLaserContainerXaxisRotation(1 * angle);
            Matrix T = Matrix.CreateFromAxisAngle(right, angle);//rotating around the right vector.
            SetLaserAndCamera(T);
        }

        public void RotateAroundYAxis(float angle)
        {
            Game1.Instance.Rover.SetbodyCameraCylinderJoint(1 * angle);
            Matrix T = Matrix.CreateRotationY(angle);
            SetLaserAndCamera(T);
        }

        private void SetLaserAndCamera(Matrix T)
        {
            Game1.Instance.Rover.SetLaserLook(Vector3.Transform(Game1.Instance.Rover.GetLaserLook(), T));
            look = Vector3.Transform(Game1.Instance.Rover.GetLaserLook(), T);
            pos = Game1.Instance.Rover.cameraLaserContainer.body.Position + (Game1.Instance.Rover.GetLaserLook() * 3);
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
