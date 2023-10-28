# The WindroseAnimator

This is the performer for [Windrose Animations](../animations/windrose-animation.md).

In addition to allowing the usage of `SetOnFrame()` and `SetOnEnd()` methods like the [SingleCycleAnimator](single-cycle-animator.md), the WindroseAnimator allows you to set the direction in wich you
want that animation to played.

You can set direction by telling an specific direction of the [WindroseDirection](../../api/SpriteAnimations.WindroseDirection.yml) enum:

| WindroseDirection |
| ----------------- |
| North             |
| NorthEast         |
| East              |
| SouthEast         |
| South             |
| SouthWest         |
| West              |
| NorthWest         |

```csharp
using SpriteAnimations; // <- Important so you can use the WindroseDirection enum

public class MyClass : MonoBehaviour
{
    public SpriteAnimator _animator;

    private void Start()
    {
        _animator.Play<WindroseAnimation>("Walk").SetDirection(WindroseDirection.East);
    }
}
```

or you can pass in a Vector2 and it will resolve wich direction that Vector2 is pointing using the following setup conditions:

| Direction | Vector Conditions |
| :-------- | :---------------: |
| North     |  x == 0 && y > 0  |
| NorthEast |  x > 0 && y > 0   |
| East      |  x > 0 && y == 0  |
| SouthEast |  x > 0 && y < 0   |
| South     |  x == 0 && y < 0  |
| SouthWest |  x < 0 && y < 0   |
| West      |  x < 0 && y == 0  |
| NorthWest |  x < 0 && y > 0   |

```csharp
using SpriteAnimations;

public class MyClass : MonoBehaviour
{
    public SpriteAnimator _animator;

    private void Start()
    {
        Vector2 movementInput = new Vector2(0, -1); // A Vector2 representing South. Vector2(0, -0.7f), for example, would also work
        _animator.Play<WindroseAnimation>("Walk").SetDirection(_movementInput);
    }
}
```

You might wanna cache the performer to change direction when the player inputs new movement data:

```csharp
using SpriteAnimations;

public class WalkPlayer : MonoBehaviour
{
    public SpriteAnimator _animator;

    private WindroseAnimator _perfomer;

    // This would be called whenever the animator should play the walk animation.
    public void PlayWalk(Vector2 startingDirection)
    {
        _performer = _animator.Play<WindroseAnimation>("Walk");
        _perfomer.SetDirection(startingDirection);
    }

    // This would be called whenenever new input is recognized
    public void ChangeDirection(Vector2 newDirection)
    {
        _performer?.SetDirection(newDirection);
    }
}
```

And if you do not have West animations (NorthWest, West and SouthWest) or simply do not want to craft them because it would just mean to flip the sprite horizontally, you can pass [WindroseFlipStrategy.FlipEastToPlayWest](../../api/SpriteAnimations.WindroseFlipStrategy.yml) as the second parameter to the `SetDirection()` method and tell the performer to flip accordingly:

```csharp
using SpriteAnimations; // <- Important to use the WindroseFlipStrategy enum.

public class WalkPlayer : MonoBehaviour
{
    public SpriteAnimator _animator;

    private void Start()
    {
        // This will play the East animation flipped horizontally.
        _animator.Play<WindroseAnimation>("Walk").SetDirection(WindroseDirection.West, WindroseFlipStrategy.FlipEastToPlayWest);
    }
}
```

The same goes to the Vector2 signature:

```csharp
using SpriteAnimations; // <- Important to use the WindroseFlipStrategy enum.

public class WalkPlayer : MonoBehaviour
{
    public SpriteAnimator _animator;

    private void Start()
    {
        Vector2 movementInput = new Vector2(1,0); // A Vector2 representing East.

        // This will play the East animation flipped horizontally.
        _animator.Play<WindroseAnimation>("Walk").SetDirection(movementInput, WindroseFlipStrategy.FlipEastToPlayWest);
    }
}
```

> [!Warning]
> These examples should only inform you how to interact with the performer. Since your project has its own peculiar ways, these are not meant to be copy pasted and expected to work out of the box.

## Forcing to play from Start

Sometimes you might tell the [SpriteAnimator](./index.md) to play the same animation it has already been playing. This
will cause the animator to skip animation changing and just return the animation performer. For the case you want the
animation to be "restarted", just call the `FromStart()` method of the WindroseAnimator.

```csharp
public SpriteAnimator _animator;
public Vector2 _movementInput;
private bool _shouldRepeat;

private IEnumerator PlayRepeatedly()
{
    while(_shouldRepeat)
    {
        _animator.Play<WindroseAnimator>("MyAnimation").FromStart().SetDirection(_movementInput);
        yield retun new WaitForSeconds(0.5f);
    }
}
```

Have in mind that if the current animation has not yet come to an end, this will restart the animation.
