using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CutsceneNPCDialog : CutsceneData
{
    [SerializeField]
    public DialogNPC npc;
    public GameObject interactor;
    public bool getInteractorByName;
    public string interactorName;

    public bool overrideDialog = true;

    public string dialog;

    public bool restorePriorDialog;

    private DialogSource priorDialog;

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
        if (getInteractorByName)
            interactor = GameObject.Find(interactorName);

        npc.interactor = interactor;
        if (overrideDialog)
        {
            if(restorePriorDialog)
                priorDialog = npc.dialogSource;
            npc.setNewSource(new DialogSource(dialog));
            npc.openDialog();
        }
        else
            npc.interact(interactor);
    }
    public override void finishedSegment()
    {
        npc.closedDialog -= finishedSegment;
        if (restorePriorDialog)
            npc.setNewSource(priorDialog);
        base.finishedSegment();

    }

    public override void abort()
    {
        base.abort();
        npc.exitDialog();
        if (restorePriorDialog)
            npc.setNewSource(priorDialog);

    }

}