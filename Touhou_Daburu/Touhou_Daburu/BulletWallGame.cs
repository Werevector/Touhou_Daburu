using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;

namespace Touhou_Daburu
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BulletWallGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player mPlayer;

        List<Enemy> mEnemies;
        EnemySpawner mEnemySpawner;

        List<Bullet> mBullets;
        List<int> mBulletDestroyQueue;
        List<int> mPlayerBulletsDestroyQueue;
        BulletSpawner mBulletSpawner;

        List<int> mEnemyDestroyQueue;

        List<Bullet> mPlayerBullets;

        PatternGenerator mPatternGenerator;

        StageSchedule mSchedule;

        SpriteFont font;

        FrameCounter mFrameCounter;

        Texture2D DebugBackground;

        public BulletWallGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            mPlayer = new Player();
            mPlayer.Initalize(new Vector2(700, 150));
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            mEnemySpawner = new EnemySpawner();
            mBulletSpawner = new BulletSpawner();
            mPatternGenerator = new PatternGenerator();

            mEnemies = new List<Enemy>();
            mBullets = new List<Bullet>();
            mPlayerBullets = new List<Bullet>();
            mBulletDestroyQueue = new List<int>();
            mPlayerBulletsDestroyQueue = new List<int>();
            mEnemyDestroyQueue = new List<int>();
            mFrameCounter = new FrameCounter();
            mSchedule = new StageSchedule();
            base.Initialize();

            
            mPlayer.mPlayerBulletsPtr = mPlayerBullets;

            mBulletSpawner.SetBulletList(mBullets);
            mPatternGenerator.SetBulletSpawner(mBulletSpawner);
            mEnemySpawner.SetPatternGenerator(mPatternGenerator);
            mEnemySpawner.SetEnemyListPtr(mEnemies);

            mSchedule.Init("Content/Stages/teststagegenerators.json", mEnemySpawner);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mPlayer.LoadContent(this.Content, "reimu");
            mEnemySpawner.LoadContent(this.Content);
            mBulletSpawner.LoadContent(this.Content);
            font = Content.Load<SpriteFont>("Debug");

            DebugBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
            DebugBackground.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mFrameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.R))
            {
                ResetStage();
            }
            if (state.IsKeyDown(Keys.C))
            {
                ClearStage();
            }


            base.Update(gameTime);

            mSchedule.Update(gameTime);
            mPlayer.Update(gameTime);
            foreach (var bullet in mBullets)
            {
                bullet.Update(gameTime);
            }
            foreach (var bullet in mPlayerBullets)
            {
                bullet.Update(gameTime);
            }
            foreach (var enemy in mEnemies)
            {
                enemy.Update(gameTime);
            }
            CheckCollisions();
            CheckBulletBounds();
            ProccessDestroyQueue();
            
        }

        private void ResetStage()
        {
            mBullets.Clear();
            mEnemies.Clear();
            mSchedule.Init("Content/Stages/teststagegenerators.json", mEnemySpawner);
        }

        private void ClearStage()
        {
            mBullets.Clear();
            mEnemies.Clear();
            mSchedule.Init("Content/Stages/clearstage.json", mEnemySpawner);

        }

        private void CheckBulletBounds()
        {
            Rectangle bound = graphics.GraphicsDevice.Viewport.Bounds;
            int pad = 50;
            bound.X -= pad;
            bound.Y -= pad;
            bound.Width += pad*2;
            bound.Height += pad*2;
            for (int i = 0; i < mBullets.Count; i++)
            {
                if (IsOutsideRect(bound, mBullets[i].mPosition))
                {
                    mBulletDestroyQueue.Add(i);
                }
            }
        }

        private void ProccessDestroyQueue()
        {
            mBulletDestroyQueue.Sort();
            mBulletDestroyQueue.Reverse();
            foreach (var index in mBulletDestroyQueue)
            {
                mBullets.RemoveAt(index);
            }
            mBulletDestroyQueue.Clear();

            mPlayerBulletsDestroyQueue.Sort();
            mPlayerBulletsDestroyQueue.Reverse();
            foreach (var index in mPlayerBulletsDestroyQueue)
            {
                mPlayerBullets.RemoveAt(index);
            }
            mPlayerBulletsDestroyQueue.Clear();

            mEnemyDestroyQueue.Sort();
            mEnemyDestroyQueue.Reverse();
            //List<int> unique = mEnemyDestroyQueue.Distinct().ToList();
            foreach (var index in mEnemyDestroyQueue)
            {
                mEnemies.RemoveAt(index);
            }
            mEnemyDestroyQueue.Clear();
        }

        private void CheckCollisions()
        {
            foreach (var bullet in mBullets)
            {
                if (bullet.mHitBox.Intersects(mPlayer.GetHitbox()))
                {
                    mBulletDestroyQueue.Add(mBullets.IndexOf(bullet));
                }
            }
            foreach (var bullet in mPlayerBullets)
            {
                foreach(var enemy in mEnemies)
                {
                    if (bullet.mHitBox.Intersects(enemy.GetHitbox()))
                    {
                        mEnemyDestroyQueue.Add(mEnemies.IndexOf(enemy));
                        mPlayerBulletsDestroyQueue.Add(mPlayerBullets.IndexOf(bullet));
                    }
                }
                
            }
        }

        private bool IsOutsideRect(Rectangle r, Vector2 p)
        {
            return p.X > r.X + r.Width || p.Y > r.Y + r.Height || p.X < r.X || p.Y < r.Y;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //for (int j = 0; j < 1920; j += 100)
            //{
            //    Utility.DrawLine(spriteBatch, DebugBackground, new Vector2(j, 0), new Vector2(j, 1080), Color.White);
            //}
            //for (int i = 0; i < 1080; i += 100)
            //{
            //    Utility.DrawLine(spriteBatch, DebugBackground, new Vector2(0, i), new Vector2(1920, i), Color.White);
            //}

            mPlayer.Draw(spriteBatch);
            foreach (var bullet in mBullets)
            {
                bullet.Draw(spriteBatch);
            }
            mPlayer.Draw(spriteBatch);
            foreach (var bullet in mPlayerBullets)
            {
                bullet.Draw(spriteBatch);
            }
            foreach (var enemy in mEnemies)
            {
                enemy.Draw(spriteBatch);
            }

            Utility.DrawRectangle(spriteBatch, DebugBackground, new Vector2(0, 0), new Rectangle(0, 0, 400, 200), Color.Black);
            spriteBatch.Draw(DebugBackground, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(font, "Bullets: " + mBullets.Count, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(font, "FrameTime: " + gameTime.ElapsedGameTime.TotalMilliseconds, new Vector2(0, 20), Color.White);
            //spriteBatch.DrawString(font, "FPS: " + mFrameCounter.CurrentFramesPerSecond, new Vector2(0, 40), Color.Red);
            //spriteBatch.DrawString(font, "Average FPS: " + mFrameCounter.AverageFramesPerSecond, new Vector2(0, 60), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
