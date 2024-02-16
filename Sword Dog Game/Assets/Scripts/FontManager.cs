using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
    public List<TMP_FontAsset> fonts;
    public Settings.FontOption current;
    public static FontManager main;
    public TMP_FontAsset currFont
    {
        get
        {
            return fonts[(int)current];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        current = SettingsManager.currentSettings.fontFace;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO this is an absolutely garbage proof of concept implementation
        // in actuality we will want delegates to respond to settings and update all present/future text objects
        // individual text objects should be setting their font according to the current setting, not us setting all text objects from here
        //if (SettingsManager.currentSettings.fontFace != current)
        //{
        //    SettingsManager.currentSettings.fontFace = current;
        //    TextMeshPro[] TMP = FindObjectsOfType<TextMeshPro>();
        //    foreach (TextMeshPro t in TMP)
        //    {
        //        t.font = fonts[(int)current];
        //    }
        //    TextMeshProUGUI[] TMPGUI = FindObjectsOfType<TextMeshProUGUI>();
        //    foreach (TextMeshProUGUI t in TMPGUI)
        //    {
        //        t.font = fonts[(int)current];
        //    }
        //}
    }
}
