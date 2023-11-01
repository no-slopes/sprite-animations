using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpriteAnimations.Sampling
{
    public class SingleCycleAnimationTemplate : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public SpriteAnimationSingleCycle _template;
        public List<Sprite> _cycle;

        private SpriteAnimationSingleCycle _animation;

        private void Awake()
        {
            _animation = _template.UseAsTemplate(_cycle);
        }

        private void Start()
        {
            _animator.Play(_animation).SetOnFrame("Slash", frame => Debug.Log("Slash frame played"));
        }
    }
}
