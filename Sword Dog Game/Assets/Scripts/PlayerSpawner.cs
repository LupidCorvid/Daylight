using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Awake()
    {
        if (Player.controller != null)
        {
            return;
        }
        else
        {
            var newPlayer = Instantiate(player, transform.position, transform.rotation);
            if (GameSaver.loading && GameSaver.player != null)
            {
                newPlayer.transform.position = GameSaver.player.spawnpoint.position;
                newPlayer.GetComponent<PlayerMovement>().noFall = true;
                // data transfers
                GameSaver.player.controller.SetValues(newPlayer);
                GameSaver.player.health.SetValues(newPlayer);
                newPlayer.GetComponent<PlayerHealth>().UpdateHealth(false);
                // GameSaver.player.inventory.SetValues(newPlayer);
                // GameSaver.player.attack.SetValues(newPlayer);
                GameSaver.loading = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
