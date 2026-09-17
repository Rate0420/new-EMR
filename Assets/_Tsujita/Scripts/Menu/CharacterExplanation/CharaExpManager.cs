using UnityEngine;

public class CharaExpManager : MonoBehaviour
{
    [SerializeField] CharacterDatabase database;

    // ‰¼’u‚«
    private int storyNo;
    private string characterName;

    private string nowChara;

    private void Start()
    {
        nowChara = characterName;
        CN(characterName);
    }

    private void CN(string characterName)
    {
        //CharacterData character = database.GetCharacter(characterName);
        
    }

    private void TextUpdate()
    {
        switch (storyNo)
        {
            case 3:
                break;
        }
    }
}
