using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpriteAnimations.Sampling
{
    public class SingleCycleAnimationOnDemand : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public int _fps;
        public List<Sprite> _cycle;

        private SpriteAnimationSingleCycle _animation;

        private void Awake()
        {
            _animation = SpriteAnimationSingleCycle.OnDemand(_fps, _cycle, true);
        }

        private void Start()
        {
            _animator.Play(_animation);
        }
    }
}
