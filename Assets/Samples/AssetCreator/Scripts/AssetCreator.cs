using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace HachiKuGames.AssetCreator
{
    /// <summary>
    /// アセット生成
    /// </summary>
    public class AssetCreator : MonoBehaviour
    {
        private const string RootDirectory = "_Asset";

        /// <summary>
        /// 生成したいアセットの情報
        /// </summary>
        [SerializeField]
        private AssetGroup[] _assetGroups;

        private List<RenderTexture> _renderTextureList = new List<RenderTexture>();

        private void Start()
        {
            // 起動と同時にアセットを作成
            CreateAsset();
        }

        private async void CreateAsset()
        {
            // 不要なものが映り込まないように非表示にする
            foreach (var group in _assetGroups)
            {
                group.Root.SetActive(false);
            }

            // 各アセットを生成
            foreach (var group in _assetGroups)
            {
                if (group.Enable == false)
                    continue;
                await Capture(group);
            }

            // アセットの作成が完了したら自動で停止する
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private async Task Capture(AssetGroup group)
        {
            var width = group.Size.x;
            var height = group.Size.y;

            group.Root.SetActive(true);

            SetRenderTexture(group);

            // 先頭のカメラから書き込む
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            await Awaitable.EndOfFrameAsync();
            RenderTexture.active = group.Cameras[0].targetTexture;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            WriteTexture(tex, group.ID);

            Destroy(tex);

            // RenderTextureを待機
            await Awaitable.EndOfFrameAsync();

            group.Root.SetActive(false);
        }

        private void SetRenderTexture(AssetGroup group)
        {
            for (var i = 0; i < group.Cameras.Length; i++)
            {
                var camera = group.Cameras[i];
                camera.targetTexture = null;
                camera.targetTexture = GetRenderTexture(i, group.Size.x, group.Size.y);
            }
        }

        private RenderTexture GetRenderTexture(int index, int width, int height)
        {
            // 必要な分RenderTextureを生成
            for (var i = 0; i <= index; i++)
            {
                if (index >= _renderTextureList.Count)
                {
                    _renderTextureList.Add(
                        new RenderTexture(width, height, 0)
                        {
                            depthStencilFormat = GraphicsFormat.D32_SFloat_S8_UInt,
                            graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB,
                        });
                }
            }

            // RenderTextureをリサイズ
            var renderTexture = _renderTextureList[index];
            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
            renderTexture.Create();

            return renderTexture;
        }

        private void WriteTexture(Texture2D tex, string filename)
        {
            var bytes = tex.EncodeToPNG();
            if (Directory.Exists(RootDirectory) == false)
            {
                Directory.CreateDirectory(RootDirectory);
            }
            File.WriteAllBytes(Path.Combine(RootDirectory, $"{filename}.png"), bytes);
        }

        [Serializable]
        public class AssetGroup
        {
            public bool Enable = true;
            public string ID;
            public GameObject Root;
            public Vector2Int Size;
            public Camera[] Cameras;
        }
    }
}