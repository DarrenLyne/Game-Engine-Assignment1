using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework.Audio;

namespace GamesAssignmentMars
{
    //created this class and the code myself
    public class MarsRover: BepuEntity
    {
        public BepuEntity cameraLaserContainer;//camera & laser container
        BepuEntity roverBase;//base body for mars rover
        BepuEntity cameraArm;//cylinder that camera & laser wil be on top of
        BepuEntity drillArm;//cylinder that willhold the drill

        private float maximumTurnAngle = MathHelper.Pi * .2f;
        private float driveSpeed = 1000;
        float fireRate = 1.0f;
        float elapsed = 1000.0f; // Set this to a high number so it lets me start firing straight away...
        public bool exploded = false;// holds the state of if the rover has exploded or not5
        MarsRoverMovement roverMovement = new MarsRoverMovement();// creates the aspects of the rovers movement
        MarsRoverBody marsRoverBody = new MarsRoverBody();//Creates the body of the mars rover
        private SoundEffect laserShot;
        private SoundEffect explosionSound;

        public MarsRover(): base()
        {
            laserShot = Game1.Instance.Content.Load<SoundEffect>("lasershot");
            explosionSound = Game1.Instance.Content.Load<SoundEffect>("explosion-01");
            cameraArm = new BepuEntity();
            drillArm = new BepuEntity();

            //construct mars rover ase body
            roverBase = marsRoverBody.AddBox(new Vector3(260, 45, -260));

            //create arms to hold both drill and camera/laser
            drillArm = marsRoverBody.AddCylinder("cylinder", new Vector3(267, 44, -244), false);
            cameraArm = marsRoverBody.AddCylinder("cylinder", new Vector3(254, 54, -252), true);

            //Create joint between rover body and drill arm
            roverMovement.CreateRevoluteJoint(out roverMovement.bodyDrillJoint, roverBase, drillArm, new Vector3(267, 44, -250), 3500, Vector3.Right);
            roverMovement.roverJoints.Add(roverMovement.bodyDrillJoint);

            //Create joint between rover body and camera/laser arm
            roverMovement.CreateRevoluteJoint(out roverMovement.bodyCameraCylinderJoint, roverBase, cameraArm, cameraArm.body.Position, 1500, Vector3.Up);
            roverMovement.roverJoints.Add(roverMovement.bodyCameraCylinderJoint);

            //create container for camera/laser
            cameraLaserContainer = marsRoverBody.AddCylinder("cube", new Vector3(254, 61, -252), true);

            //Create joint between camera/laser arm and container for camera/laser
            roverMovement.CreateRevoluteJoint(out roverMovement.cameraLaserContainerXaxisRotation, cameraArm, cameraLaserContainer, cameraLaserContainer.body.Position, 2500, -Vector3.Right);
            roverMovement.roverJoints.Add(roverMovement.cameraLaserContainerXaxisRotation);

            //create the four back wheels of the rover and connect them to the body
            Entity backWheel1 = marsRoverBody.AddWheel(new Vector3(250, 42, -267), roverBase.body);
            roverMovement.ConnectWheelBody(backWheel1, roverBase);
            Entity backWheel2 = marsRoverBody.AddWheel(new Vector3(270, 42, -267), roverBase.body);
            roverMovement.ConnectWheelBody(backWheel2, roverBase);
            Entity backWheel3 = marsRoverBody.AddWheel(new Vector3(250, 42, -260), roverBase.body);
            roverMovement.ConnectWheelBody(backWheel3, roverBase);
            Entity backWheel4 = marsRoverBody.AddWheel(new Vector3(270, 42, -260), roverBase.body);
            roverMovement.ConnectWheelBody(backWheel4, roverBase);

            //Create the two driving wheels and connect them to the body
            var wheel1 = marsRoverBody.AddWheel(new Vector3(270, 42, -255), roverBase.body);
            roverMovement.SetUpFrontMotorWheels(roverBase, wheel1, out roverMovement.drivingMotor1, out roverMovement.steeringMotor1);
            var wheel2 = marsRoverBody.AddWheel(new Vector3(250, 42, -255), roverBase.body);
            roverMovement.SetUpFrontMotorWheels(roverBase, wheel2, out roverMovement.drivingMotor2, out roverMovement.steeringMotor2);

            var steeringStabilizer = new RevoluteAngularJoint(wheel1, wheel2, Vector3.Right);
            Game1.Instance.Space.Add(steeringStabilizer);
        }

        public override void LoadContent()
        {
        }


        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (!Game1.Instance.Camera.isRoverCamera)
            {
                if (keyState.IsKeyDown(Keys.B))
                {
                    //turn the container for camera laser left
                    roverMovement.RotationCameraLaserYAxis(0.01f);
                }
                if (keyState.IsKeyDown(Keys.M))
                {
                    //turn the container for camera laser right
                    roverMovement.RotationCameraLaserYAxis(-0.01f);
                }
                if (keyState.IsKeyDown(Keys.H))
                {
                    // So the camera/laser can only look up a certain amount
                    if (roverMovement.cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal >= -maximumTurnAngle)
                    {
                        roverMovement.RotationXAxis(-0.01f, right);
                    }
                }
                if (keyState.IsKeyDown(Keys.N))
                {
                    // So the camera/laser can only look down a certain amount
                    if (roverMovement.cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal <= maximumTurnAngle)
                    {
                        roverMovement.RotationXAxis(0.01f, right);
                    }
                }
            }

