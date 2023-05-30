using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : InteractionObject
{
    [SerializeField] private float popupDuration;
    [SerializeField] private float hideDuration;
    [SerializeField] private float startOffset;
    [SerializeField] private int thornDamage;
    [SerializeField] private bool isActivatedByTrigger;

    private float x;
    private float z;
    private float width;
    private bool isThornActivated = false;
    private bool isStopped = false;

    private bool isThornTerminated = false;

    private WaitForSeconds waitForPointOneSeconds = new WaitForSeconds(0.1f);
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private Coroutine currentInteractingCoroutine;

    private Vector3 popPosition = new Vector3(0.14f, 0, 0);
    private Vector3 hidePosition = new Vector3(0.14f, -2.55f, 0);

    public override void DoInteraction(PlayerEventHandler playerEventHandler)
    {
        playerEventHandler.GetDamage(thornDamage);
    }

    public override bool IsPlayerInActiveArea(float x, float z)
    {
        return x >= this.x - width && x <= this.x + width && z >= this.z - width && z <= this.z + width;
    }

    public override void OnPlayerScreenModeChanged(bool isSelfCameraMode)
    {
        if (isSelfCameraMode)
        {
            StopThorn();
        }
        else
        {
            if (isThornTerminated || isActivatedByTrigger)
            {
                return;
            }

            currentInteractingCoroutine = StartCoroutine(InteractWithPlayerCoroutine());
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            isStopped = false;
        }
    }

    public override bool IsTrap()
    {
        return true;
    }

    public override string GetInteractionMessage(IPlayerInteractionAvailableChecker checker)
    {
        return null;
    }

    public override void UpdateObjectStateWithoutAnimation()
    {
        isThornTerminated = true;
    }

    public void StartThorn()
    {
        currentInteractingCoroutine = StartCoroutine(InteractWithPlayerCoroutine());

        StartCoroutine(PopAndHideCoroutine());
    }

    public void TerminateThorn()
    {
        isThornTerminated = true;
    }

    public void SetupThornForGenerating(
        float popupDuration, 
        float hideDuration, 
        float startOffset, 
        bool isActivatedByTrigger
    )
    {
#if UNITY_EDITOR
        if (popupDuration > 0)
        {
            this.popupDuration = popupDuration;
        }
        if (hideDuration > 0)
        {
            this.hideDuration = hideDuration;
        }
              
        this.startOffset = startOffset;
        this.isActivatedByTrigger = isActivatedByTrigger;
#endif
    }

    protected override void SetMediator()
    {
        mediator = this.gameObject.transform.parent.parent.GetComponent<PlayerInteractionMediator>();
        mediator.AddPlayerStatusChangeObserver(this);
    }

    private void Start()
    {
        x = this.gameObject.transform.position.x;
        z = this.gameObject.transform.position.z;
        width = this.gameObject.transform.lossyScale.x * 4.5f;

        this.gameObject.transform.localPosition = hidePosition;

        if (!isThornTerminated && !isActivatedByTrigger)
        {
            StartThorn();
        }
    }

    private void StopThorn()
    {
        if (currentInteractingCoroutine == null)
        {
            return;
        }

        StopCoroutine(currentInteractingCoroutine);
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        currentInteractingCoroutine = null;
        isStopped = true;
    }

    private IEnumerator InteractWithPlayerCoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => isThornActivated && IsPlayerInActiveArea(playerPosition.localPosition.x, playerPosition.localPosition.z));
            mediator.SendPlayerInInteractionAreaEvent(DoInteraction);
        } 
    }

    private IEnumerator PopAndHideCoroutine()
    {
        WaitUntil waitUntilThornStart = new WaitUntil(() => !isStopped);
        float currentProgress = 0;
        float currentTime = 0;

        yield return waitUntilThornStart;

        while (currentTime < startOffset)
        {
            yield return waitForPointOneSeconds;
            yield return waitUntilThornStart;
            currentTime += 0.1f;
        }

        while (true)
        {
            currentTime = 0;
            currentProgress = 0;

            while (currentProgress < 1)
            {
                yield return waitForFixedUpdate;
                yield return waitUntilThornStart;
                currentProgress += 0.1f;
                this.gameObject.transform.localPosition = Vector3.Lerp(hidePosition, popPosition, currentProgress);
            }
            isThornActivated = true;

            currentTime = 0;
            while (currentTime < popupDuration)
            {
                yield return waitForPointOneSeconds;
                yield return waitUntilThornStart;
                currentTime += 0.1f;
            }

            isThornActivated = false;

            currentProgress = 0;
            while (currentProgress < 1)
            {
                yield return waitForFixedUpdate;
                yield return waitUntilThornStart;
                currentProgress += 0.1f;
                this.gameObject.transform.localPosition = Vector3.Lerp(popPosition, hidePosition, currentProgress);
            }

            currentTime = 0;
            while (currentTime < hideDuration)
            {
                yield return waitForPointOneSeconds;
                yield return waitUntilThornStart;
                currentTime += 0.1f;
            }

            if (isThornTerminated)
            {
                StopThorn();
                mediator.RemovePlayerStatusChangeObserver(this);
            }
        } 
    }
}
