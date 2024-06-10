using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


//This script destroys an object that enters its exit state in the animator


public class selfDestruct : MonoBehaviour
{
    bool flip;

    private void Start()
    {
        if (Player.instance != null)
        {
            if (!Player.instance.GetComponent<PlayerMovement>().facingRight)
            {
                flip = true;
            }
            else flip = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flip)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;

        if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("exitTime"))
        {
            Destroy(gameObject);
        }
    }
}
