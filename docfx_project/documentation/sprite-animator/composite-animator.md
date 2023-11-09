# The CompositeAnimator

This is the performer for [Composite Animations](../animations/composite-animation.md).

## How it works

When you tell the [Sprite Animator](index.md) to play a Composite Animation it will behave as follows:

- First it will play the entire antecipation cycle and at its end it will fire the `OnAntecipationEnd` action if you registered one.
- Then it will play the core cycle. If you use the method `SetLoopableCore(true)` upon the animation start, the core cycle will loop until the
  `ExitCoreLoop()` method is called. Either way, at the end of the core cycle, the `OnCoreEnd` action is fired if you registered one.
- At last it will play the recovery cycle and at its end the `OnRecoveryEnd` action is fired if you registered one. Also, this means the animation has
  come to an end and the `OnEnd` action is fired in case you registered one.

## Playing a Composite Animation

```csharp
public SpriteAnimator _animator;
private CompositeAnimator _compositeAnimator;

public void StartAnimation()
{
    _compositeAnimator = _animator.Play<CompositeAnimator>("Hadouken");
}
```

## Registering the cycles Actions

```csharp
public SpriteAnimator _animator;
private CompositeAnimator _compositeAnimator;

public void StartAnimation()
{
    _compositeAnimator = _animator.Play<CompositeAnimator>("Hadouken");
    _compositeAnimator.SetOnAntecipationEnd(() => Debug.Log("Antecipation Ended"));
    _compositeAnimator.SetOnCoreEnd(() => Debug.Log("Core Ended"));
    _compositeAnimator.SetOnRecoveryEnd(() => Debug.Log("Recovery Ended"));
}
```

## Setting the core cycle as loopable

If you do not know when the animation should stop, like when a character is sliding under a platform and you need it to play
that animation until there is no longer any ceiling above the character, you can tell the CompositeAnimator to loop the core
until you use its `ExitCoreLopp` method meaning the CompositeAnimator should now play the recovery cycle and end the animation.

```csharp
public SpriteAnimator _animator;
private CompositeAnimator _compositeAnimator;

public void StartAnimation()
{
    _compositeAnimator = _animator.Play<CompositeAnimator>("Slide");
    _compositeAnimator.SetLoopableCore(true);
}

// Example of method to be called when the core cycle should stop
public void SlideMustFinish()
{
    // This tells the CompositeAnimator to end the core cycle and play the recovery cycle.
    _compositeAnimator.ExitCoreLoop();
}
```
