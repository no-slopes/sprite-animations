using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations.Sampling
{
    public class ComboAnimationChanger : MonoBehaviour
    {
        public SpriteAnimator _animator;

        private bool _performingCombo = false;
        private bool _waitingForInput;
        private ComboAnimator _comboAnimator;

        private void Start()
        {
            _animator.Play("Idle");
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            if (!_performingCombo)
            {
                StartCombo();
            }
            else
            {
                NextCycle();
            }
        }

        private void StartCombo()
        {
            _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
            _comboAnimator.SetOnCycleEnded(OnCycleEnded);
            _comboAnimator.SetOnInterrupted(EndAndPlayIdle);
            _comboAnimator.SetOnEnd(EndAndPlayIdle);

            _performingCombo = true;
            _waitingForInput = true;
        }

        private void NextCycle()
        {
            if (!_waitingForInput) return;
            _comboAnimator?.Next();
            _waitingForInput = false;
        }

        private void OnCycleEnded()
        {
            _waitingForInput = true;
        }

        private void EndAndPlayIdle()
        {
            _waitingForInput = false;
            _performingCombo = false;
            _animator.Play("Idle");
        }
    }
}
