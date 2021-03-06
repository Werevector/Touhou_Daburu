﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Touhou_Daburu
{
    class Player
    {

        /// <summary>
        /// Structures for storing JSON data specific to the player, ignored by the rest of the program.
        /// </summary>
        private struct _PlayerInfo
        {
            public String Name { get; set; }
            public String Atlas { get; set; }
            public List<_SequenceInfo> Sequences { get; set; }
            public int Speed { get; set; }

        }
        private struct _SequenceInfo
        {
            public String Key       { get; set; }
            public bool Looping     { get; set; }
            public int SubLoop      { get; set; }
            public List<int> Seq    { get; set; }
        }
        private struct _AtlasInfo
        {
            public string Image { get; set; }
            public List<_ClipSetInfo> ClipSets { get; set; }
        }
        private struct _ClipSetInfo
        {
            public String Key { get; set; }
            public List<List<int>> Set { get; set; }
        }
        enum PState
        {
            idle,
            moveRight,
            moveLeft
        }

        String  mName;
        Vector2 mPosition;
        float   mSpeed;
        int     mLives;
        int     mBombs;
        int     mScore;
        PState  mPlayerState;
        Rectangle mHitbox;
        double mTick;

        public List<Bullet> mPlayerBulletsPtr;

        public TextureAtlas mPlayerAtlas { get; set; }
        private Dictionary<String,SpriteSequence> mSequenceMap;
        public SpriteSequence mCurrentSequence { get; set; }



        public void Initalize(Vector2 pos)
        {
            mCurrentSequence = new SpriteSequence();
            mSequenceMap = new Dictionary<string, SpriteSequence>();


            mPosition = pos;
            mLives = 3;
            mBombs = 3;
            mScore = 0;
            mSpeed = 5;
            mPlayerState = PState.idle;

            mHitbox = new Rectangle(0, 0, 4, 4);
            mTick = 0.0;
        }

        public void LoadContent(ContentManager content, string girl)
        {
            string pJson = File.ReadAllText("Content/Descriptors/Objects/Player/"+girl+".json");
            _PlayerInfo pInfo = JsonConvert.DeserializeObject<_PlayerInfo>(pJson);
            mName = pInfo.Name;
            mSpeed = pInfo.Speed;

            //Copy over sequences into map
            foreach (var seq in pInfo.Sequences)
            {
                SpriteSequence s = new SpriteSequence();
                s.mSequence = seq.Seq;
                s.Looping = seq.Looping;
                s.mSubLoop = seq.SubLoop;
                mSequenceMap.Add(seq.Key, s);
            }

            //Load the Atlas
            string aJson = File.ReadAllText("Content/Descriptors/Images/Player/" + pInfo.Atlas);
            _AtlasInfo aInfo = JsonConvert.DeserializeObject<_AtlasInfo>(aJson);

            Texture2D image = content.Load<Texture2D>("Images/Player/" + aInfo.Image);

            Dictionary<String, List<Rectangle>> setMap = new Dictionary<String, List<Rectangle>>();
            foreach (var set in aInfo.ClipSets)
            {
                List<Rectangle> clips = new List<Rectangle>();
                foreach (var clip in set.Set)
                {
                    clips.Add(new Rectangle(clip[0], clip[1], clip[2], clip[3]));
                }
                setMap.Add(set.Key, clips);
            }
            mPlayerAtlas = new TextureAtlas(image, setMap);
            
        }

        public void Update(GameTime gameTime)
        {


            mTick += gameTime.ElapsedGameTime.TotalSeconds;

            mCurrentSequence = mSequenceMap["Idle"];
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Up))
            {
                mPosition.Y -= mSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                mPosition.Y += mSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                mPosition.X -= mSpeed;
                mCurrentSequence = mSequenceMap["MoveLeft"];
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                mPosition.X += mSpeed;
                mCurrentSequence = mSequenceMap["MoveRight"];
            }

            if (keyboard.IsKeyDown(Keys.Z))
            {
                double t = 1 / (double)20;
                if (mTick > t)
                {
                    Fire();
                    mTick = 0.0;
                }
            }

            mHitbox.X = (int)mPosition.X-mHitbox.Width/2;
            mHitbox.Y = (int)mPosition.Y-mHitbox.Height/2;
            mCurrentSequence.Update();
        }

        private void Fire()
        {
            Bullet bullet = new Bullet();
            bullet.mAtlas = mPlayerAtlas;
            bullet.mBulletType = "Shot";
            bullet.mBulletColor = 0;
            bullet.mPosition = new Vector2(mPosition.X - 10, mPosition.Y);
            bullet.mVelocity = new Vector2(0,-30);
            bullet.mAcceleration = new Vector2(0,0);
            bullet.mHitBox = new Rectangle(0,0,10,20);
            bullet.mDirectional = true;
            bullet.mAllied = true;
            mPlayerBulletsPtr.Add(bullet);

            Bullet bullet2 = new Bullet();
            bullet2.mAtlas = mPlayerAtlas;
            bullet2.mBulletType = "Shot";
            bullet2.mBulletColor = 0;
            bullet2.mPosition = new Vector2(mPosition.X + 10, mPosition.Y); ;
            bullet2.mVelocity = new Vector2(0, -30);
            bullet2.mAcceleration = new Vector2(0, 0);
            bullet2.mHitBox = new Rectangle(0, 0, 11, 56);
            bullet2.mDirectional = true;
            bullet2.mAllied = true;
            mPlayerBulletsPtr.Add(bullet2);
        }

        public void Draw(SpriteBatch sb)
        {
            mPlayerAtlas.Draw(sb, "Character", mCurrentSequence.GetCurrentIndex(), mPosition);
        }

        public Rectangle GetHitbox()
        {
            return mHitbox;
        }
    }
}
