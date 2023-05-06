using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicTrigger : MonoBehaviour
{
    public MusicClip newTrack;
    private MusicClip oldTrack;
    private AudioManager theAM;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && newTrack != null && !CutsceneController.inCutscene)
        {
            theAM = FindObjectOfType<AudioManager>();
            oldTrack = theAM.currentSong;
            theAM.ChangeBGM(newTrack, theAM.currentArea);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && oldTrack != null && !CutsceneController.inCutscene)
        {
            theAM = FindObjectOfType<AudioManager>();
            theAM.ChangeBGM(oldTrack, theAM.currentArea);
        }
    }
}
