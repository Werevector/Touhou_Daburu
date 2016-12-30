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
    class StageSchedule
    {
        private struct _StageInfo
        {
            public List<EnemyGenerator.GeneratorInfo> Generators { get; set; }
        }

        List<EnemyGenerator> mGenerators;
        List<int> mDestroyGeneratorQueue;
        EnemySpawner mGameEnemySpawnerPtr;

        double mStageTime;

        public StageSchedule()
        {
            mStageTime = 0;
        }

        public void Init(string stagefilePath, EnemySpawner spawner)
        {
            mGenerators = new List<EnemyGenerator>();
            mDestroyGeneratorQueue = new List<int>();
            SetEnemySpawner(spawner);
            LoadStage(stagefilePath);
            
        }

        public void Update(GameTime gameTime)
        {
            mStageTime += gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < mGenerators.Count; i++)
            {
                if ((float)mStageTime >= mGenerators[i].GetActivationTime())
                {
                    mGenerators[i].Start();
                }

                mGenerators[i].Update(gameTime);

                if (mGenerators[i].IsFinished())
                {
                    mDestroyGeneratorQueue.Add(i);
                }
            }

            ProccessDestroyQueue();
        }

        private void ProccessDestroyQueue()
        {
            mDestroyGeneratorQueue.Sort();
            mDestroyGeneratorQueue.Reverse();
            foreach (var index in mDestroyGeneratorQueue)
            {
                mGenerators.RemoveAt(index);
            }
            mDestroyGeneratorQueue.Clear();
        }

        private void LoadStage(string stagefilePath)
        {
            string sJson = File.ReadAllText(stagefilePath);
            _StageInfo sInfo = JsonConvert.DeserializeObject<_StageInfo>(sJson);

            foreach (var generatorInfo in sInfo.Generators)
            {
                EnemyGenerator generator = new EnemyGenerator();
                generator.Init(generatorInfo, mGameEnemySpawnerPtr);
                mGenerators.Add(generator);
            }
        }

        public void SetEnemySpawner(EnemySpawner spawner)
        {
            mGameEnemySpawnerPtr = spawner;
        }



    }
}
