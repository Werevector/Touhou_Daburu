using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Touhou_Daburu
{
    class BulletSpawner
    {
        private struct _AtlasInfo
        {
            public string Image { get; set; }
            public List<_ClipSetInfo> ClipSets { get; set; }
        }
        private struct _ClipSetInfo
        {
            public string Key { get; set; }
            public List<List<int>> Set { get; set; }
        }

        TextureAtlas mBulletAtlas;
        Dictionary<string, List<Texture2D>> mTextureMap;
        List<Bullet> mGameBulletListPtr;

        public BulletSpawner()
        {

        }

        public void LoadContent(ContentManager content)
        {
            // Load the Atlas(es)
            string aJson = File.ReadAllText("Content/Descriptors/Images/Bullet/Bullet1.json");
            _AtlasInfo aInfo = JsonConvert.DeserializeObject<_AtlasInfo>(aJson);

            Texture2D image = content.Load<Texture2D>("Images/Bullet/" + aInfo.Image);
            Dictionary<String, List<Rectangle>> clipMap = new Dictionary<string, List<Rectangle>>();
            foreach (var clipset in aInfo.ClipSets)
            {
                List<Rectangle> clips = new List<Rectangle>();
                foreach (var clip in clipset.Set)
                {
                    clips.Add(new Rectangle(clip[0], clip[1], clip[2], clip[3]));
                }
                clipMap.Add(clipset.Key, clips);
            }
            mBulletAtlas = new TextureAtlas(image, clipMap);
            mTextureMap = mBulletAtlas.Texture2DCopyAll();
        }

        public void SpawnBullet(string bulletType, int bulletColor, Vector2 position, Vector2 velocity, Vector2 acceleration, Rectangle hitBox, bool angled)
        {
            Bullet bullet = new Bullet();
            bullet.mTexture = mTextureMap[bulletType][bulletColor];
            bullet.mPosition = position;
            bullet.mVelocity = velocity;
            bullet.mAcceleration = acceleration;
            bullet.mHitBox = hitBox;
            bullet.mDirectional = angled;
            mGameBulletListPtr.Add(bullet);
        }

        public void SetBulletList(List<Bullet> bulletList)
        {
            mGameBulletListPtr = bulletList;
        }
    }
}
