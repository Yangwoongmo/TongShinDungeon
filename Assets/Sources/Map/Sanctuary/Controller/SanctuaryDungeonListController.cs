using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SanctuaryDungeonListController : SanctuaryController
{
    private const float PlayerGoToDungeonSpeed = 0.015f;
    private const float PlayerMoveSpeed = 0.033f;

    private const string WhiteFadeOutAnimationKey = "whiteFadeOut";
    private readonly string[][] stageNames = new string[][]
    {
        new string[] { "최하층", "미로의 층", "가시의 층", "무수한 방", "묘지 입구" }
    };

    [SerializeField] private GameObject buttonBackground;
    [SerializeField] private Button openDungeonListButton;
    [SerializeField] private GameObject selectDungeonButtons;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject sceneFadeOutLayer;
    [SerializeField] private Animator sceneFadeOutAnimator;
    [SerializeField] private GameObject sanctuaryCamera;
    [SerializeField] private Button[] dungeonButtonList;
    [SerializeField] private Text stageName;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject rotateButton;
    [SerializeField] private Button startDungeonButton;

    [SerializeField] private SanctuaryCharacterDialogController[] dialogControllers;

    private Coroutine currentMovingCoroutine;

    private SanctuaryInfoRepository sanctuaryInfoRepository = SanctuaryInfoRepository.GetInstance();
    private List<List<int>> totalDungeonList;

    private (int, int) currentSelectedPortalId = (-1, -1);
    private int currentPage = 0;

    public override void SetEnable(bool isEnabled)
    {
        base.SetEnable(isEnabled);
        openDungeonListButton.gameObject.SetActive(isEnabled);

        bool isEventDialogRemained = false;
        for (int i = 0; i < dialogControllers.Length; i++)
        {
            isEventDialogRemained |= dialogControllers[i].IsEventDialogRemained();
        }
        openDungeonListButton.interactable = !isEventDialogRemained;
    }

    public void DungeonStartClick()
    {
        if (!isButtonClickAvailable || currentMovingCoroutine != null)
        {
            return;
        }

        currentMovingCoroutine = StartCoroutine(GoToDungeonCoroutine());
    }

    public void OnGoToDungeonButtonClick()
    {
        ToggleDungeonList(5.0f);
    }

    public void OnGoBackButtonClick()
    {
        ToggleDungeonList(-5.0f);
    }

    public void OnNextClick()
    {
        ShowAvailableDungeonList(1, currentPage + 1, totalDungeonList.Count);
    }

    public void OnPrevClick()
    {
        ShowAvailableDungeonList(-1, currentPage - 1, -1);
    }

    private void Start()
    {
        totalDungeonList = new List<List<int>>();
        totalDungeonList.AddRange(sanctuaryInfoRepository.GetTotalPurifiedPortalList());
        currentSelectedPortalId = sanctuaryInfoRepository.GetLatestEnteredPortalId();
        if (!totalDungeonList[0].Contains(0))
        {
            totalDungeonList[0].Insert(0, 0);
        }
    }

    private void ShowAvailableDungeonList(int direction, int start, int dest)
    {
        eventSystem.SetSelectedGameObject(null);
        for (int i = 0; i < dungeonButtonList.Length; i++)
        {
            dungeonButtonList[i].gameObject.SetActive(false);
        }

        for (int i = start; i != dest; i += direction)
        {
            if (totalDungeonList[i].Count == 0)
            {
                continue;
            }

            stageName.text = stageNames[i][0];
            currentPage = i;

            for (int j = 0; j < totalDungeonList[i].Count; j++)
            {
                int stageId = totalDungeonList[i][j] + 1;

                dungeonButtonList[j].onClick.RemoveAllListeners();
                dungeonButtonList[j].onClick.AddListener(() =>
                    {
                        currentSelectedPortalId = (stageId, 0);
                        startDungeonButton.interactable = true;
                    }    
                );
                dungeonButtonList[j].transform.GetChild(0).GetComponent<Text>().text = stageNames[i][stageId - 1];
                dungeonButtonList[j].gameObject.SetActive(true);

                if (stageId == currentSelectedPortalId.Item1)
                {
                    dungeonButtonList[j].Select();
                }
            }

            nextButton.SetActive(HasNext());
            prevButton.SetActive(HasPrev());
            return;
        }
    }

    private bool HasNext()
    {
        for (int i = currentPage + 1; i < totalDungeonList.Count; i++)
        {
            if (totalDungeonList[i].Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasPrev()
    {
        for (int i = currentPage - 1; i >= 0; i--)
        {
            if (totalDungeonList[i].Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void ToggleDungeonList(float distance)
    {
        if (currentMovingCoroutine != null || !isButtonClickAvailable)
        {
            return;
        }

        currentMovingCoroutine = StartCoroutine(MoveFowardCoroutine(distance));

        bool needToOpenDungeonList = distance > 0;
        nextButton.SetActive(false);
        prevButton.SetActive(false);

        currentPage = 0;
        if (needToOpenDungeonList)
        {
            ShowAvailableDungeonList(1, 0, totalDungeonList.Count);
            startDungeonButton.interactable = currentSelectedPortalId.Item1 >= 0 && currentSelectedPortalId.Item2 >= 0;
        }

        buttonBackground.SetActive(needToOpenDungeonList);
        openDungeonListButton.gameObject.SetActive(!needToOpenDungeonList);
        inventoryButton.SetActive(!needToOpenDungeonList);
        selectDungeonButtons.SetActive(needToOpenDungeonList);
        rotateButton.SetActive(!needToOpenDungeonList);
    }

    private IEnumerator MoveFowardCoroutine(float distance)
    {
        float currentLerpDistance = 0.0f;
        float y = sanctuaryCamera.transform.localPosition.y;
        float z = sanctuaryCamera.transform.localPosition.z;
        float x = sanctuaryCamera.transform.localPosition.x;

        while (true)
        {
            if (currentLerpDistance > 1.0f)
            {
                break;
            }
            currentLerpDistance += PlayerMoveSpeed;
            sanctuaryCamera.transform.localPosition = Vector3.Lerp(new Vector3(x, y, z), new Vector3(x + distance, y, z), currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }
        sanctuaryCamera.transform.localPosition = new Vector3(x + distance, y, z);

        currentMovingCoroutine = null;
    }

    private IEnumerator GoToDungeonCoroutine()
    {
        float currentLerpDistance = 0.0f;
        float y = sanctuaryCamera.transform.localPosition.y;
        float z = sanctuaryCamera.transform.localPosition.z;
        float x = sanctuaryCamera.transform.localPosition.x;

        sceneFadeOutLayer.SetActive(true);
        selectDungeonButtons.SetActive(false);

        sceneFadeOutAnimator.SetBool(WhiteFadeOutAnimationKey, true);
        while (true)
        {
            if (currentLerpDistance > 1.0f)
            {
                break;
            }
            currentLerpDistance += PlayerGoToDungeonSpeed;
            sanctuaryCamera.transform.localPosition = Vector3.Lerp(new Vector3(x, y, z), new Vector3(x + 3, y, z), currentLerpDistance);
            yield return new WaitForFixedUpdate();
        }
        sanctuaryCamera.transform.localPosition = new Vector3(x + 3, y, z);

        yield return new WaitForSeconds(0.5f);

        currentMovingCoroutine = null;

        sanctuaryInfoRepository.SaveStartPortalId(0);
        MapObjectStatusManager.GetInstance().SetStageId(currentSelectedPortalId.Item1);
        StartPointDataManager.GetInstance().SetNeedToSaveAfterTransitionDone(true);

        SceneManager.LoadScene("MapStage" + currentSelectedPortalId.Item1 + "Scene");
    }
}
