using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DialogFileAndStatusManager
{
    private static DialogFileAndStatusManager instance;

    private Dictionary<string, AdventureDialogStatus> adventureDialogStatusMap = 
        new Dictionary<string, AdventureDialogStatus>();

    private Dictionary<string, int> dialogUseCountMap =
        new Dictionary<string, int>();

    private DialogFileAndStatusManager()
    {
    }

    public static DialogFileAndStatusManager GetInstance()
    {
        if (instance == null)
        {
            instance = new DialogFileAndStatusManager();
        }
        return instance;
    }

    public DialogModel GetTargetAdventureDialog(string dialogId, int targetIndex = 0)
    {
        try
        {
            string dialogFilePath = "DialogFile/Adventure/" + dialogId;
            JArray dialogList = ParseDialogArray(dialogFilePath);

            if (targetIndex > 0)
            {
                return dialogList[targetIndex - 1].ToObject<DialogModel>();
            }

            AdventureDialogStatus status = null;
            if (adventureDialogStatusMap.ContainsKey(dialogId))
            {
                status = adventureDialogStatusMap[dialogId];
            }
            else
            {
                status = new AdventureDialogStatus(dialogList.Count);
                adventureDialogStatusMap[dialogId] = status;
            }

            int dialogIndex = status.GetNextDialogIndex();
            if (dialogIndex < 0)
            {
                return null;
            }
            else
            {
                return dialogList[dialogIndex].ToObject<DialogModel>();
            }
        }
        catch
        {
            return null;
        }
    }

    public List<EnteranceDialog> GetSelfCameraEnteranceDialogList()
    {
        try
        {
            string dialogFilePath = "DialogFile/Entrance/EnteranceDialogs";
            JArray dialogList = ParseDialogArray(dialogFilePath);

            List<EnteranceDialog> dialogModelList = new List<EnteranceDialog>();
            for (int i = 0; i < dialogList.Count; i++)
            {
                DialogModel dialog = dialogList[i]["dialog"].ToObject<DialogModel>();
                string dialogId = dialogList[i]["id"].ToString();
                dialogModelList.Add(new EnteranceDialog(dialogId, dialog));
            }

            return dialogModelList;
        }
        catch
        {
            return null;
        }
    }

    public List<SanctuaryNormalDialog> GetSanctuaryExcessiveDialogList(CharacterType type)
    {
        try
        {
            string characterDir = GetCharacterDir(type);
            string dialogFilePath = "DialogFile/SanctuaryNormal/" + characterDir + "Excessive";
            JArray dialogList = ParseDialogArray(dialogFilePath);

            List<SanctuaryNormalDialog> targetDialogList = new List<SanctuaryNormalDialog>();
            for (int i = 0; i < dialogList.Count; i++)
            {
                targetDialogList.Add(ParseSanctuaryNormalDialog(dialogList[i]));
            }

            return targetDialogList;
        }
        catch
        {
            return new List<SanctuaryNormalDialog>();
        }
    }

    public Queue<SanctuaryNormalDialog> GetSanctuaryNormalDialogList(int progress, CharacterType type)
    {
        try
        {
            List<SanctuaryNormalDialog> dialogCandidateList = new List<SanctuaryNormalDialog>();
            string characterDir = GetCharacterDir(type);
            string dialogFilePathPrefix = "DialogFile/SanctuaryNormal/" + characterDir;
            for (int i = 0; i <= progress; i++)
            {
                JArray dialogList = ParseDialogArray(dialogFilePathPrefix + "Progress" + i);
                for (int j = 0; j < dialogList.Count; j++)
                {
                    dialogCandidateList.Add(ParseSanctuaryNormalDialog(dialogList[j]));
                }
            }

            dialogCandidateList.Sort(delegate (SanctuaryNormalDialog dialogA, SanctuaryNormalDialog dialogB)
            {
                return GetDialogUseCount(dialogA.GetId()) - GetDialogUseCount(dialogB.GetId());
            });

            List<SanctuaryNormalDialog> targetDialogListByUseCount = new List<SanctuaryNormalDialog>();
            Queue<SanctuaryNormalDialog> selectedDialogQueue = new Queue<SanctuaryNormalDialog>();

            int useCount = GetDialogUseCount(dialogCandidateList[0].GetId());
            for (int i = 0; i < dialogCandidateList.Count; i++)
            {
                int currentUseCount = GetDialogUseCount(dialogCandidateList[i].GetId());
                if (useCount == currentUseCount)
                {
                    targetDialogListByUseCount.Add(dialogCandidateList[i]);
                }
                
                if (useCount < currentUseCount || i == dialogCandidateList.Count - 1)
                {
                    while (selectedDialogQueue.Count < 4 && targetDialogListByUseCount.Count > 0)
                    {
                        int random = Random.Range(0, targetDialogListByUseCount.Count);
                        selectedDialogQueue.Enqueue(targetDialogListByUseCount[random]);
                        targetDialogListByUseCount.RemoveAt(random);
                    }

                    if (selectedDialogQueue.Count >= 4)
                    {
                        break;
                    }

                    useCount = GetDialogUseCount(dialogCandidateList[i].GetId());
                    targetDialogListByUseCount.Add(dialogCandidateList[i]);
                }
            }

            return selectedDialogQueue;
            
        }
        catch
        {
            return new Queue<SanctuaryNormalDialog>();
        }
    }

    public EventDialog GetEventDialog(string id, string path)
    {
        try
        {
            string dialogFilePath = "DialogFile/" + path + "/" + id;
            string dialogJson = Resources.Load<TextAsset>(dialogFilePath).text;
            JObject dialogJsonObject = JObject.Parse(dialogJson);

            return ParseEventDialog(dialogJsonObject);
        }
        catch
        {
            return null;
        }
    }

    public (EventDialog, int) GetSanctuaryEventDialog(string id)
    {
        try
        {
            string dialogFilePath = "DialogFile/SanctuaryEvent/" + id;
            string dialogJson = Resources.Load<TextAsset>(dialogFilePath).text;
            JObject dialogJsonObject = JObject.Parse(dialogJson);

            int open = (int)dialogJsonObject["open"];
            return (ParseEventDialog(dialogJsonObject), open);
        }
        catch
        {
            return (null, -1);
        }
    }

    public SanctuaryNormalDialog GetSanctuaryDeathDialog(string id)
    {
        try
        {
            string dialogFilePath = "DialogFile/SanctuaryEvent/Death/" + id;
            JArray dialogArray = ParseDialogArray(dialogFilePath);
            int selectedIndex = Random.Range(0, dialogArray.Count);

            return ParseSanctuaryNormalDialog(dialogArray[selectedIndex]);
        }
        catch
        {
            return null;
        }
    }

    public int GetDialogUseCount(string id)
    {
        return dialogUseCountMap.ContainsKey(id) ? dialogUseCountMap[id] : 0;
    }

    public void UseDialog(string id)
    {
        int useCount = GetDialogUseCount(id);
        dialogUseCountMap[id] = useCount + 1;
    }

    private EventDialog ParseEventDialog(JObject dialogJsonObject)
    {
        JArray modules = dialogJsonObject["modules"] as JArray;
        List<List<DialogModel>> moduleList = new List<List<DialogModel>>();
        for (int i = 0; i < modules.Count; i++)
        {
            List<DialogModel> sequence = new List<DialogModel>();
            JArray dialogList = modules[i]["sequence"] as JArray;
            for (int j = 0; j < dialogList.Count; j++)
            {
                sequence.Add(dialogList[j].ToObject<DialogModel>());
            }
            moduleList.Add(sequence);
        }

        JArray selectionJson = dialogJsonObject["selection"] as JArray;
        Dictionary<string, EventDialogSelection> selectionDictionary = new Dictionary<string, EventDialogSelection>();
        for (int i = 0; i < selectionJson.Count; i++)
        {
            string selectionId = selectionJson[i]["id"].ToString();
            string type = selectionJson[i]["type"].ToString();
            List<(string, string)> options = new List<(string, string)>();

            JArray optionListJson = selectionJson[i]["options"] as JArray;
            for (int j = 0; j < optionListJson.Count; j++)
            {
                options.Add((optionListJson[j]["optionText"].ToString(), optionListJson[j]["moveTarget"].ToString()));
            }
            selectionDictionary.Add(selectionId, new EventDialogSelection(type, options));
        }

        return new EventDialog(moduleList, selectionDictionary);
    }

    private JArray ParseDialogArray(string dialogFilePath)
    {
        string dialogJson = Resources.Load<TextAsset>(dialogFilePath).text;
        JObject dialogJsonObject = JObject.Parse(dialogJson);
        return dialogJsonObject["dialogs"] as JArray;
    }

    private SanctuaryNormalDialog ParseSanctuaryNormalDialog(JToken token)
    {
        string id = token["id"] == null ? "" : token["id"].ToString();
        List<DialogModel> dialogList = new List<DialogModel>();
        JArray sequence = token["sequence"] as JArray;
        for (int i = 0; i < sequence.Count; i++)
        {
            dialogList.Add(sequence[i].ToObject<DialogModel>());
        }

        return new SanctuaryNormalDialog(id, dialogList);
    }

    private string GetCharacterDir(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.WARRIOR:
                return "Warrior/";
            case CharacterType.MAGICIAN:
                return "Magician/";
            case CharacterType.MERCHANT:
                return "Merchant/";
        }
        return "";
    }

    class AdventureDialogStatus
    {
        private Queue<int> indexQueue;
        private float lastDialogTimeStamp;

        private int totalDialogCount = 0;

        public AdventureDialogStatus(int totalCount)
        {
            totalDialogCount = totalCount;
            indexQueue = new Queue<int>();
            lastDialogTimeStamp = -1f;
        }
        
        public int GetNextDialogIndex()
        {
            float currentTime = Time.time;
            if (lastDialogTimeStamp >=0 && currentTime - lastDialogTimeStamp <= 10)
            {
                return -1;
            }

            lastDialogTimeStamp = currentTime;

            if (indexQueue.Count > 0)
            {
                return indexQueue.Dequeue();
            }

            List<int> indexList = new List<int>();
            for (int i = 0; i < totalDialogCount; i++)
            {
                indexList.Add(i);
            }

            for (int i = 0; i < totalDialogCount; i++)
            {
                int random = Random.Range(0, indexList.Count);
                indexQueue.Enqueue(indexList[random]);
                indexList.RemoveAt(random);
            }

            return indexQueue.Dequeue();
        }
    }
}
