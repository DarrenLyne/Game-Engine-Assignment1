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
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Vehicle;


namespace GamesAssignmentMars
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MarsRoverArm : BepuEntity
    {
        private Random random = new Random();
        RevoluteJoint hinge;
        RevoluteJoint hinge2;
        RevoluteJoint wheelJoint1;
        RevoluteJoint wheelJoint2;
        RevoluteJoint wheelJoint3;
        RevoluteJoint wheelJoint4;
        
        public override void LoadContent()
        {
            BepuEntity boxBase = new BepuEntity();
            boxBase.modelName = "cube";
            boxBase.localTransform = Matrix.CreateScale(new Vector3(20, 5, 20));
            boxBase.body = new Box(new Vector3(5, 1, 0), 20, 5, 20, 1);
            boxBase.body.BecomeKinematic();
            boxBase.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            Game1.Instance.Space.Add(boxBase.body);

            Game1.Instance.Children.Add(boxBase);

           BepuEntity armBase = new BepuEntity();
           armBase.modelName = "cylinder";
           armBase.localTransform = Matrix.CreateScale(new Vector3(2.5f, 2, 5));
           armBase.body= new Cylinder(new Vector3(15, 2, 20), 2.5f, 2, 1);
           Game1.Instance.Space.Add(armBase.body);

           Game1.Instance.Children.Add(armBase);
           hinge = new RevoluteJoint(boxBase.body, armBase.body, new Vector3(10, 1.5f, 10), new Vector3(0, 1, 0));
           hinge.Motor.IsActive = true;
           hinge.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
           hinge.Motor.Settings.MaximumForce = 3500;

           BepuEntity armPart2 = new BepuEntity();
           armPart2.modelName = "cylinder2";
           armPart2.localTransform = Matrix.CreateScale(new Vector3(2.5f, 2, 5));
           armPart2.body = new Cylinder(new Vector3(15, 2, 31), 2.5f, 2, 1);
           armPart2.body.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(1,0,0),90);
           Game1.Instance.Space.Add(armPart2.body);

           Game1.Instance.Children.Add(armPart2);

           Game1.Instance.Space.Add(hinge);
           hinge2 = new RevoluteJoint(armBase.body, armPart2.body,armBase.pos, new Vector3(1, 0, 0));
           hinge2.Motor.IsActive = true;
           hinge2.Motor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
           hinge2.Motor.Settings.MaximumForce = 3500;
           Game1.Instance.Space.Add(hinge2);

           BepuEntity wheel = new BepuEntity();
           wheel.modelName = "wheel";
           wheel.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
           wheel.body = new Cylinder(new Vector3(35, 5, 31), 2.5f, 0, 1);
           wheel.body.BecomeKinematic();
           wheel.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
           Game1.Instance.Space.Add(wheel.body);

           BepuEntity wheel2 = new BepuEntity();
           wheel2.modelName = "wheel";
           wheel2.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
           wheel2.body = new Cylinder(new Vector3(50, 5, 31), 2.5f, 0, 1);
           wheel2.body.BecomeKinematic();
           wheel2.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
           Game1.Instance.Space.Add(wheel2.body);

           BepuEntity wheel3 = new BepuEntity();
           wheel3.modelName = "wheel";
           wheel3.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
           wheel3.body = new Cylinder(new Vector3(35, 5, 21), 2.5f, 0, 1);
           wheel3.body.BecomeKinematic();
           wheel3.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
           Game1.Instance.Space.Add(wheel3.body);

           BepuEntity wheel4 = new BepuEntity();
           wheel4.modelName = "wheel";
           wheel4.localTransform = Matrix.CreateScale(new Vector3(3, 3, 3));
           wheel4.body = new Cylinder(new Vector3(50, 5, 21), 2.5f, 0, 1);
           wheel4.body.BecomeKinematic();
           wheel4.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
           Game1.Instance.Space.Add(wheel4.body);

           Game1.Instance.Children.Add(wheel);
           Game1.Instance.Children.Add(wheel2);
           Game1.Instance.Children.Add(wheel3);
           Game1.Instance.Children.Add(wheel4);

           BepuEntity boxBase2 = new BepuEntity();
           boxBase2.modelName = "cube";
           boxBase2.localTransform = Matrix.CreateScale(new Vector3(20, 5, 20));
           boxBase2.body = new Box(new Vector3(45, 10, 25), 20, 5, 20, 1);
           //boxBase2.body.BecomeKinematic();
           boxBase2.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
           Game1.Instance.Space.Add(boxBase2.body);
           Game1.Instance.Children.Add(boxBase2);

           wheelJoint1 = new RevoluteJoint(boxBase2.body, wheel.body, new Vector3(35, 5, 31), new Vector3(0, 0, 1));
           Game1.Instance.Space.Add(wheelJoint1);

           wheelJoint2 = new RevoluteJoint(boxBase2.body, wheel2.body, new Vector3(50, 5, 31), new Vector3(0, 0, 1));
           Game1.Instance.Space.Add(wheelJoint2);

           wheelJoint3 = new RevoluteJoint(boxBase2.body, wheel3.body, new Vector3(35, 5, 21), new Vector3(0, 0, 1));
           Game1.Instance.Space.Add(wheelJoint3);

           wheelJoint4 = new RevoluteJoint(boxBase2.body, wheel4.body, new Vector3(50, 5, 21), new Vector3(0, 0, 1));
           Game1.Instance.Space.Add(wheelJoint4);



        }


        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if(keyState.IsKeyDown(Keys.R))
                hinge.Motor.Settings.Servo.Goal += 1 * 0.02f;
            if (keyState.IsKeyDown(Keys.T))
                hinge.Motor.Settings.Servo.Goal -= 1 * 0.02f;

            if (keyState.IsKeyDown(Keys.Y))
                hinge2.Motor.Settings.Servo.Goal += 1 * 0.001f;
            if (keyState.IsKeyDown(Keys.U))
                hinge2.Motor.Settings.Servo.Goal = MathHelper.Max(hinge2.Motor.Settings.Servo.Goal - .5f * 0.02f, hinge2.Limit.MinimumAngle);

            if (keyState.IsKeyDown(Keys.P))
            {
                wheelJoint1.Motor.Settings.Servo.Goal += 1 * 0.02f;
                wheelJoint2.Motor.Settings.Servo.Goal += 1 * 0.02f;
                wheelJoint3.Motor.Settings.Servo.Goal += 1 * 0.02f;
                wheelJoint4.Motor.Settings.Servo.Goal += 1 * 0.02f;
            }
        }
    }
}
