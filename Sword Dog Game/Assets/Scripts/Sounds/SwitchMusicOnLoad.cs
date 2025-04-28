using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public MusicClip newTrack;
    // TODO deprecate
    public AudioManager.GameArea newArea;
    private AudioManager theAM;
    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (newTrack != null)
        {
            theAM = FindObjectOfType<AudioManager>();
            // TODO this in the future
            // theAM.ChangeBGM(newTrack);

            if (CutsceneController.inCutscene && CutsceneController.cutsceneControlMusic)
            {
                return;
            }

            theAM.ChangeBGM(newTrack, newArea);
            if (delay > 0)
            {
                theAM.PauseCurrent();
                StartCoroutine(PlayMusicDelayed());
            }
        }
    }

    IEnumerator PlayMusicDelayed()
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.UnPauseCurrent();
    }
}
