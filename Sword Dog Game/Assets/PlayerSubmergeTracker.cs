using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubmergeTracker : MonoBehaviour
{
    public PlayerMovement player;

    public WaterFX inWater;

    public WadeChecker wade;


    public void EnteringWater(WaterFX water)
    {
        player.EnterWater();
        inWater = water;
    }

    public void LeavingWater()
    {
        player.LeaveWater();
    }

    private void Update()
    {
        transform.position = player.transform.position;

        if (player == null || player.gameObject == null)
            Destroy(gameObject);
    }


}
