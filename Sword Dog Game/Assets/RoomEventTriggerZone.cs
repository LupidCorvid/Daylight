using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventTriggerZone : MonoBehaviour
{
    public string eventName;

    public bool oneTimeTrigger = true;
    public bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && (!oneTimeTrigger || !triggered))
        {
            triggered = true;
            RoomManager.currentRoom.callRoomEvent(eventName);
        }
        
        
    }
}
