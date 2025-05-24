using UnityEngine;

namespace HachiKuGames.RecordingCamera
{
    /// <summary>
    /// 録画用カメラ
    /// </summary>
    public class RecordingCamera : MonoBehaviour
    {
        /// <summary>
        /// 録画を行うカメラ
        /// </summary>
        [SerializeField]
        private Camera _targetCamera;
        /// <summary>
        /// 録画内容の出力先
        /// </summary>
        [SerializeField]
        private RenderTexture _renderTexture;

        private void Start()
        {
            // メインカメラの設定をコピー
            _targetCamera.CopyFrom(Camera.main);
            // 出力先を変更
            _targetCamera.targetTexture = _renderTexture;
        }

        private void LateUpdate()
        {
            // メインカメラの位置と角度を元に水平に設定
            _targetCamera.transform.position = Camera.main.transform.position;
            _targetCamera.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }
    }
}