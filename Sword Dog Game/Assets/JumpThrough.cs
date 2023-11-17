using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThrough : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement player;
    [HideInInspector]
    public float height;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Collider2D cldr;
    private Collider2D[] playerColliders;

    private float startTime = -5;
    private float holdTime = .1f;

    // Start is called before the first frame update
    void Start()
    {
        height = spriteRenderer.bounds.size.y / 2;
        cldr = GetComponent<Collider2D>();

        //playerColliders = Player.controller.GetComponentsInChildren<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = Player.controller;
            playerColliders = Player.controller.GetComponentsInChildren<Collider2D>();
        }
        else
        {
            if(playerColliders?.Length == 0 )
                playerColliders = Player.controller.GetComponentsInChildren<Collider2D>();

            // Debug.DrawLine((Vector3)player.bottom, transform.position - new Vector3(0, height));
            bool playerBelow = player.bottom.y < transform.position.y - height;

            if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().y < -.5f)
                startTime = Time.time;

            //gameObject.layer = LayerMask.NameToLayer(playerBelow || Input.GetAxisRaw("Vertical") < -.5f ? "Utility": "Terrain");
            if (playerBelow || startTime + holdTime > Time.time)
            {
                Physics2D.IgnoreCollision(cldr, Player.controller.cldr, true);
                for (int i = 0; i < playerColliders?.Length; i++)
                {
                    Physics2D.IgnoreCollision(cldr, playerColliders[i], true);
                }
            }
            else
            {
                Physics2D.IgnoreCollision(cldr, Player.controller.cldr, false);
                for (int i = 0; i < playerColliders?.Length; i++)
                {
                    Physics2D.IgnoreCollision(cldr, playerColliders[i], false);
                }
            }
        }
    }
}
