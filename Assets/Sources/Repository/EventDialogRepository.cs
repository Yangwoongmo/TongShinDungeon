using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventDialogRepository
{
    private static EventDialogRepository instance;

    private const string EventDialogPrefKeySuffix = "sanctuary_event_dialog";
    private const char DialogIdDelemeter = ',';

    private EventDialogRepository()
    {
    }

    public static EventDialogRepository GetInstance()
    {
        if (instance == null)
        {
            instance = new EventDialogRepository();
        }
        return instance;
    }

    public void SetNeedToShowDilaog(string dialogId, CharacterType characterType)
    {
        bool isDialogShown = PlayerPrefs.GetInt(dialogId, 0) > 0;
        if (isDialogShown)
        {
            return;
        }

        string dialogKey = GetDialogPrefKey(characterType);
        string ids = PlayerPrefs.GetString(dialogKey, "");
        string nextIdString = ids.Length > 0 ? DialogIdDelemeter + dialogId : dialogId;

        PlayerPrefs.SetString(dialogKey, ids + nextIdString);
        PlayerPrefs.SetInt(dialogId, 1);
    }

    public string[] GetActiveEventDialogList(CharacterType characterType)
    {
        string[] activeList = PlayerPrefs.GetString(GetDialogPrefKey(characterType), "").Split(DialogIdDelemeter);

        return activeList[0].Length == 0 ? new string[0] : activeList;
    }

    public void SetEventDialogShown(string dialogId, CharacterType characterType)
    {
        string[] dialogIdList = GetActiveEventDialogList(characterType);
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < dialogIdList.Length; i++)
        {
            string id = dialogIdList[i];
            if (id != dialogId)
            {
                builder.Append(id + DialogIdDelemeter);
            }
        }

        int totalLength = builder.ToString().Length;
        if (totalLength > 0)
        {
            builder.Remove(builder.ToString().Length - 1, 1);
        }
        
        PlayerPrefs.SetString(GetDialogPrefKey(characterType), builder.ToString());
    }

    private string GetDialogPrefKey(CharacterType characterType)
    {
        return characterType.ToString() + "/" + EventDialogPrefKeySuffix;
    }
}
