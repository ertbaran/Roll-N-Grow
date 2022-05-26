using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControllerNameSpace;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveLimitX;

    float currentX;
    float moveFactor = 100;

    [SerializeField] Transform _transform;
    [SerializeField] Rigidbody _rigidbody;

    Vector2 moveVal;

    private void FixedUpdate()
    {
        MoveLimit();
    }

    void MoveLimit()
    {
        currentX = Mathf.Clamp(_transform.position.x, -moveLimitX, moveLimitX);
        _transform.position = new Vector3(currentX, _transform.position.y, _transform.position.z);
    }

    public void Moving(InputAction.CallbackContext value)
    {
        if (value.performed & GameManager.gameState == GameManager.GameState.Started)
        {
            moveVal = value.ReadValue<Vector2>();

            _rigidbody.AddForce(new Vector3(moveVal.x * moveSpeed / moveFactor, 0, 0), ForceMode.Impulse);
        }
    }

}