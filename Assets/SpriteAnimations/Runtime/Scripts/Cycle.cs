using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpriteAnimations
{
    [Serializable]
    public class Cycle
    {
        [SerializeField]
        protected List<Frame> _frames;

        [SerializeField]
        protected bool _identifiable = true;

        public bool Identifiable { get => _identifiable; set => _identifiable = value; }
        public List<Frame> Frames => _frames;
        public int Size => _frames.Count;

        public Cycle()
        {
            _frames = new List<Frame>();
        }

        public float CalculateDuration(float fps)
        {
            return _frames.Count * 1 / fps;
        }
    }
}