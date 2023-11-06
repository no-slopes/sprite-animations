using System.Collections;
using System.Collections.Generic;
using SpriteAnimations;
using UnityEngine;

public class CompositeAnimationChanger : MonoBehaviour
{
    public SpriteAnimator _animator;
    public bool _loopableCore;

    private bool _sliding;
    private CompositeAnimator _performer;

    private void Start()
    {
        _animator.Play("Idle");
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E) || _sliding) return;
        PlaySlideAnimation();
    }

    private void PlaySlideAnimation()
    {
        _sliding = true;
        _performer = _animator.Play<CompositeAnimator>("Slide");
        _performer.SetLoopableCore(_loopableCore);
        _performer.SetOnAntecipationEnd(OnAntecipationEnds);
        _performer.SetOnCoreEnd(OnCoreEnds);
        _performer.SetOnRecoveryEnd(OnRecoveryEnds);
        _performer.SetOnEnd(OnAnimationEnds);
    }

    private void OnAntecipationEnds()
    {
        Debug.Log($"Antecipation ended at {Time.time}");
        if (_loopableCore)
            StartCoroutine(WaitAndStopCore());
    }

    private void OnCoreEnds()
    {
        Debug.Log($"Core ended at {Time.time}");
    }

    private void OnRecoveryEnds()
    {
        Debug.Log($"Recovery ended at {Time.time}");
    }

    private void OnAnimationEnds()
    {
        Debug.Log($"Animation ended at {Time.time}");
        _sliding = false;
        _animator.Play("Idle");
    }

    private IEnumerator WaitAndStopCore()
    {
        yield return new WaitForSeconds(2f);
        _performer?.ExitCoreLoop();
    }
}
