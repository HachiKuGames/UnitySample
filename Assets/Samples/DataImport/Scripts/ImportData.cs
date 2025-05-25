using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HachiKuGames.DataImport
{
    /// <summary>
    /// インポートしたデータ
    /// </summary>
    public class ImportData : ScriptableObject
    {
        /// <summary>
        /// 確率値
        /// </summary>
        [SerializeField]
        private int _odds;

        /// <summary>
        /// パラメータ
        /// </summary>
        [SerializeField]
        private int _parameter;

        // エディタ上で取り込む際にのみデータを設定できるようにする
#if UNITY_EDITOR
        public void SetData(int odds, int parameter)
        {
            _odds = odds;
            _parameter = parameter;
        }
#endif
    }
}