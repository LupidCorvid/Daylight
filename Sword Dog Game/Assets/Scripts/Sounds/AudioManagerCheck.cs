using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;

    private void Awake()
    {
        AudioManager.WwiseGlobal = GameObject.Find("WwiseGlobal"); // TODO this is like really awful practice ngl but it'll work :D
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!FindObjectOfType<AudioManager>())
        {
            Instantiate(audioManager, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
