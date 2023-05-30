using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattlePrototypeSceneController : MonoBehaviour, IBattlePlayerButtonClickListener
{
    private const string SpellUIDisapperedKey = "spellListAnimation";
    private const string ButtonChangeAnimationKey = "toBattle";
    private const string ButtonAppearAfterBlinkAnimationKey = "blink";
    private const string MagicianCharacterCastAnimationKey = "cast";
    private const string MagicianCharacterSpellAnimationKey = "spell";
    private const string MagicianCharacterBlinkAnimationKey = "blink";
    private const string MagicianCharacterBlinkFailAnimationKey = "blinkFail";
    private const string BlinkSuccessFadeAnimationKey = "blinkSuccess";
    private const string BlinkFailFadeAnimationKey = "blinkFail";

    private readonly string[,] spellButtonTexts = new string[5, 3] {{"화염의", "파도의", "대지의"},
                                                                    {"뜨거운", "차가운", "무거운"},
                                                                    {"태우는", "적시는", "부수는"},
                                                                    {"태우는", "적시는", "부수는"},
                                                                    {"태우는", "적시는", "부수는"}};
    [SerializeField] private Sprite[] spellButtonSprites;
    [SerializeField] private MonsterController monsterController;
    [SerializeField] private BattlePlayerController battlePlayerController;
    [SerializeField] private GameObject warriorUI;
    [SerializeField] private GameObject magicianUI;
    [SerializeField] private GameObject magicianCompleteCasting;
    [SerializeField] private GameObject spellListUI;
    [SerializeField] private GameObject magician;
    [SerializeField] private Animator spellUIAnimator;
    [SerializeField] private Animator buttonChangeAnimator;
    [SerializeField] private GameObject dimLayer;
    [SerializeField] private GameObject exploreUI;
    [SerializeField] private float startMp;
    [SerializeField] private Animator playerCameraAnimator;
    [SerializeField] private Button warriorActiveSkillButton;
    [SerializeField] private Animator magicianAnimator;
    [SerializeField] private Animator blinkFadeAnimator;

    [SerializeField] private SpriteRenderer[] warriorSprites;
    [SerializeField] private SpriteRenderer[] magicianSprites;

    [SerializeField] private MagicOrb[] magicOrbs;
    [SerializeField] private GameObject magicOrbParent;

    [SerializeField] private GameObject gameCamera;

    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private Image healProgress;
    [SerializeField] private Text holyWaterCountText;
    [SerializeField] private Image warriorSkillCoolDownProgress;

    [SerializeField] private Sprite[] buttonProgressSprites;

    [SerializeField] private GameObject cameraChangeEffectObject;
    [SerializeField] private Animator selfCameraChangeAnimator;

    [SerializeField] private BossController bossController;

    private int spellCount = 0;
    private bool isMagicianCameraMode = false;
    private GameObject currentSpellListUI;
    private List<int> spellOrder = new List<int> { 1, 2, 3 };
    private List<int> spellSetOrder = new List<int> { 0, 1, 2, 3, 4 };
    private Coroutine currentCompleteCastingSpellCoroutine;
    private Coroutine currentSpellUIDisapperedCoroutine;
    private WarriorController warriorController;
    private MagicianController magicianController;
    private float playerCameraXRotation = CameraTransformConst.CameraXRotation;
    private bool isBlinked = false;
    private bool isMagicOrbShootCompleted = false;

    private Player player;

    private Coroutine currentHealCoroutine;
    private ItemFactory itemFactory = new ItemFactory();

    private int blinkSuccessRatePercentage;

    private Coroutine currentWarriorSkillCoolDownCoroutine;

    private BattleActionController battleActionController = new BattleActionController();

    public void OnChangeCameraClick()
    {
        if (currentCompleteCastingSpellCoroutine != null)
        {
            return;
        }
        ChangeCameraModeInternal();
        OnHolyWaterTouchUp();
    }
    public void OnNormalAttackClick()
    {
        warriorController.NormalAttack();
    }
    public void OnAvoidClick(PlayerDirection direction)
    {
        if (direction == PlayerDirection.LEFT)
        {
            warriorController.LeftAvoid();
            return;
        }
        warriorController.RightAvoid();
    }

    public void OnWarriorSkillClick()
    {
        if (warriorController.GetWarriorAttackCoroutine() != null)
        {
            return;
        }
        warriorController.PokeClick();
        currentWarriorSkillCoolDownCoroutine = StartCoroutine(WarriorSkillCoolDownCoroutine());
    }

    public void OnHolyWaterTouchDown()
    {
        if (currentHealCoroutine != null || player.GetItemCount(1) <= 0)
        {
            return;
        }
        healProgress.gameObject.SetActive(true);
        currentHealCoroutine = StartCoroutine(HealProgressCoroutine());
    }

    public void OnHolyWaterTouchUp()
    {
        if (currentHealCoroutine == null)
        {
            return;
        }

        StopCoroutine(currentHealCoroutine);
        currentHealCoroutine = null;
        healProgress.fillAmount = 0;
        healProgress.gameObject.SetActive(false);
    }

    public void OnBlinkClick()
    {
        if (currentSpellUIDisapperedCoroutine != null || currentCompleteCastingSpellCoroutine != null)
        {
            return;
        }

        dimLayer.SetActive(true);
        currentSpellUIDisapperedCoroutine = StartCoroutine(BlinkStartCoroutine());
    }

    public void OnSpellClick(int buttonNumber)
    {
        if (currentSpellUIDisapperedCoroutine != null)
        {
            return;
        }
        Element element = (Element)spellOrder[buttonNumber];

        spellCount += 1;
        SetCompleteCastingButtonInteractable(true);
        currentSpellUIDisapperedCoroutine = StartCoroutine(SpellUIDisapperedCoroutine(element));
    }

    public void OnCompleteCastingClick()
    {
        if (currentCompleteCastingSpellCoroutine != null || currentSpellUIDisapperedCoroutine != null)
        {
            return;
        }
        currentCompleteCastingSpellCoroutine = StartCoroutine(CompleteCastingSpellCoroutine());
    }

    public void SetWarriorStatusObserver(IWarriorStatusObserver observer)
    {
        battlePlayerController.GetWarriorController().SetWarriorStatusObserver(observer);
    }

    public void AppearCharacters()
    {
        ChangeWarriorMagicianSpritesVisibility(true);
        battlePlayerController.AppearCharacters();
        magicOrbParent.SetActive(true);
    }

    public void DisappearCharacters()
    {
        battlePlayerController.DisappearCharacters();
        magicOrbParent.SetActive(false);
    }

    public void PrepareBattle(Monster monster)
    {
        AppearCharacters();
        StartCoroutine(PrepareBattleCoroutine(monster));
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        battlePlayerController.SetPlayer(player);
    }

    public void FinishBattleWithMonsterDeath()
    {
        StopAllCoroutines();
        ResetAllCoroutines();
        StartCoroutine(BattleEndWithMonsterDeathCoroutine());
    }

    public void FinishBattleWithPlayerDeath()
    {
        StopAllCoroutines();
        ResetAllCoroutines();
        ResetMagicianValues();
        OnHolyWaterTouchUp();
        ChangeWarriorActionSkillCoolDownVisibility(false);
        monsterController.StopMonster();
    }

    public void FinishBattleWithBlink()
    {
        StopAllCoroutines();
        ResetAllCoroutines();
        StartCoroutine(FinishBattleWithBlinkCoroutine());
    }

    public void AppearButtonsAfterBlink()
    {
        StartCoroutine(AppearButtonAfterBlinkCoroutine());
    }

    public bool IsMonsterDead()
    {
        return monsterController.GetMonster() != null && monsterController.GetMonster().IsDead();
    }

    public int GetMonsterId()
    {
        return monsterController.GetMonster().GetMonsterId();
    }

    public bool IsMagicianCameraMode()
    {
        return this.isMagicianCameraMode;
    }

    public bool IsBlinked()
    {
        return this.isBlinked;
    }

    public WarriorBattleStatistic GetWarriorBattleStatistic()
    {
        return battlePlayerController.GetWarriorBattleStatistic();
    }

    private void ChangeCameraModeInternal()
    {
        Vector3 vec;
        StartCoroutine(CameraChangeStingerAnimationCoroutine());
        if (!isMagicianCameraMode)
        {
            vec = new Vector3(playerCameraXRotation, CameraTransformConst.BattleMagicianCameraYRotation);
        }
        else
        {
            vec = new Vector3(playerCameraXRotation, CameraTransformConst.BattleWarriorCameraYRotation);
        }

        isMagicianCameraMode = !isMagicianCameraMode;
        ChangeWarriorMagicianSpritesVisibility(!isMagicianCameraMode);
        monsterController.ChangeMonsterSpriteVisibility(!isMagicianCameraMode);

        playerCameraAnimator.SetBool(CameraAnimationKeyConst.MagicianCameraTransitionAnimationKey, isMagicianCameraMode);
        warriorUI.SetActive(!isMagicianCameraMode);
        SetMagicianCanvasVisibility(isMagicianCameraMode);

        if (isMagicianCameraMode && spellSetOrder.Count > 0)
        {
            SetSpellSetOrder(ShuffleList(spellSetOrder));
            ShowSpellList();
        }

        gameCamera.transform.localRotation = Quaternion.Euler(vec);
        monsterController.SetWarningCanvaseVisibility(!isMagicianCameraMode);
    }

    private void ShowSpellList()
    {
        HideSpellList();

        ShuffleSpell(currentSpellListUI);
        currentSpellListUI.SetActive(true);
    }

    private void HideSpellList()
    {
        currentSpellListUI.SetActive(false);
    }

    private void ShuffleSpell(GameObject spellList)
    {
        spellOrder = ShuffleList(spellOrder);

        for (int i = 0; i < 3; i++)
        {
            spellList.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = spellButtonSprites[spellOrder[i] - 1];
            spellList.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().text = spellButtonTexts[spellSetOrder[0], spellOrder[i] - 1];
        }
    }

    private int GetStartMagicOrbIndex()
    {
        if (player.GetMagicianSpellCount() == 2)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        List<T> temp = new List<T>();
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, list.Count);
            temp.Add(list[rand]);
            list.RemoveAt(rand);
        }
        return temp;
    }

    private IEnumerator CompleteCastingSpellCoroutine()
    {
        HideSpellList();
        float[] damageList = new float[3] { 0f, 0f, 0f };
        magicianAnimator.SetBool(MagicianCharacterSpellAnimationKey, true);
        for (int i = 0; i < magicOrbs.Length; i++)
        {
            if (magicOrbs[i].gameObject.activeSelf)
            {
                int elementIndex = (int)magicOrbs[i].GetElement() - 1;
                if (elementIndex >= 0)
                {
                    damageList[elementIndex] += magicOrbs[i].GetOrbDamage() * player.GetIntelligence();
                }
                magicOrbs[i].ShootOrb();
            }
        }

        yield return new WaitUntil(() => isMagicOrbShootCompleted);
        ChangeCameraModeInternal();

        yield return new WaitForSeconds(1.1f);

        magicianController.SpellAttack(damageList, player.GetAwakeMagicDamageAddition(), player.GetStunMagicDamageAddition());

        ResetMagicianValues();

        ShowSpellList();
        ResetCompleteCastingSpellCoroutine();
    }

    private void ResetMagicianValues()
    {
        magicianAnimator.SetBool(MagicianCharacterSpellAnimationKey, false);
        magicianAnimator.SetBool(MagicianCharacterCastAnimationKey, false);
        magicianAnimator.SetBool(MagicianCharacterBlinkAnimationKey, false);
        magicianAnimator.SetBool(MagicianCharacterBlinkFailAnimationKey, false);

        blinkFadeAnimator.SetBool(BlinkSuccessFadeAnimationKey, false);
        blinkFadeAnimator.SetBool(BlinkFailFadeAnimationKey, false);

        spellCount = 0;
        SetCompleteCastingButtonInteractable(false);
        spellSetOrder = new List<int> { 0, 1, 2, 3, 4 };
        SetSpellSetOrder(ShuffleList(spellSetOrder));

        for (int i = 0; i < magicOrbs.Length; i++)
        {
            magicOrbs[i].ResetTotalMagicOrb();
            magicOrbs[i].gameObject.SetActive(false);
        }

        isMagicOrbShootCompleted = false;
        ResetCompleteCastingSpellCoroutine();
    }

    private IEnumerator SpellUIDisapperedCoroutine(Element element)
    {
        spellUIAnimator.SetBool(SpellUIDisapperedKey, true);
        magicianAnimator.SetBool(MagicianCharacterCastAnimationKey, true);

        if (spellCount <= 3)
        {
            int startIndex = GetStartMagicOrbIndex();
            magicOrbs[startIndex + spellCount - 1].gameObject.SetActive(true);
            magicOrbs[startIndex + spellCount - 1].SetupMagicOrb(element, () => isMagicOrbShootCompleted = true);
        }
        else
        {
            for (int i = 0; i < magicOrbs.Length; i++)
            {
                if (magicOrbs[i].gameObject.activeSelf)
                {
                    magicOrbs[i].ReinforceOrb(element);
                }
            }
        }

        yield return new WaitForSeconds(0.6f);

        spellUIAnimator.SetBool(SpellUIDisapperedKey, false);
        spellSetOrder.RemoveAt(0);
        if (spellSetOrder.Count == 0)
        {
            HideSpellList();
        }
        else
        {
            ShowSpellList();
        }

        yield return new WaitForSeconds(0.15f);
        magicianAnimator.SetBool(MagicianCharacterCastAnimationKey, false);

        ResetSpellUIDisapperedCoroutine();
    }

    private void ResetCompleteCastingSpellCoroutine()
    {
        currentCompleteCastingSpellCoroutine = null;
    }

    private void ResetSpellUIDisapperedCoroutine()
    {
        currentSpellUIDisapperedCoroutine = null;
    }

    private void Awake()
    {
        warriorController = battlePlayerController.GetWarriorController();
        magicianController = battlePlayerController.GetMagicianController();
        currentSpellListUI = spellListUI.transform.GetChild(0).gameObject;
    }

    private void SetMagicianCanvasVisibility(bool isVisible)
    {
        magicianUI.SetActive(isVisible);
    }

    private void SetCompleteCastingButtonInteractable(bool isVisible)
    {
        magicianCompleteCasting.GetComponent<Button>().interactable = isVisible;
    }

    private IEnumerator PrepareBattleCoroutine(Monster monster)
    {
        warriorActiveSkillButton.interactable = player.IsWarriorSkillAvailable(Player.WarriorSkill.SMITE);
        holyWaterCountText.text = player.GetItemCount(1).ToString();
        warriorUI.SetActive(true);
        SetMagicianCanvasVisibility(false);
        SetCompleteCastingButtonInteractable(false);

        buttonChangeAnimator.SetBool(ButtonChangeAnimationKey, true);
        monsterController.SetMonster(monster);

        yield return new WaitForSeconds(0.5f);

        exploreUI.SetActive(false);
        battlePlayerController.SetMonsterHitHandler(monster);
        warriorController.SetBattleActionObserver(battleActionController);
        monsterController.SetBattleActionObserver(battleActionController);
        SetupActionController();

        if ((monster is MonsterBloodStarvedBeast) && bossController != null)
        {
            MonsterBloodStarvedBeast boss = monster as MonsterBloodStarvedBeast;
            StartCoroutine(BossBattleStartAnimationCoroutine());
            StartCoroutine(BossPhaseChangeAnimationCoroutine(boss));
        }
        else
        {
            dimLayer.SetActive(false);
            monsterController.StartMonsterPattern();
        }
        blinkSuccessRatePercentage = 50;
    }

    public void SetupActionController()
    {
        battleActionController.Setup(player, monsterController.GetMonster(), warriorController.GetWarrior());
    }

    private IEnumerator BossPhaseChangeAnimationCoroutine(MonsterBloodStarvedBeast boss)
    {
        yield return new WaitUntil(() => boss.GetIsPhase2);

        dimLayer.SetActive(true);
        CameraChangeToWarrior();
        boss.BossPhaseChangeAnimation();
        bossController.BossPhaseChangeUIAnimation();

        yield return new WaitUntil(() => !bossController.IsPhaseChanging);

        dimLayer.SetActive(false);
    }

    private IEnumerator BossBattleStartAnimationCoroutine()
    {
        dimLayer.SetActive(true);
        bossController.BossBattleStartAnimation();

        yield return new WaitUntil(() => !bossController.IsStarting);

        dimLayer.SetActive(false);

        yield return new WaitForSeconds(1f);

        monsterController.StartMonsterPattern();
    }

    private IEnumerator BattleEndWithMonsterDeathCoroutine()
    {
        dimLayer.SetActive(true);
        exploreUI.SetActive(true);

        monsterController.ResetMonster();
        warriorController.ResetWarrior();
        OnHolyWaterTouchUp();
        ChangeWarriorActionSkillCoolDownVisibility(false);

        yield return new WaitForSeconds(0.5f);

        DisappearCharacters();

        yield return new WaitForSeconds(0.5f);

        buttonChangeAnimator.SetBool(ButtonChangeAnimationKey, false);

        dimLayer.SetActive(false);

        isMagicianCameraMode = false;

        warriorUI.SetActive(false);
        magicOrbParent.SetActive(false);
        SetMagicianCanvasVisibility(false);

        ResetMagicianValues();
    }

    private IEnumerator FinishBattleWithBlinkCoroutine()
    {
        buttonChangeAnimator.SetBool(ButtonChangeAnimationKey, false);
        monsterController.HideMonster();
        warriorController.ResetWarrior();
        OnHolyWaterTouchUp();
        ChangeWarriorActionSkillCoolDownVisibility(false);
        yield return new WaitForSeconds(0.5f);

        ResetMagicianValues();
        battlePlayerController.DisappearCharactersWithoutAnimation();

        exploreUI.SetActive(true);
        isMagicianCameraMode = false;
        warriorUI.SetActive(false);
        magicOrbParent.SetActive(false);
        SetMagicianCanvasVisibility(false);

        isBlinked = false;
    }

    private void ResetAllCoroutines()
    {
        ResetCompleteCastingSpellCoroutine();
        ResetSpellUIDisapperedCoroutine();
        currentHealCoroutine = null;
        currentWarriorSkillCoolDownCoroutine = null;
    }

    private IEnumerator AppearButtonAfterBlinkCoroutine()
    {
        buttonChangeAnimator.SetBool(ButtonAppearAfterBlinkAnimationKey, true);
        yield return new WaitForSeconds(0.5f);
        buttonChangeAnimator.SetBool(ButtonAppearAfterBlinkAnimationKey, false);
    }

    private void SetSpellSetOrder(List<int> order)
    {
        int residualSpellCount = Mathf.Min(player.GetMagicianSpellCount(), order.Count);
        spellSetOrder = order.GetRange(0, residualSpellCount);
    }

    private void ChangeWarriorMagicianSpritesVisibility(bool isWarriorVisible)
    {
        for (int i = 0; i < warriorSprites.Length; i++)
        {
            warriorSprites[i].enabled = isWarriorVisible;
        }
        for (int i = 0; i < magicianSprites.Length; i++)
        {
            magicianSprites[i].enabled = !isWarriorVisible;
        }
    }

    private void ChangeWarriorActionSkillCoolDownVisibility(bool isVisible)
    {
        warriorSkillCoolDownProgress.gameObject.SetActive(isVisible);
        warriorActiveSkillButton.gameObject.SetActive(!isVisible);
    }

    private void CameraChangeToWarrior()
    {
        if(isMagicianCameraMode) 
        {
            ChangeCameraModeInternal();
        }
    }

    private IEnumerator BlinkStartCoroutine()
    {
        magicianAnimator.SetBool(MagicianCharacterBlinkAnimationKey, true);
        yield return new WaitForSeconds(0.42f);

        if (Random.Range(1, 101) > blinkSuccessRatePercentage)
        {
            StartCoroutine(BlinkFailCoroutine());
            blinkSuccessRatePercentage = Mathf.Min(100, blinkSuccessRatePercentage + 10);
        }
        else
        {
            warriorController.SetWarriorInvicible();
            blinkFadeAnimator.SetBool(BlinkSuccessFadeAnimationKey, true);
            isBlinked = true;
            blinkSuccessRatePercentage = 50;
        }
    }

    private IEnumerator BlinkFailCoroutine()
    {
        blinkFadeAnimator.SetBool(BlinkFailFadeAnimationKey, true);
        yield return new WaitForSeconds(0.15f);

        magicianAnimator.SetBool(MagicianCharacterBlinkFailAnimationKey, true);
        yield return new WaitForSeconds(0.35f);

        ResetSpellUIDisapperedCoroutine();
        magicianAnimator.SetBool(MagicianCharacterBlinkAnimationKey, false);
        magicianAnimator.SetBool(MagicianCharacterBlinkFailAnimationKey, false);
        blinkFadeAnimator.SetBool(BlinkFailFadeAnimationKey, false);

        dimLayer.SetActive(false);
    }

    private IEnumerator HealProgressCoroutine()
    {
        healProgress.sprite = buttonProgressSprites[0];

        for (int i = 1; i < 101; i++)
        {
            yield return new WaitForFixedUpdate();
            healProgress.sprite = buttonProgressSprites[i / 10];

            if (battlePlayerController.IsWarriorInAction())
            {
                healProgress.sprite = buttonProgressSprites[0];
                yield return new WaitUntil(() => !battlePlayerController.IsWarriorInAction());
                currentHealCoroutine = StartCoroutine(HealProgressCoroutine());
                yield break;
            }
        }

        inventoryController.UseItem(itemFactory.createItem(1));
        int itemCount = player.GetItemCount(1);
        holyWaterCountText.text = itemCount.ToString();

        if (itemCount > 0)
        {
            currentHealCoroutine = StartCoroutine(HealProgressCoroutine());
        }
        else
        {
            OnHolyWaterTouchUp();
        }
    }

    private IEnumerator WarriorSkillCoolDownCoroutine()
    {
        warriorSkillCoolDownProgress.sprite = buttonProgressSprites[0];
        ChangeWarriorActionSkillCoolDownVisibility(true);

        for (int i = 1; i < 11; i++)
        {
            yield return new WaitForSeconds(0.5f);
            warriorSkillCoolDownProgress.sprite = buttonProgressSprites[i];
        }

        ChangeWarriorActionSkillCoolDownVisibility(false);
        currentWarriorSkillCoolDownCoroutine = null;
    }

    private IEnumerator CameraChangeStingerAnimationCoroutine()
    {
        cameraChangeEffectObject.SetActive(true);
        selfCameraChangeAnimator.SetBool("cameraModeChange", true);
        yield return new WaitForSeconds(0.25f);
        selfCameraChangeAnimator.SetBool("cameraModeChange", false);
        cameraChangeEffectObject.SetActive(false);
    }
}
