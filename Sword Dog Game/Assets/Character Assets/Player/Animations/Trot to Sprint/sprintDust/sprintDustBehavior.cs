using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


//This script instantiates the sprint dust thingy when transitioning from trot to sprint


public class sprintDustBehavior : MonoBehaviour
{
    [SerializeField] GameObject dust;
    Animator stateInfo;
    bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Player.instance != null)
        {
            stateInfo = Player.instance.GetComponent<Animator>();

            //Makes the dust appear
            if (stateInfo.GetCurrentAnimatorStateInfo(0).IsName("trotToSprint") && !triggered)
            {
                Vector3 pos = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Player.instance.transform.position.z);
                Instantiate(dust, pos, Quaternion.identity);
                

                triggered = true;
            }

            //Resets the flag so that the dust can appear on next sprint
            if (!stateInfo.GetCurrentAnimatorStateInfo(0).IsName("trotToSprint") && !stateInfo.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
            {
                triggered = false;
            }
        }
    }
}
