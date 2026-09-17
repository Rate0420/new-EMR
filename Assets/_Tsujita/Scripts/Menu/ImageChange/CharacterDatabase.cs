using UnityEngine;

public class CharacterDatabase : MonoBehaviour
{
    [SerializeField] private CharacterData[] characterData;

    public CharacterData GetCharacter(CharactorType ctype)
    {
        foreach (CharacterData data in characterData)
        {
            if (data.charactorType == ctype)
                return data;
        }
        return null;
    }
}
