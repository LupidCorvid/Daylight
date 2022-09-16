using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawnpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public string scene;
    public Vector2 position;
    public bool savedOnce;

    // Update is called once per frame
    void Update()
    {
        if (!savedOnce)
        {
            scene = "Test";
            position = new Vector2(6.62f, -1.350911f);
        }
    }

    public void SetSpawnpoint(Transform location)
    {
        scene = SceneManager.GetActiveScene().name;
        position = location.position;
        savedOnce = true;
    }
}
