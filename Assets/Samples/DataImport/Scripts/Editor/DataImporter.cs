using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HachiKuGames.DataImport
{
    /// <summary>
    /// データの取り込み
    /// </summary>
    public class DataImporter
    {
        private const string SampleDataDirectory = "Assets/Samples/DataImport/Data";
        private const string SampleDataFilename = "Sample.csv";

        /// <summary>
        /// 取り込みメニュー
        /// </summary>
        [MenuItem("HachiKuGames/Import Data")]
        private static void Import()
        {
            ImportCSV();
            EditorUtility.DisplayDialog($"Import SampleData", "Import SampleData Completed", "OK");
        }

        /// <summary>
        /// CSVの取り込み
        /// </summary>
        private static void ImportCSV()
        {
            // CSVを読み込み
            var read = File.ReadAllText(Path.Combine(SampleDataDirectory, SampleDataFilename), Encoding.UTF8);

            // 行毎のデータに分割
            var rows = read.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < rows.Length; i++)
            {
                // 1行目は項目名を表示しているので除外
                if (i == 0)
                    continue;

                // 列毎のデータに分割
                var columns = rows[i].Split(",");

                // データを取り込み
                // 2列目は表示用の確率なので除外
                var filename = columns[0];
                var importData = ScriptableObject.CreateInstance<ImportData>();
                importData.SetData(int.Parse(columns[1]), int.Parse(columns[3]));

                // ファイルに書き込み
                AssetDatabase.CreateAsset(importData, Path.Combine(SampleDataDirectory, $"{filename}.asset"));
            }

            // metaファイルのguidは変更されないが、Missingになる場合があるのでImportし直す
            AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive);
        }
    }
}