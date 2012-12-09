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
        private  RevoluteMotor drivingMotor1;
        private readonly RevoluteMotor drivingMotor2;
        private  RevoluteMotor steeringMotor1;
        private readonly RevoluteMotor steeringMotor2;
        public BepuEntity cameraLaser = new BepuEntity();
        BepuEntity baseBox = new BepuEntity();
        private float maximumTurnAngle = MathHelper.Pi * .2f;
        private float driveSpeed = 1000;

        List<RevoluteJoint> roverJoints = new List<RevoluteJoint>();
        List<Motor> motors = new List<Motor>();
        List<LinearAxisMotor> suspensionSprings = new List<LinearAxisMotor>();
        List<Joint> joints = new List<Joint>();

        public MarsRover(): base()
        {
            BepuEntity cameraArm = new BepuEntity();
            BepuEntity drillArm = new BepuEntity();

            baseBox.modelName = "cube";
            baseBox.LoadContent();
            baseBox.localTransform = Matrix.CreateScale(new Vector3(15, 5, 20));
            baseBox.body = new Box(new Vector3(260, 45, -260), 15, 5, 20);
            baseBox.body.BecomeDynamic(100);
            baseBox.body.CollisionInformation.LocalPosition = new Vector3(0, .8f, 0);
            baseBox.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(baseBox.body);
            Game1.Instance.Children.Add(baseBox);

            drillArm = AddCylinder("cylinder",new Vector3(267, 44, -244), false);
            cameraArm = AddCylinder("cylinder",new Vector3(254, 54, -252), true);

            CreateRevoluteJoint(out baseDrillJoint, baseBox, drillArm, new Vector3(267, 44, -250), 3500, Vector3.Right);
            roverJoints.Add(baseDrillJoint);

            CreateRevoluteJoint(out baseCameraJoint, baseBox, cameraArm, cameraArm.body.Position, 1500, Vector3.Up);
            roverJoints.Add(baseCameraJoint);

            cameraLaser=AddCylinder("cube",new Vector3(254, 61, -252), true);

            CreateRevoluteJoint(out tester, cameraArm, cameraLaser, cameraLaser.body.Position, 2500, -Vector3.Right);
            roverJoints.Add(tester);

            Entity backWheel1 = AddBackWheel(new Vector3(251, 40, -267), baseBox.body);
            ConnectWheelBody(backWheel1);

            Entity backWheel2 = AddBackWheel(new Vector3(269, 40, -267), baseBox.body);
            ConnectWheelBody(backWheel2);

           // AddBackWheel(new Vector3(-7, 4, 10), baseBox.body);
            //AddBackWheel(new Vector3(11, 4, 10), baseBox.body);
            var wheel1 = AddDriveWheel(new Vector3(269, 40, -257), baseBox.body);
            SetUpFrontMotorWheels(wheel1, out drivingMotor1,out steeringMotor1);

            var wheel2 = AddDriveWheel(new Vector3(251, 40, -257), baseBox.body);
            SetUpFrontMotorWheels(wheel2, out drivingMotor2, out steeringMotor2);

            var steeringStabilizer = new RevoluteAngularJoint(wheel1, wheel2, Vector3.Right);
            Game1.Instance.Space.Add(steeringStabilizer);
        }

        private void SetUpFrontMotorWheels(Entity wheel1,out RevoluteMotor drivingMotor, out RevoluteMotor steeringMotor)
        {
            //Connect the wheel to the body.
            PointOnLineJoint pointOnLineJoint = new PointOnLineJoint(baseBox.body, wheel1, wheel1.Position, Vector3.Down, wheel1.Position);
            LinearAxisLimit suspensionLimit = new LinearAxisLimit(baseBox.body, wheel1, wheel1.Position, wheel1.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            CreateSuspensionString(baseBox, wheel1);

            SwivelHingeAngularJoint swivelHingeAngularJoint = new SwivelHingeAngularJoint(baseBox.body, wheel1, Vector3.Up, Vector3.Right);
            //Make the swivel hinge extremely rigid.  There are going to be extreme conditions when the wheels get up to speed;
            //we don't want the forces involved to torque the wheel off the frame!
            swivelHingeAngularJoint.SpringSettings.DampingConstant *= 1000;
            swivelHingeAngularJoint.SpringSettings.StiffnessConstant *= 1000;
            //Motorize the wheel.
            drivingMotor = new RevoluteMotor(baseBox.body, wheel1, Vector3.Left);
            drivingMotor.Settings.VelocityMotor.Softness = .3f;
            drivingMotor.Settings.MaximumForce = 100;
            //Let it roll when the user isn't giving specific commands.
            drivingMotor.IsActive = false;
            steeringMotor = new RevoluteMotor(baseBox.body, wheel1, Vector3.Up);
            steeringMotor.Settings.Mode = MotorMode.Servomechanism;

            steeringMotor.Basis.SetWorldAxes(Vector3.Up, Vector3.Right);
            steeringMotor.TestAxis = Vector3.Right;

            steeringMotor.Settings.Servo.SpringSettings.Advanced.UseAdvancedSettings = true;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.Softness = 0;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.ErrorReductionFactor = 0f;

            var steeringConstraint = new RevoluteLimit(baseBox.body, wheel1, Vector3.Up, Vector3.Right, -maximumTurnAngle, maximumTurnAngle);

            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            Game1.Instance.Space.Add(swivelHingeAngularJoint);
            Game1.Instance.Space.Add(drivingMotor);
            Game1.Instance.Space.Add(steeringMotor);
            Game1.Instance.Space.Add(steeringConstraint);
            joints.Add(pointOnLineJoint);
            joints.Add(suspensionLimit);
            joints.Add(swivelHingeAngularJoint);
            motors.Add(drivingMotor);
            motors.Add(steeringMotor);
        }
        private void ConnectWheelBody(Entity backWheel)
        {
            //Connect the wheel to the body.
            PointOnLineJoint pointOnLineJoint = new PointOnLineJoint(baseBox.body, backWheel, backWheel.Position, Vector3.Down, backWheel.Position);
            LinearAxisLimit suspensionLimit = new LinearAxisLimit(baseBox.body, backWheel, backWheel.Position, backWheel.Position, Vector3.Down, -1, 0);
            //This linear axis motor will give the suspension its springiness by pushing the wheels outward.
            CreateSuspensionString(baseBox, backWheel);

            RevoluteAngularJoint revoluteAngularJoint = new RevoluteAngularJoint(baseBox.body, backWheel, Vector3.Right);

            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            Game1.Instance.Space.Add(revoluteAngularJoint);
            joints.Add(pointOnLineJoint);
            joints.Add(suspensionLimit);
            joints.Add(revoluteAngularJoint);

        }

        private void CreateSuspensionString(BepuEntity connectionA, Entity connectionB)
        {
            LinearAxisMotor suspensionSpring = new LinearAxisMotor(baseBox.body, connectionB, connectionB.Position, connectionB.Position, Vector3.Down);
            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring.Settings.Servo.SpringSettings.DampingConstant = 70;
            Game1.Instance.Space.Add(suspensionSpring);
            suspensionSprings.Add(suspensionSpring);
        }
        private void CreateRevoluteJoint(out RevoluteJoint joint,BepuEntity connectionA,BepuEntity connectionB,Vector3 anchor,int maxForce,Vector3 freeAxis)
        {
            joint = new RevoluteJoint(connectionA.body, connectionB.body, anchor, freeAxis);
            joint.Motor.IsActive = true;
            joint.Motor.Settings.Mode = MotorMode.Servomechanism;
            joint.Motor.Settings.MaximumForce = maxForce;
            Game1.Instance.Space.Add(joint);
        }

        public override void LoadContent()
        {
        }
        public Vector3 laserLook = new Vector3(-0.2f, 0, 1);
        public void RotationYAxis(float angle)
        {
            baseCameraJoint.Motor.Settings.Servo.Goal += 1 * angle;
            Matrix T = Matrix.CreateRotationY(-0.01f);
            laserLook = Vector3.Transform(laserLook, T);
        }
        public void RotationXAxis(float angle)
        {
            tester.Motor.Settings.Servo.Goal += 1 * angle;
            Matrix T = Matrix.CreateFromAxisAngle(right, -0.01f);//rotating around the right vector.
            laserLook = Vector3.Transform(laserLook, T);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.N))
            {
                RotationYAxis(-0.01f);
            }
            if (keyState.IsKeyDown(Keys.M))
            {
                RotationYAxis(0.01f);
            }
            if (keyState.IsKeyDown(Keys.B))
            {
                RotationXAxis(-0.01f);
            }
            if (keyState.IsKeyDown(Keys.V))
            {
                RotationXAxis(0.01f);
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
                MoveForwardBackward(-driveSpeed);
            }
            else if (keyState.IsKeyDown(Keys.J))
            {
                MoveForwardBackward(driveSpeed);
            }
            else
            {
                SetMotorActive(false);
            }

            if (keyState.IsKeyDown(Keys.K))
            {
                TurnLeftRight(maximumTurnAngle);
            }
            else if (keyState.IsKeyDown(Keys.L))
            {
                TurnLeftRight(-maximumTurnAngle);
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

                foreach (RevoluteJoint joint in roverJoints)
                    joint.IsActive = false;

                foreach (var suspensionString in suspensionSprings)
                    suspensionString.IsActive = false;

                foreach (var joint in joints)
                    joint.IsActive = false;
                foreach (var motor in motors)
                    motor.IsActive = false;

                kapowMaker.Explode();
            }

        }

        private void TurnLeftRight(float turnAngle)
        {
            steeringMotor1.Settings.Servo.Goal = turnAngle;
            steeringMotor2.Settings.Servo.Goal = turnAngle;
        }

        private void MoveForwardBackward(float speed)
        {
            drivingMotor1.Settings.VelocityMotor.GoalVelocity = speed;
            drivingMotor2.Settings.VelocityMotor.GoalVelocity = speed;
            SetMotorActive(true);
        }

        private void SetMotorActive(bool isActive)
        {
            drivingMotor1.IsActive = isActive;
            drivingMotor2.IsActive = isActive;
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

        BepuEntity AddCylinder(string modelName,Vector3 Postion, bool Orientation)
        {
            BepuEntity cylinder= new BepuEntity();
            cylinder.modelName = modelName;
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
