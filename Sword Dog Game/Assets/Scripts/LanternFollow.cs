using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanternFollow : MonoBehaviour
{
    Vector3 lantTargetLocation;
    Vector3 lantPreviousLocation;
    public SpriteRenderer lantern, lanternLight;
    public Rigidbody2D rb;
    public bool autoActivated = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Player.instance != null)
        {
            // TODO very temp implementation to auto activate lantern in cave levels
            if (!autoActivated)
            {
                autoActivated = true;
                Player.instance.activateLantern();
            }

            if (Player.instance.hasLantern)
            {
                lantern.enabled = true;
                lanternLight.enabled = true;

                GameObject mouth = Player.instance.mouthLantern;
                lantPreviousLocation = transform.position;

                //Check for being flipped
                if (Player.controller.facingRight)
                    lantTargetLocation = new Vector3(mouth.transform.position.x, mouth.transform.position.y - .75f, mouth.transform.position.z);
                else
                    lantTargetLocation = new Vector3(mouth.transform.position.x, mouth.transform.position.y - .75f, mouth.transform.position.z);


                if (Player.instance != null)
                {
                    transform.position = Vector3.Lerp(lantPreviousLocation, lantTargetLocation, 1);
                }
            }
            else
            {
                // TODO do this better - hides lantern if inactive
                //Player.darkness.GetComponent<SpriteRenderer>().enabled = false;
                lantern.enabled = false;
                lanternLight.enabled = false;
            }
        }
    }

    public void AttackMove()
    {
        

    }
    
    public void Freeze()
    {
        if (PlayerHealth.dead)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
        }
    }
}
