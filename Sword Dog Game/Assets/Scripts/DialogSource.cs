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
    public float speed = 0.075f;
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

    public Vector2 defaultBarkAcceleration = new Vector2(0, 1);
    public Vector2 defaultBarkVelocity = new Vector2(1, -1);

    //public event Action barkDefault;
    public event Action<Vector2, Vector2> bark;

    public event Action exit;

    public event Action<string[]> requestOptionsStart;

    public event Action<string> changeHeaderName;

    public event Action startWaitingForInput;

    public event Action clear;

    public event Action<string[]> addEffect;

    public event Action<string> removeEffect;

    //Is just used for things like DialogNpc to have reactions to certain events
    public event Action<string[]> callEvent;

    bool waiting = false;
    public bool waitingForButtonInput = false;

    public Dictionary<string, string> responseOutputs = new Dictionary<string, string>();
    public List<string> responseOutputsNumeric = new List<string>();

    public Dictionary<string, string> dialogBlocks = new Dictionary<string, string>();

    public const string BLOCKS_SIGNATURE = "-Blocks";
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
     * 
     * 
     * CONSTRAIN VAR SIZES SO THAT DIALOG ISNT OFFSET BY IT TOO MUCH
     * [svar, var, val] sets a var with a value. If it does not exist it is created
     * [gvar, var] reads a var and puts the value of it on the dialog out
     * [gvar, var, exists, else] //Outputs one option if a var exists, and the other if it does not
     * [rvar, var] //Removes a var. Gives a warning if the var does not exist.
     * [gvar, var, var2, true, false] //Checks if a var equals another var
     * 
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
     * [lf, path, block] //Loads a block from a specific path
     * [llf, block] //Loads a different block from within the current file
     * 
     * [wi] //Waits for user input to continue
     * 
     * [IA, text] //Instantly adds some text. Meant for use with tmppro styling
     * 
     * [CE, p1, p2, p3, ...] //Calls an event. Only works if the thing running it has a listener to the event. Parameters are parsed by the listener.
     * 
     */
    public DialogSource(string dialog)
    {
        this.originalDialog = dialog;
    }

    public static Dictionary<string, string> getBlocks(string loadedText)
    {
        Dictionary<string, string> blocks = new Dictionary<string, string>();
        if (loadedText.Length > BLOCKS_SIGNATURE.Length && loadedText.Substring(0, 7) == BLOCKS_SIGNATURE)
        {
            int lastBlockStart = 0;
            for (int i = BLOCKS_SIGNATURE.Length; i < loadedText.Length; i++)
            {
                if (loadedText[i] == '{')
                {
                    
                    string titleChunk = loadedText.Substring(lastBlockStart, i - lastBlockStart);
                    //titleChunk = titleChunk.LastIndexOf('\n');
                    string blockName;
                    if (titleChunk.LastIndexOf('\n') != -1)
                        blockName = loadedText.Substring(titleChunk.LastIndexOf('\n') + lastBlockStart + 1, i - 1 - (titleChunk.LastIndexOf('\n') + lastBlockStart));
                    else
                        continue;
                    //Maybe change to use a short string series instead?
                    string blockText = loadedText.Substring(i + 3, getCommandEnd(loadedText, i, '{', '}') - (i + 3));
                    blocks.Add(blockName, blockText);
                    lastBlockStart = i + 1;
                }
            }
        }
        return blocks;
    }
    public static DialogSource LoadFromFile(string filePath)
    {
        string loadedText = LoadFile(filePath);
        Dictionary<string, string> blocks = getBlocks(loadedText);

        DialogSource returnDialog;
        if (blocks.ContainsKey("Default"))
            returnDialog = new DialogSource(blocks["Default"]);
        else
            returnDialog = new DialogSource(loadedText);
        returnDialog.dialogBlocks = blocks;
        return returnDialog;
    }

    public static string LoadFile(string filePath)
    {
        Debug.Log(Application.streamingAssetsPath);
        string gottenText = File.ReadAllText(Path.Combine(Application.streamingAssetsPath + @"\Dialog\", filePath));
        return gottenText;
    }

    public void changeToFile(string filePath)
    {
        string loadedText = LoadFile(filePath);
        Dictionary<string, string> blocks = getBlocks(loadedText);
        if (blocks.ContainsKey("Default"))
            dialog = blocks["Default"];
        else
            dialog = loadedText;
        dialogBlocks = blocks;
        position = 0;
        waiting = false;
    }

    public void changeToBlock(string block)
    {
        if (dialogBlocks.ContainsKey(block))
        {
            //dialog = dialogBlocks[block];
            //position = 0;
            dialog = dialog.Insert(position, dialogBlocks[block]);
            waiting = false;
        }
        else
            Debug.LogError("Couldnt find a block called " + block + ". Make sure you are in the right file!");
    }
    //public static DialogSource fromFile(string filePath)
    //{
    //    Debug.LogError("DialogSource.fromFile is not yet implemented!");
    //    return null;
    //    //return new DialogSource()
    //}

    public string read()
    {
        if (waiting || waitingForButtonInput)
            return outString;
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

    private static int getCommandEnd(string input, int startPosition, char openChar = '[', char closeChar = ']')
    {
        int depth = 0;
        for(int i = startPosition; i < input.Length; i++)
        {
            if (input[i] == openChar)
                depth++;
            if (input[i] == closeChar)
                depth--;

            if (depth == 0)
                return i;
        }
        return input.Length;
    }
    

    private void readDialog()
    {
        if (waiting || waitingForButtonInput)
            return;
        ///TODO: calling loadfile seems to delay text appearing by like half a second, since the prompts always take half a second to appear.
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
                clear?.Invoke();
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
                if (input.Length == 1)
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
                else if(input.Length == 3)
                {
                    changeToFile(input[1]);
                    changeToBlock(input[2]);
                }
                else
                    Debug.LogWarning("Invalid number of arguments for loadFile [lf]!");
                break;
            case "llf":
                if (input.Length == 2)
                {
                    changeToBlock(input[1]);
                }
                else
                    Debug.LogError("Invalid number of arguments for loadLocalFile [llf]!");
                break;
            case "wi":
                if (input.Length == 1)
                {
                    waitingForButtonInput = true;
                    startWaitingForInput?.Invoke();
                }
                else
                    Debug.LogError("Invalid number or arguments for waitInput[wi]!");
                break;
            case "svar":
                if(input.Length == 3)
                {
                    if (!stringVariables.ContainsKey(input[1]))
                        stringVariables.Add(input[1], input[2]);
                    else
                        stringVariables[input[1]] = input[2];
                }
                else
                    Debug.LogError("Invalid number or arguments for set var [svar]!");
                break;
            case "gvar":
                if(input.Length == 2)
                {
                    if(stringVariables.ContainsKey(input[1]))
                        dialog = dialog.Insert(position, stringVariables[input[1]]);
                }
                else if(input.Length == 4)
                {
                    if(stringVariables.ContainsKey(input[1]))
                        dialog = dialog.Insert(position, input[2]);
                    else
                        dialog = dialog.Insert(position, input[3]);
                }
                else if (input.Length == 5)
                {
                    if(stringVariables.ContainsKey(input[1]))
                    {
                        if(stringVariables[input[1]] == input[2])
                            dialog = dialog.Insert(position, input[3]);
                        else
                            dialog = dialog.Insert(position, input[4]);
                    }
                }
                else
                    Debug.LogError("Invalid number or arguments for get var [gvar]!");
                break;
            case "rvar":
                if(input.Length == 2)
                {
                    if (stringVariables.ContainsKey(input[1]))
                        stringVariables.Remove(input[1]);
                    else
                        Debug.LogWarning("No variable called " + input[1] + " found, this may be intentional, but make sure that the referenced variable is named correctly " +
                            "(remember whitespace is counted)");
                }
                break;
            case "IA":
                if (input.Length == 2)
                {
                    outString += input[1];
                }
                else
                    Debug.LogWarning("Instant Add has too many parameters!");
                break;
            case "TFX":
                addEffect?.Invoke(input);
                break;
            case "/TFX":
                if (input.Length == 2)
                    removeEffect?.Invoke(input[1]);
                else
                    Debug.LogWarning("/TFX only takes one parameter!");
                break;
            case "CE":
                callEvent?.Invoke(input[1..]);
                break;
            default:
                Debug.LogWarning("Found empty or invalid dialog command " + input[0]);
                //Maybe make it just output the input (i.e. [tester]) if there is no command found and assume that it was not intended as a command call
                break;

        }
    }

    public void resetDialog()
    {
        dialog = originalDialog;
        outString = "";
        position = 0;
        clear?.Invoke();
    }

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
        else if (response % responseOutputsNumeric.Count < responseOutputsNumeric.Count)
            dialog += responseOutputsNumeric[response % responseOutputsNumeric.Count];
        waiting = false;
    }

    public void receiveButtonInput()
    {
        if(!waitingForButtonInput)
        {
            //skip dialog? Skip to next jump or load statement or wait for input statement
        }
        waitingForButtonInput = false;
    }

    public void setDisplayHeader(string newHeader)
    {
        changeHeaderName?.Invoke(newHeader);
    }
}
