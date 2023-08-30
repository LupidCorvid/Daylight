using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventTriggerZone : MonoBehaviour
{
    public string eventName;

    public bool oneTimeTrigger = true;
    public bool triggered = false;

    public float triggerCooldown = 0;
    private float lastTrigger = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (!oneTimeTrigger || !triggered))
        {
            if (triggerCooldown + lastTrigger > Time.time)
                return;
            triggered = true;
            lastTrigger = Time.time;
            RoomManager.currentRoom.callRoomEvent(eventName);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (!oneTimeTrigger || !triggered))
        {
            if (triggerCooldown + lastTrigger > Time.time)
                return;
            triggered = true;
            lastTrigger = Time.time;
            RoomManager.currentRoom.callRoomEvent(eventName);
        }
    }
}
