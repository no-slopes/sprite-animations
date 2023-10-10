using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationCycle
    {
        [SerializeField]
        protected List<SpriteAnimationFrame> _frames;

        public SpriteAnimationCycle()
        {
            _frames = new List<SpriteAnimationFrame>();
        }

        public List<SpriteAnimationFrame> Frames => _frames;
        public int Size => _frames.Count;

        public float CalculateDuration(float fps)
        {
            return _frames.Count * 1 / fps;
        }

        public void AdjustToSize(int targetSize)
        {
            if (targetSize < 0 || targetSize == _frames.Count) return;

            _frames.Capacity = targetSize;

            if (targetSize < _frames.Count)
            {
                _frames.RemoveRange(targetSize, _frames.Count - targetSize);
            }
            else if (targetSize > _frames.Count)
            {
                int numToAdd = targetSize - _frames.Count;
                _frames.InsertRange(_frames.Count, Enumerable.Repeat(new SpriteAnimationFrame(), numToAdd));
            }
        }
    }
}