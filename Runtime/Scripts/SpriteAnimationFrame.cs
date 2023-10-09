using UnityEngine;
using System;

namespace SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        #region Setup

        [SerializeField]
        protected int _index;

        [SerializeField]
        protected Sprite _sprite;

        [SerializeField]
        protected string _id;

        #endregion

        #region Getters

        public int Index { get => _index; set => _index = value; }
        public Sprite Sprite { get => _sprite; set => _sprite = value; }
        public string Id { get => _id; set => _id = value; }

        #endregion
    }
}