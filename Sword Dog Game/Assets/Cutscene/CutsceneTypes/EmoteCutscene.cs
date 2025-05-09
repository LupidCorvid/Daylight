using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteCutscene : CutsceneData
{
    public DialogNPC character;
    public int emoteType;
    public float lifetime;

    public override void startSegment()
    {
        character.spawnEmote(emoteType, lifetime);
        finishedSegment();
    }
}
