using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPoseController : MonoBehaviour
{
    // For Warrior pose and face
    [SerializeField] private GameObject warriorHolder;
    [SerializeField] private GameObject warriorNonHolder;

    [SerializeField] private SpriteRenderer warriorFace;
    [SerializeField] private SpriteRenderer warriorHolderFace;
    [SerializeField] private SpriteRenderer warriorArm;
    [SerializeField] private SpriteRenderer warriorSword;
    [SerializeField] private SpriteRenderer warriorPose;

    // For Magician pose and face
    [SerializeField] private GameObject magicianHolder;
    [SerializeField] private GameObject magicianNonHolder;

    [SerializeField] private SpriteRenderer magicianFace;
    [SerializeField] private SpriteRenderer magicianHolderFace;
    [SerializeField] private SpriteRenderer magicianBackArm;
    [SerializeField] private SpriteRenderer magicianFrontArm;
    [SerializeField] private SpriteRenderer magicianPose;

    // For changeable sprites
    [SerializeField] private Sprite[] warriorFaceList;
    [SerializeField] private Sprite[] warriorHolderFaceList;
    [SerializeField] private Sprite[] magicianFaceList;
    [SerializeField] private Sprite[] magicianHolderFaceList;

    [SerializeField] private Sprite[] warriorPoseList;
    [SerializeField] private Sprite[] magicianPoseList;

    [SerializeField] private Sprite[] warriorArmList;
    [SerializeField] private Sprite[] magicianArmList;

    public void SetCameraHolder(CharacterType holder)
    {
        bool isHolderWarrior = holder == CharacterType.WARRIOR;
        warriorHolder.SetActive(isHolderWarrior);
        warriorNonHolder.SetActive(!isHolderWarrior);
        magicianHolder.SetActive(!isHolderWarrior);
        magicianNonHolder.SetActive(isHolderWarrior);
    }

    public void SetPoseAndFace(int warriorFace, int warriorPose, int magicianFace, int magicianPose)
    {
        if (warriorFace <= 0 || warriorPose <= 0 || magicianFace <= 0 || magicianPose <= 0)
        {
            return;
        }

        this.warriorFace.sprite = warriorFaceList[warriorFace - 1];
        this.warriorHolderFace.sprite = warriorHolderFaceList[warriorFace - 1];
        this.magicianFace.sprite = magicianFaceList[magicianFace - 1];
        this.magicianHolderFace.sprite = magicianHolderFaceList[magicianFace - 1];

        this.warriorArm.sprite = warriorArmList[warriorPose - 1];
        this.warriorSword.gameObject.SetActive(warriorPose % 2 == 1);
        this.warriorPose.sprite = warriorPoseList[warriorPose - 1];

        this.magicianPose.sprite = magicianPoseList[magicianPose - 1];

        bool needBackArm = magicianPose < 3;
        this.magicianFrontArm.gameObject.SetActive(!needBackArm);
        this.magicianBackArm.gameObject.SetActive(needBackArm);
        if (needBackArm)
        {
            this.magicianBackArm.sprite = magicianArmList[magicianPose - 1];
        }
    }
}
