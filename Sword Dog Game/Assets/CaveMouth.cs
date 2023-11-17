using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMouth : MonoBehaviour
{
    [SerializeField] private Collider2D edge1, edge2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        edge1.enabled = Player.instance.transform.position.y > -9;
        edge2.enabled = Player.instance.transform.position.y > -9;
    }
}
