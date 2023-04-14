using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThrough : MonoBehaviour
{
    private PlayerMovement player;
    private float height;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        height = spriteRenderer.bounds.size.y / 2;
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
            gameObject.layer = LayerMask.NameToLayer(playerBelow ? "Utility": "Terrain");
        }
    }
}
