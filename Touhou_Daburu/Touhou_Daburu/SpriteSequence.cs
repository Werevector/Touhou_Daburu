using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Touhou_Daburu
{
    class SpriteSequence
    {
        public List<int> mSequence { get; set; }
        public bool Looping { get; set; }
        public int mUpdateDelay { get; set; }
        public int mSubLoop = 0;
        int mSequenceIndex;
        private int mCurrentTick = 0;

        public SpriteSequence()
        {
            mSequence = new List<int> {0};
            Looping = true;
            mUpdateDelay = 5;
            mSequenceIndex = 0;
        }

        public SpriteSequence(List<int> sequence)
        {
            mSequence = sequence;
            Looping = true;
            mUpdateDelay = 5;
            mSequenceIndex = 0;
        }

        public void Update()
        {
            mCurrentTick++;
            if (mCurrentTick == mUpdateDelay)
            {
                mCurrentTick = 0;
                mSequenceIndex++;

                if (mSequenceIndex == mSequence.Count)
                {
                    if (Looping)
                        mSequenceIndex = mSubLoop;
                    else
                        mSequenceIndex = mSequence.Count - 1;
                }
            }
        }

        public void Reset()
        {
            mCurrentTick = 0;
            mSequenceIndex = 0;
        }

        public int GetCurrentIndex()
        {
            return mSequence[mSequenceIndex];
        }

        public int GetKeyAt(int index)
        {
            return mSequence[index];
        }
    }
}
