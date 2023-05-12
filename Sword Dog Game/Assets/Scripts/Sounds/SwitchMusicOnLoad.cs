using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public MusicClip newTrack;
    // TODO deprecate
    public AudioManager.GameArea newArea;
    private AudioManager theAM;

    // Start is called before the first frame update
    void Start()
    {
        if (newTrack != null)
        {
            theAM = FindObjectOfType<AudioManager>();
            // TODO this in the future
            // theAM.ChangeBGM(newTrack);
            theAM.ChangeBGM(newTrack, newArea);
            if (CutsceneController.inCutscene && CutsceneController.cutsceneStopMusic)
            {
                theAM.PauseCurrent();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
