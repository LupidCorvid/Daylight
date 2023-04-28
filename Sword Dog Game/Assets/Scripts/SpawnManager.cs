using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static string spawningAt;
    public Transform player;

    private GameObject spawnPointObj;
    private Transform spawnPoint;

    void OnEnable()
    {
        if (PlayerMovement.instance != null)
        {
            player = PlayerMovement.instance.transform;
        }
        if (player != null && spawningAt != null && !GameSaver.loading)
        {
            spawnPointObj = GameObject.Find(spawningAt);
            if (spawnPointObj != null)
            {
                spawnPoint = spawnPointObj.transform;
                //update saved scene to current scene
                PlayerMovement.instance.GetComponent<Spawnpoint>().SetSpawnpoint(spawnPointObj);
                Vector3 newPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
                player.position = newPos;
                SwordFollow.newPos = newPos;
                SwordFollow.sceneChange?.Invoke();
            }
            transform.position = player.position + new Vector3(0, 2, -10);
            CameraController.OverrideMovement(player);

        }
        GameSaver.loading = false;
    }

    private void LateUpdate()
    {
        SceneHelper.changedSceneThisFrame = false;
    }
}
