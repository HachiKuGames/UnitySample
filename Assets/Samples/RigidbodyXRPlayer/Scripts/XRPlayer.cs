using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace HachiKuGames.XR
{
    /// <summary>
    /// XR用のプレイヤー
    /// </summary>
    public class XRPlayer : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset _inputAction;

        [SerializeField]
        private InputActionReference _moveAction;
        [SerializeField]
        private InputActionReference _turnAction;

        /// <summary>
        /// 頭の位置
        /// </summary>
        [SerializeField]
        private Transform _head;
        /// <summary>
        /// 首の位置
        /// </summary>
        [SerializeField]
        private Transform _neck;

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private CapsuleCollider _bodyCollider;

        private Vector2 _inputMove;

        private void OnEnable()
        {
            _inputAction.Enable();

            _moveAction.action.performed += OnMoveActionEnter;
            _moveAction.action.canceled += OnMoveActionExit;

            _turnAction.action.performed += OnTurnActionEnter;
            _turnAction.action.canceled += OnTurnActionExit;
        }

        private void OnDisable()
        {
            _inputAction.Enable();

            _moveAction.action.performed -= OnMoveActionEnter;
            _moveAction.action.canceled -= OnMoveActionExit;

            _turnAction.action.performed -= OnTurnActionEnter;
            _turnAction.action.canceled -= OnTurnActionExit;
        }

        private void Update()
        {
            UpdateMove();
        }

        private void LateUpdate()
        {
            UpdateBodyCollider();
        }

        public void Move(Vector3 direction, float speed)
        {
            var moveVelocity = direction * speed;
            var velocity = _rigidbody.velocity;
            velocity.x = moveVelocity.x;
            velocity.z = moveVelocity.z;
            _rigidbody.velocity = velocity;
        }

        public void StopMove()
        {
            var velocity = _rigidbody.velocity;
            velocity.x = 0;
            velocity.z = 0;
            _rigidbody.velocity = velocity;
        }

        public void Turn(float angle)
        {
            transform.RotateAround(_head.position, Vector3.up, angle);
        }

        private void UpdateMove()
        {
            if (_inputMove.sqrMagnitude > 0)
            {
                // 顔の向きの水平方向を計算
                var forwardDirection = _head.forward;
                forwardDirection.y = 0;
                forwardDirection.Normalize();
                forwardDirection *= _inputMove.y;
                var rightDirection = _head.right;
                rightDirection.y = 0;
                rightDirection.Normalize();
                rightDirection *= _inputMove.x;

                // 実際の移動方向を計算
                var inputXMagnitude = Mathf.Abs(_inputMove.x);
                var inputYMagnitude = Mathf.Abs(_inputMove.y);
                var moveDirection = Vector3.Slerp(forwardDirection, rightDirection, inputXMagnitude / (inputXMagnitude + inputYMagnitude));

                Move(moveDirection, 3f);
            }
            else
            {
                StopMove();
            }
        }

        private void UpdateBodyCollider()
        {
            // 基準点の位置へ胴体のコライダーを合わせる
            var rootPosition = transform.InverseTransformPoint(_neck.position);
            _bodyCollider.height = rootPosition.y;
            _bodyCollider.center = new Vector3(rootPosition.x, _bodyCollider.height * 0.5f, rootPosition.z);
        }

        protected virtual void OnMoveActionEnter(CallbackContext context)
        {
            // 入力を記録しておく
            _inputMove = context.ReadValue<Vector2>();
        }

        protected virtual void OnMoveActionExit(CallbackContext context)
        {
            _inputMove = Vector2.zero;
        }

        protected virtual void OnTurnActionEnter(CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            Turn(45f * (input.x < 0 ? -1f : 1f));
        }

        protected virtual void OnTurnActionExit(CallbackContext context)
        {
        }
    }
}