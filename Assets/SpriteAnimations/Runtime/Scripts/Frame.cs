using UnityEngine;
using System;

namespace SpriteAnimations
{
    public delegate void SpriteChangedEvent(Sprite sprite);
    [Serializable]
    public class Frame
    {
        #region Setup

        [SerializeField]
        protected Sprite _sprite;

        [SerializeField]
        protected string _id;

        #endregion

        #region Getters

        /// <summary>
        /// The sprite that will be displayed when the frame is played
        /// </summary>
        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                SpriteChanged?.Invoke(_sprite);
            }
        }

        /// <summary>
        /// The ID of the frame
        /// </summary>
        public string Id { get => _id; set => _id = value; }

        #endregion

        #region Events

        public event SpriteChangedEvent SpriteChanged;

        #endregion
    }
}