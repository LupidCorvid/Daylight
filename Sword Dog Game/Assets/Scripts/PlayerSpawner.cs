using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerMovement.controller != null)
        {
            return;
        }
        else
        {
            // get save data here ok
            Instantiate(player, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
