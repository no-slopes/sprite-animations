using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpriteAnimations
{
    [Serializable]
    public class Cycle
    {
        #region Inspector

        [SerializeField]
        protected SpriteAnimation _animation;

        [SerializeField]
        protected List<Frame> _frames;

        [SerializeField]
        protected bool _identifiable = true;

        #endregion

        #region Getters

        /// <summary>
        /// The animation wich this cycle belongs to
        /// </summary>
        public SpriteAnimation Animation { get => _animation; set => _animation = value; }

        /// <summary>
        /// If the cycle can have identifiable frames
        /// </summary>
        public bool Identifiable { get => _identifiable; set => _identifiable = value; }

        public List<Frame> Frames => _frames;

        /// <summary>
        /// The size of the cycle (number of frames)
        /// </summary>
        public int Size => _frames.Count;

        #endregion

        #region Constructors

        public Cycle()
        {
            _frames = new List<Frame>();
        }

        public Cycle(SpriteAnimation animation)
        {
            _frames = new List<Frame>();
            _animation = animation;
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculate the total duration of the cycle based on the given FPS rate.
        /// </summary>
        /// <param name="fps">The number of frames per second.</param>
        /// <returns>The duration of the sequence of frames.</returns>
        public float CalculateDuration()
        {
            float duration = _frames.Count * (1f / _animation.FPS);
            return duration;
        }

        #endregion

        #region Frames

        public void AddFrame(Frame frame)
        {
            _frames.Add(frame);
        }

        public void AddFrame(Sprite sprite, string id)
        {
            Frame frame = new()
            {
                Sprite = sprite,
                Id = id
            };

            AddFrame(frame);
        }

        public void AddFrame(Sprite sprite)
        {
            Frame frame = new()
            {
                Sprite = sprite
            };

            AddFrame(frame);
        }

        public Frame GetFirstFrame()
        {
            return _frames.First();
        }

        public bool TryGetFrame(int index, out Frame frame)
        {
            if (index >= _frames.Count)
            {
                Logger.LogWarning($"Trying to get frame at {index} but index is out of range.");
                frame = null;
                return false;
            }

            frame = _frames[index];
            return true;
        }

        /// <summary>
        /// Evaluates the frame based on the elapsed time.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time in seconds.</param>
        /// <returns>The evaluated frame.</returns>
        public Frame EvaluateFrame(float elapsedTime)
        {
            // Get the total number of frames
            int amountOfFrames = _frames.Count;

            // Calculate the total duration of the animation
            float duration = CalculateDuration();

            // Calculate the index of the frame based on the elapsed time
            int frameIndex = Mathf.FloorToInt(elapsedTime * amountOfFrames / duration);

            // Return the frame at the calculated index, or null if the index is out of range
            return _frames.ElementAtOrDefault(frameIndex);
        }

        /// <summary>
        /// Evaluates the frame at a given elapsed time.
        /// </summary>
        /// <param name="fps">The frames per second.</param>
        /// <param name="elapsedTime">The elapsed time in seconds.</param>
        /// <returns>A tuple containing the index of the frame and the evaluated frame.</returns>
        public (int index, Frame frame) EvaluateIndexAndFrame(float elapsedTime)
        {
            int amountOfFrames = _frames.Count;

            // Calculate the total duration of all frames
            float duration = CalculateDuration();

            // Calculate the index of the frame based on the elapsed time
            int frameIndex = Mathf.FloorToInt(elapsedTime * amountOfFrames / duration);

            // Get the evaluated frame at the calculated index
            Frame evaluatedFrame = _frames.ElementAtOrDefault(frameIndex);

            // Return the frame index and the evaluated frame
            return (frameIndex, evaluatedFrame);
        }

        #endregion
    }
}