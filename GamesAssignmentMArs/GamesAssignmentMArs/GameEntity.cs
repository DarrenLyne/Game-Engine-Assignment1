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
    public abstract class GameEntity
    {
        public string modelName;
        public Model model = null;
        public Vector3 pos = Vector3.Zero;

        public Vector3 velocity = Vector3.Zero;
        public Vector3 diffuse = new Vector3(1, 1, 1);
        public Quaternion quaternion;

        public Vector3 right = new Vector3(1.0f, 0.0f, 0.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 look = new Vector3(0, 0, -1);
        public Vector3 basis = new Vector3(0, 0, -1);
        public Vector3 globalUp = new Vector3(0, 0, 1);
        public bool Alive = true;
        public float scale;

        float mass = 1.0f;
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Matrix worldTransform = new Matrix();
        public Matrix localTransform = Matrix.Identity;
        public Vector3 force = Vector3.Zero;

        public virtual void LoadContent()
        {
            model = Game1.Instance.Content.Load<Model>(modelName);
        }

        public abstract void Update(GameTime gameTime);


        public virtual void Draw(GameTime gameTime)
        {
            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.DiffuseColor = diffuse;
                        effect.World = localTransform * worldTransform;
                        effect.Projection = Game1.Instance.Camera.getProjection();
                        effect.View = Game1.Instance.Camera.getView();
                    }
                    mesh.Draw();
                }
            }
        }

        public virtual void UnloadContent()
        {
        }

        public void yaw(float angle)
        {
            Matrix T = Matrix.CreateRotationY(angle);
            right = Vector3.Transform(right, T);
            look = Vector3.Transform(look, T);
        }

        public void pitch(float angle)
        {
            Matrix T = Matrix.CreateFromAxisAngle(right, angle);
            //_up = Vector3.Transform(_up, T);
            look = Vector3.Transform(look, T);
        }

        public void walk(float amount)
        {
            pos += look * amount;
        }

        public void strafe(float amount)
        {

            pos += right * amount;

        }

        public float getYaw()
        {

            Vector3 localLook = look;
            localLook.Y = basis.Y;
            localLook.Normalize();
            float angle = (float)Math.Acos(Vector3.Dot(basis, localLook));

            if (look.X > 0)
            {
                angle = (MathHelper.Pi * 2.0f) - angle;
            }
            return angle;

        }

        public float getPitch()
        {
            if (look.Y == basis.Y)
            {
                return 0;
            }
            Vector3 localBasis = new Vector3(look.X, 0, look.Z);
            localBasis.Normalize();
            float dot = Vector3.Dot(localBasis, look);
            float angle = (float)Math.Acos(dot);

            if (look.Y < 0)
            {
                angle = (MathHelper.Pi * 2.0f) - angle;
            }

            return angle;
        }

    }
}
