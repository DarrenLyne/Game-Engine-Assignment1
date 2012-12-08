using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GamesAssignmentMars
{
    class StarsSphere : GameEntity
    {
        Model StarsSphereModel;
        Effect StarsSphereEffect;

        public override void Update(GameTime gameTime)
        {

        }

        public override void UnloadContent()
        {

        }

        public override void LoadContent()
        {

            StarsSphereModel = Game1.Instance.Content.Load<Model>("SphereHighPoly");
            TextureCube SkySphereTexture = Game1.Instance.Content.Load<TextureCube>("SkySphereTexture");
            StarsSphereEffect = Game1.Instance.Content.Load<Effect>("SkySphere");
            StarsSphereEffect.Parameters["ViewMatrix"].SetValue(Game1.Instance.Camera.view);
            StarsSphereEffect.Parameters["ProjectionMatrix"].SetValue(Game1.Instance.Camera.projection);
            StarsSphereEffect.Parameters["SkyboxTexture"].SetValue(SkySphereTexture);

            StarsSphereEffect = Game1.Instance.Content.Load<Effect>("SkySphere");

            foreach (ModelMesh mesh in StarsSphereModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = StarsSphereEffect;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            StarsSphereEffect.Parameters["ViewMatrix"].SetValue(Game1.Instance.Camera.view);
            StarsSphereEffect.Parameters["ProjectionMatrix"].SetValue(Game1.Instance.Camera.projection);

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            Game1.Instance.GraphicsDevice.DepthStencilState = dss;
            foreach (ModelMesh mesh in StarsSphereModel.Meshes)
            {

                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            Game1.Instance.GraphicsDevice.DepthStencilState = dss;
        }
    }
}
