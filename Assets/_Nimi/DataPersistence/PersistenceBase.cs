namespace HH.Persistence
{
    /// <summary>
    /// データ永続化クラスの基底クラス。
    /// 継承先では保存ファイル名と保存形式を定義するだけで、
    /// データの保存・読み込み・削除機能を利用できる。
    ///
    /// 例:
    /// <code>
    /// public sealed class PlayerDataPersistence
    ///     : PersistenceBase<PlayerData>
    /// {
    ///     protected override string FileName => "PlayerData";
    ///
    ///     protected override PersistenceType PersistenceType
    ///         => PersistenceType.Json;
    /// }
    /// </code>
    ///
    /// T は保存対象のデータ型を表す。
    /// </summary>
    /// <typeparam name="T">保存対象のデータ型</typeparam>
    public abstract class PersistenceBase<T> where T : new()
    {
        /// <summary>
        /// 保存ファイル名。
        /// </summary>
        protected abstract string FileName { get; }

        /// <summary>
        /// 保存形式。既定値は Json。
        /// </summary>
        protected virtual PersistenceType PersistenceType => PersistenceType.Json;

        /// <summary>
        /// 実際のファイル操作を担当する永続化オブジェクト。
        /// </summary>
        private IDataPersistence _persistence;


        /// <summary>
        /// 永続化オブジェクトを取得する。
        /// </summary>
        protected IDataPersistence Persistence =>
            _persistence ??=
                DataPersistenceFactory.CreatePersistence(
                    FileName,
                    PersistenceType);

        /// <summary>
        /// 保存先にファイルがあるかどうか
        /// </summary>
        public bool IsSaveFile => Persistence.IsSavaFile();


        /// <summary>
        /// ファイルからデータを読み込み、キャッシュも更新する。
        /// キャッシュを無視して再読み込みしたい場合に使用する。
        /// </summary>
        public T Load()
        {
            return Persistence.Load<T>();
        }

        /// <summary>
        /// 指定データをファイルへ保存し、キャッシュも更新する。
        /// </summary>
        /// <param name="data">保存するデータ</param>
        public void Save(T data)
        {
            Persistence.Save(data);
        }


        /// <summary>
        /// 保存ファイルを削除し、キャッシュも破棄する。
        /// 次回取得時には再度 Load が実行される。
        /// </summary>
        public void Delete() => Persistence.Delete();
    }
}