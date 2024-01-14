using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static string spawningAt;
    public Transform player;

    private GameObject spawnPointObj;
    private Transform spawnPoint;

    void Start()
    {
        if (Player.instance != null)
        {
            player = Player.instance.transform;
        }
        if (player != null)
        {
            if (spawningAt != null && !GameSaver.loading) {
                spawnPointObj = GameObject.Find(spawningAt);
                if (spawnPointObj != null)
                {
                    spawnPoint = spawnPointObj.transform;
                    //update saved scene to current scene
                    Player.instance.GetComponent<Spawnpoint>().SetSpawnpoint(spawnPointObj);
                    Vector3 newPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
                    player.position = newPos;
                    SwordFollow.newPos = player.position;
                    SwordFollow.sceneChange?.Invoke();
                }
            }
            CameraController.OverrideMovement(player, 0.3f);
            transform.position = player.position + new Vector3(0, 2, -10);
        }
    }

    private void LateUpdate()
    {
        SceneHelper.changedSceneThisFrame = false;
    }
}
