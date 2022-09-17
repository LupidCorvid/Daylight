using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=wBol2xzxCOU
public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 effectMultiplier;
    [SerializeField] private bool infiniteHorizontal, infiniteVertical;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;


    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = Vector3.zero;

        // adjust tiling size for infinite scrolling
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(spriteRenderer.size.x * (infiniteHorizontal ? 3 : 1), spriteRenderer.size.y * (infiniteVertical ? 3 : 1));
        
        // calculate texture unit size for infinite scrolling
        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit * transform.localScale.x;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit * transform.localScale.y;
        
    }

    private void FixedUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * effectMultiplier.x, deltaMovement.y * effectMultiplier.y);
        lastCameraPosition = cameraTransform.position;

        // repeat infinitely horizontally
        if (infiniteHorizontal && Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {  
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }

        // repeat infinitely vertically
        if (infiniteVertical && Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
        {
            float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
        }
    }
}
