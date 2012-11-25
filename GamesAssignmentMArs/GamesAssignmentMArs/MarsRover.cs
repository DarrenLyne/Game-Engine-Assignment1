using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Microsoft.Xna.Framework.Input;

namespace GamesAssignmentMars
{
    class MarsRover: BepuEntity
    {
        RevoluteJoint baseCameraJoint;
        BepuEntity cameraArm = new BepuEntity();
        public override void LoadContent()
        {
            BepuEntity baseBox = new BepuEntity();
            baseBox.modelName = "cube";
            baseBox.LoadContent();
            baseBox.localTransform = Matrix.CreateScale(new Vector3(15,5,20));
            baseBox.body = new Box(new Vector3(2,2, 10), 15, 5, 20);
            baseBox.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(baseBox.body);
            Game1.Instance.Children.Add(baseBox);


            cameraArm.modelName = "cylinder";
            cameraArm.LoadContent();
            cameraArm.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            cameraArm.body = new Cylinder(new Vector3(-4, 10, 18), 3, 3, 3);
            cameraArm.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            cameraArm.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
            Game1.Instance.Space.Add(cameraArm.body);
            Game1.Instance.Children.Add(cameraArm);

            baseCameraJoint = new RevoluteJoint(baseBox.body, cameraArm.body,new Vector3(0,1,13), Vector3.Up);
            baseCameraJoint.Motor.IsActive = true;
            baseCameraJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
           baseCameraJoint.Motor.Settings.MaximumForce = 3500;
            Game1.Instance.Space.Add(baseCameraJoint);

            BepuEntity cameraLaserBox = new BepuEntity();
            cameraLaserBox.modelName = "cube";
            cameraLaserBox.LoadContent();
            cameraLaserBox.localTransform = Matrix.CreateScale(new Vector3(5, 3, 5));
            cameraLaserBox.body = new Box(new Vector3(-3, 20, 25), 5, 2, 3);
            cameraLaserBox.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(cameraLaserBox.body);
            Game1.Instance.Children.Add(cameraLaserBox);

            //baseCameraJoint = new RevoluteJoint(cameraArm.body, cameraLaserBox.body, new Vector3(-4, 20, 18), Vector3.Up);
           // baseCameraJoint.Motor.IsActive = true;
           // baseCameraJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
           // baseCameraJoint.Motor.Settings.MaximumForce = 3500;
           // Game1.Instance.Space.Add(baseCameraJoint);
//


        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.N))
            {
                Matrix T = Matrix.CreateRotationY(5.0f);
                cameraArm.right = Vector3.Transform(cameraArm.right, T);
                cameraArm.look = Vector3.Transform(cameraArm.look, T);
            }
                //baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.5f;
            if (keyState.IsKeyDown(Keys.M))
                baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.5f;
            //if (keyState.IsKeyDown(Keys.N))
                //baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.5f;
           // if (keyState.IsKeyDown(Keys.M))
               // baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.5f;
        }
    }
}
