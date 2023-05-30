using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private InteractionObject interactionObject;
    [SerializeField] private Wall[] wallDirection = new Wall[4];
    [SerializeField] private Floor[] floorDirection = new Floor[4];
    [SerializeField] private bool isMonsterFloor;
    [SerializeField] private bool isBlinkPoint;

    [SerializeField] private TriggerObject triggerObject;

    // This is used only for tutorial event
    [SerializeField] private int tutorialButtonShowIndex;

#if UNITY_EDITOR
    public int materialMainId;
#endif

    private bool isStartFloor;
    private float x;
    private float y;

    public bool IsWallExist(int direction)
    {
        if (direction < 0 || direction >= 4)
        {
            return true;
        }
        return wallDirection[direction] != null;
    }

    public InteractionObject GetInteractionObject()
    {
        return interactionObject;
    }

    public TriggerObject GetTriggerObject()
    {
        return triggerObject;
    }

    public void SetWall(Wall wall, int direction)
    {
        wallDirection[direction] = wall;
    }

    public void SetFloor(Floor floor, int direction)
    {
        floorDirection[direction] = floor;
    }

    public Wall GetWall(int direction)
    {
        return wallDirection[direction];
    }

    public Floor GetFloor(int direction)
    {
        return floorDirection[direction];
    }

    public bool IsMonsterFloor()
    {
        return this.isMonsterFloor && interactionObject != null;
    }

    public void SetMonsterFloor(bool isMonsterFloor)
    {
        this.isMonsterFloor = isMonsterFloor;
    }

    public void SetBlinkPoint(bool isBlinkPoint)
    {
        this.isBlinkPoint = isBlinkPoint;
    }

    public bool IsBlinkPoint()
    {
        return isBlinkPoint;
    }

    public void SetId(string id)
    {
        this.id = id;
    }

    public string GetId()
    {
        return id;
    }

    public int GetTutorialButtonShowIndex()
    {
        return tutorialButtonShowIndex;
    }

    public void SetInteractionObject(InteractionObject interaction)
    {
#if UNITY_EDITOR
        this.interactionObject = interaction;
#endif
    }

    public void SetTriggerObject(TriggerObject trigger)
    {
#if UNITY_EDITOR
        triggerObject = trigger;
#endif
    }

    public virtual void SetFloorMaterial(Material material)
    {
#if UNITY_EDITOR
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
#endif
    }

    public void SetTutorialIndex(int index)
    {
#if UNITY_EDITOR
        tutorialButtonShowIndex = index;
#endif
    }

    private void Awake()
    {
        x = this.gameObject.transform.localPosition.x;
        y = this.gameObject.transform.localPosition.y;

        if (MapObjectStatusManager.GetInstance().HasObjectStateChanged(id))
        {
            if (interactionObject != null)
            {
                interactionObject.UpdateObjectStateWithoutAnimation();
            }
            
            if (triggerObject != null)
            {
                triggerObject.UpdateObjectStateWithoutAnimation();
            }
        }
    }

    private void Start()
    {
        StartPointDataManager manager = StartPointDataManager.GetInstance();
        if (!manager.IsInitialStartFloorLoad() && !manager.IsInitialBlinkPointLoad())
        {
            return;
        }

        StartFloorChangeMediator mediator = transform.parent.GetComponent<StartFloorChangeMediator>();
        StartPointData startPoint = manager.GetStartPoint();
        StartPointData blinkPoint = manager.GetBlinkPoint();

        if (manager.IsInitialStartFloorLoad() && startPoint != null && startPoint.GetFloorId() == id)
        {
            mediator.SetStartFloor(this);
        }
        else if (manager.IsInitialBlinkPointLoad() && blinkPoint != null && blinkPoint.GetFloorId() == id)
        {
            mediator.SetBlinkPoint(this);
        }
    }
}
