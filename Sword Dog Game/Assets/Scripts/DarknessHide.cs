using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DarknessHide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.instance != null)
        {
            if (SceneManager.GetActiveScene().name == "Forest 2" || SceneManager.GetActiveScene().name == "Forest 6")
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
