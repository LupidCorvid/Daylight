using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CutsceneNPCDialog : CutsceneData
{
    [SerializeField]
    public DialogNPC npc;
    public GameObject interactor;
    public bool overrideDialog = true;

    public string dialog;

    public CutsceneNPCDialog(DialogNPC npc, GameObject user, string dialog)
    {
        this.npc = npc;
        interactor = user;
        this.dialog = dialog;
    }

    public CutsceneNPCDialog()
    {

    }
    public override void Start()
    {
        
    }

    public override void startSegment()
    {
        base.startSegment();
        npc.closedDialog += finishedSegment;
        npc.interactor = interactor;
        if (overrideDialog)
        {
            npc.setNewSource(new DialogSource(dialog));
            npc.openDialog();
        }
        else
            npc.interact(interactor);
    }
    public override void finishedSegment()
    {
        npc.closedDialog -= finishedSegment;
        base.finishedSegment();
    }

    public override void abort()
    {
        base.abort();
        npc.exitDialog();
    }

}