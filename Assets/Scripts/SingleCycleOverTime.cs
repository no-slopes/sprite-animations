using System.Collections;
using System.Collections.Generic;
using SpriteAnimations;
using UnityEngine;

public class SingleCycleOverTime : MonoBehaviour
{
    public SpriteAnimator _spriteAnimator;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PlayAnimation", 0, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayAnimation()
    {
        _spriteAnimator.Play<SingleAnimator>("Attack").FromStart();
    }
}
