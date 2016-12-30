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
    class EnemyGenerator
    {
        public struct GeneratorInfo
        {
            public int Amount { get; set; }
            public float Time { get; set; }
            public float Interval { get; set; }
            public int[][] PathControlPoints { get; set; }
            public EnemySpawner.EnemyInfo Enemy { get; set; }
        }

        EnemySpawner mSpawnerPtr;

        int mAmount;
        float mTime;
        float mInterval;
        EnemySpawner.EnemyInfo mEnemyInfo;

        int mCurrentAmount;
        float mCurrentInterval;

        bool mRunning;
        bool mFinished;

        List<Vector2> mPathPoints;

        public EnemyGenerator()
        {
            mAmount = 0;
            mTime = 0;
            mInterval = 0;

            mCurrentAmount = 0;
            mCurrentInterval = 0;
            mRunning = false;
            mFinished = false;
            mPathPoints = new List<Vector2>();
        }

        public void Init(GeneratorInfo gInfo, EnemySpawner spawner)
        {
            Load(gInfo);
            SetSpawner(spawner);
        }

        public void Update(GameTime gameTime)
        {
            if (!mFinished && mRunning)
            {
                mCurrentInterval += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (mCurrentInterval > mInterval)
                {
                    Generate();
                    mCurrentInterval = 0;
                    if (CheckFinished()) { Stop(); }
                }
            }
        }

        public void Load(GeneratorInfo gInfo)
        {
            mAmount = gInfo.Amount;
            mTime = gInfo.Time;
            mInterval = gInfo.Interval;
            mEnemyInfo = gInfo.Enemy;

            List<Vector2> controlPoints = new List<Vector2>();
            for (int i = 0; i < gInfo.PathControlPoints.Length; i++)
            {
                controlPoints.Add(new Vector2(gInfo.PathControlPoints[i][0], gInfo.PathControlPoints[i][1]));
            }
            mPathPoints = Utility.CreateSplinePoints(controlPoints, 20);

            mCurrentInterval = mInterval;
        }

        private void Generate()
        {
            mSpawnerPtr.SpawnEnemy(mEnemyInfo, mPathPoints);
            mCurrentAmount++;
        }

        public void Start()
        {
            mRunning = true;
        }

        private void Stop()
        {
            mRunning = false;
            mFinished = true;
        }

        private bool CheckFinished()
        {
            return mCurrentAmount == mAmount;
        }

        public void SetSpawner(EnemySpawner spawner)
        {
            mSpawnerPtr = spawner;
        }

        public bool IsFinished() { return mFinished; }
        public float GetActivationTime() { return mTime; }
    }
}
