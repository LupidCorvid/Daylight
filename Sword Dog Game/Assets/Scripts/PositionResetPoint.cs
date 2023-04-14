using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameObject.FindObjectOfType<PlayerSpawner>().transform.position;
    }
}