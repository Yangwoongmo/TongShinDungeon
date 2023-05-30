using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PlayerSelfCameraDialogGenerator
{
    private const string WarriorSecondaryDialog = "아하!";
    private const string MagicianSecondaryDialog = "알았어요";
    private readonly string[] WarriorRandomDialog = new string[3]
    {
        "잠시만요...",
        "제 감에 의하면...",
        "으으음..."
    };
    private readonly string[] WarriorResultDialog = new string[5]
    {
        "중요한 물건이 있는 것 같아요!",
        "성스러운 기운이 느껴져요!",
        "아마도 잠긴 문이 있는 것 같아요!",
        "여러 일이 보여서 잘 모르겠어요",
        "여긴 이제 아무것도 없네요!"
    };
    private readonly string[] MagicianRandomDialog = new string[3]
    {
        "주변을 좀 볼게요",
        "주변의 습기를 생각했을 때...",
        "벽의 이끼를 보면..."
    };
    private readonly string[] MagicianResultDialog = new string[2]
    {
        "가 남아 있을 것으로 생각되요",
        "이 곳의 영식물은 전부 채집한 것 같아요"
    };

    public List<DialogModel> GenerateWarriorDialog(InteractionObjectCountStorage storage)
    {
        List<DialogModel> dialogList = new List<DialogModel>();

        dialogList.Add(new DialogModel(WarriorRandomDialog[Random.Range(0, 3)], CharacterType.WARRIOR, 1, 1, 1, 1));
        dialogList.Add(new DialogModel(WarriorSecondaryDialog, CharacterType.WARRIOR, 1, 1, 1, 1));
        dialogList.Add(new DialogModel(GetWarriorResultDialog(storage), CharacterType.WARRIOR, 1, 1, 1, 1));

        return dialogList;
    }

    public List<DialogModel> GenerateMagicianDialog(InteractionObjectCountStorage storage)
    {
        List<DialogModel> dialogList = new List<DialogModel>();

        dialogList.Add(new DialogModel(MagicianRandomDialog[Random.Range(0, 3)], CharacterType.MAGICIAN, 1, 1, 1, 1));
        dialogList.Add(new DialogModel(MagicianSecondaryDialog, CharacterType.MAGICIAN, 1, 1, 1, 1));
        dialogList.Add(new DialogModel(GetMagicianResultDialog(storage), CharacterType.MAGICIAN, 1, 1, 1, 1));

        return dialogList;
    }

    private string GetWarriorResultDialog(InteractionObjectCountStorage storage)
    {
        int keyItemCount = storage.GetKeyItemsCount();
        int sanctuaryPortalCount = storage.GetSanctuaryPortalCount();
        int lockedDoorCount = storage.GetLockedDoorCount();

        if (keyItemCount > 0)
        {
            if (sanctuaryPortalCount > 0 || lockedDoorCount > 0)
            {
                return WarriorResultDialog[3];
            }
            else
            {
                return WarriorResultDialog[0];
            }
        }
        else if (sanctuaryPortalCount > 0)
        {
            if (keyItemCount > 0 || lockedDoorCount > 0)
            {
                return WarriorResultDialog[3];
            }
            else
            {
                return WarriorResultDialog[1];
            }
        }
        else if (lockedDoorCount > 0)
        {
            if (keyItemCount > 0 || sanctuaryPortalCount > 0)
            {
                return WarriorResultDialog[3];
            }
            else
            {
                return WarriorResultDialog[2];
            }
        }
        else
        {
            return WarriorResultDialog[4];
        }
    }

    private string GetMagicianResultDialog(InteractionObjectCountStorage storage)
    {
        Dictionary<string, int> plantItemDict = storage.GetPlantItemDict();
        if (plantItemDict.Count > 0)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, int> item in plantItemDict)
            {
                builder.Append(item.Key).Append(" ").Append(item.Value.ToString()).Append("개, ");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append(MagicianResultDialog[0]);

            return builder.ToString();
        }
        else
        {
            return MagicianResultDialog[1];
        }
    }
}
