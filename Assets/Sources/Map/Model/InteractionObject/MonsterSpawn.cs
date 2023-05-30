using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : InteractionObject
{
    private const string MonsterAppearAnimationKey = "appear";
    private const string RewardAppearAnimationKey = "itemAppear";

    private const float monsterPositionY = 5f;
    

    [SerializeField] private int monsterId;
    [SerializeField] private ItemHolder[] rewardList;
    [SerializeField] private bool isFixedMonster;
    [SerializeField] private bool isBossAttack;
    private Monster monster;

    private int groupIndex;
    private int spawnIndex;

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        float currentRotation = mediator.GetPlayerRotationY();
        (float, float) monsterSpawnXZPosition = monster.GetMonsterSpawnXZPosition(currentRotation);

        monster.gameObject.transform.localPosition = new Vector3(
            monsterSpawnXZPosition.Item1,
            monsterPositionY,
            monsterSpawnXZPosition.Item2
        );
        monster.gameObject.transform.localEulerAngles = new Vector3(
            CameraTransformConst.CameraXRotation,
            currentRotation + CameraTransformConst.BattleWarriorCameraYRotation,
            monster.gameObject.transform.localEulerAngles.z
        );
        monster.gameObject.SetActive(true);

        StartCoroutine(MonsterAppearCoroutine(playerEventHandler));
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        return null;
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return false;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        // Do Nothing
    }

    public bool HasMonster()
    {
        return monsterId > 0 && monster != null;
    }

    public bool IsBossAttack()
    {
        return isBossAttack;
    }

    public int GetMonsterId()
    {
        return monsterId;
    }

    protected override void SetMediator()
    {
        mediator = this.gameObject.transform.parent.parent.GetComponent<PlayerInteractionMediator>();
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        // Do nothing
    }

    public void SetMonsterId(int monsterId)
    {
        this.monsterId = monsterId;

        monster = GetPool().GetMonster(monsterId, this.gameObject.transform.parent).GetComponent<Monster>();
        monster.gameObject.SetActive(false);
    }

    public void CleanupMonster()
    {
        if (this.monsterId > 0 && this.monster != null)
        {
            GetPool().RecycleMonster(this.monsterId, this.monster.gameObject);
        }
        this.monsterId = 0;
        this.monster = null;
        this.gameObject.transform.parent.GetComponent<Floor>().SetMonsterFloor(false);
        this.gameObject.SetActive(false);
        this.rewardList[0].gameObject.SetActive(false);
    }

    public void GetReward()
    {
        ItemHolder rewardHolder = rewardList[0];
        List<MonsterReward> rewards = monster.GetRewardList();
        float prob = Random.Range(0f, 1f);
        for (int i = 0; i < rewards.Count; i++)
        {
            prob -= rewards[i].GetProbability();
            if (prob > 0)
            {
                continue;
            }
            (int itemId, int gold) = rewards[i].GetReward();
            rewardHolder.SetItemId(itemId);
            rewardHolder.SetGold(gold);
            break;
        }

        rewardHolder.StartObservePlayer();
        rewardHolder.gameObject.SetActive(true);
        rewardHolder.GetComponent<Animator>().SetBool(RewardAppearAnimationKey, true);
        rewardHolder.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

        if (isFixedMonster)
        {
            string id = this.gameObject.transform.parent.GetComponent<Floor>().GetId();
            MapObjectStatusManager.GetInstance().UpdateObjectStatus(id);
        }
    }

    public void SetGroupIndex(int idx)
    {
        groupIndex = idx;
    }

    public void SetSpawnIndex(int idx)
    {
        spawnIndex = idx;
    }

    public int GetGroupIndex()
    {
        return groupIndex;
    }

    public int GetSpawnIndex()
    {
        return spawnIndex;
    }

    public void SetReward(ItemHolder reward)
    {
#if UNITY_EDITOR
        rewardList = new ItemHolder[1];
        rewardList[0] = reward;
#endif
    }

    public void SetMonsterInfo(int monsterId, bool isFixed, bool isBossAttack)
    {
#if UNITY_EDITOR
        this.monsterId = monsterId;
        this.isFixedMonster = isFixed;
        this.isBossAttack = isBossAttack;
#endif
    }

    private IEnumerator MonsterAppearCoroutine(PlayerEventHandler playerEventHandler)
    {
        Animator animator = monster.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
        animator.SetBool(MonsterAppearAnimationKey, true);

        yield return new WaitForSeconds(1.2f);
        animator.SetBool(MonsterAppearAnimationKey, false);

        playerEventHandler.EncountMonster(monster);
    }

    private MonsterRecyclePool GetPool()
    {
        return this.gameObject.transform.parent.parent.parent.GetComponent<MonsterRecyclePool>();
    }
}
