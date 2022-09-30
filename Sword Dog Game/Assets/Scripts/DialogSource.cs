using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogSource
{
    public string dialog;
    public int position;

    //Time to wait between words
    public float speed;
    float lastReadTime = 0;

    public bool skipSpaceWait = true;

    float waitTime = 0;
    float waitStart;

    public int autoBarkFrequency = 3;
    private int barkCooldown = 0;
    public Vector2 randBarkRange = new Vector2(3, 3);

    public string outString = "";

    public static Dictionary<string, string> stringVariables = new Dictionary<string, string>();

    private bool waitFrameForChar = false;

    public event Action barkDefault;
    public event Action<Vector2, Vector2> bark;

    public event Action exit;
    //number of characters/second
    /*Dialogue guide:
     * [] is escape
     * [b] is bark 
     * [b, a, v, c] is bark, acceleration, velocity, count
     * [w, t] is wait with time
     * [ss, s] is setspeed with speed
     * [abf] is auto bark frequency
     * [abf, min, max] sets the random range of abf, with max being exclusive
     * [j, pos] jumps position to new position
     * CONSTRAIN VAR SIZES SO THAT DIALOG ISNT OFFSET BY IT TOO MUCH
     * [svar, val] sets a var with a value. If it does not exist it is created
     * [var] reads a var and puts the value of it on the dialog out
     * [exit] exits dialog (closes the box, but is typically controlled by the thing holding it)
     * 
     * [c] clear output box
     * [ip] wait for input to progress
     * 
     */
    public DialogSource(string dialog)
    {
        this.dialog = dialog;
    }

    public static DialogSource fromFile(string filePath)
    {
        Debug.LogError("DialogSource.fromFile is not yet implemented!");
        return null;
        //return new DialogSource()
    }

    public string read()
    {
        while ((lastReadTime + speed < Time.time && Time.time > waitStart + waitTime))
        {
            lastReadTime = Time.time;
            readDialog();

            if (position == dialog.Length - 1 && speed == 0)
            {
                Debug.LogWarning("Speed was left at 0, this could prevent anything else from running! Always return speed to non-zero once finishing");
            }
        }
        return outString;
    }
    public void readDialog()
    {
        while (position < dialog.Length && dialog[position] == '[')
        {
            int endPos = dialog.IndexOf(']', position);
            List<string> parameters = new List<string>();

            int lastProcessPosition = position;
            for (int i = lastProcessPosition; i <= endPos; i++)
            {
                if (dialog[i] == ',' || i == endPos)
                {
                    parameters.Add(dialog.Substring(lastProcessPosition + 1, i - lastProcessPosition - 1));
                    lastProcessPosition = i;
                }
            }
            position = endPos + 1;
            processStringEffect(parameters.ToArray());
        }
        if (position >= dialog.Length)
            return;

        if (waitFrameForChar)
        {
            waitFrameForChar = false;
            return;
        }
        outString += dialog[position];
        position++;


        if (dialog.Length > position && !(dialog[position] == ' ' || dialog[position] == '\n' || dialog[position] == '\t'))
        {
            if (barkCooldown > autoBarkFrequency)
            {
                barkEffect();
                barkCooldown = 0;
                if (randBarkRange.x < randBarkRange.y)
                {
                    autoBarkFrequency = (int)UnityEngine.Random.Range(randBarkRange.x, randBarkRange.y);
                }
            }
            barkCooldown++;

        }

        while (skipSpaceWait && dialog.Length > position && (dialog[position] == ' ' || dialog[position] == '\n' || dialog[position] == '\t'))
        {
            outString += dialog[position];
            position++;
        }

    }
    public void processStringEffect(params string[] input)
    {
        if (input.Length == 0)
        {
            return;
        }
        switch (input[0])
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
                if (input.Length == 2)
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
            case "c":
                outString = "";
                break;
            case "abf":
                if (input.Length == 2)
                {
                    autoBarkFrequency = int.Parse(input[1]);
                    randBarkRange = new Vector2(autoBarkFrequency, autoBarkFrequency);

                }
                else if (input.Length == 3)
                {
                    randBarkRange = new Vector2(int.Parse(input[1]), int.Parse(input[2]));
                    autoBarkFrequency = (int)UnityEngine.Random.Range(randBarkRange.x, randBarkRange.y);
                }
                else
                    Debug.LogWarning("Invalid number of parameters for set auto bark frequency (abf)!");
                break;
            case "j":
                if (input.Length == 2)
                {
                    position = int.Parse(input[1]);
                    waitFrameForChar = true;
                }
                else
                    Debug.LogWarning("Invalid number of parameters for jump (j)!");
                break;
            case "exit":
                exit?.Invoke();
                break;
            default:
                Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
                break;

        }
    }

    public void interact()
    {
        DialogController.main.openBox();
    }

    public void barkEffect(float velocity = -1, float acceleration = 3)
    {
        //GameObject addedObject = Instantiate(barkFXPrefab, transform.position, transform.rotation);
        //SpeakParticle addedParticle = addedObject.GetComponent<SpeakParticle>();
        //addedParticle.velocity.y = velocity;
        //addedParticle.acceleration.y = acceleration;
        //addedParticle.startTime = Time.time;
        bark?.Invoke(new Vector2(0, velocity), new Vector2(0, acceleration));

    }
}
