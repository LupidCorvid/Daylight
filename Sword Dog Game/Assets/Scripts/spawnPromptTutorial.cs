using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPromptTutorial : MonoBehaviour
{
    //public GameObject miniBubblePrefab;
    //public Vector2 miniBubbleOffset = new (0, 2);

    private MiniBubbleController bubble;
    public GameObject miniBubblePrefab;
    public float lastAdvert = -15;
    public float advertisingCooldown = 5;


    void spawnPrompt()
    {
        if (lastAdvert + advertisingCooldown < Time.time)
        {
            print(transform.position);
            GameObject addedObj = Instantiate(miniBubblePrefab, transform.position, Quaternion.identity);
            bubble = addedObj.GetComponent<MiniBubbleController>();
            bubble.enableBarks = false;
            bubble.enableVoice = false;
            bubble.speaker = null;
            bubble.voiceVol = 0;
            bubble.offset = Vector2.zero;
            
            string[] dialogOptions = { "[ss, 0.035] [IA,<size=140%><align=center><margin-right=0.5em><voffset=-0.5em>][btn,Jump] to Jump [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=140%><align=center><margin-right=0.5em><voffset=-0.5em>][btn,Dash] to Dash [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=140%><align=center><voffset=-0.5em>][btn,Sprint] to Sprint [wit] [exit]",
                                        "[ss, 0.035][IA,<size=110%><align=center><voffset=-0.75em><mspace=1.2em><sprite=\"keyboard\" name=\"Keyboard_a\"><sprite=\"keyboard\" name=\"Keyboard_d\"></mspace>] to Move [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=130%><align=center><margin-right=0.5em><voffset=-0.5em>][btn,Attack] to Attack [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=120%><align=center><margin-right=0.5em><voffset=-0.5em>][btn,Inventory] to open inventory [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=140%><align=center><margin-right=0.5em><voffset=-0.5em>][btn,Block] to Block [wit] [exit]",
                                        "[ss, 0.035] [IA,<size=140%><align=center><margin-right=0.5em><sprite=\"keyboard\" name=\"Keyboard_s\">] to Fall through [wit] [exit]"};
            bubble.setPosition = transform.position;

            //Check which prompt is being done
            if(gameObject.name == "jumpPrompt") bubble.setSource(new DialogSource(dialogOptions[0]));
            else if (gameObject.name == "dashPrompt") bubble.setSource(new DialogSource(dialogOptions[1]));
            else if (gameObject.name == "sprintPrompt") bubble.setSource(new DialogSource(dialogOptions[2]));
            else if (gameObject.name == "movePrompt") bubble.setSource(new DialogSource(dialogOptions[3]));
            else if (gameObject.name == "attackPrompt") bubble.setSource(new DialogSource(dialogOptions[4]));
            else if (gameObject.name == "inventoryPrompt") bubble.setSource(new DialogSource(dialogOptions[5]));
            else if (gameObject.name == "blockPrompt") bubble.setSource(new DialogSource(dialogOptions[6]));
            else if (gameObject.name == "fallPrompt") bubble.setSource(new DialogSource(dialogOptions[7]));
            else bubble.setSource(new DialogSource("???"));


            lastAdvert = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            spawnPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            bubble.close();
        }
    }
}
