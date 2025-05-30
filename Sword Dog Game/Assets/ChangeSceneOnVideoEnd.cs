using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ChangeSceneOnVideoEnd : MonoBehaviour
{

    public string toScene = "";
    public VideoPlayer videoPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        CanvasManager.HideHUD(true);
        videoPlayer.loopPointReached += VideoEnded;
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
