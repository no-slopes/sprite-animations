# Composite Animation

![Composite Animation Window](../../images/animator-manager-composite-animation.png)

The composite animation has 3 [Frame Cycles](../animations-manager/index.md#frame-cycle). One representing the antecipation, one representing the
core segment of the animation and one representing the recovery of the movement.

The main use case for Composite Animations is when you need to "warn" players about what that movement will do. Such as in fighting games or when crafting boss
fights.

When telling [the animator](../sprite-animator/index.md) to play a Composite Animation you will have the oportunity to listen to when each of the cycles has
ended so you can associate some logic to it.

Refer to the [Composite Animator](../sprite-animator/composite-animator.md) documentation in order to have a better understanding on how you can
operate a Composite Animation through code.

## Creating on demand

You can create Composite animations on demand by acessing the static method `SpriteAnimationComposite.OnDemand`. Just provide the fps, and each of [cycles](../animations-manager/index.md#frame-cycle) as lists of Sprites.

Example:

```csharp
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
```

## Creating from a template

You can use any other Composite Animation as template for creating another Composite Animation. Just use the `UseAsTemplate` method providing
each of the [cycles](../animations-manager/index.md#frame-cycle) as lists of Sprites.

Example:

```csharp
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
```
