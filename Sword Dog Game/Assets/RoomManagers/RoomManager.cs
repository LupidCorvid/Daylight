using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    public event Action<string, object[]> roomEvent;

    public static RoomManager currentRoom;

    public virtual void Awake()
    {
        currentRoom = this;
        roomEvent += receivedEvent;
    }

    public void callRoomEvent(string name, params object[] parameters)
    {
        roomEvent?.Invoke(name, parameters);
    }

    public virtual void receivedEvent(string name, params object[] parameters)
    {

    }
}
