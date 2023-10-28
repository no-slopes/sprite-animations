# The ComboAnimator

This is the performer for [Combo Animations](../animations/combo-animation.md).

## How it works

When you tell the [Sprite Animator](index.md) to play a Combo Animation it will behave as follows:

- First it will play the entire first cycle.
- At the end of the cycle it will fire the `OnCycleEnded` action (in case you registered one)
- Now the combo animator enters the "waiting state" where it awaits for the `Next()` method being called.
- From here, if the `Next()` method doesn't get called within the `Input Waiting Time` set when configuring the animation
  while using the [Animations Manage](../animations-manager/index.md), the animation is considered interrupted and the
  combo animator will fire the `OnInterrupted` action (if registered). The common practice here would be getting the animated
  object back to its default animation.
- In the case the `Next()` method do get called within the `Input Waiting Time` it will play the next cycle and enter the "waiting state" again.
- This process repeats until the last cycle is played, when the Combo Animator calls the `OnEnd` action.

## Playing a Combo Animation

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;

public void StartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
}
```

## Registering the OnInterrupted Action

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;

public void StartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
    _comboAnimator.SetOnInterrupted(OnAnimationInterrupted)
}

private void OnAnimationInterrupted()
{
    // Go back to the Idle state and / or
    // apply any needed logic.
}
```

## Registering the OnCycleEnded Action

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;

public void StartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
    _comboAnimator.SetOnCycleEnded(OnCycleEnded)
}

private void OnCycleEnded(int cycleIndex)
{
    // This is most usefull for cases where a new input
    // should only be handled if the current cycle if fully played.
    // The cycle index can be used in favor of your logic.
}
```

## On Frame Operations

The `SetOnFrame` remains as the [Single Cycle Animator](./single-cycle-animator.md) here. The catch is only
that you must be carefull wuen setting by index. The index in this scenario would be the index of each cycle.

So, considering you have an animation with 3 cycles (0, 1 and 2) and do as follows:

```csharp
_comboAnimator.SetOnFrame(0, frame => Debug.Log(frame));
```

this will get fired 3 times throught the complete execution of the animation.

as this:

```csharp
_comboAnimator.SetOnFrame("Shoot", frame => Debug.Log(frame));
```

will only get fired for the frame that has the "Shoot" identification.

## Overriding the waiting time

For the cases the waiting time should vary according to some logic of yours, you can use the `OverrideInputWait()` method and
tell the Combo Animator how much time it should wait for the `Next()` method being called before interrupting the animation:

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;

public void StartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
    _comboAnimator.OverrideInputWait(3); // Will wait 3 seconds before interrupting.
}
```

## Finally, playing the next cycle

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;
private bool _canPlayNext;

public void StartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo");
    _comboAnimator.SetOnCycleEnded(OnCycleEnded);
    _canPlayNext = false;
}

// This could be called in another script or even called within the same script.
// As said many times, it is better to setup an FSM scenario for these animations,
// but you can check Samples to checkout how you can make this work within the same script.
public void PlayNextCycle()
{
    if (!_canPlayNext) return;
    _comboAnimator?.Next();
    _canPlayNext = false;
}

private void OnCycleEnded(int cycleIndex)
{
    _canPlayNext = true;
}
```

## Playing from start

In case you need to reset the animation, just use the `FromStart()` method:

```csharp
public SpriteAnimator _animator;
private ComboAnimator _comboAnimator;

public void RestartCombo()
{
    _comboAnimator = _animator.Play<ComboAnimator>("Attack_Combo").FromStart();
}
```
