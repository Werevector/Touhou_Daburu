using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Touhou_Daburu
{
    class EnemySpawner
    {
        #region JsonInfoStructures
        private struct _AtlasInfo
        {
            public string Image { get; set; }
            public List<_ClipSetInfo> ClipSets { get; set; }
            public List<_SequenceList> EnemySequences { get; set; }
        }
        private struct _SequenceList
        {
            public string Key { get; set; }
            public List<_SequenceInfo> Sequences;
        }
        private struct _SequenceInfo
        {
            public string Key { get; set; }
            public bool Looping { get; set; }
            public int SubLoop { get; set; }
            public List<int> Seq { get; set; }
        }
        private struct _ClipSetInfo
        {
            public string Key { get; set; }
            public List<List<int>> Set { get; set; }
        }
        public struct EnemyInfo
        {
            public string Sprite            { get; set; }
            public List<int> HitBox         { get; set; }
            public int Health               { get; set; }
            public List<int> Position       { get; set; }
            public float MoveSpeed          { get; set; }
            public List<PatternGenerator.PatternInfo> FirePatterns { get; set; }
        }
        #endregion

        TextureAtlas mEnemyAtlas;
        Dictionary<string, Dictionary<string, SpriteSequence>> mSequenceMaps;
        PatternGenerator mGamePatternGeneratorPtr;
        List<Enemy> mGameEnemyListPtr;

        public EnemySpawner()
        {
            mSequenceMaps = new Dictionary<string, Dictionary<string, SpriteSequence>>();
        }

        public void LoadContent(ContentManager content)
        {
            // Load the Atlas(es)
            string aJson = File.ReadAllText("Content/Descriptors/Images/Enemy/enemy.json");
            _AtlasInfo aInfo = JsonConvert.DeserializeObject<_AtlasInfo>(aJson);

            Texture2D image = content.Load<Texture2D>("Images/Enemy/" + aInfo.Image);
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
            mEnemyAtlas = new TextureAtlas(image, clipMap);

            //Load and copy animation sequences
            foreach (var enemy in aInfo.EnemySequences)
            {
                Dictionary<string, SpriteSequence> sequenceMap = new Dictionary<string, SpriteSequence>();
                foreach (var sequence in enemy.Sequences)
                {
                    SpriteSequence s = new SpriteSequence();
                    s.mSequence = sequence.Seq;
                    s.Looping = sequence.Looping;
                    s.mSubLoop = sequence.SubLoop;
                    sequenceMap.Add(sequence.Key, s);
                }
                mSequenceMaps.Add(enemy.Key, sequenceMap);
            }
        }

        public void SetPatternGenerator(PatternGenerator gen)
        {
            mGamePatternGeneratorPtr = gen;
        }

        public void SpawnEnemy(EnemyInfo eInfo, List<Vector2> path)
        {
            Enemy e = new Enemy();

            e.SetTextureAtlas(mEnemyAtlas);
            e.SetSpriteName(eInfo.Sprite);
            //e.mSequenceMap = mSequenceMaps[eInfo.Sprite];
            //e.mCurrentSequence = e.mSequenceMap["Idle"];
            //Dictionary<string, SpriteSequence> temp = new Dictionary<string, SpriteSequence>();
            //temp = mSequenceMaps[eInfo.Sprite];
            e.SetSequenceMap(mSequenceMaps[eInfo.Sprite]);
            e.mHitBox = new Rectangle(eInfo.HitBox[0],
                                            eInfo.HitBox[1],
                                            eInfo.HitBox[2],
                                            eInfo.HitBox[3]);
            e.mHealth = eInfo.Health;
            e.mPosition = new Vector2(eInfo.Position[0], eInfo.Position[1]);
            e.mMoveSpeed = eInfo.MoveSpeed;
            e.mPathPoints = path;

            //Generate a pattern from the structure
            foreach (var pInfo in eInfo.FirePatterns)
            {
                e.AddPattern(mGamePatternGeneratorPtr.GeneratePatternFromStruct(pInfo));
            }

            mGameEnemyListPtr.Add(e);
        }

        public void SetEnemyListPtr(List<Enemy> enemyList)
        {
            mGameEnemyListPtr = enemyList;
        }
    }
}
