using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpriteAnimations.Sampling
{
    public class CompositeAnimationOnDemand : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public int _fps;

        public List<Sprite> _antecipationCycle;
        public List<Sprite> _coreCycle;
        public List<Sprite> _recoveryCycle;

        private SpriteAnimationComposite _animation;

        private void Awake()
        {
            _animation = SpriteAnimationComposite.OnDemand(_fps, _antecipationCycle, _coreCycle, _recoveryCycle);
        }

        private void Start()
        {
            _animator.Play<CompositeAnimator>(_animation).SetOnEnd(() =>
            {
                _animator.Play<CompositeAnimator>(_animation).FromStart();
            });
        }
    }
}
