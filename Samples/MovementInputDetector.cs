using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementInputDetector : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Vector2, Vector2Int> _movmentSignChanged;

    private Vector2 _movementInput;
    private Vector2Int _movementInputSign;

    public Vector2 Movement => _movementInput;
    public Vector2Int MovementSign => _movementInputSign;
    public UnityEvent<Vector2, Vector2Int> MovementSignChanged => _movmentSignChanged;

    private void Update()
    {
        // This code is just a simple way to detect directions so I can 
        // change the animation being played based on that. This should not be
        // used as base to create your own movement system.

        // The idea here is that we get the Input every frame but we will only fire the
        // event if hte change on input means the direction has changed.

        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        _movementInput = new Vector2(xAxis, yAxis);

        int x = xAxis == 0 ? 0 : xAxis > 0 ? 1 : -1;
        int y = yAxis == 0 ? 0 : yAxis > 0 ? 1 : -1;

        Vector2Int movementInputSign = new(x, y);

        if (_movementInputSign != movementInputSign)
        {
            _movementInputSign = movementInputSign;
            _movmentSignChanged.Invoke(_movementInput, _movementInputSign);
        }
    }
}
