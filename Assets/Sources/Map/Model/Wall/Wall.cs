using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] protected string id;
    [SerializeField] protected InteractionObject interactionObject;
    [SerializeField] protected bool isPassable;

#if UNITY_EDITOR
    public int materialMainId;
    public int materialSubId;
#endif

    private MapObjectStatusManager manager = MapObjectStatusManager.GetInstance();

    public bool IsPassable()
    {
        return isPassable;
    }

    public InteractionObject GetInteractionObject()
    {
        return interactionObject;
    }

    public void SetId(string id)
    {
        this.id = id;
    }

    public string GetId()
    {
        return id;
    }

    public virtual void SetDoorPassable(bool isPassable)
    {
        this.isPassable = isPassable;
    }

    public virtual bool UpdateChangeWithoutAnimationIfNeed()
    {
        if (manager.HasObjectStateChanged(id))
        {
            if (interactionObject != null)
            {
                interactionObject.UpdateObjectStateWithoutAnimation();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaterial(Material material)
    {
#if UNITY_EDITOR
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
#endif
    }

    protected void UpdateObjectState()
    {
        manager.UpdateObjectStatus(id);
    }

    private void Awake()
    {
        UpdateChangeWithoutAnimationIfNeed();
    }
}
