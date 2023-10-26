using System.Collections;
using System.Collections.Generic;
using SpriteAnimations;
using UnityEngine;

public class WindroseOverTime : MonoBehaviour
{
    public SpriteAnimator _spriteAnimator;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PlayAnimation", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayAnimation()
    {
        _spriteAnimator.Play<WindroseAnimator>("Walk").FromStart().SetDirection(new Vector2Int(1, 0), WindroseFlipStrategy.FlipEastToPlayWest);
    }
}
