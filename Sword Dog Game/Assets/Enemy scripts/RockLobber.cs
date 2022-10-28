using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLobber : EnemyBase
{
    public GameObject rockPrefab;

    public void Awake()
    {
        ai = new RockLobberAI(this, rockPrefab);
    }

    public override void Start()
    {
        base.Start();
        ai.target = GameObject.Find("Player(Clone)").transform;
    }
    

}
