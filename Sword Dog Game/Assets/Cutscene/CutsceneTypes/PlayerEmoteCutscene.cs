using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmoteCutscene : CutsceneData
{
    Player character;
    public int emoteType;
    public float lifetime;

    public override void startSegment()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        character.spawnReEmote(emoteType, lifetime);
        finishedSegment();
    }
}
