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
using BEPUphysics.DeactivationManagement;

namespace GamesAssignmentMars
{
    public class MarsRover: BepuEntity
    {
        public RevoluteJoint baseCameraJoint;
        RevoluteJoint baseDrillJoint;
        public RevoluteJoint tester;
        private readonly RevoluteMotor drivingMotor1;
        private readonly RevoluteMotor drivingMotor2;
        private readonly RevoluteMotor steeringMotor1;
        private readonly RevoluteMotor steeringMotor2;
        PointOnLineJoint pointOnLineJoint1;
        LinearAxisLimit suspensionLimit1;
        //This linear axis motor
        LinearAxisMotor suspensionSpring1;
        RevoluteAngularJoint revoluteAngularJoint1;
        PointOnLineJoint pointOnLineJoint2;
        LinearAxisLimit suspensionLimit2;
        //This linear axis motor
        LinearAxisMotor suspensionSpring2;
        RevoluteAngularJoint revoluteAngularJoint2;
        PointOnLineJoint pointOnLineJoint3;
        LinearAxisLimit suspensionLimit3;
        //This linear axis motor
        LinearAxisMotor suspensionSpring3;
        SwivelHingeAngularJoint swivelHingeAngularJoint1;
        PointOnLineJoint pointOnLineJoint4;
        LinearAxisLimit suspensionLimit4;
        //This linear axis motor
        LinearAxisMotor suspensionSpring4;
        SwivelHingeAngularJoint swivelHingeAngularJoint2;
        public BepuEntity cameraLaser = new BepuEntity();
        BepuEntity baseBox = new BepuEntity();
        private float maximumTurnAngle = MathHelper.Pi * .2f;
        private float driveSpeed = 1000;

        public MarsRover()
            : base()
        {
            BepuEntity cameraArm = new BepuEntity();
            BepuEntity drillArm = new BepuEntity();

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


            cameraLaser.modelName = "cube";
            cameraLaser.LoadContent();
            cameraLaser.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            cameraLaser.body = new Cylinder(new Vector3(254, 64,- 252), 3, 3, 3);
            cameraLaser.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            cameraLaser.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -MathHelper.PiOver2);
            Game1.Instance.Space.Add(cameraLaser.body);
            Game1.Instance.Children.Add(cameraLaser);

            tester = new RevoluteJoint(cameraArm.body, cameraLaser.body, cameraLaser.body.Position, -Vector3.Right);
            tester.Motor.IsActive = true;
            tester.Motor.Settings.Mode = MotorMode.Servomechanism;
            tester.Motor.Settings.MaximumForce = 2500;
            Game1.Instance.Space.Add(tester);

            Entity backWheel1 = AddBackWheel(new Vector3(-7 + 258, 10 + 30, 3 - 270), baseBox.body);

