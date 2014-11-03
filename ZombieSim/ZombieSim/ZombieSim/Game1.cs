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

namespace ZombieSim
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Vars
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Collision collison;
        private List<Creature> creaturesAlive;
        private int maxCreatures;
        private int numOfCreatures;
        private Random rand;

        private Texture2D aliveSprite;
        private Texture2D heroSprite;
        private Texture2D deadSprite;
        private Texture2D bittenSprite;
        private Texture2D zombieSprite;
        private Texture2D backGround;
        private Rectangle drawRect;

        private int screenHeight;
        private int screenWidth;
        private Vector2 gameBounds;

        public long currentTime { get; set; }
        public long elapsedTime { get; set; }
        public double eTime { get; set; }
        public double cTime { get; set; }
        public SpriteFont Font1 { get; set; }

        //Events
        public delegate void UpdateHandler(GameTime gameTime);
        public delegate void DrawHandler(SpriteBatch spriteBatch);
        public static event UpdateHandler onGameUpdate;
        public static event DrawHandler onDrawUpdate;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 980;
            graphics.PreferredBackBufferWidth = 1820;
            IsFixedTimeStep = false;
        }

        private void GameUpdate(GameTime gameTime)
        {
            UpdateHandler handler = onGameUpdate;
            if(handler != null)
            {
                handler(gameTime);
            }
        }
        
        private void DrawUpdate(SpriteBatch batch)
        {
            DrawHandler handler = onDrawUpdate;
            if (handler != null)
            {
                handler(batch);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aliveSprite = Content.Load<Texture2D>("AliveSprite");
            heroSprite = Content.Load<Texture2D>("Hero");
            deadSprite = Content.Load<Texture2D>("DeadSprite");
            bittenSprite = Content.Load<Texture2D>("Bitten");
            zombieSprite = Content.Load<Texture2D>("ZombieSprite");
            backGround = Content.Load<Texture2D>("ZombieSimBackGround");

            // TODO: use this.Content to load your game content here
            screenWidth = Window.ClientBounds.Width;
            screenHeight = Window.ClientBounds.Height;
            maxCreatures = 10000;
            creaturesAlive = new List<Creature>();
            collison = new Collision(screenHeight, screenWidth, 100);
            rand = new Random();
            gameBounds = new Vector2(collison.widthBounds, collison.heightBounds);
            drawRect.Width = screenWidth;
            drawRect.Height = screenHeight;


            Creature tempCreature = new Creature(aliveSprite, heroSprite, deadSprite, bittenSprite, zombieSprite, screenHeight, screenWidth, collison, rand, gameBounds, 1);
            tempCreature.currentState = Creature.State.Zombie;
            creaturesAlive.Add(tempCreature);

            int collisionTimer = 1;
            //Create some basic creatures
            for (int i = 0; i < (maxCreatures-1); i++)
            {
                if (collisionTimer >= 4)
                    collisionTimer = 0;

                collisionTimer++;
                creaturesAlive.Add(new Creature(aliveSprite, heroSprite, deadSprite, bittenSprite, zombieSprite, screenHeight, screenWidth, collison, rand, gameBounds, collisionTimer));
            }
            numOfCreatures = creaturesAlive.Count;
        }

        protected override void Update(GameTime gameTime)
        {
            currentTime = gameTime.TotalGameTime.Ticks;
            elapsedTime = gameTime.ElapsedGameTime.Ticks;
            eTime = (double)elapsedTime / (double)TimeSpan.TicksPerSecond;
            cTime = (double)currentTime / (double)TimeSpan.TicksPerSecond;

            GetInput(gameTime);
            GameUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Snow);

            spriteBatch.Begin();
            spriteBatch.Draw(backGround, drawRect, Color.White);
            DrawUpdate(spriteBatch);
            //double fps = 1.0 / eTime;
            //spriteBatch.DrawString(Font1, "fps " + fps.ToString("f6"), new Vector2(10, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void GetInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }
    }
}
