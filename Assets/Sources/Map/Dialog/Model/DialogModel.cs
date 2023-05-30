using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogModel
{
    [SerializeField] private string message;
    private float delayPerOneCharacter;

    [SerializeField] private CharacterType speaker;

    [SerializeField] private int warriorFaceType;
    [SerializeField] private int warriorPoseType;
    [SerializeField] private int magicianFaceType;
    [SerializeField] private int magicianPoseType;

    public DialogModel(
        string message, 
        CharacterType speaker, 
        int warriorFaceType = -1, 
        int warriorPoseType = -1, 
        int magicianFaceType = -1, 
        int magicianPoseType = -1, 
        float delay = 0.1f
    ) {
        this.message = message;
        this.delayPerOneCharacter = delay;
        this.speaker = speaker;
        this.warriorFaceType = warriorFaceType;
        this.warriorPoseType = warriorPoseType;
        this.magicianFaceType = magicianFaceType;
        this.magicianPoseType = magicianPoseType;
    }

    public string GetMessage()
    {
        return message;
    }

    public float GetDelayPerOneCharacter()
    {
        return delayPerOneCharacter;
    }

    public CharacterType GetSpeaker()
    {
        return speaker;
    }

    public int GetWarriorFaceType()
    {
        return warriorFaceType;
    }

    public int GetWarriorPoseType()
    {
        return warriorPoseType;
    }

    public int GetMagicianFaceType()
    {
        return magicianFaceType;
    }

    public int GetMagicianPoseType()
    {
        return magicianPoseType;
    }
}

public enum CharacterType
{
    WARRIOR,
    MAGICIAN,
    MERCHANT,
    PLAYER
}
