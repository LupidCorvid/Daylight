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
            //scene = "Test";
            scene = SceneManager.GetActiveScene().name;
            position = new Vector2(6.62f, -2.191553f);
        }
    }

    public void SetSpawnpoint(Transform location)
    {
        scene = SceneManager.GetActiveScene().name;
        position = location.position;
        savedOnce = true;
    }

    public void SetSpawnpoint(GameObject location)
    {
        scene = location.scene.name;
        position = location.transform.position;
        savedOnce = true;
    }
}
