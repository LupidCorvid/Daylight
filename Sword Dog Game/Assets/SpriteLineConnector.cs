using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLineConnector : MonoBehaviour
{
    public Transform point1;
    public Transform point2;

    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (point1 == null || point2 == null)
        {
            if(gameObject != null)
                Destroy(gameObject);
            return;
        }

        transform.position = (point1.position - point2.position) / 2f + point2.position;
        sprite.size = new Vector2(sprite.size.x, Vector2.Distance(point1.position, point2.position));

        transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(point1.position - point2.position, Vector2.up));
    }
}
