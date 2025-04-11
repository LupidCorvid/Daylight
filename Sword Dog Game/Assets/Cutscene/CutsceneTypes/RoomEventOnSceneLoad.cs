using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomEventOnSceneLoad : MonoBehaviour
{
    public string eventName;
    bool called = false;

    public bool oneTimeTrigger = true;
    public bool triggered = false;

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null && called == false)
        {
            RoomManager.currentRoom.callRoomEvent(eventName);
            called = true;
            Destroy(gameObject);
        }
    }
}
