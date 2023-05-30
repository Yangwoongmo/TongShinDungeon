using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionMediator : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    private List<PlayerInteractionStatusChangeObserver> playerObservers = new List<PlayerInteractionStatusChangeObserver>();
    private PlayerInteractionAreaStatusChangeObserver playerAreaStatusObserver;

    public void AddPlayerStatusChangeObserver(PlayerInteractionStatusChangeObserver observer)
    {
        playerObservers.Add(observer);
        observer.SetPlayerPositionTransform(playerPosition);
    }

    public void SetPlayerAreaStatusObserver(PlayerInteractionAreaStatusChangeObserver observer)
    {
        playerAreaStatusObserver = observer;
    }

    public void SendPlayerScreenModeChangeEvent(bool isSelfCameraMode)
    {
        for (int i = 0; i < playerObservers.Count; i++)
        {
            if (!isSelfCameraMode)
            {
                playerObservers[i].gameObject.SetActive(true);
            }
            playerObservers[i].OnPlayerScreenModeChanged(isSelfCameraMode);
        }
    }

    public void SendPlayerInInteractionAreaEvent(PlayerInteractionAreaStatusChangeObserver.DoInteraction interaction)
    {
        playerAreaStatusObserver.OnPlayerInInteractionAreaStatusChanged(interaction);
    }

    public void RemovePlayerStatusChangeObserver(PlayerInteractionStatusChangeObserver observer)
    {
        playerObservers.Remove(observer);
    }

    public float GetPlayerRotationY()
    {
        return playerPosition.localEulerAngles.y;
    }

    public void SetPlayerPosition(Transform position)
    {
#if UNITY_EDITOR
        playerPosition = position;
#endif
    }
}
