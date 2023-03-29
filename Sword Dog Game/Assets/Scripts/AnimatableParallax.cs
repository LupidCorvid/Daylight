using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatableParallax : MonoBehaviour
{
    public bool simplifiedOptions = false;
    public float simpleDistance;
    public bool autoChangeColor = false;
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public Vector2 distance = new Vector2(0, 0);
    public Camera cam;
    public Vector3 startPos;

    public bool changeOnZoom = true;

    // Start is called before the first frame update
    void Start()
    {
        cam ??= Camera.main;
        startPos = transform.position;
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        simpleDistance = Random.Range(0, 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        UpdateDistances();
        if (cam == null)
            cam = Camera.main;
        if (changeOnZoom)
            transform.position = new Vector3(startPos.x + ((startPos.x - cam.transform.position.x) * distance.x * 5f / cam.orthographicSize), startPos.y + ((startPos.y - cam.transform.position.y) * distance.y * 5f / cam.orthographicSize), transform.position.z);
        else
            transform.position = new Vector3(startPos.x + ((startPos.x - cam.transform.position.x) * distance.x), startPos.y + ((startPos.y - cam.transform.position.y) * distance.y), transform.position.z);
        simpleDistance -= (Mathf.Cos(Time.time * 5));
    }


    private void Awake()
    {
        
    }

    protected void UpdateDistances()
    {
        Color suggestedColor;

        distance.x = (simpleDistance / 5f) * -.05f;
        distance.y = (simpleDistance / 5f) * .0175f;
        transform.localScale = new Vector3(1 / ((simpleDistance / 30f) + 1), 1 / ((simpleDistance / 30f) + 1), 1);
        if (simpleDistance > 0)
            suggestedColor = Color.Lerp(new Color(255f / 255, 221f / 255, 28f / 255), Color.black, 1 / ((simpleDistance / 10 + 1)));
        else
            suggestedColor = Color.black;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = suggestedColor;
            spriteRenderer.sortingOrder = -(int)simpleDistance;
        }
    }
}
