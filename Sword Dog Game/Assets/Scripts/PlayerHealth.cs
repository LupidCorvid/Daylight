using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Player playerBase;

    private int maxHealth
    {
        get
        {
            return playerBase.maxHealth;
        }
        set
        {
            playerBase.maxHealth = value;
        }
    }

    public int health
    {
        get
        {
            return playerBase.health;
        }
        set
        {
            playerBase.health = value;
        }
    }

    public static bool dead, gettingUp;
    private float iFrameTime = 1.0f, lastDamaged = 0f;
    private Animator anim;
    private Rigidbody2D rb;

    public bool invincible
    {
        get
        {
            return playerBase.invincible;
        }

        set
        {
            playerBase.invincible = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        gettingUp = false;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        BaseAI.possibleTargets.Add(transform);

        playerBase = GetComponent<Player>();
        playerBase.playerHealth = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        // TODO remove debug keybinds 
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
    }

    public void UpdateHealth()
    {
        int temp = health;
        for (int i = 0; i < CanvasManager.hearts.Count; i++)
        {
            if (health <= Heart.MinWobbleHealth) {
                CanvasManager.hearts[i].Wobble(Heart.MinWobbleHealth - health + 2f);
            }
            else
                CanvasManager.hearts[i].wobbling = false;
            
            CanvasManager.hearts[i].SetSprite(temp);
            temp -= 2;
        }
    }

    // TODO add directional damage parameter for knockback
    public void TakeDamage(int damage, bool bypass = false)
    {
        if (!dead && !invincible)
        {
            if ((!bypass && Time.time > lastDamaged + iFrameTime) || bypass)
            {
                lastDamaged = Time.time;
                health -= damage;
                if (health <= 0)
                {
                    Die();
                }

                SwordFollow.instance.GetComponent<SimpleFlash>().Flash(1f, 3, true);
                GetComponent<SimpleFlash>().Flash(1f, 3, true);
                GetComponent<TimeStop>().StopTime();
                StartCoroutine(AudioManager.instance.Muffle());
                UpdateHealth();
            }
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        UpdateHealth();
        if (dead)
        {
            anim.SetTrigger("wakeup");
        }
    }

    // TODO something more fancy later for deaths
    public void Die()
    {
        health = 0;
        dead = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetTrigger("death");
        CutsceneController.PlayCutscene("PlayerDeath");
    }

    public void WakeUp()
    {
        gettingUp = false;
        dead = false;
        anim.ResetTrigger("wakeup");
    }

    public void GettingUp()
    {
        gettingUp = true;
    }
}