            if(keyState.IsKeyDown(Keys.F))
            {
                //the drill cylinder looks up
                SetDrillOrientation(-0.01f);
            }
            if (keyState.IsKeyDown(Keys.V))
            { 
                //the drill cylinder looks down
                SetDrillOrientation(0.01f);
            }

            if(keyState.IsKeyDown(Keys.Space))
            {
                //create the laser to shoot & add to game
                Lazer laser = new Lazer();
                laserShot.Play();
                laser.pos = cameraLaserContainer.body.Position;
                laser.look = GetLaserLook();
                Game1.Instance.Children.Add(laser);
            }

            //code taken from bepu physics demo suspensioncardemo.cs and adapted for this project
            roverMovement.steeringMotor1.Settings.Servo.BaseCorrectiveSpeed = 3 + 7 * Math.Min(roverMovement.steeringMotor1.ConnectionB.AngularVelocity.Length() / 100, 1);
            roverMovement.steeringMotor2.Settings.Servo.BaseCorrectiveSpeed = 3 + 7 * Math.Min(roverMovement.steeringMotor2.ConnectionB.AngularVelocity.Length() / 100, 1);

            if (keyState.IsKeyDown(Keys.Down))
            {
                //move rover backward
                roverMovement.MoveForwardBackward(-driveSpeed);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    SetRoverCamera(0);
                }
            }
            else if (keyState.IsKeyDown(Keys.Up))
            {
                //move rover forward
                roverMovement.MoveForwardBackward(driveSpeed);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    SetRoverCamera(0);
                }
            }
            else
            {
                //turn driving motor off
                roverMovement.SetMotorActive(false);
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                //turn rover left
                roverMovement.TurnLeftRight(maximumTurnAngle);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    SetRoverCamera(0.01f);
                }
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                //turn rover right
                roverMovement.TurnLeftRight(-maximumTurnAngle);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    SetRoverCamera(-0.01f);
                }
            }
            else
            {
                //Face forward
                roverMovement.steeringMotor1.Settings.Servo.Goal = 0;
                roverMovement.steeringMotor2.Settings.Servo.Goal = 0;
            }

            if (keyState.IsKeyDown(Keys.P))
            {
                if (elapsed > (1.0f / fireRate))
                {
                    //Change to or out of the rovers camera
                    if (Game1.Instance.Camera.isRoverCamera == false)
                    {
                        //set look vector and position of camera and if it is rover camera, changes controls if it is
                        ChangeCameraBeingUsed(GetCameraRoverPosition(), GetLaserLook(), true);
                    }
                    else
                    {
                        //put camera back to normal
                        ChangeCameraBeingUsed(new Vector3(248, 64, -200), new Vector3(0.0f, 0.0f, -1.0f),false);
                    }
                    elapsed = 0.0f;
                }
            }
            elapsed += (float) gameTime.ElapsedGameTime.TotalSeconds;//used so cant chnge between normal camera and rover camera too quickly
            if (elapsed >= 25.0f)
            {
                elapsed = 25.0f;
            }
            if (keyState.IsKeyDown(Keys.E))
            {
                //set the rover to explode
                if (!exploded)
                {
                    Explosion createExplosion = new Explosion(Vector3.Zero, 2000, 30, Game1.Instance.Space);
                    createExplosion.Position = roverBase.body.Position;

                    //disable all the joints
                    foreach (RevoluteJoint joint in roverMovement.roverJoints)
                        joint.IsActive = false;
                    foreach (var suspensionString in roverMovement.suspensionSprings)
                        suspensionString.IsActive = false;
                    foreach (var joint in roverMovement.joints)
                        joint.IsActive = false;
                    foreach (var motor in roverMovement.motors)
                        motor.IsActive = false;

                    //start the explosion
                    createExplosion.Explode();
                    explosionSound.Play();
                    exploded = true;
                }
            }

        }

        private Vector3 GetCameraRoverPosition()
        {
            return cameraLaserContainer.body.Position + (GetLaserLook() * 3);
        }

        private static void ChangeCameraBeingUsed(Vector3 postion,Vector3 look, bool isRoverCamera)
        {
            Game1.Instance.Camera.pos = postion;
            Game1.Instance.Camera.look = look;
            Game1.Instance.Camera.isRoverCamera = isRoverCamera;
        }

        private void SetRoverCamera(float angle)
        {
            Game1.Instance.Camera.RotateAroundYAxis(angle);
            Game1.Instance.Camera.look = GetLaserLook();
            Game1.Instance.Camera.pos = GetCameraRoverPosition();
        }

        private void SetDrillOrientation(float angle)
        {
            roverMovement.bodyDrillJoint.Motor.Settings.Servo.Goal += 1 * angle;
        }
        public Vector3 GetLaserLook()
        {
            return roverMovement.laserLook;
        }
        public void SetLaserLook(Vector3 value)
        {
            roverMovement.laserLook = value;
        }

        public void SetCameraLaserContainerXaxisRotation(float value)
        {
            roverMovement.cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal += value;
        }

        public void SetbodyCameraCylinderJoint(float value)
        {
             roverMovement.bodyCameraCylinderJoint.Motor.Settings.Servo.Goal += value;
        }
        public bool IfMaxMovementAngleUp()
        {
            if (roverMovement.cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal >= -(roverMovement.maximumTurnAngle))
                return true;
            else
                return false;
        }
        public bool IfMaxMovementAngleDown()
        {
            if (roverMovement.cameraLaserContainerXaxisRotation.Motor.Settings.Servo.Goal <= (roverMovement.maximumTurnAngle))
                return true;
            else
                return false;
        }
    }
}
