using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicTrigger : MonoBehaviour
{
    public AudioClip newTrack;
    private AudioClip oldTrack;
    public int BPM, timeSignature, barsLength;
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
        if (other.tag == "Player" && newTrack != null)
        {
            theAM = FindObjectOfType<AudioManager>();
            oldTrack = theAM.currentSong;
            theAM.ChangeBGM(newTrack, BPM, timeSignature, barsLength, theAM.currentArea);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && oldTrack != null)
        {
            theAM = FindObjectOfType<AudioManager>();
            theAM.ChangeBGM(oldTrack, BPM, timeSignature, barsLength, theAM.currentArea);
        }
    }
}
