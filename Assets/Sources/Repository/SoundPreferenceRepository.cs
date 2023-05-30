using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPreferenceRepository
{
    private static SoundPreferenceRepository instance;

    private const string SoundSettingPrefKey = "SoundSetting";

    private SoundPreferenceRepository()
    {
    }

    public static SoundPreferenceRepository GetInstance()
    {
        if (instance == null)
        {
            instance = new SoundPreferenceRepository();
        }

        return instance;
    }

    public SoundSetting GetSoundSetting()
    {
        string value = PlayerPrefs.GetString(SoundSettingPrefKey, "");

        if (value.Length == 0)
        {
            return new SoundSetting(100, 100);
        }
        else
        {
            return JsonUtility.FromJson<SoundSetting>(value);
        }
    }

    public void SetSoundSetting(SoundSetting setting)
    {
        string value = JsonUtility.ToJson(setting);
        PlayerPrefs.SetString(SoundSettingPrefKey, value);
    }
}

[System.Serializable]
public class SoundSetting
{
    [SerializeField] private int bgmVolume;
    [SerializeField] private int effectVolume;

    public SoundSetting(int bgm, int effect)
    {
        bgmVolume = bgm;
        effectVolume = effect;
    }

    public int GetBgmVolume()
    {
        return bgmVolume;
    }

    public int GetEffectVolume()
    {
        return effectVolume;
    }
}
