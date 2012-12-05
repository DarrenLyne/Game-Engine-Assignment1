using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Entities;

namespace GamesAssignmentMars
{
    class MarsRover: BepuEntity
    {
        RevoluteJoint baseCameraJoint;
        RevoluteJoint baseDrillJoint;
        RevoluteJoint tester;
        private readonly RevoluteMotor drivingMotor1;
        private readonly RevoluteMotor drivingMotor2;
        private readonly RevoluteMotor steeringMotor1;
        private readonly RevoluteMotor steeringMotor2;
        BepuEntity test2 = new BepuEntity();
        private float maximumTurnAngle = MathHelper.Pi * .2f;
        private float driveSpeed = 1000;

        public MarsRover()
            : base()
        {
            BepuEntity cameraArm = new BepuEntity();
            BepuEntity drillArm = new BepuEntity();

            BepuEntity baseBox = new BepuEntity();
            baseBox.modelName = "cube";
            baseBox.LoadContent();
            baseBox.localTransform = Matrix.CreateScale(new Vector3(15, 5, 20));
            baseBox.body = new Box(new Vector3(2 + 258, 15 + 30, 10 - 270), 15, 5, 20);
            baseBox.body.BecomeDynamic(100);
            baseBox.body.CollisionInformation.LocalPosition = new Vector3(0, .8f, 0);
            baseBox.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(baseBox.body);
            Game1.Instance.Children.Add(baseBox);

            drillArm = AddCylinder(new Vector3(9 + 258, 14 + 30, 26 - 270), false);
            cameraArm = AddCylinder(new Vector3(-4 + 258, 24 + 30, 18 - 270), true);

            baseDrillJoint = new RevoluteJoint(baseBox.body, drillArm.body, new Vector3(9 + 258, 14 + 30, 20 - 270), Vector3.Right);
            baseDrillJoint.Motor.IsActive = true;
            baseDrillJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            baseDrillJoint.Motor.Settings.MaximumForce = 3500;
            Game1.Instance.Space.Add(baseDrillJoint);

            baseCameraJoint = new RevoluteJoint(baseBox.body, cameraArm.body, cameraArm.body.Position, Vector3.Up);
            baseCameraJoint.Motor.IsActive = true;
            baseCameraJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            baseCameraJoint.Motor.Settings.MaximumForce = 1500;
            Game1.Instance.Space.Add(baseCameraJoint);


            test2.modelName = "cube";
            test2.LoadContent();
            test2.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            test2.body = new Cylinder(new Vector3(-4 + 258, 34 + 30, 18 - 270), 3, 3, 3);
            test2.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            test2.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2);
            Game1.Instance.Space.Add(test2.body);
            Game1.Instance.Children.Add(test2);

            tester = new RevoluteJoint(cameraArm.body, test2.body, test2.body.Position, -Vector3.Right);
            tester.Motor.IsActive = true;
            tester.Motor.Settings.Mode = MotorMode.Servomechanism;
            tester.Motor.Settings.MaximumForce = 2500;
            Game1.Instance.Space.Add(tester);

            AddBackWheel(new Vector3(-7 + 258, 10 + 30, 3 - 270), baseBox.body);
            AddBackWheel(new Vector3(11 + 258, 10 + 30, 3 - 270), baseBox.body);
           // AddBackWheel(new Vector3(-7, 4, 10), baseBox.body);
            //AddBackWheel(new Vector3(11, 4, 10), baseBox.body);
            var wheel1 = AddDriveWheel(new Vector3(11 + 258, 10 + 30, 13 - 270), baseBox.body, out drivingMotor1, out steeringMotor1);
            var wheel2 = AddDriveWheel(new Vector3(-7 + 258, 10 + 30, 13 - 270), baseBox.body, out drivingMotor2, out steeringMotor2);
            var steeringStabilizer = new RevoluteAngularJoint(wheel1, wheel2, Vector3.Right);
            Game1.Instance.Space.Add(steeringStabilizer);
        }

