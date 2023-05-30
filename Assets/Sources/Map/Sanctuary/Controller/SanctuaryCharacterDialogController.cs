using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryCharacterDialogController : SanctuaryController, IPlayerEventDialogListener
{
    [SerializeField] private GameObject dialogButton;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private GameObject eventMarkImage;
    [SerializeField] protected SanctuaryDialogController dialogController;

    protected Player player;

    private List<string> eventDialogList = new List<string>();
    private EventDialogRepository dialogRepository = EventDialogRepository.GetInstance();
    private PlayerManager manager = PlayerManager.GetInstance();

    public override void SetEnable(bool isEnabled)
    {
        base.SetEnable(isEnabled);
        dialogButton.SetActive(isEnabled);
        eventMarkImage.SetActive(isEnabled && eventDialogList.Count > 0);
        dialogController.HideDialog();
    }

    public override void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void OnDialogButtonClick()
    {
        if (eventDialogList.Count > 0)
        {
            string eventDialogId = eventDialogList[0];
            if (dialogController.ShowSanctuaryEventDialog(eventDialogId))
            {
                dialogRepository.SetEventDialogShown(eventDialogId, characterType);
                eventDialogList.RemoveAt(0);

                saveDataCallback.Invoke();

                if (eventDialogList.Count == 0)
                {
                    eventMarkImage.SetActive(false);
                }
            }
        }
        else
        {
            dialogController.ShowNormalDialg();
        } 
    }

    public bool IsEventDialogRemained()
    {
        return eventDialogList.Count > 0;
    }

    public void ObtainWarriorSkill(Player.WarriorSkill skill)
    {
        player.ObtainWarriorSkill(skill);
    }

    public void IncreaseMagicianSpellCount()
    {
        player.IncreaseMagicianSpellCount();
    }

    private void Awake()
    {
        eventDialogList.AddRange(dialogRepository.GetActiveEventDialogList(characterType));
        eventDialogList.Sort();

        dialogController.SetPlayerEventDialogListener(this);
        dialogController.LoadNormalDialogQueue(0, characterType);
        dialogController.LoadExcessiveDialogList(characterType);
    }
}