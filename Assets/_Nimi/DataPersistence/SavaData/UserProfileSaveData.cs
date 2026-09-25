namespace HH.Persistence
{
    public class UserProfileSaveData : PersistenceBase<UserProfileData>
    {
        protected override string FileName => "UserProfileSaveData";
        protected override PersistenceType PersistenceType => PersistenceType.Json;

    }

    /// <summary>
    /// ユーザープロフィールのデータ構造
    /// </summary>
    public sealed class UserProfileData
    {
        // プレイヤー名
        public const string DefaultPlayerName = "Unknown Name";
        public string PlayerName = DefaultPlayerName;
    }
}