using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Touhou_Daburu
{
    class Bullet
    {
        public Texture2D mTexture;
        public TextureAtlas mAtlas;
        public string mBulletType;
        public int mBulletColor;
        public Vector2 mPosition;
        public Vector2 mVelocity;
        public Vector2 mAcceleration;
        public Rectangle mHitBox;
        public bool mDirectional;
        private float mAngle;
        public bool mAllied;

        public Bullet()
        {
            mPosition = new Vector2(0, 0);
            mVelocity = new Vector2(0, 0);
            mAcceleration = new Vector2(0, 0);
            mHitBox = new Rectangle(0, 0, 0, 0);
            mDirectional = false;
            mAngle = 0;
            mAllied = false;
        }

        public Bullet(Texture2D tex, Vector2 pos, Vector2 vel, Vector2 acc, Rectangle hit)
        {
            mTexture = tex;
            mPosition = pos;
            mVelocity = vel;
            mAcceleration = acc;
            mHitBox = hit;
        }

        public void Update(GameTime gameTime)
        {
            mVelocity.X += mAcceleration.X;
            mVelocity.Y += mAcceleration.Y;

            mPosition.X += mVelocity.X;
            mPosition.Y += mVelocity.Y;

            mHitBox.X = (int)mPosition.X - mHitBox.Width / 2;
            mHitBox.Y = (int)mPosition.Y - mHitBox.Height / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Manually center sprite since we dont use TextureAtlas, but a raw texture.
            if (mDirectional)
            {
                //mAngle = (float)Math.Atan2(mVelocity.Y, mVelocity.X) + (float)(90*Math.PI/180);
                mAngle = (float)Math.Atan2(mVelocity.Y, mVelocity.X);
            }
            //Vector2 origin = new Vector2(mTexture.Width / 2, mTexture.Height / 2);
            //spriteBatch.Draw(mTexture, mPosition, null, Color.White, mAngle, origin, 1.0f, SpriteEffects.None, 1.0f);
            if (mAllied)
                mAtlas.Draw(spriteBatch, mBulletType, mBulletColor, mPosition, mAngle, 1.0f, SpriteEffects.None, 1.0f, 0.5f);
            else
                mAtlas.Draw(spriteBatch, mBulletType, mBulletColor, mPosition, mAngle, 1.0f, SpriteEffects.None, 1.0f, 1.0f);
        }

    }
}
