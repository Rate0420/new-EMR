using System.IO;
using UnityEngine;

namespace HH.Persistence
{
    /// <summary>
    /// Json形式でデータを永続化を行う。
    /// </summary>
    public sealed class JsonDataPersistence : IDataPersistence
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
        /// <param name="fileName">ファイル名</param>
        /// <param name="fileSaveDestination">ファイルの保存先</param>
        public JsonDataPersistence(string fileSaveDestination)
        {
            FileSaveDestination = fileSaveDestination + FileExtension;
        }


        /// <summary>
        /// JSON方式で保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void Save<T>(T data)
        {
            // 保存先フォルダを取得
            string directory = Path.GetDirectoryName(FileSaveDestination);

            // フォルダが存在しない場合は作成
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // dataをJson形式に変換
            string json = JsonUtility.ToJson(data);

            File.WriteAllText(FileSaveDestination, json);

            Debug.Log($"保存しました : {FileSaveDestination}");
        }


        /// <summary>
        /// JSONファイルを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>() where T : new()
        {
            // ファイルが存在しない場合は空文字を返す
            if (!IsSavaFile())
            {
                Debug.Log($"保存データがないため、デフォルト値を使用します: {FileSaveDestination}");

                T defaultData = new T();
                return defaultData;
            }

            string json = File.ReadAllText(FileSaveDestination);

            if (string.IsNullOrWhiteSpace(json))
            {
                T defaultData = new T();
                return defaultData;
            }

            T data = JsonUtility.FromJson<T>(json);

            if (data == null)
            {
                T defaultData = new T();
                return defaultData;
            }

            return data;
        }

        /// <summary>
        /// 保存データを削除
        /// </summary>
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

        /// <summary>
        /// 保存ファイルがあるかどうか
        /// 保存ファイルがある場合:true
        /// </summary>
        public bool IsSavaFile() => File.Exists(FileSaveDestination);
    }
}