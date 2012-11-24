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
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;

namespace GamesAssignmentMars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
         static Game1 instance = null;
        GraphicsDeviceManager graphics;

        Random random = new Random();
        Space space;
        MarsTerrain terrain;
        public Space Space
        {
            get { return space; }
            set { space = value; }
        }
        Cylinder cameraCylindar;

        float lastFired = 1.0f;

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }
        SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch1
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        private Ground ground = null;

        public Ground Ground
        {
            get { return ground; }
            set { ground = value; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        private Camera camera;
        List<GameEntity> children = new List<GameEntity>();

        public List<GameEntity> Children
        {
            get { return children; }
            set { children = value; }
        }

        public static Game1 Instance
        {
            get { return Game1.instance; }
            set { Game1.instance = value; }
        }

        public Game1()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera();
            int midX = GraphicsDeviceManager.DefaultBackBufferHeight / 2;
            int midY = GraphicsDeviceManager.DefaultBackBufferWidth / 2;
            Mouse.SetPosition(midX, midY);

                                   
            children.Add(camera);
            ground = new Ground();                        
            children.Add(ground);

            base.Initialize();
        }


        //void createTower()
       // {
        //    for (float y = 100; y > 20; y -= 5)
         //   {
         //       createCube(new Vector3(0, y, 0), 4, 4, 10);
        // //   }
        //}

       //void createWall()
        //{
        //    for (float z = -20; z < 20; z += 5)
        //    {
         //       for (float y = 60; y > 0; y -= 5)
         //       {
         //           createCube(new Vector3(-20, y, z), 4, 4, 4);
         //      }
         //   }
       // }

        void fireBall()
        {
            BepuEntity ball = new BepuEntity();
            ball.modelName = "sphere";
            float size = 1;
            ball.localTransform = Matrix.CreateScale(new Vector3(size, size, size));
            ball.body = new Sphere(Camera.pos + (Camera.look * 5), size, size);
            ball.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            space.Add(ball.body);
            ball.LoadContent();
            ball.body.ApplyImpulse(Vector3.Zero, Camera.look * 50);
            children.Add(ball);

        }

       // void createCube(Vector3 position, float width, float height, float length)
       // {
        //    BepuEntity theBox = new BepuEntity();
       //     theBox.modelName = "cube";
        //    theBox.localTransform = Matrix.CreateScale(new Vector3(width, height, length));
        //    theBox.body = new Box(position, width, height, length, 1);
        //    theBox.diffuse = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
         //   space.Add(theBox.body);
         //   children.Add(theBox);
       // }
        MarsRoverArm arm;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>       
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            Box groundBox = new Box(Vector3.Zero, ground.width, 0.1f, ground.height);
            space.Add(groundBox);

            arm = new MarsRoverArm();
            arm.LoadContent();

            cameraCylindar = new Cylinder(Camera.pos, 5, 5);
            space.Add(cameraCylindar);
            terrain = new MarsTerrain();
            children.Add(terrain);

            //createTower();
            //createWall();

            foreach (GameEntity child in children)
            {
                child.LoadContent();
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (GameEntity child in children)
            {
                child.UnloadContent();
            }

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (mouseState.LeftButton == ButtonState.Pressed & lastFired > 0.25f)
            {
                fireBall();
                lastFired = 0.0f;
            }
            lastFired += timeDelta;

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Update(gameTime);
            }

            arm.Update(gameTime);
            cameraCylindar.Position = camera.pos;
            space.Update(timeDelta);

            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (GameEntity child in children)
            {
                DepthStencilState state = new DepthStencilState();
                state.DepthBufferEnable = true;                
                GraphicsDevice.DepthStencilState = state;
                child.Draw(gameTime);
            }
            // Draw any lines
            Line.Draw();

            spriteBatch.End();            
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return graphics;
            }
        }
    }
}
