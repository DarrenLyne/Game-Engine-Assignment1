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
    //Majority of Code was in starter code for this assignment but have added new parts
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        static Game1 instance = null;
        GraphicsDeviceManager graphics;

        Random random = new Random();
        Space space;
        float lastFired = 1.0f;
        SpriteBatch spriteBatch;
        private Camera camera;
        List<GameEntity> children = new List<GameEntity>();
        MarsRover rover;
        MarsTerrain terrain;

        public Space Space
        {
            get { return space; }
            set { space = value; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

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

        public MarsRover Rover
        {
            get { return rover; }
            set { rover = value; }
        }

        public MarsTerrain Terrain
        {
            get { return terrain; }
            set { terrain = value; }
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

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -3.711f, 0);//-3.711 is the gravity for mars

            children.Add(new StarsSphere());//Create the stars for the sky
            terrain = new MarsTerrain();//Create the terrain
            children.Add(terrain);
            rover = new MarsRover();//Create the rover
            children.Add(rover);

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
            if(keyState.IsKeyDown(Keys.R))//Suppose to reset game, not working correctly
            {
                rover.UnloadContent();
                children.Remove(rover);
                rover = new MarsRover();
                children.Add(rover);
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
            Line.Draw(); Console.WriteLine(camera.look.X);


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
    }
}
