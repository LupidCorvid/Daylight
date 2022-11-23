using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineController : MonoBehaviour
{
    public List<Sprite> segmentSprites = new List<Sprite>();
    public Sprite end;

    public float length = 5;

    public int numSegments = 5;

    Rigidbody2D rb;

    List<VineSegment> segments = new List<VineSegment>();

    public GameObject segmentPrefab;

    public bool generateVinesRuntime = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (generateVinesRuntime)
            loadSegments();
    }

    void AddNewSegment()
    {
        GameObject newSegment = Instantiate(segmentPrefab, transform);
        Collider2D sgmntCldr = newSegment.GetComponent<Collider2D>();
        VineSegment vinePart = newSegment.GetComponent<VineSegment>();

        float sizePerSegment = (length / numSegments) * sgmntCldr.bounds.size.y;


        newSegment.transform.localScale = new Vector3((length / numSegments), (length / numSegments), 1);

        
        if (segments.Count > 0)
        {
            vinePart.transform.position = segments[segments.Count - 1].transform.position - (sizePerSegment  * Vector3.up);
            vinePart.connection.connectedBody = segments[segments.Count - 1].rb;
            //vinePart.connection.connectedAnchor = Vector2.down * 1;
            vinePart.connection.connectedAnchor = sgmntCldr.bounds.extents.y * Vector2.down;
        }
        else
        {
            vinePart.transform.position = transform.position - (sizePerSegment / 2 * Vector3.up);
            vinePart.connection.connectedBody = rb;
        }

        vinePart.connection.anchor = Vector2.up * sgmntCldr.bounds.extents.y;

        segments.Add(vinePart);
    }

    public void loadSegments()
    {
        for(int i = 0; i < numSegments; i++)
        {
            AddNewSegment();
        }
    }
}
