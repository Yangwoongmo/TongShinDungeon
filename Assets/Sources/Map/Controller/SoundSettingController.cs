using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingController : MonoBehaviour
{
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider effectVolumeSlider;
    [SerializeField] private Text currentBgmVolume;
    [SerializeField] private Text currentEffectVolume;

    [SerializeField] private GameObject closeMainPopupButton;
    [SerializeField] private GameObject mainPopup;

    private SoundPreferenceRepository repository = SoundPreferenceRepository.GetInstance();

    private void Awake()
    {
        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        effectVolumeSlider.onValueChanged.RemoveAllListeners();

        bgmVolumeSlider.onValueChanged.AddListener(SetBgmValueText);
        effectVolumeSlider.onValueChanged.AddListener(SetEffectValueText);
    }

    public void OpenSoundSetting()
    {
        SoundSetting setting = repository.GetSoundSetting();
        int bgm = setting.GetBgmVolume();
        int effect = setting.GetEffectVolume();

        currentBgmVolume.text = bgm.ToString();
        bgmVolumeSlider.value = bgm;

        currentEffectVolume.text = effect.ToString();
        effectVolumeSlider.value = effect;

        gameObject.SetActive(true);
        mainPopup.SetActive(false);
        closeMainPopupButton.SetActive(false);
    }

    public void CloseSoundSetting()
    {
        gameObject.SetActive(false);
        mainPopup.SetActive(true);
        closeMainPopupButton.SetActive(true);
    }

    public void SaveSoundSetting()
    {
        SoundSetting currentSetting = 
            new SoundSetting((int)bgmVolumeSlider.value, (int)effectVolumeSlider.value);
        repository.SetSoundSetting(currentSetting);
        
        CloseSoundSetting();
    }

    private void SetBgmValueText(float value)
    {
        currentBgmVolume.text = ((int)value).ToString();
    }

    private void SetEffectValueText(float value)
    {
        currentEffectVolume.text = ((int)value).ToString();
    }
}
