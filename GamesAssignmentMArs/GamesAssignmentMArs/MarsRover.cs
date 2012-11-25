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
        RevoluteJoint baseCameraJoint2;
        BepuEntity cameraArm = new BepuEntity();
        public override void LoadContent()
        {
            BepuEntity baseBox = new BepuEntity();
            baseBox.modelName = "cube";
            //baseBox.LoadContent();
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

            baseCameraJoint = new RevoluteJoint(baseBox.body, cameraArm.body,cameraArm.body.Position, Vector3.Up);
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

            BepuEntity wheel1= createWheel(new Vector3(-7,2,5),2,2);
            BepuEntity wheel2 = createWheel(new Vector3(12, 2, 15), 2, 2);
            BepuEntity wheel3 = createWheel(new Vector3(12, 2, 10), 2, 2);
            BepuEntity wheel4 = createWheel(new Vector3(12, 2, 5), 2, 2);
            BepuEntity wheel5 = createWheel(new Vector3(-7, 2, 15), 2, 2);
            BepuEntity wheel6 = createWheel(new Vector3(-7, 2, 10), 2, 2);

            //BepuEntity cylinder1 = createCylinder(new Vector3(-8, 2, 5), 2, 2);
            //BepuEntity cylinder2 = createCylinder(new Vector3(-20, 2, 15), 2, 2);
            //BepuEntity cylinder3 = createCylinder(new Vector3(-30, 2, 10), 2, 2);
            //BepuEntity cylinder4 = createCylinder(new Vector3(-40, 2, 5), 2, 2);
            //BepuEntity cylinder5 = createCylinder(new Vector3(-50, 2, 15), 2, 2);
            //BepuEntity cylinder6 = createCylinder(new Vector3(-60, 2, 10), 2, 2);

           //baseCameraJoint2 = new RevoluteJoint(cameraArm.body, cameraLaserBox.body,cameraLaserBox.body.Position, Vector3.Right);
           //baseCameraJoint2.Motor.IsActive = true;
           //baseCameraJoint2.Motor.Settings.Mode = MotorMode.Servomechanism;
           //baseCameraJoint2.Motor.Settings.MaximumForce = 3500;
           //Game1.Instance.Space.Add(baseCameraJoint2);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.N))
                baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.1f;
            if (keyState.IsKeyDown(Keys.M))
                baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.01f;
            if (keyState.IsKeyDown(Keys.B))
                baseCameraJoint2.Motor.Settings.Servo.Goal += 1 * 0.01f;
        }

        BepuEntity createWheel(Vector3 position, float wheelWidth, float wheelRadius)
        {
            BepuEntity wheelEntity = new BepuEntity();
            wheelEntity.modelName = "cyl";
            wheelEntity.body = new Cylinder(position, wheelWidth, wheelRadius, wheelRadius);
           wheelEntity.localTransform = Matrix.CreateScale(wheelRadius, wheelWidth, wheelRadius);
            wheelEntity.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.PiOver2);
           wheelEntity.diffuse = new Vector3(0, 0,0);
            Game1.Instance.Space.Add(wheelEntity.body);
            Game1.Instance.Children.Add(wheelEntity);
            return null;

        }

        BepuEntity createCylinder(Vector3 position, float cylinderheight, float cylinderRadius)
        {
            BepuEntity cylinderEntity = new BepuEntity();
            cylinderEntity.modelName = "cylinder";
            cylinderEntity.body = new Cylinder(position, cylinderheight, cylinderRadius, cylinderRadius);
            cylinderEntity.localTransform = Matrix.CreateScale(cylinderRadius, cylinderheight, cylinderRadius);
            cylinderEntity.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.PiOver2);
            cylinderEntity.diffuse = new Vector3(0, 0, 0);
            Game1.Instance.Space.Add(cylinderEntity.body);
            Game1.Instance.Children.Add(cylinderEntity);
            return null;

        }
    }
}
