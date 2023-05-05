using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaverCheck : MonoBehaviour
{
    public GameObject gameSaver;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!FindObjectOfType<GameSaver>())
        {
            Instantiate(gameSaver, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
