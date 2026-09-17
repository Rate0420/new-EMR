using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターごと・ステータス種別ごとの数値を管理するシングルトン。
/// 内部は Dictionary[characterId][statusKey] = value の二重辞書。
/// ChooseManager から ApplyDeltas() を呼んで変化させる。
/// </summary>
public class S_AffinityManager : MonoBehaviour
{
    public static S_AffinityManager Instance { get; private set; }

    // characterId → (statusKey → value)
    private Dictionary<string, Dictionary<string, int>> _data
        = new Dictionary<string, Dictionary<string, int>>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ----------------------------------------------------------------
    //  書き込み
    // ----------------------------------------------------------------

    /// <summary>
    /// ChoiceEntry の statusDeltas をまとめて適用する。
    /// ChooseManager から渡されたリストをそのまま渡せばOK。
    /// </summary>
    public void ApplyDeltas(List<S_StoryData.StatusDelta> deltas)
    {
        if (deltas == null) return;
        foreach (var d in deltas)
            Add(d.characterId, d.statusKey, d.delta);
    }

    /// <summary> キャラID・ステータスキーを指定して値を加算する </summary>
    public void Add(string characterId, string statusKey, int delta)
    {
        if (string.IsNullOrEmpty(characterId) || string.IsNullOrEmpty(statusKey)) return;

        if (!_data.ContainsKey(characterId))
            _data[characterId] = new Dictionary<string, int>();

        if (!_data[characterId].ContainsKey(statusKey))
            _data[characterId][statusKey] = 0;

        _data[characterId][statusKey] += delta;

        Debug.Log($"[AffinityManager] {characterId}.{statusKey}: {_data[characterId][statusKey]} ({(delta >= 0 ? "+" : "")}{delta})");
    }

    /// <summary> 値を直接セットする </summary>
    public void Set(string characterId, string statusKey, int value)
    {
        if (string.IsNullOrEmpty(characterId) || string.IsNullOrEmpty(statusKey)) return;

        if (!_data.ContainsKey(characterId))
            _data[characterId] = new Dictionary<string, int>();

        _data[characterId][statusKey] = value;
    }

    // ----------------------------------------------------------------
    //  読み込み
    // ----------------------------------------------------------------

    /// <summary> 値を取得する。未初期化なら 0 を返す </summary>
    public int Get(string characterId, string statusKey)
    {
        if (_data.TryGetValue(characterId, out var statuses))
            if (statuses.TryGetValue(statusKey, out int value))
                return value;
        return 0;
    }

    /// <summary> 指定キャラの全ステータスを取得する（UI表示などに使用）</summary>
    public IReadOnlyDictionary<string, int> GetAllStatuses(string characterId)
    {
        if (_data.TryGetValue(characterId, out var statuses))
            return statuses;
        return new Dictionary<string, int>();
    }

    // ----------------------------------------------------------------
    //  リセット
    // ----------------------------------------------------------------

    /// <summary> 特定キャラの全ステータスをリセット </summary>
    public void ResetCharacter(string characterId)
    {
        if (_data.ContainsKey(characterId))
            _data[characterId].Clear();
    }

    /// <summary> 全データをリセット </summary>
    public void ResetAll() => _data.Clear();
}
