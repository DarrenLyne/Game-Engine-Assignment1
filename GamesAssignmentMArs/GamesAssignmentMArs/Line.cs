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
    //Code was in starter code for this assignment,not edited
    public class Line
    {
        private static VertexPositionColor[] pointList = new VertexPositionColor[400];
        private static BasicEffect basicEffect = new BasicEffect(Game1.Instance.GraphicsDevice);
        static int currentLine = 0;

        static Line()
        {
            basicEffect.VertexColorEnabled = true;
        }

        static public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Game1.Instance.Camera.getView();
            basicEffect.Projection = Game1.Instance.Camera.getProjection();
            pointList[currentLine++] = new VertexPositionColor(start, color);
            pointList[currentLine++] = new VertexPositionColor(end, color);

        }

        static public void Draw()
        {
            if (currentLine != 0)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, pointList, 0, currentLine / 2);
                }
                currentLine = 0;
            }
        }

    }
}
