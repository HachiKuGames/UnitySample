using UnityEngine;
using UnityEngine.EventSystems;

namespace HachiKuGames.Common
{
    /// <summary>
    /// カメラ操作
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// ドラッグでの回転速度
        /// </summary>
        private const float DragAngleSpeed = 0.04f;
        /// <summary>
        /// キーボードでの回転速度
        /// </summary>
        private const float KeybaordAngleSpeed = 2f;

        /// <summary>
        /// 現在のカメラ
        /// </summary>
        private Vector3 _cameraAngle;

        private void Update()
        {
            UpdateCameraControll();
        }

        /// <summary>
        /// キーボード操作
        /// </summary>
        private void UpdateCameraControll()
        {
            var xAngle = 0f;
            if (Input.GetKey(KeyCode.S))
            {
                xAngle += KeybaordAngleSpeed;
            }
            if (Input.GetKey(KeyCode.W))
            {
                xAngle -= KeybaordAngleSpeed;
            }

            var yAngle = 0f;
            if (Input.GetKey(KeyCode.D))
            {
                yAngle += KeybaordAngleSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                yAngle -= KeybaordAngleSpeed;
            }

            var zAngle = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                zAngle += KeybaordAngleSpeed;
            }
            if (Input.GetKey(KeyCode.E))
            {
                zAngle -= KeybaordAngleSpeed;
            }

            SetCameraAngle(xAngle, yAngle, zAngle);
        }

        /// <summary>
        /// カメラ角度を反映
        /// </summary>
        private void ApplyCameraAngle()
        {
            Camera.main.transform.rotation = Quaternion.Euler(_cameraAngle);
        }

        /// <summary>
        /// カメラ角度を設定
        /// </summary>
        /// <param name="xAngle">X方向</param>
        /// <param name="yAngle">Y方向</param>
        /// <param name="zAngle">Z方向</param>
        private void SetCameraAngle(float xAngle, float yAngle, float zAngle)
        {
            _cameraAngle.x = Mathf.Clamp(_cameraAngle.x + xAngle, -80, 80);
            _cameraAngle.y = _cameraAngle.y + yAngle;
            _cameraAngle.z = Mathf.Clamp(_cameraAngle.z + zAngle, -80, 80);
            ApplyCameraAngle();
        }

        /// <summary>
        /// XY方向のカメラ角度を設定
        /// </summary>
        /// <param name="xAngle">X方向</param>
        /// <param name="yAngle">Y方向</param>
        private void SetCameraAngleXY(float xAngle, float yAngle)
        {
            SetCameraAngle(xAngle, yAngle, _cameraAngle.z);
        }

        /// <summary>
        /// ドラッグ操作
        /// </summary>
        /// <param name="eventData">ドラッグ情報</param>
        public void OnDrag(BaseEventData eventData)
        {
            var pointerEventData = eventData as PointerEventData;
            if (pointerEventData == null)
                return;
            var dragDelta = pointerEventData.delta;
            SetCameraAngleXY(-dragDelta.y * DragAngleSpeed, dragDelta.x * DragAngleSpeed);
        }
    }
}