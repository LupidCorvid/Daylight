using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CutsceneNPCDialog : CutsceneData
{
    [SerializeField]
    public DialogNPC npc;
    public GameObject interactor;

    public CutsceneNPCDialog(DialogNPC npc, GameObject user)
    {
        this.npc = npc;
        interactor = user;
    }

    public CutsceneNPCDialog()
    {

    }
    public override void Start()
    {
        npc.closedDialog += finishedSegment;
    }

    public override void startSegment()
    {
        base.startSegment();
        npc.interact(interactor);
    }

    public override void abort()
    {
        base.abort();
        npc.exitDialog();
    }

}