        public override void LoadContent()
        {
        }
        Vector3 lookTester = new Vector3(-0.2f, 0, 1);
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.N))
            {
                baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                Matrix T = Matrix.CreateRotationY(-0.01f);
                lookTester = Vector3.Transform(lookTester, T);

            }
            if (keyState.IsKeyDown(Keys.M))
            {
                baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.01f;
                Matrix T = Matrix.CreateRotationY(0.01f);
                lookTester = Vector3.Transform(lookTester, T);
            }
            if (keyState.IsKeyDown(Keys.B))
            {
                tester.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                Matrix T = Matrix.CreateFromAxisAngle(right, -0.01f);//rotating around the right vector.
                lookTester = Vector3.Transform(lookTester, T);
            }
            if (keyState.IsKeyDown(Keys.V))
            {
                tester.Motor.Settings.Servo.Goal += 1 * 0.01f;
                Matrix T = Matrix.CreateFromAxisAngle(right, 0.01f);//rotating around the right vector.
                lookTester = Vector3.Transform(lookTester, T);
            }
            if(keyState.IsKeyDown(Keys.C))
            {
                baseDrillJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
            }

            if(keyState.IsKeyDown(Keys.Space))
            {
                Lazer bullet = new Lazer();
                bullet.pos = test2.body.Position;
                bullet.look = lookTester;
                 Game1.Instance.Children.Add(bullet);
            }

            ////Scale the corrective velocity by the wheel angular velocity to compensate for a long time step duration.
            ////If the simulation is running at a fast time step, this is probably not necessary.
            steeringMotor1.Settings.Servo.BaseCorrectiveSpeed = 3 + 7 * Math.Min(steeringMotor1.ConnectionB.AngularVelocity.Length() / 100, 1);
            steeringMotor2.Settings.Servo.BaseCorrectiveSpeed = 3 + 7 * Math.Min(steeringMotor2.ConnectionB.AngularVelocity.Length() / 100, 1);
            if (keyState.IsKeyDown(Keys.H))
            {
                //Go forward
                drivingMotor1.Settings.VelocityMotor.GoalVelocity = -driveSpeed;
                drivingMotor2.Settings.VelocityMotor.GoalVelocity = -driveSpeed;
                //The driving motors are disabled when no button is pressed, so need to turn it on.
                drivingMotor1.IsActive = true;
                drivingMotor2.IsActive = true;
            }
            else if (keyState.IsKeyDown(Keys.J))
            {
                //Go backward
                drivingMotor1.Settings.VelocityMotor.GoalVelocity = driveSpeed;
                drivingMotor2.Settings.VelocityMotor.GoalVelocity = driveSpeed;
                //The driving motors are disabled when no button is pressed, so need to turn it on.
                drivingMotor1.IsActive = true;
                drivingMotor2.IsActive = true;
            }
            else
            {
                //Let it roll.
                drivingMotor1.IsActive = false;
                drivingMotor2.IsActive = false;
            }

            if (keyState.IsKeyDown(Keys.K))
            {
                //Turn left
                steeringMotor1.Settings.Servo.Goal = maximumTurnAngle;
                steeringMotor2.Settings.Servo.Goal = maximumTurnAngle;
            }
            else if (keyState.IsKeyDown(Keys.L))
            {
                //Turn right
                steeringMotor1.Settings.Servo.Goal = -maximumTurnAngle;
                steeringMotor2.Settings.Servo.Goal = -maximumTurnAngle;
            }
            else
            {
                //Face forward
                steeringMotor1.Settings.Servo.Goal = 0;
                steeringMotor2.Settings.Servo.Goal = 0;
            }

        }

        void AddBackWheel(Vector3 wheelPosition,Entity baseBody)
        {
            var wheel = new BepuEntity();
            wheel.modelName = "cyl";
            wheel.LoadContent();
            wheel.body = new Cylinder(wheelPosition, 2, 2, 2);
            wheel.localTransform = Matrix.CreateScale(2f, 2f, 2f);
            wheel.body.Material.KineticFriction = 2.5f;
            wheel.body.Material.StaticFriction = 2.5f;
            wheel.body.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);

            //Preventing the occasional pointless collision pair can speed things up.
            CollisionRules.AddRule(wheel.body, baseBody, CollisionRule.NoBroadPhase);

            //Connect the wheel to the body.
            var pointOnLineJoint = new PointOnLineJoint(baseBody, wheel.body, wheel.body.Position, Vector3.Down, wheel.body.Position);
            var suspensionLimit = new LinearAxisLimit(baseBody, wheel.body, wheel.body.Position, wheel.body.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            var suspensionSpring = new LinearAxisMotor(baseBody, wheel.body, wheel.body.Position, wheel.body.Position, Vector3.Down);
            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring.Settings.Servo.SpringSettings.DampingConstant = 70;

            var revoluteAngularJoint = new RevoluteAngularJoint(baseBody, wheel.body, Vector3.Right);

            //Add the wheel and connection to the space.
            Game1.Instance.Space.Add(wheel.body);
            Game1.Instance.Children.Add(wheel);
            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            //Game1.Instance.Space.Add(suspensionSpring);
            Game1.Instance.Space.Add(revoluteAngularJoint);
        }

        Entity AddDriveWheel(Vector3 wheelPosition, Entity boxBase, out RevoluteMotor drivingMotor, out RevoluteMotor steeringMotor)
        {
            var wheel = new BepuEntity();
            wheel.modelName = "cyl";
            wheel.LoadContent();
            wheel.body = new Cylinder(wheelPosition, 2, 2, 2);
            wheel.localTransform = Matrix.CreateScale(2f, 2f, 2f);
            wheel.body.Material.KineticFriction = 2.5f;
            wheel.body.Material.StaticFriction = 2.5f;
            wheel.body.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);

            //Preventing the occasional pointless collision pair can speed things up.
            CollisionRules.AddRule(wheel.body, boxBase, CollisionRule.NoBroadPhase);

            //Connect the wheel to the body.
            var pointOnLineJoint = new PointOnLineJoint(boxBase, wheel.body, wheel.body.Position, Vector3.Down, wheel.body.Position);
            var suspensionLimit = new LinearAxisLimit(boxBase, wheel.body, wheel.body.Position, wheel.body.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            var suspensionSpring = new LinearAxisMotor(boxBase, wheel.body, wheel.body.Position, wheel.body.Position, Vector3.Down);
            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring.Settings.Servo.SpringSettings.DampingConstant = 70;

            var swivelHingeAngularJoint = new SwivelHingeAngularJoint(boxBase, wheel.body, Vector3.Up, Vector3.Right);
            //Make the swivel hinge extremely rigid.  There are going to be extreme conditions when the wheels get up to speed;
            //we don't want the forces involved to torque the wheel off the frame!
            swivelHingeAngularJoint.SpringSettings.DampingConstant *= 1000;
            swivelHingeAngularJoint.SpringSettings.StiffnessConstant *= 1000;
            //Motorize the wheel.
            drivingMotor = new RevoluteMotor(boxBase, wheel.body, Vector3.Left);
            drivingMotor.Settings.VelocityMotor.Softness = .3f;
            drivingMotor.Settings.MaximumForce = 100;
            //Let it roll when the user isn't giving specific commands.
            drivingMotor.IsActive = false;
            steeringMotor = new RevoluteMotor(boxBase, wheel.body, Vector3.Up);
            steeringMotor.Settings.Mode = MotorMode.Servomechanism;

            steeringMotor.Basis.SetWorldAxes(Vector3.Up, Vector3.Right);
            steeringMotor.TestAxis = Vector3.Right;


            steeringMotor.Settings.Servo.SpringSettings.Advanced.UseAdvancedSettings = true;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.Softness = 0;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.ErrorReductionFactor = 0f;

            var steeringConstraint = new RevoluteLimit(boxBase, wheel.body, Vector3.Up, Vector3.Right, -maximumTurnAngle, maximumTurnAngle);


            //Add the wheel and connection to the space.
            Game1.Instance.Space.Add(wheel.body);
            Game1.Instance.Children.Add(wheel);
            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            Game1.Instance.Space.Add(suspensionSpring);
            Game1.Instance.Space.Add(swivelHingeAngularJoint);
            Game1.Instance.Space.Add(drivingMotor);
            Game1.Instance.Space.Add(steeringMotor);
            Game1.Instance.Space.Add(steeringConstraint);

            return wheel.body;
        }

        BepuEntity AddCylinder(Vector3 Postion, bool Orientation)
        {
            BepuEntity cylinder= new BepuEntity();
            cylinder.modelName = "cylinder";
            cylinder.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            cylinder.body = new Cylinder(Postion, 3, 3, 3);
            cylinder.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            if(Orientation)
                cylinder.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
            Game1.Instance.Space.Add(cylinder.body);
            Game1.Instance.Children.Add(cylinder);

            return cylinder;
        }
    }
}
