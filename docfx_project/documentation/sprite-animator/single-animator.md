# The SingleAnimator

This is the performer for [Single Animations](../animations/single-animation.md).

As [Single Animations](../animations/single-animation.md) are the most basic type of animation we have,
its performer is also really simple to interact with.

## Registering Frame Actions

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

## Forcing to play from Start

Sometimes you might tell the [SpriteAnimator](./index.md) to play the same animation it has already been playing. This
will cause the animator to skip animation changing and just return the animation performer. For the case you want the
animation to be "restarted", just call the `FromStart()` method of the SingleAnimator.

```csharp
public SpriteAnimator _animator;
private bool _shouldRepeat;

private IEnumerator PlayRepeatedly()
{
    while(_shouldRepeat)
    {
        _animator.Play<SingleAnimator>("MyAnimation").FromStart();
        yield retun new WaitForSeconds(0.5f);
    }
}
```

Have in mind that if the current animation has not yet come to an end, this will restart the animation.
