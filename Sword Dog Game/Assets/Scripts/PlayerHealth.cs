using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth = 5;
    public int health = 5;
    public bool dead;
    private float iFrameTime = 1.0f, lastDamaged = 0f;

    public GameObject[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO need better way to retain hearts in different scenes -- perhaps singleton canvas?
        // hearts = GameObject.FindGameObjectsWithTag("Heart");
        // Array.Sort(hearts, CompareObNames);

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // for (int i = 0; i < hearts.Length; i++)
        // {
        //     if (i < health)
        //     {
        //         //if they have at least three health, then the other two must be full
        //         hearts[i].GetComponent<Image>().sprite = fullHeart;
        //     }
        //     else
        //     {
        //         //hearts more then the heath they have need to be empty
        //         hearts[i].GetComponent<Image>().sprite = emptyHeart;
        //     }

        //     if (i < maxHealth)
        //     {
        //         hearts[i].GetComponent<Image>().enabled = true;
        //     }
        //     else
        //     {
        //         hearts[i].GetComponent<Image>().enabled = false;
        //     }
        // }

        // TODO remove debug keybind 
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(1);
        }
    }

    // TODO add directional damage parameter for knockback
    public void TakeDamage(int damage, bool bypass = false)
    {
        if ((!bypass && Time.time > lastDamaged + iFrameTime) || bypass)
        {
            lastDamaged = Time.time;
            health -= damage;
            if (health <= 0) {
                Die();
                
            }
            // TODO this code is absolutely horrible, in the future we will prob want an instance variable reference to the sword object or to do this in swordfollow
            GameObject.FindObjectOfType<SwordFollow>().GetComponent<SimpleFlash>().Flash(1f, 3, true);
            GetComponent<SimpleFlash>().Flash(1f, 3, true);
            GetComponent<TimeStop>().StopTimeDefault();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    // TODO something more fancy later for deaths
    public void Die()
    {
        health = 0;
        dead = true;
    }

    // necessary to keep hearts in order on canvas
    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
}
