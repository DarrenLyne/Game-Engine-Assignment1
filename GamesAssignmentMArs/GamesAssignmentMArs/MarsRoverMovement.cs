using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.JointLimits;

namespace GamesAssignmentMars
{
    public class MarsRoverMovement
    {
        public RevoluteJoint bodyCameraCylinderJoint;//joint between the rover body and the cylinder that camera & laser stands on
        public RevoluteJoint bodyDrillJoint;////joint between the rover body and the cylinder that holds the drill
        public RevoluteJoint cameraLaserContainerXaxisRotation;//Joint between camera & laser container and cylinder that it stands on
        public RevoluteMotor drivingMotor1;//driving motor for one of the front wheels
        public RevoluteMotor drivingMotor2;//driving motor for one of the front wheels
        public RevoluteMotor steeringMotor1;//sterring motor for one of the front wheels
        public RevoluteMotor steeringMotor2;//sterring motor for one of the front wheels

        //Lists to hold each joint for when they will be disabled
        public List<RevoluteJoint> roverJoints = new List<RevoluteJoint>();
        public List<Motor> motors = new List<Motor>();
        public List<LinearAxisMotor> suspensionSprings = new List<LinearAxisMotor>();
        public List<Joint> joints = new List<Joint>();


        public Vector3 laserLook = new Vector3(-0.2f, 0, 1);// vector that holds where the laser is looking
        public float maximumTurnAngle = MathHelper.Pi * .2f;

        public void RotationCameraLaserYAxis(float angle)
        {
            //Rotate the camera/laser cylinder around y axis which will rotate container for camera/laser
            bodyCameraCylinderJoint.Motor.Settings.Servo.Goal += 1 * angle;
            Matrix T = Matrix.CreateRotationY(angle);
            laserLook = Vector3.Transform(laserLook, T);
        }
        public void RotationXAxis(float angle,Vector3 right)
        {
            //Rotate the camera/laser container around x axis for it to look up and down
            cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal += 1 * angle;
            Matrix T = Matrix.CreateFromAxisAngle(right, angle);
            laserLook = Vector3.Transform(laserLook, T);
        }

        public void SetUpFrontMotorWheels(BepuEntity roverBody,Entity wheel1, out RevoluteMotor drivingMotor, out RevoluteMotor steeringMotor)
        {
            //code taken from bepu physics demo suspensioncardemo.cs and adapted for this project
            PointOnLineJoint pointOnLineJoint = new PointOnLineJoint(roverBody.body, wheel1, wheel1.Position, Vector3.Down, wheel1.Position);
            LinearAxisLimit suspensionLimit = new LinearAxisLimit(roverBody.body, wheel1, wheel1.Position, wheel1.Position, Vector3.Down, -1, 0);
            CreateSuspensionString(roverBody, wheel1);

            SwivelHingeAngularJoint swivelHingeAngularJoint = new SwivelHingeAngularJoint(roverBody.body, wheel1, Vector3.Up, Vector3.Right);
            swivelHingeAngularJoint.SpringSettings.DampingConstant *= 1000;
            swivelHingeAngularJoint.SpringSettings.StiffnessConstant *= 1000;

            drivingMotor = new RevoluteMotor(roverBody.body, wheel1, -Vector3.Left);
            drivingMotor.Settings.VelocityMotor.Softness = .3f;
            drivingMotor.Settings.MaximumForce = 100;
            drivingMotor.IsActive = false;

            steeringMotor = new RevoluteMotor(roverBody.body, wheel1, Vector3.Up);
            steeringMotor.Settings.Mode = MotorMode.Servomechanism;
            steeringMotor.Basis.SetWorldAxes(Vector3.Up, Vector3.Right);
            steeringMotor.TestAxis = Vector3.Right;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.UseAdvancedSettings = true;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.Softness = 0;
            steeringMotor.Settings.Servo.SpringSettings.Advanced.ErrorReductionFactor = 0f;

            var steeringConstraint = new RevoluteLimit(roverBody.body, wheel1, Vector3.Up, Vector3.Right, -maximumTurnAngle, maximumTurnAngle);

            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            Game1.Instance.Space.Add(swivelHingeAngularJoint);
            Game1.Instance.Space.Add(drivingMotor);
            Game1.Instance.Space.Add(steeringMotor);
            Game1.Instance.Space.Add(steeringConstraint);

            //add joints to lists of joints for explosion
            joints.Add(pointOnLineJoint);
            joints.Add(suspensionLimit);
            joints.Add(swivelHingeAngularJoint);
            motors.Add(drivingMotor);
            motors.Add(steeringMotor);
        }

