# Sprite Animator

The Sprite Animator is the main component of the Sprite Animations solution. It is responsible for changing sprites
of the targeted [SpriteRenderer](https://docs.unity3d.com/Manual/class-SpriteRenderer.html) based on the FPS (Frames per second) defined
for the current active anination.

## Inspector Window

![Sprite Animator](../../images/sprite-animator-component.png)

It is super easy to configure your Sprite Animator Component once attached to a GameObject.

1. Provide a [SpriteRenderer](https://docs.unity3d.com/Manual/class-SpriteRenderer.html) component, or leave the field empty
   if you want the animator to use the SpriteRenderer present on the very same object as it is.
2. Define the update mode in wich frames will be evaluated. Set it to `Update`, `FixedUpdate` or `LateUpdate`. Each of these
   correspond to the [MonoBehaviour](https://docs.unity3d.com/Manual/class-MonoBehaviour.html) method with the same name.
3. If you want it to start playing mark the `Play on start` toggle. You can then set a `Default Animation` to be played, or
   leave the field blank so the animator will start using the first animation of the list of animations if any.
4. Register animations. You can always edit the animations list here on inspector. Meaning you can add, delete and remove the
   animations registered into the animator. But one must use the [Animations Manager](../animations-manager/index.md) window to configure
   the animations behaviour. There is a big juicy button available so you won't miss how to open that.
5. If you want to keep track of animations being changed or the animator's states, just use the corresponding Unity event we've prepared for you.

And that is it. You are ready to tell the animator when to start playing stuff.

## Playing animations

Now you just need a script so you can tell the animator to play the animation you want:

```csharp
public SpriteAnimator _animator;

private void Start()
{
    _animator.Play("Idle");
}
```

or, if you do not like working with strings, you can reference an animation through inspector and use it:

```csharp
public SpriteAnimator _animator;
public SpriteAnimation _idleAnimation;

private void Start()
{
    _animator.Play(_idleAnimation);
}
```

## States

The animator will always be in one of these 3 states:

- **Stopped** - When there is no current animation being played.
- **Paused** - When there is a current animation being played, but you paused the animator and it won't evaluate new frames for that animation until the animator is resumed.
- **Playing** - When the animator is currently playing and evaluating new frames for an animation.

You can use the `Stop()`, `Pause()` and `Resume()` methods to... well... stop, pause and resume the animator. This will fire these will all fire the `StateChanged` UnityEvent updating listeners about the current state of the animator.

The other method wich alters the animator's state is the already mentioned `Play()`. It works by changing the current animation into to requested animation to be played and then changing the animator's state into the `Playing` state.
