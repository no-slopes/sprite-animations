using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteAnimations.Sampling
{
    public class WinroseAnimationOnDemand : MonoBehaviour
    {
        public SpriteAnimator _animator;
        public int _fps;

        public List<Sprite> _northCycle;
        public List<Sprite> _eastCycle;
        public List<Sprite> _southCycle;
        public List<Sprite> _westCycle;

        private SpriteAnimationWindrose _animation;
        private WindroseAnimator _performer;

        private void Awake()
        {
            Dictionary<WindroseDirection, List<Sprite>> cycles = new()
            {
                { WindroseDirection.North, _northCycle },
                { WindroseDirection.East, _eastCycle },
                { WindroseDirection.South, _southCycle },
                { WindroseDirection.West, _westCycle }
            };

            _animation = SpriteAnimationWindrose.OnDemand(_fps, cycles, true);
        }

        private void Start()
        {
            _performer = _animator.Play<WindroseAnimator>(_animation);
            StartCoroutine(CycleThrough());
        }

        private IEnumerator CycleThrough()
        {
            int counter = 0;

            while (true)
            {
                switch (counter)
                {
                    case 0:
                        _performer.SetDirection(WindroseDirection.East);
                        break;
                    case 1:
                        _performer.SetDirection(WindroseDirection.South);
                        break;
                    case 2:
                        _performer.SetDirection(WindroseDirection.West);
                        break;
                    case 3:
                        _performer.SetDirection(WindroseDirection.North);
                        break;
                }

                yield return new WaitForSeconds(2);

                counter++;
                if (counter == 4)
                {
                    counter = 0;
                }
            }
        }
    }
}
