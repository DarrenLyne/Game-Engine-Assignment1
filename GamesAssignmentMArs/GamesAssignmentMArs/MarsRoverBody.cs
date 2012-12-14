using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;
using BEPUphysics.CollisionRuleManagement;

namespace GamesAssignmentMars
{
    //My own class, handles the aspects that create the body for the rover
    class MarsRoverBody
    {
        public BepuEntity AddCylinder(string modelName, Vector3 Postion, bool Orientation)
        {
            BepuEntity cylinder = new BepuEntity();
            cylinder.modelName = modelName;
            cylinder.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            cylinder.body = new Cylinder(Postion, 3, 3, 3);
            cylinder.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            if (Orientation)//if cylinder needs to be rotated
                cylinder.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
            Game1.Instance.Space.Add(cylinder.body);
            Game1.Instance.Children.Add(cylinder);

            return cylinder;
        }

        public Entity AddWheel(Vector3 wheelPosition, Entity baseBody)
        {
            BepuEntity wheel = new BepuEntity();
            wheel.modelName = "cyl";
            wheel.LoadContent();
            wheel.body = new Cylinder(wheelPosition, 2, 2, 2);
            wheel.localTransform = Matrix.CreateScale(2f, 2f, 2f);
            wheel.body.Material.KineticFriction = 2.5f;
            wheel.body.Material.StaticFriction = 2.5f;
            wheel.body.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);
            wheel.diffuse = new Vector3(0, 0, 0);

            //Prevents collisionf from happening
            CollisionRules.AddRule(wheel.body, baseBody, CollisionRule.NoBroadPhase);

            //Add the wheel and connection to the space.
            Game1.Instance.Space.Add(wheel.body);
            Game1.Instance.Children.Add(wheel);

            return wheel.body;
        }

        public BepuEntity AddBox(Vector3 boxPosition)
        {
            BepuEntity body = new BepuEntity();
            body.modelName = "cube";
            body.LoadContent();
            body.localTransform = Matrix.CreateScale(new Vector3(15, 5, 20));
            body.body = new Box(new Vector3(260, 45, -260), 15, 5, 20);
            body.body.BecomeDynamic(100);
            body.body.CollisionInformation.LocalPosition = new Vector3(0, .8f, 0);
            body.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(body.body);
            Game1.Instance.Children.Add(body);

            return body;
        }
    }
}
