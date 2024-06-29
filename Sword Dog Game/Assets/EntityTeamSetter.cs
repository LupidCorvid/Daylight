using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTeamSetter : MonoBehaviour
{
    //Meant to allow for setting an entities team on start from the inspector
    public ITeam.Team allies;
    public ITeam.Team enemies;

    public Entity targetEntity;

    // Start is called before the first frame update
    void Start()
    {
        targetEntity ??= GetComponent<Entity>();

        targetEntity.allies = allies;
        targetEntity.enemies = enemies;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
