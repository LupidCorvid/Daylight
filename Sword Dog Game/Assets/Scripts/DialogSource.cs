using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DialogSource
{
    public string originalDialog;

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

    public Vector2 defaultBarkAcceleration;
    public Vector2 defaultBarkVelocity;

    public event Action barkDefault;
    public event Action<Vector2, Vector2> bark;

    public event Action exit;

    public event Action<string[]> requestOptionsStart;

    public event Action<string> changeHeaderName;

    bool waiting = false;

    public Dictionary<string, string> responseOutputs = new Dictionary<string, string>();
    public List<string> responseOutputsNumeric = new List<string>();
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
     * [sdb, x, y, x,y ] sets default bark settings
     * 
     * [c] clear output box
     * [ip] wait for input to progress
     * 
     * [sh, val] // sets header to a new string
     * [sh] //clears header
     * 
     * [prompt, 1, 1r, 2, 2r, ...] //Prompts with a number of options with their names and their results
     * 
     * [lf, path] //Sets output to be dialog from a text file at the path
     * 
     */
    public DialogSource(string dialog)
    {
        this.originalDialog = dialog;
    }

    public static DialogSource LoadFromFile(string filePath)
    {
        //string gottenText = File.ReadAllText(Path.Combine(Application.persistentDataPath, filePath));
        ////Have system for further selection within files, like grouping of text
        return new DialogSource(LoadFile(filePath));
    }

    public static string LoadFile(string filePath)
    {
        string gottenText = File.ReadAllText(Path.Combine(Application.dataPath + @"\Dialog\", filePath));
        //Have system for further selection within files, like grouping of text
        return gottenText;
    }

    public void changeToFile(string filePath)
    {
        dialog = LoadFile(filePath);
        position = 0;
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

    private int getCommandEnd(string input, int startPosition)
    {
        int depth = 0;
        for(int i = startPosition; i < input.Length; i++)
        {
            if (input[i] == '[')
                depth++;
            if (input[i] == ']')
                depth--;

            if (depth == 0)
                return i;
        }
        return input.Length;
    }

    private void readDialog()
    {
        if (waiting)
            return;
        while (position < dialog.Length && dialog[position] == '[')
        {
            //int endPos = dialog.IndexOf(']', position);
            int endPos = getCommandEnd(dialog, position);
            List<string> parameters = new List<string>();
            int depth = 0;

            int lastProcessPosition = position;
            for (int i = lastProcessPosition; i <= endPos; i++)
            {
                if (dialog[i] == '[')
                    depth++;
                if (dialog[i] == ']')
                    depth--;
                if ((dialog[i] == ',' && depth == 1) || i == endPos)
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
                else if (input.Length == 5)
                {
                    barkEffect(float.Parse(input[1]), float.Parse(input[2]), float.Parse(input[3]), float.Parse(input[4]));
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
            case "sdb":
                if (input.Length == 5)
                {
                    defaultBarkVelocity = new Vector2(float.Parse(input[1]), float.Parse(input[2]));
                    defaultBarkAcceleration = new Vector2(float.Parse(input[3]), float.Parse(input[4]));
                }
                else
                    Debug.LogWarning("Invalid number of parameters for set default bark (sdb)!");
                break;
            case "prompt":
                waiting = true;
                responseOutputs.Clear();
                responseOutputsNumeric.Clear();

                List<string> options = new List<string>();
                if(input.Length % 2 == 0)
                {
                    Debug.LogError("Not all responses have an output!");
                }
                for(int i = 1; i < input.Length; i += 2)
                {
                    options.Add(input[i]);
                    responseOutputs.Add(input[i], input[i + 1]);
                    responseOutputsNumeric.Add(input[i + 1]);
                }
                promptResponse(options.ToArray());

                break;
            case "sh":
                if (input.Length > 2)
                {

                }
                else if (input.Length == 1)
                {
                    setDisplayHeader("");
                }
                else if (input.Length == 2)
                {
                    setDisplayHeader(input[1]);
                }
                else
                    Debug.LogWarning("Invalid number of arguments for setHeader[sh]!");
                break;
            case "lf":
                if (input.Length == 2)
                {
                    changeToFile(input[1]);
                }
                else
                    Debug.LogWarning("Invalid number of arguments for loadFile [lf]!");
                break;
            default:
                Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
                break;

        }
    }

    public void resetDialog()
    {
        dialog = originalDialog;
    }
    //public void interact()
    //{
    //    DialogController.main.openBox();
    //}
    public void barkEffect()
    {
        bark?.Invoke(defaultBarkVelocity, defaultBarkAcceleration);
    }
    public void barkEffect(float velocity, float acceleration)
    {
        bark?.Invoke(new Vector2(0, velocity), new Vector2(0, acceleration));
    }
    public void barkEffect(float velocityX, float velocityY, float accelerationX, float accelerationY)
    {
        bark?.Invoke(new Vector2(velocityX, velocityY), new Vector2(accelerationX, accelerationY));
    }

    public void promptResponse(params string[] options)
    {
        if(requestOptionsStart?.Method == null)
        {
            Debug.LogError("No set responder to requestOptionsStart, prompt will never open and dialog will freeze!");
        }
        requestOptionsStart(options);
    }

    public void receiveResponse(int response)
    {
        if (position < dialog.Length)
            dialog = dialog.Insert(position, responseOutputsNumeric[response % responseOutputs.Count]);
        else
            dialog += responseOutputsNumeric[response];
        waiting = false;
    }

    public void setDisplayHeader(string newHeader)
    {
        changeHeaderName?.Invoke(newHeader);
    }
}
