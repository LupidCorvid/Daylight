using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public static bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Temp implementation. Should work to escape menus and check if not in menus first
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0;
                foreach (AudioSource source in sources)
                {
                    source.Pause();
                }
                Debug.Log("Paused");
            }
            else
            {
                paused = false;
                Time.timeScale = 1;
                foreach (AudioSource source in sources)
                {
                    source.UnPause();
                }
                Debug.Log("Unpause");
            }
        }
    }
}
