using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Teleports objects into place for cutscenes that need setup
public class MoveObjectsCutscene : CutsceneData
{
    public List<MoveObject> objectsToMove = new List<MoveObject>();


    public override void startSegment()
    {
        base.startSegment();
        foreach(MoveObject transform in objectsToMove)
        {
            if (!transform.getFromName)
            {
                transform.objectToMove.transform.position = transform.newLocation;
                transform.objectToMove.transform.localScale = transform.newScale;
            }
            else
            {
                Transform toMove = GameObject.Find(transform.transformName)?.transform;
                if (toMove == null)
                {
                    Debug.LogError("Could not find object of name " + transform.transformName + " to move!");
                    continue;
                }
                toMove.transform.position = transform.newLocation;
                toMove.transform.localScale = transform.newScale;

            }
        }
        finishedSegment();
    }

    [Serializable]
    public struct MoveObject
    {
        public Transform objectToMove;
        public bool getFromName;
        public string transformName;

        public Vector3 newLocation;
        public Vector3 newScale;

        public MoveObject(Transform objectToMove, Vector3 location, Vector3 newScale, bool GetFromName = false, string TransformName = "")
        {
            this.objectToMove = objectToMove;
            newLocation = location;
            this.newScale = newScale;
            transformName = TransformName;
            getFromName = GetFromName;
        }
    }
}
