using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class DevConsole : MonoBehaviour
{

    public CanvasGroup consoleGroup;
    public TextMeshProUGUI consoleOut;

    public TMP_InputField consoleInput;

    public static bool inConsole = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(consoleGroup.alpha == 1)
            {
                CloseConsole();
            }
            else
            {
                OpenConsole();
            }
        }
    }

    void OpenConsole()
    {
        inConsole = true;
        consoleGroup.alpha = 1.0f;
        consoleGroup.blocksRaycasts = true;
        consoleGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
    }

    void CloseConsole()
    {
        consoleGroup.alpha = 0;
        consoleGroup.blocksRaycasts = false;
        consoleGroup.interactable = false;
        inConsole = false;
    }

    public void TextChanged(string newText)
    {
        if(newText.EndsWith("\n"))
        {
            string result = RunCommand(newText.Remove(newText.Length-1));
            Debug.Log(newText);
            consoleInput.text = "";
            consoleOut.text += "\n" + result;
            EventSystem.current.SetSelectedGameObject(consoleInput.gameObject);
            consoleInput.OnPointerClick(new PointerEventData(EventSystem.current));
        }
        if(newText.EndsWith("`"))
        {
            consoleInput.text = newText.Remove(newText.Length-1);
        }
    }

    

    public string RunCommand(string inCommand)
    {
        string[] parts = inCommand.Split(' ');

        string commandResult = "";

        if (parts.Length > 0)
        {
            try
            {
                switch (parts[0])
                {
                    case "give_item":
                        if (parts.Length > 2)
                        {
                            Player.controller.entityBase.getAssociatedInventory().AddItem(ItemDatabase.main.getItemFromId(int.Parse(parts[1])));
                            commandResult = "Gave 1 " + ItemDatabase.main.getItemFromId(int.Parse(parts[1])).name;
                        }
                        else
                        {
                            Player.controller.entityBase.getAssociatedInventory().AddItem(ItemDatabase.main.getItemFromId(int.Parse(parts[1]), int.Parse(parts[2])));
                            commandResult = "Gave " + int.Parse(parts[2]) + " " + ItemDatabase.main.getItemFromId(int.Parse(parts[1])).name;
                        }
                        break;
                    case "play_cutscene":
                        CutsceneController.PlayCutscene(parts[1]);
                        commandResult = "Attempted to start cutscene " + parts[1];
                        break;
                    case "kill":
                        Player.controller.entityBase.TakeDamage(999999, null);
                        commandResult = "Killed player";
                        break;
                    case "load_scene":
                        SceneHelper.LoadScene(parts[1]);
                        commandResult = "Attempted to load scene \"" + parts[1] + "\"";
                        break;
                    case "assign_quest":
                        QuestsManager.main.AssignQuest(int.Parse(parts[1]));
                        commandResult = "Assigned quest " + QuestsManager.main.getQuest(int.Parse(parts[1])).questDescription;
                        break;
                    case "complete_quest":
                        QuestsManager.main.setQuestCompletion(int.Parse(parts[1]), true);
                        commandResult = "Completed quest " + QuestsManager.main.getQuest(int.Parse(parts[1])).questDescription;
                        break;
                    default:
                        commandResult = "Unrecognized command " + parts[0];
                        break;
                }
            }
            catch 
            {
                commandResult = "Command failed";
            }

        }

        return commandResult;
    }
}
