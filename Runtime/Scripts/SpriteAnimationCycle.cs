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

        [SerializeField]
        protected bool _identifiable = true;

        public bool Identifiable { get => _identifiable; set => _identifiable = value; }
        public List<SpriteAnimationFrame> Frames => _frames;
        public int Size => _frames.Count;

        public SpriteAnimationCycle()
        {
            _frames = new List<SpriteAnimationFrame>();
        }

        public float CalculateDuration(float fps)
        {
            return _frames.Count * 1 / fps;
        }
    }
}