        public void ConnectWheelBody(Entity backWheel, BepuEntity roverBody)
        {
            //code taken from bepu physics demo suspensioncardemo.cs and adapted for this project
            PointOnLineJoint pointOnLineJoint = new PointOnLineJoint(roverBody.body, backWheel, backWheel.Position, Vector3.Down, backWheel.Position);
            LinearAxisLimit suspensionLimit = new LinearAxisLimit(roverBody.body, backWheel, backWheel.Position, backWheel.Position, Vector3.Down, -1, 0);
            CreateSuspensionString(roverBody, backWheel);

            RevoluteAngularJoint revoluteAngularJoint = new RevoluteAngularJoint(roverBody.body, backWheel, Vector3.Right);

            Game1.Instance.Space.Add(pointOnLineJoint);
            Game1.Instance.Space.Add(suspensionLimit);
            Game1.Instance.Space.Add(revoluteAngularJoint);

            //add joints to lists of joints for explosion
            joints.Add(pointOnLineJoint);
            joints.Add(suspensionLimit);
            joints.Add(revoluteAngularJoint);

        }

        public void CreateSuspensionString(BepuEntity connectionA, Entity connectionB)
        {
            //code taken from bepu physics demo suspensioncardemo.cs and adapted for this project
            LinearAxisMotor suspensionSpring = new LinearAxisMotor(connectionA.body, connectionB, connectionB.Position, connectionB.Position, Vector3.Down);
            suspensionSpring.Settings.Mode = MotorMode.Servomechanism;
            suspensionSpring.Settings.Servo.Goal = 0;
            suspensionSpring.Settings.Servo.SpringSettings.StiffnessConstant = 300;
            suspensionSpring.Settings.Servo.SpringSettings.DampingConstant = 70;
            Game1.Instance.Space.Add(suspensionSpring);
            //add joints to lists of joints for explosion
            suspensionSprings.Add(suspensionSpring);
        }

        public void CreateRevoluteJoint(out RevoluteJoint joint, BepuEntity connectionA, BepuEntity connectionB, Vector3 anchor, int maxForce, Vector3 freeAxis)
        {
            //Creates joints for such things as camera/laser rotating around the body, or up and down
            joint = new RevoluteJoint(connectionA.body, connectionB.body, anchor, freeAxis);
            joint.Motor.IsActive = true;
            joint.Motor.Settings.Mode = MotorMode.Servomechanism;
            joint.Motor.Settings.MaximumForce = maxForce;
            Game1.Instance.Space.Add(joint);
        }

        public void TurnLeftRight(float turnAngle)
        {
            steeringMotor1.Settings.Servo.Goal = turnAngle;
            steeringMotor2.Settings.Servo.Goal = turnAngle;
        }

        public void MoveForwardBackward(float speed)
        {
            drivingMotor1.Settings.VelocityMotor.GoalVelocity = speed;
            drivingMotor2.Settings.VelocityMotor.GoalVelocity = speed;
            SetMotorActive(true);
        }

        public void SetMotorActive(bool isActive)
        {
            drivingMotor1.IsActive = isActive;
            drivingMotor2.IsActive = isActive;
        }

    }


}
