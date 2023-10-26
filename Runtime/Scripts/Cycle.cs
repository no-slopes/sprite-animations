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

        /// <summary>
        /// Calculate the total duration of the cycle based on the given FPS rate.
        /// </summary>
        /// <param name="fps">The number of frames per second.</param>
        /// <returns>The duration of the sequence of frames.</returns>
        public float CalculateDuration(float fps)
        {
            return _frames.Count * (1 / fps);
        }

        /// <summary>
        /// Evaluates the frame at a given elapsed time.
        /// </summary>
        /// <param name="fps">The frames per second.</param>
        /// <param name="elapsedTime">The elapsed time in seconds.</param>
        /// <returns>A tuple containing the index of the frame and the evaluated frame.</returns>
        public (int index, Frame frame) EvaluateFrame(float fps, float elapsedTime)
        {
            // Calculate the total duration of all frames
            float duration = _frames.Count * (1 / fps);

            // Calculate the index of the frame based on the elapsed time
            int frameIndex = Mathf.FloorToInt(elapsedTime * _frames.Count / duration);

            // Get the evaluated frame at the calculated index
            Frame evaluatedFrame = _frames.ElementAtOrDefault(frameIndex);

            // Return the frame index and the evaluated frame
            return (frameIndex, evaluatedFrame);
        }
    }
}