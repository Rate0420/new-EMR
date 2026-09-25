using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace HH.Persistence
{
    /// <summary>
    /// Newtonsoft.Json形式でデータを永続化する。
    /// </summary>
    public sealed class NewtonsoftJsonDataPersistence : IDataPersistence
    {
        /// <summary>
        /// ファイル拡張子
        /// </summary>
        public const string FileExtension = ".json";

        /// <summary>
        /// ファイルの保存先
        /// </summary>
        public readonly string FileSaveDestination = "";


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileSaveDestination">ファイルの保存先</param>
        public NewtonsoftJsonDataPersistence(string fileSaveDestination)
        {
            FileSaveDestination = fileSaveDestination + FileExtension;
        }


        public void Save<T>(T data)
        {
            string directory = Path.GetDirectoryName(FileSaveDestination);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(
                data,
                Formatting.Indented);

            File.WriteAllText(FileSaveDestination, json);

            Debug.Log($"保存しました : {FileSaveDestination}");
        }

        public T Load<T>() where T : new()
        {
            if (!IsSavaFile())
            {
                Debug.LogWarning($"保存データが見つかりませんでした。{FileSaveDestination}");
                return default;
            }

            string json = File.ReadAllText(FileSaveDestination);

            Debug.Log($"読み込みました : {FileSaveDestination}");

            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Delete()
        {
            if (File.Exists(FileSaveDestination))
            {
                File.Delete(FileSaveDestination);
                Debug.Log($"保存データを削除しました : {FileSaveDestination}");
            }
            else
            {
                Debug.LogWarning($"保存データの削除するファイルが見つかりませんでした : {FileSaveDestination}");
            }
        }

        public bool IsSavaFile() => File.Exists(FileSaveDestination);
    }
}