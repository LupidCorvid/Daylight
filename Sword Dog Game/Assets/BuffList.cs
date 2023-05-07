using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffList : MonoBehaviour
{
    public Dictionary<int, GameObject> buffs = new Dictionary<int, GameObject>();

    public static BuffList main;

    private void Awake()
    {
        main = this;
    }

    public GameObject instantiateAndAddBuffIcon(int id, GameObject buffObj)
    {
        if (this == null || gameObject?.transform == null)
            return null;
        GameObject newBuff = Instantiate(buffObj, transform);
        buffs.Add(id, newBuff);
        return newBuff;
    }

    public void removeBuffIcon(int id)
    {
        Destroy(buffs[id]);
        buffs.Remove(id);
    }
}
