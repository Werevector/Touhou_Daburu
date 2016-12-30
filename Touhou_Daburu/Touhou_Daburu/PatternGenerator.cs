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
    class PatternGenerator
    {
        //Shared with other generators
        public struct PatternInfo
        {
            public int ArrayNumber              { get; set; }
            public int BulletsPerArray          { get; set; }

            public float IndividualArraySpread  { get; set; }
            public float TotalArraySpread       { get; set; }

            public double DirectionAngle        { get; set; }

            public float Speed                  { get; set; }
            public float Acceleration           { get; set; }

            public double RotationSpeed         { get; set; }
            public double RotationSpeedDelta    { get; set; }
            public double MaxRotationSpeed      { get; set; }

            public bool BoundedRotation         { get; set; }
            public double MaxRotation           { get; set; }
            public double MinRotation           { get; set; }
            public bool FlipRotation            { get; set; }

            public int FireRate                 { get; set; }
            
            public List<int> HitBox             { get; set; }

            public string BulletType            { get; set; }
            public int BulletColor              { get; set; }
            public bool Directional             { get; set; }
        }

        private BulletSpawner mGameBulletSpawnerPtr;

        public Pattern GeneratePatternFromStruct(PatternInfo pInfo)
        {
            Pattern pattern = new Pattern();
            pattern.mAcceleration = pInfo.Acceleration;
            pattern.mSpeed = pInfo.Speed;

            pattern.mArrayNumber = pInfo.ArrayNumber;
            pattern.mBulletsPerArray = pInfo.BulletsPerArray;

            pattern.mDirectionAngle = pInfo.DirectionAngle;
            pattern.mFireRate = pInfo.FireRate;
            pattern.mHitBox = new Rectangle(pInfo.HitBox[0],
                                            pInfo.HitBox[1],
                                            pInfo.HitBox[2],
                                            pInfo.HitBox[3]);
            pattern.mIndvArraySpread = pInfo.IndividualArraySpread;
            pattern.mTotalArraySpread = pInfo.TotalArraySpread;

            pattern.mRotationSpeed = pInfo.RotationSpeed;
            pattern.mRotationSpeedDelta = pInfo.RotationSpeedDelta;
            pattern.mMaxRotationSpeed = pInfo.MaxRotationSpeed;
            pattern.mBoundedRotation = pInfo.BoundedRotation;
            pattern.mMaxRotation = pInfo.MaxRotation;
            pattern.mMinRotation = pInfo.MinRotation;
            pattern.mFlipRotation = pInfo.FlipRotation;

            pattern.SetBulletSpawner(mGameBulletSpawnerPtr);

            pattern.mBulletType = pInfo.BulletType;
            pattern.mBulletColor = pInfo.BulletColor;
            pattern.mDirectional = pInfo.Directional;

            return pattern;
        }

        public void SetBulletSpawner(BulletSpawner spawner) { mGameBulletSpawnerPtr = spawner; }

    }
}
