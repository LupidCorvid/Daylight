using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThrough : MonoBehaviour
{
    private PlayerMovement player;
    private float height;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Collider2D cldr;


    // Start is called before the first frame update
    void Start()
    {
        height = spriteRenderer.bounds.size.y / 2;
        cldr = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = PlayerMovement.controller;
        }
        else
        {
            // Debug.DrawLine((Vector3)player.bottom, transform.position - new Vector3(0, height));
            bool playerBelow = player.bottom.y < transform.position.y - height;
            
            //gameObject.layer = LayerMask.NameToLayer(playerBelow || Input.GetAxisRaw("Vertical") < -.5f ? "Utility": "Terrain");
            if(playerBelow || Input.GetAxisRaw("Vertical") < -.5f)
            {
                Physics2D.IgnoreCollision(cldr, PlayerMovement.controller.cldr, true);
            }
            else
            {
                Physics2D.IgnoreCollision(cldr, PlayerMovement.controller.cldr, false);
            }
        }
    }
}
