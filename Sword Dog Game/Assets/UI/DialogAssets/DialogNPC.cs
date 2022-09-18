using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNPC : MonoBehaviour
{
    public GameObject barkFXPrefab;

    public string dialog;
    public int position;


    //Time to wait between words
    public float speed;
    float lastReadTime = 0;

    public bool skipSpaceWait = true;

    float waitTime = 0;
    float waitStart;

    public string outString = "";
    //number of characters/second
    /*Dialogue guide:
     * [] is escape
     * [b] is bark 
     * [b, a, v, c] is bark, acceleration, velocity, count
     * [w, t] is wait with time
     * [ss, s] is setspeed with speed
     * 
     * Maybe add:
     * [c] clear output box
     * [ip] wait for input to progress
     * 
     */
    // Start is called before the first frame update
    void Start()
    {
        lastReadTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(lastReadTime + speed < Time.time && Time.time > waitStart + waitTime)
        {
            lastReadTime = Time.time;
            readDialog();
        }
    }
    public void readDialog()
    {
        if (position >= dialog.Length)
            return;
        while(dialog[position] == '[')
        {
            int endPos = dialog.IndexOf(']', position);
            List<string> parameters = new List<string>();

            int lastProcessPosition = position;
            for (int i = lastProcessPosition; i <= endPos; i++)
            {
                if(dialog[i] == ',' || i == endPos)
                {
                    parameters.Add(dialog.Substring(lastProcessPosition + 1, i - lastProcessPosition - 1));
                    lastProcessPosition = i;
                }
            }
            processStringEffect(parameters.ToArray());
            position = endPos + 1;
        }

        outString += dialog[position];
        position++;

        while(skipSpaceWait && dialog.Length > position && (dialog[position] == ' ' || dialog[position] == '\n' || dialog[position] == '\t'))
        {
            outString += dialog[position];
            position++;
        }
        
    }
    public void processStringEffect(params string[] input)
    {
        if(input.Length == 0)
        {
            return;
        }
        switch(input[0])
        {
            case "b":
                if (input.Length == 3)
                {
                    barkEffect(float.Parse(input[1]), float.Parse(input[2]));
                }
                else if (input.Length == 1)
                {
                    barkEffect();
                }
                else
                    Debug.LogWarning("Invalid number of parameters for bark(b)!");
                break;

            case "ss":
                if(input.Length == 2)
                {
                    speed = float.Parse(input[1]);
                }
                else
                    Debug.LogWarning("Invalid number of parameters for set speed(ss)!");
                break;

            case "w":
                if (input.Length == 2)
                {
                    waitTime = float.Parse(input[1]);
                    waitStart = Time.time;
                }
                else
                    Debug.LogWarning("Invalid number of parameters for wait(w)!");
                break;
            default:
                Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
                break;

        }
        //if(input[0] == "b")
        //{
        //    if (input.Length == 3)
        //    {
        //        barkEffect(float.Parse(input[1]), float.Parse(input[2]));
        //    }
        //    else if (input.Length == 1)
        //    {
        //        barkEffect();
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
        //}
    }
    public void barkEffect(float velocity = -1, float acceleration = 3)
    {
        GameObject addedObject = Instantiate(barkFXPrefab, transform.position, transform.rotation);
        SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        addedParticle.velocity.y = velocity;
        addedParticle.acceleration.y = acceleration;
        addedParticle.startTime = Time.time;

    }
}