            //Connect the wheel to the body.
            pointOnLineJoint1 = new PointOnLineJoint(baseBox.body, backWheel1, backWheel1.Position, Vector3.Down, backWheel1.Position);
            suspensionLimit1 = new LinearAxisLimit(baseBox.body, backWheel1, backWheel1.Position, backWheel1.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            suspensionSpring1 = new LinearAxisMotor(baseBox.body, backWheel1, backWheel1.Position, backWheel1.Position, Vector3.Down);
            suspensionSpring1.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring1.Settings.Servo.Goal = 0;
            suspensionSpring1.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring1.Settings.Servo.SpringSettings.DampingConstant = 70;

            revoluteAngularJoint1 = new RevoluteAngularJoint(baseBox.body, backWheel1, Vector3.Right);

            Game1.Instance.Space.Add(pointOnLineJoint1);
            Game1.Instance.Space.Add(suspensionLimit1);
            Game1.Instance.Space.Add(suspensionSpring1);
            Game1.Instance.Space.Add(revoluteAngularJoint1);

            Entity backWheel2 = AddBackWheel(new Vector3(11 + 258, 10 + 30, 3 - 270), baseBox.body);

            //Connect the wheel to the body.
            pointOnLineJoint2 = new PointOnLineJoint(baseBox.body, backWheel2, backWheel2.Position, Vector3.Down, backWheel2.Position);
            suspensionLimit2 = new LinearAxisLimit(baseBox.body, backWheel2, backWheel2.Position, backWheel2.Position, Vector3.Down, -1, 0);
            suspensionSpring2 = new LinearAxisMotor(baseBox.body, backWheel2, backWheel2.Position, backWheel2.Position, Vector3.Down);
            suspensionSpring2.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring2.Settings.Servo.Goal = 0;
            suspensionSpring2.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring2.Settings.Servo.SpringSettings.DampingConstant = 70;

            revoluteAngularJoint2 = new RevoluteAngularJoint(baseBox.body, backWheel2, Vector3.Right);
            Game1.Instance.Space.Add(pointOnLineJoint2);
            Game1.Instance.Space.Add(suspensionLimit2);
            Game1.Instance.Space.Add(suspensionSpring2);
            Game1.Instance.Space.Add(revoluteAngularJoint2);

           // AddBackWheel(new Vector3(-7, 4, 10), baseBox.body);
            //AddBackWheel(new Vector3(11, 4, 10), baseBox.body);
            var wheel1 = AddDriveWheel(new Vector3(11 + 258, 10 + 30, 13 - 270), baseBox.body);

            //Connect the wheel to the body.
            pointOnLineJoint3 = new PointOnLineJoint(baseBox.body, wheel1, wheel1.Position, Vector3.Down, wheel1.Position);
            suspensionLimit3 = new LinearAxisLimit(baseBox.body, wheel1, wheel1.Position, wheel1.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            suspensionSpring3= new LinearAxisMotor(baseBox.body, wheel1, wheel1.Position, wheel1.Position, Vector3.Down);
            suspensionSpring3.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring3.Settings.Servo.Goal = 0;
            suspensionSpring3.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring3.Settings.Servo.SpringSettings.DampingConstant = 70;

            swivelHingeAngularJoint1 = new SwivelHingeAngularJoint(baseBox.body, wheel1, Vector3.Up, Vector3.Right);
            //Make the swivel hinge extremely rigid.  There are going to be extreme conditions when the wheels get up to speed;
            //we don't want the forces involved to torque the wheel off the frame!
            swivelHingeAngularJoint1.SpringSettings.DampingConstant *= 1000;
            swivelHingeAngularJoint1.SpringSettings.StiffnessConstant *= 1000;
            //Motorize the wheel.
            drivingMotor1 = new RevoluteMotor(baseBox.body, wheel1, Vector3.Left);
            drivingMotor1.Settings.VelocityMotor.Softness = .3f;
            drivingMotor1.Settings.MaximumForce = 100;
            //Let it roll when the user isn't giving specific commands.
            drivingMotor1.IsActive = false;
            steeringMotor1 = new RevoluteMotor(baseBox.body, wheel1, Vector3.Up);
            steeringMotor1.Settings.Mode = MotorMode.Servomechanism;

            steeringMotor1.Basis.SetWorldAxes(Vector3.Up, Vector3.Right);
            steeringMotor1.TestAxis = Vector3.Right;


            steeringMotor1.Settings.Servo.SpringSettings.Advanced.UseAdvancedSettings = true;
            steeringMotor1.Settings.Servo.SpringSettings.Advanced.Softness = 0;
            steeringMotor1.Settings.Servo.SpringSettings.Advanced.ErrorReductionFactor = 0f;

            var steeringConstraint = new RevoluteLimit(baseBox.body, wheel1, Vector3.Up, Vector3.Right, -maximumTurnAngle, maximumTurnAngle);

            Game1.Instance.Space.Add(pointOnLineJoint3);
            Game1.Instance.Space.Add(suspensionLimit3);
            Game1.Instance.Space.Add(suspensionSpring3);
            Game1.Instance.Space.Add(swivelHingeAngularJoint1);
            Game1.Instance.Space.Add(drivingMotor1);
            Game1.Instance.Space.Add(steeringMotor1);
            Game1.Instance.Space.Add(steeringConstraint);

            var wheel2 = AddDriveWheel(new Vector3(-7 + 258, 10 + 30, 13 - 270), baseBox.body);

            //Connect the wheel to the body.
            pointOnLineJoint4 = new PointOnLineJoint(baseBox.body, wheel2, wheel2.Position, Vector3.Down, wheel2.Position);
            suspensionLimit4 = new LinearAxisLimit(baseBox.body, wheel2, wheel2.Position, wheel2.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            suspensionSpring4 = new LinearAxisMotor(baseBox.body, wheel2, wheel2.Position, wheel2.Position, Vector3.Down);
            suspensionSpring4.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring4.Settings.Servo.Goal = 0;
            suspensionSpring4.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring4.Settings.Servo.SpringSettings.DampingConstant = 70;

            swivelHingeAngularJoint2 = new SwivelHingeAngularJoint(baseBox.body, wheel2, Vector3.Up, Vector3.Right);
            //Make the swivel hinge extremely rigid.  There are going to be extreme conditions when the wheels get up to speed;
            //we don't want the forces involved to torque the wheel off the frame!
            swivelHingeAngularJoint2.SpringSettings.DampingConstant *= 1000;
            swivelHingeAngularJoint2.SpringSettings.StiffnessConstant *= 1000;
            //Motorize the wheel.
            drivingMotor2 = new RevoluteMotor(baseBox.body, wheel2, Vector3.Left);
            drivingMotor2.Settings.VelocityMotor.Softness = .3f;
            drivingMotor2.Settings.MaximumForce = 100;
            //Let it roll when the user isn't giving specific commands.
            drivingMotor2.IsActive = false;
            steeringMotor2 = new RevoluteMotor(baseBox.body, wheel2, Vector3.Up);
            steeringMotor2.Settings.Mode = MotorMode.Servomechanism;

            steeringMotor2.Basis.SetWorldAxes(Vector3.Up, Vector3.Right);
            steeringMotor2.TestAxis = Vector3.Right;


            steeringMotor2.Settings.Servo.SpringSettings.Advanced.UseAdvancedSettings = true;
            steeringMotor2.Settings.Servo.SpringSettings.Advanced.Softness = 0;
            steeringMotor2.Settings.Servo.SpringSettings.Advanced.ErrorReductionFactor = 0f;

            steeringConstraint = new RevoluteLimit(baseBox.body, wheel2, Vector3.Up, Vector3.Right, -maximumTurnAngle, maximumTurnAngle);

            Game1.Instance.Space.Add(pointOnLineJoint4);
            Game1.Instance.Space.Add(suspensionLimit4);
            Game1.Instance.Space.Add(suspensionSpring4);
            Game1.Instance.Space.Add(swivelHingeAngularJoint2);
            Game1.Instance.Space.Add(drivingMotor2);
            Game1.Instance.Space.Add(steeringMotor2);
            Game1.Instance.Space.Add(steeringConstraint);

            var steeringStabilizer = new RevoluteAngularJoint(wheel1, wheel2, Vector3.Right);
            Game1.Instance.Space.Add(steeringStabilizer);
        }

        public override void LoadContent()
        {
        }
        public Vector3 laserLook = new Vector3(-0.2f, 0, 1);
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.N))
            {
                baseCameraJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                Matrix T = Matrix.CreateRotationY(-0.01f);
                laserLook = Vector3.Transform(laserLook, T);

            }
            if (keyState.IsKeyDown(Keys.M))
            {
                baseCameraJoint.Motor.Settings.Servo.Goal += 1 * 0.01f;
                Matrix T = Matrix.CreateRotationY(0.01f);
                laserLook = Vector3.Transform(laserLook, T);
            }
            if (keyState.IsKeyDown(Keys.B))
            {
                tester.Motor.Settings.Servo.Goal -= 1 * 0.01f;
                Matrix T = Matrix.CreateFromAxisAngle(right, -0.01f);//rotating around the right vector.
                laserLook = Vector3.Transform(laserLook, T);
            }
            if (keyState.IsKeyDown(Keys.V))
            {
                tester.Motor.Settings.Servo.Goal += 1 * 0.01f;
                Matrix T = Matrix.CreateFromAxisAngle(right, 0.01f);//rotating around the right vector.
                laserLook = Vector3.Transform(laserLook, T);
            }
            if(keyState.IsKeyDown(Keys.C))
            {
                baseDrillJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
            }

