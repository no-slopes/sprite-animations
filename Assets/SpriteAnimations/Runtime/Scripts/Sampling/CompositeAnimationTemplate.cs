using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpriteAnimations.Sampling
{
    public class CompositeAnimationTemplate : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public SpriteAnimationComposite _template;

        public List<Sprite> _antecipationCycle;
        public List<Sprite> _coreCycle;
        public List<Sprite> _recoveryCycle;

        private SpriteAnimationComposite _animation;

        private void Awake()
        {
            _animation = _template.UseAsTemplate(_antecipationCycle, _coreCycle, _recoveryCycle);
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
