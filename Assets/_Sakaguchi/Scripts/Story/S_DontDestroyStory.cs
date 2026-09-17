using UnityEngine;

public class S_DontDestroyStory : MonoBehaviour
{
    // シングルトンのインスタンス
    public static S_DontDestroyStory instance;

    private void Awake()
    {
        // すでにインスタンスが存在する場合は、このオブジェクトを破棄する
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // インスタンスを設定し、このオブジェクトをシーン間で保持する
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public S_StoryData story;
    public S_CharacterStory characterStory;
}
