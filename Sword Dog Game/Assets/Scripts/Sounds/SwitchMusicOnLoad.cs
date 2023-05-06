using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public MusicClip newTrack;
    public AudioManager.GameArea newArea;
    private AudioManager theAM;

    // Start is called before the first frame update
    void Start()
    {
        if (newTrack != null && !CutsceneController.inCutscene)
        {
            theAM = FindObjectOfType<AudioManager>();
            theAM.ChangeBGM(newTrack, newArea);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
