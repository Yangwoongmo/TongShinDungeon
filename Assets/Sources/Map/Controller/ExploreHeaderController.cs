using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreHeaderController : MonoBehaviour
{
    private const string SaveTextFadeAnimationFadeKey = "fade";

    [SerializeField] private Text currentTime;
    [SerializeField] private Canvas[] otherUis;
    [SerializeField] private GameObject moreMenuPopup;
    [SerializeField] private GameObject moreMenuButton;
    [SerializeField] private GameObject moreMenuPopupCloseButton;
    [SerializeField] private Animator saveTextFadeAnimator;

    [SerializeField] private TestModeSettingController settingController;

    private Player player;
    private ISettingMenuClickListener listener;

    // Update is called once per frame
    void Update()
    {
        currentTime.text = DateTime.Now.ToString("tth:mm");
    }

    public void OnMoreMenuClick()
    {
        Time.timeScale = 0;
        SetMoreMenuPopupVisibility(true);
        if (listener != null)
        {
            listener.OnClickOpenSettingMenu();
        }
    }

    public void OnMoreMenuPopupCloseClick()
    {
        Time.timeScale = 1;
        SetMoreMenuPopupVisibility(false);
        if (listener != null)
        {
            listener.OnClickCloseSettingMenu();
        }
    }
    public void OpenSetting()
    {
        settingController.OpenSetting(player);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void SaveData()
    {
        if (listener != null)
        {
            listener.OnClickSaveData();
        }
    }

    public void SaveDataAndQuit()
    {
        if (listener != null)
        {
            listener.OnClickSaveDataAndQuit();
        }
    }

    public void SetSettingMenuClickListener(ISettingMenuClickListener listener)
    {
        this.listener = listener;
    }

    public void SetHeaderVisibility(bool isVisible)
    {
        moreMenuButton.SetActive(isVisible);
    }

    public void StartSaveTextFadeAnimation()
    {
        StartCoroutine(SaveTextFadeAnimationCoroutine());
    }

    private void SetMoreMenuPopupVisibility(bool isVisible)
    {
        moreMenuPopup.SetActive(isVisible);
        moreMenuPopupCloseButton.SetActive(isVisible);
        for (int i = 0; i < otherUis.Length; i++)
        {
            otherUis[i].enabled = !isVisible;
        }
        currentTime.gameObject.SetActive(!isVisible);
        moreMenuButton.SetActive(!isVisible);
    }

    private IEnumerator SaveTextFadeAnimationCoroutine()
    {
        saveTextFadeAnimator.gameObject.SetActive(true);
        saveTextFadeAnimator.SetBool(SaveTextFadeAnimationFadeKey, true);

        yield return new WaitForSeconds(1.6f);
        saveTextFadeAnimator.SetBool(SaveTextFadeAnimationFadeKey, false);
        saveTextFadeAnimator.gameObject.SetActive(false);
    }
}
