namespace HH.Persistence
{
    /// <summary>
    /// データの永続化機能を提供するインターフェース。
    /// データの保存、読み込み、削除を行う。
    /// </summary>
    public interface IDataPersistence
    {
        /// <summary>
        /// 指定されたデータを保存する。
        /// </summary>
        /// <param name="data">保存するデータ</param>
        public void Save<T>(T data);

        /// <summary>
        /// 保存されたデータを読み込む。
        /// </summary>
        /// <returns></returns>
        public T Load<T>() where T : new();

        /// <summary>
        /// 保存されたデータを削除する。
        /// </summary>
        public void Delete();

        /// <summary>
        /// 保存ファイルがあるかどうか
        /// </summary>
        public bool IsSavaFile();
    }
}