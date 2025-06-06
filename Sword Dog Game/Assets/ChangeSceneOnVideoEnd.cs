using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ChangeSceneOnVideoEnd : MonoBehaviour
{
    public string toScene = "";
    public VideoPlayer videoPlayer;
    public AK.Wwise.Event music;
    
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.started += VideoStarted;
        CanvasManager.HideHUD(true);
        videoPlayer.loopPointReached += VideoEnded;
    }

    void VideoStarted(VideoPlayer source)
    {
        videoPlayer.started -= VideoStarted;
        music?.Post(AudioManager.WwiseGlobal);
    }

    void VideoEnded(VideoPlayer source)
    {
        ChangeScene.LoadScene(toScene, "", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
