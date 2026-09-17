using UnityEngine;
[CreateAssetMenu(fileName = "S_CharacterStory", menuName = "Game/S_CharacterStory")]

public class S_CharacterStory : ScriptableObject
{
    public string characterName; // キャラクターの名前
    public S_StoryData[] storyParts; // キャラクターのストーリーの各部分
}
