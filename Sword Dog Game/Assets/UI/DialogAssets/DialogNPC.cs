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

    public string outString = "";
    //number of characters/second
    /*Dialogue guide:
     * [] is escape
     * [b] is bark 
     * [b, a, v, c] is bark, acceleration, velocity, count
     * [w, t] is wait with time
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
        if(lastReadTime + speed < Time.time)
        {
            lastReadTime = Time.time;
            readDialog();
        }
    }
    public void readDialog()
    {
        if (position >= dialog.Length)
            return;
        if (dialog[position] == '[')
        {
            int endPos = dialog.IndexOf(']', position);
            List<string> parameters = new List<string>();

            int processPosition = dialog.IndexOf(',', position);
            int endProcessPosition = dialog.IndexOf(',', processPosition + 1);
            if (endProcessPosition == -1)
                endProcessPosition = endPos;
            parameters.Add(dialog.Substring(position + 1, endProcessPosition - position - 1));
            while (true)
            {
                if (processPosition != -1 && processPosition <= endPos)
                    parameters.Add(dialog.Substring(processPosition, endProcessPosition));
                else
                    break;
                processPosition = dialog.IndexOf(',', processPosition + 1);
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
        if(input[0] == "b")
        {
            if (input.Length == 3)
            {
                barkEffect(float.Parse(input[1]), float.Parse(input[2]));
            }
            else if (input.Length == 1)
            {
                barkEffect();
            }
        }
        else
        {
            Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
        }
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
