using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDialog
{
    private CharacterType cameraHolder;
    private DialogModel firstDialog;
    private List<List<DialogModel>> dialogModule;
    private Dictionary<string, EventDialogSelection> selections;

    public EventDialog(List<List<DialogModel>> module, Dictionary<string, EventDialogSelection> selections)
    {
        this.dialogModule = new List<List<DialogModel>>();
        for (int i = 0; i < module.Count; i++)
        {
            List<DialogModel> dialogList = new List<DialogModel>();
            dialogList.AddRange(module[i]);
            this.dialogModule.Add(dialogList);
        }

        this.selections = new Dictionary<string, EventDialogSelection>();
        foreach (KeyValuePair<string, EventDialogSelection> pair in selections)
        {
            this.selections.Add(pair.Key, pair.Value);
        }

        this.firstDialog = dialogModule[0][0];
        this.cameraHolder = firstDialog.GetSpeaker();  
    }

    public CharacterType GetCameraHolder()
    {
        return cameraHolder;
    }

    public DialogModel GetFirstDialog()
    {
        return firstDialog;
    }

    public List<DialogModel> GetNextDialogModule(int moduleIndex)
    {
        return dialogModule[moduleIndex];
    }

    public EventDialogSelection GetNextSelection(string selectionId)
    {
        return selections.ContainsKey(selectionId) ? selections[selectionId] : null;
    }
}
