using System;
using System.IO;
using UnityEngine;

namespace HH.Persistence
{
    /// <summary>
    /// 保存形式
    /// </summary>
    public enum PersistenceType
    {
        Json,
        NewtonsoftJson,
    };

    /// <summary>
    /// データ保存先を生成するクラス
    /// </summary>
    public static class DataPersistenceFactory
    {
        /// <summary>
        /// ファイルを作成
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="type">保存形式</param>
        /// <returns>Json形式</returns>
        public static IDataPersistence CreatePersistence(string fileName, PersistenceType type)
        {
            string rootPath =
#if UNITY_EDITOR
            // エディタ上での保存先
            Application.dataPath;
            fileName = GetEditorScopedFileName(fileName);
#else
            Application.persistentDataPath;
#endif

            string filePath = Path.Combine(rootPath, "SaveData", fileName);

            return type switch
            {
                PersistenceType.Json => new JsonDataPersistence(filePath),
                PersistenceType.NewtonsoftJson => new NewtonsoftJsonDataPersistence(filePath),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        // エディタ環境では、仮想インスタンス名をファイル名に付加して、インスタンスごとに異なる保存先を使用します。
        private static string GetEditorScopedFileName(string fileName)
        {
            // Mainは元のファイル名を使う
            if (string.IsNullOrEmpty(fileName))
            {
                return fileName;
            }

            string extension = Path.GetExtension(fileName);
            return $"{fileName}_Editor{extension}";
        }
    }
}