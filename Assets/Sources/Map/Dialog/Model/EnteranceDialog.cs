using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteranceDialog
{
    private string id;
    private int progress;
    private int stage;
    private int hp;
    private int kill;
    private DialogModel dialog;
    private CharacterType cameraHolder;

    public EnteranceDialog(string dialogId, DialogModel dialog)
    {
        this.id = dialogId;
        this.progress = int.Parse(dialogId[0].ToString());
        this.stage = int.Parse(dialogId[1].ToString());
        this.hp = int.Parse(dialogId[2].ToString());
        this.kill = int.Parse(dialogId[3].ToString());
        this.dialog = dialog;
        this.cameraHolder = dialog.GetSpeaker();
    }

    public bool CanUseDialog(int progress, int stage, int hp, int kill)
    {
        return this.progress <= progress && this.stage <= stage && this.hp <= hp && this.kill <= kill;
    }

    public DialogModel GetDialog()
    {
        return dialog;
    }

    public CharacterType GetCameraHolder()
    {
        return cameraHolder;
    }

    public string GetId()
    {
        return id;
    }
}
