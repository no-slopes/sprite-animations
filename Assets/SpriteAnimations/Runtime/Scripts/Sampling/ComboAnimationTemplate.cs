using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpriteAnimations.Sampling
{
    public class ComboAnimationTemplate : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public SpriteAnimationCombo _template;

        public List<Sprite> _cycle0;
        public List<Sprite> _cycle1;
        public List<Sprite> _cycle2;

        private SpriteAnimationCombo _animation;
        private ComboAnimator _performer;

        private void Awake()
        {
            List<List<Sprite>> cycles = new()
            {
                _cycle0,
                _cycle1,
                _cycle2
            };

            _animation = _template.UseAsTemplate(cycles);
        }

        private void Start()
        {
            StartAnimation();
        }

        private void StartAnimation()
        {
            _performer = _animator.Play<ComboAnimator>(_animation).FromStart();
            _performer.SetOnCycleEnded(CycleEnded);

        }

        private void CycleEnded(int cycleIndex)
        {
            if (cycleIndex < 2)
            {
                StartCoroutine(WaitAndPlayNext());
            }
            else
            {
                StartAnimation();
            }
        }

        private IEnumerator WaitAndPlayNext()
        {
            yield return new WaitForSeconds(0.45f);
            _performer?.Next();
        }
    }
}