            if(keyState.IsKeyDown(Keys.Space))
            {
                Lazer laser = new Lazer();
                laser.pos = cameraLaser.body.Position;
                laser.look = laserLook;
                Game1.Instance.Children.Add(laser);
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

            if (keyState.IsKeyDown(Keys.P))
            {
                Game1.Instance.Camera.look = laserLook;
                Game1.Instance.Camera.pos = new Vector3(cameraLaser.body.Position.X, cameraLaser.body.Position.Y, cameraLaser.body.Position.Z) + (laserLook*3);
                Game1.Instance.Camera.isRoverCamera = true;

            }
            if (keyState.IsKeyDown(Keys.I))
            {
                Explosion kapowMaker = new Explosion(Vector3.Zero, 400, 15, Game1.Instance.Space);
                //Detonate the bomb
                kapowMaker.Position = baseBox.body.Position;
                baseCameraJoint.IsActive = false;
                baseDrillJoint.IsActive = false;
                tester.IsActive = false;
                drivingMotor1.IsActive = false;
                drivingMotor2.IsActive = false;
                steeringMotor1.IsActive = false;
                steeringMotor2.IsActive = false;
                pointOnLineJoint1.IsActive = false;
                suspensionLimit1.IsActive = false;
                //This linear axis motor
                suspensionSpring1.IsActive = false;
                revoluteAngularJoint1.IsActive = false;

                pointOnLineJoint2.IsActive = false;
                suspensionLimit2.IsActive = false;
                //This linear axis motor
                suspensionSpring2.IsActive = false;
                revoluteAngularJoint2.IsActive = false;
                pointOnLineJoint3.IsActive = false;
                suspensionLimit3.IsActive = false;
                suspensionSpring3.IsActive = false;
                swivelHingeAngularJoint1.IsActive = false;
                pointOnLineJoint4.IsActive = false;
                suspensionLimit4.IsActive = false;
                suspensionSpring4.IsActive = false;
                swivelHingeAngularJoint2.IsActive = false;
                kapowMaker.Explode();
            }


        }

        Entity AddBackWheel(Vector3 wheelPosition,Entity baseBody)
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

            //Add the wheel and connection to the space.
            Game1.Instance.Space.Add(wheel.body);
            Game1.Instance.Children.Add(wheel);

            return wheel.body;
        }

        Entity AddDriveWheel(Vector3 wheelPosition, Entity boxBase)
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

            //Add the wheel and connection to the space.
            Game1.Instance.Space.Add(wheel.body);
            Game1.Instance.Children.Add(wheel);

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
