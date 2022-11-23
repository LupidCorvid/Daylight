using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineController : MonoBehaviour
{
    public List<Sprite> segmentSprites = new List<Sprite>();
    public Sprite end;

    public float length = 5;

    public int segmentNums = 5;

    List<GameObject> segments = new List<GameObject>();

    GameObject segmentPrefab;

    public bool generateVinesRuntime = false;

    void AddNewSegment()
    {
        GameObject newSegment = Instantiate(segmentPrefab, transform);
    }

    public void loadSegments()
    {

    }
}
