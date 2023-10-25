# The SingleAnimator

This is the performer for [Single Animations](../animations/single-animation.md).

As [Single Animations](../animations/single-animation.md) are the most basic type of animation we have,
its performer goes as far as allowing you to use the `SetOnFrame()` and `OnEnd()` methods to register Frame Actions.

```csharp
private SingleAnimator _perfomer;

private void Start()
{
    _perfomer = _animator.Play<SingleAnimator>("Idle");
    _performer.OnEnd(() => { Debug.Log("The animation ended."); })
}
```

```csharp
private SingleAnimator _perfomer;

private void Start()
{
    _perfomer = _animator.Play<SingleAnimator>("Idle");
    _performer.SetOnFrame(3, frame =>
    {
        Debug.Log("Frame indexed as 3 played.");
    }).OnEnd(() =>
    {
        Debug.Log("The animation ended.");
    });
}
```
