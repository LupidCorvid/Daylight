using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineController : MonoBehaviour
{
    public List<Sprite> segmentSprites = new List<Sprite>();
    public Sprite tailSprite;
    public Sprite headSprite;

    public bool randomizeSprites = true;

    public float length = 5;

    public int numSegments = 5;

    Rigidbody2D rb;

    public List<VineSegment> segments = new List<VineSegment>();

    public GameObject segmentPrefab;

    public bool generateVinesOnStart = false;

    public float windStrength = 5;
    public float windSpeed = 3;
    //Lower wind volatility means objects near eachother have similar swaying motion
    public float windVolatility = 0.2f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (generateVinesOnStart)
            loadSegments();
    }

    void AddNewSegment()
    {
        GameObject newSegment = Instantiate(segmentPrefab, transform);
        Collider2D sgmntCldr = newSegment.GetComponent<Collider2D>();
        VineSegment vinePart = newSegment.GetComponent<VineSegment>();
        
        vinePart.fillInComponents();

        float sizePerSegment = (length / numSegments) * sgmntCldr.bounds.size.y;


        newSegment.transform.localScale = new Vector3((length / numSegments), (length / numSegments), 1);

        
        if (segments.Count > 0)
        {
            vinePart.transform.position = segments[segments.Count - 1].transform.position - (sizePerSegment  * Vector3.up);
            vinePart.connection.connectedBody = segments[segments.Count - 1].rb;
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
        ClearSegments();

        for(int i = 0; i < numSegments; i++)
        {
            AddNewSegment();
        }

        setSegmentWind();
        setSegmentSprites();
    }

    public void setSegmentWind()
    {
        foreach(VineSegment segment in segments)
        {
            segment.windSpeed = windSpeed;
            segment.windStrength = windStrength;
            segment.windVolatility = windVolatility;
        }
    }

    public void setSegmentSprites()
    {
        if (segments.Count <= 0)
            return;

        for(int i = 0; i < segments.Count; i++)
        {
            if (segmentSprites.Count <= 0)
                break;

            if (randomizeSprites)
                segments[i].GetComponent<SpriteRenderer>().sprite = segmentSprites[Random.Range(0, segmentSprites.Count)];
            else
                segments[i].GetComponent<SpriteRenderer>().sprite = segmentSprites[i % segmentSprites.Count];
        }

        if (headSprite != null)
            segments[0].GetComponent<SpriteRenderer>().sprite = headSprite;
        if (tailSprite != null)
            segments[segments.Count - 1].GetComponent<SpriteRenderer>().sprite = tailSprite;
    }

    public void ClearSegments()
    {
        for(int i = segments.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(segments[i].gameObject);
            segments.RemoveAt(i);
        }
    }
}
