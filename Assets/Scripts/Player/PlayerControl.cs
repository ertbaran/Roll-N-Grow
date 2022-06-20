using UnityEngine;
using UnityEngine.InputSystem;

namespace GameManagerNamespace
{
    public class PlayerControl : MonoBehaviour
    {
        public static PlayerControl Instance = null;

        [SerializeField] float moveSpeed;
        [SerializeField] float moveLimitX;

        float currentX;
        float moveFactor = 100;
        public bool canMove = true;

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

        public void Move(InputAction.CallbackContext value)
        {
            if (value.performed & GameManager.Instance.gameState == GameManager.GameState.Started && canMove)
            {
                moveVal = value.ReadValue<Vector2>();
                _rigidbody.AddForce(new Vector3(moveVal.x * moveSpeed / moveFactor, 0, 0), ForceMode.Impulse);
                Debug.Log(moveVal);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}

