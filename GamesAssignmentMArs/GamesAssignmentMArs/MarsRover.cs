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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GamesAssignmentMars
{
    public class MarsRover: BepuEntity
    {
        public BepuEntity cameraLaserContainer;//camera & laser container
        BepuEntity roverBody;//base body for mars rover
        BepuEntity cameraArm;//cylinder that camera & laser wil be on top of
        BepuEntity drillArm;//cylinder that willhold the drill

        private float maximumTurnAngle = MathHelper.Pi * .2f;
        private float driveSpeed = 1000;
        float fireRate = 1.0f;
        float elapsed = 1000.0f; // Set this to a high number so it lets me start firing straight away...
        public bool exploded = false;// holds the state of if the rover has exploded or not5
        MarsRoverMovement roverMovement = new MarsRoverMovement();
        private SoundEffect laserShot;
        private SoundEffect explosionSound;


        public MarsRover(): base()
        {
            laserShot = Game1.Instance.Content.Load<SoundEffect>("lasershot");
            explosionSound = Game1.Instance.Content.Load<SoundEffect>("explosion-01");
            cameraArm = new BepuEntity();
            drillArm = new BepuEntity();

            //construct mars rover ase body
            roverBody = new BepuEntity();
            roverBody.modelName = "cube";
            roverBody.LoadContent();
            roverBody.localTransform = Matrix.CreateScale(new Vector3(15, 5, 20));
            roverBody.body = new Box(new Vector3(260, 45, -260), 15, 5, 20);
            roverBody.body.BecomeDynamic(100);
            roverBody.body.CollisionInformation.LocalPosition = new Vector3(0, .8f, 0);
            roverBody.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            Game1.Instance.Space.Add(roverBody.body);
            Game1.Instance.Children.Add(roverBody);

            //create arms to hold both drill and camera/laser
            drillArm = AddCylinder("cylinder",new Vector3(267, 44, -244), false);
            cameraArm = AddCylinder("cylinder",new Vector3(254, 54, -252), true);

            //Create joint between rover body and drill arm
            roverMovement.CreateRevoluteJoint(out roverMovement.bodyDrillJoint, roverBody, drillArm, new Vector3(267, 44, -250), 3500, Vector3.Right);
            roverMovement.roverJoints.Add(roverMovement.bodyDrillJoint);

            //Create joint between rover body and camera/laser arm
            roverMovement.CreateRevoluteJoint(out roverMovement.bodyCameraCylinderJoint, roverBody, cameraArm, cameraArm.body.Position, 1500, Vector3.Up);
            roverMovement.roverJoints.Add(roverMovement.bodyCameraCylinderJoint);

            //create container for camera/laser
            cameraLaserContainer=AddCylinder("cube",new Vector3(254, 61, -252), true);

            //Create joint between camera/laser arm and container for camera/laser
            roverMovement.CreateRevoluteJoint(out roverMovement.cameraLaserContainerXaxisRotation, cameraArm, cameraLaserContainer, cameraLaserContainer.body.Position, 2500, -Vector3.Right);
            roverMovement.roverJoints.Add(roverMovement.cameraLaserContainerXaxisRotation);

            //create the four back wheels of the rover and connect them to the body
            Entity backWheel1 = AddWheel(new Vector3(250, 42, -267), roverBody.body);
            roverMovement.ConnectWheelBody(backWheel1, roverBody);
            Entity backWheel2 = AddWheel(new Vector3(270, 42, -267), roverBody.body);
            roverMovement.ConnectWheelBody(backWheel2, roverBody);
            Entity backWheel3 = AddWheel(new Vector3(250, 42, -260), roverBody.body);
            roverMovement.ConnectWheelBody(backWheel3,roverBody);
            Entity backWheel4 = AddWheel(new Vector3(270, 42, -260), roverBody.body);
            roverMovement.ConnectWheelBody(backWheel4, roverBody);

            //Create the two driving wheels and connect them to the body
            var wheel1 = AddWheel(new Vector3(270, 42, -255), roverBody.body);
            roverMovement.SetUpFrontMotorWheels(roverBody,wheel1, out roverMovement.drivingMotor1, out roverMovement.steeringMotor1);
            var wheel2 = AddWheel(new Vector3(250, 42, -255), roverBody.body);
            roverMovement.SetUpFrontMotorWheels(roverBody,wheel2, out roverMovement.drivingMotor2, out roverMovement.steeringMotor2);

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
                    roverMovement.RotationCameraLaserYAxis(-0.01f);
                }
                if (keyState.IsKeyDown(Keys.M))
                {
                    roverMovement.RotationCameraLaserYAxis(0.01f);
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
                roverMovement.bodyDrillJoint.Motor.Settings.Servo.Goal -= 1 * 0.01f;
            }
            if (keyState.IsKeyDown(Keys.V))
            {   //the drill cylinder looks down
                roverMovement.bodyDrillJoint.Motor.Settings.Servo.Goal += 1 * 0.01f;
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
                    Game1.Instance.Camera.look = GetLaserLook();
                    Game1.Instance.Camera.pos = new Vector3(cameraLaserContainer.body.Position.X, cameraLaserContainer.body.Position.Y, cameraLaserContainer.body.Position.Z) + (GetLaserLook() * 3);
                }
            }
            else if (keyState.IsKeyDown(Keys.Up))
            {
                //move rover forward
                roverMovement.MoveForwardBackward(driveSpeed);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    Game1.Instance.Camera.look = GetLaserLook();
                    Game1.Instance.Camera.pos = new Vector3(cameraLaserContainer.body.Position.X, cameraLaserContainer.body.Position.Y, cameraLaserContainer.body.Position.Z) + (GetLaserLook() * 3);
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
                    Game1.Instance.Camera.RotateAroundYAxis(0.01f);
                    Game1.Instance.Camera.look = GetLaserLook();
                    Game1.Instance.Camera.pos = new Vector3(cameraLaserContainer.body.Position.X, cameraLaserContainer.body.Position.Y, cameraLaserContainer.body.Position.Z) + (GetLaserLook() * 3);
                }
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                //turn rover right
                roverMovement.TurnLeftRight(-maximumTurnAngle);
                if (Game1.Instance.Camera.isRoverCamera)
                {
                    Game1.Instance.Camera.RotateAroundYAxis(-0.01f);
                    Game1.Instance.Camera.look = GetLaserLook();
                    Game1.Instance.Camera.pos = new Vector3(cameraLaserContainer.body.Position.X, cameraLaserContainer.body.Position.Y, cameraLaserContainer.body.Position.Z) + (GetLaserLook() * 3);
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
                        Game1.Instance.Camera.look = GetLaserLook();
                        Game1.Instance.Camera.pos = new Vector3(cameraLaserContainer.body.Position.X, cameraLaserContainer.body.Position.Y, cameraLaserContainer.body.Position.Z) + (GetLaserLook() * 3);
                        Game1.Instance.Camera.isRoverCamera = true;
                    }
                    else
                    {
                        //put camera back to normal
                        Game1.Instance.Camera.pos = new Vector3(248, 64, -200);
                        Game1.Instance.Camera.look = new Vector3(0.0f, 0.0f, -1.0f);
                        Game1.Instance.Camera.isRoverCamera = false;
                    }
                    elapsed = 0.0f;
                }
            }
            elapsed += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= 25.0f)
            {
                elapsed = 25.0f;
            }
            if (keyState.IsKeyDown(Keys.E))
            {
                //set the rover to explode
                if (!exploded)
                {
                    Explosion createExplosion = new Explosion(Vector3.Zero, 4000, 30, Game1.Instance.Space);
                    createExplosion.Position = roverBody.body.Position;

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

        Entity AddWheel(Vector3 wheelPosition,Entity baseBody)
        {
            var wheel = new BepuEntity();
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

        BepuEntity AddCylinder(string modelName,Vector3 Postion, bool Orientation)
        {
            BepuEntity cylinder= new BepuEntity();
            cylinder.modelName = modelName;
            cylinder.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
            cylinder.body = new Cylinder(Postion, 3, 3, 3);
            cylinder.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            if(Orientation)//if cylinder needs to be rotated
                cylinder.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2);
            Game1.Instance.Space.Add(cylinder.body);
            Game1.Instance.Children.Add(cylinder);

            return cylinder;
        }

        public Vector3 GetLaserLook()
        {
            return roverMovement.laserLook;
        }
        public void SetLaserLook(Vector3 value)
        {
            roverMovement.laserLook = value;
        }

        public Vector3 test()
        {
            return roverMovement.laserLook;
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
