using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetTracker;
    public Vector3 offset = new Vector3(0, 0, -10);

    public float speed = 5;
    public float defaultZoom = 5;
    public static Vector3 newPos;

    public static CameraController main;

    public static Camera mainCam;

    Rigidbody2D rb;

    public Vector2 RbFollowVector;

    public Vector3 targetPoint
    {
        get
        {
            if (targetTracker != null)
                return targetTracker.position + offset;
            return Vector2.zero;
        }
    }

    public BoxCollider2D cldr;
    public Rigidbody2D followrb;
    public LayerMask camBounds;

    public bool externalControl = false;

    public bool lockX;
    public bool lockY;
    public bool lockZoom;

    public Vector3 lockDetails;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        main = this;
        mainCam = GetComponent<Camera>();
        cldr = GetComponent<BoxCollider2D>();

        if (Player.instance != null)
            targetTracker = Player.instance.transform;
        else
            targetTracker = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(targetTracker.transform.position.x, targetTracker.transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.instance != null)
            targetTracker = Player.instance.transform;
        else
            targetTracker = GameObject.FindGameObjectWithTag("Player").transform;
        followrb = targetTracker.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        //transform.position += (targetPoint - transform.position) * Time.deltaTime * speed;
    }

    private void FixedUpdate()
    {
        if (!externalControl)
        {
            Vector3 finalTarg = targetPoint;
            
            if (followrb != null)
            {
                //RbFollowVector += Vector2.right * Input.GetAxis("Horizontal") * Time.deltaTime * 2 * (Input.GetKey(KeyCode.LeftShift) ? 1.25f : 1);
                //RbFollowVector = Vector2.Lerp(RbFollowVector, Vector2.right * Input.GetAxisRaw("Horizontal") * 2, Time.deltaTime);

                //float magCap = Input.GetAxis("Horizontal"); //Link to slight inputs
                float magCap = 1f;
                if (InputReader.inputs.actions["Sprint"].IsPressed()) //TODO Input mapping
                {
                    magCap *= 1.5f; //Increase view range when sprinting
                }

                //RbFollowVector += Vector2.right * Input.GetAxis("Horizontal") * Time.deltaTime * 2 * (Input.GetKey(KeyCode.LeftShift) ? 1.25f : 1);
                //Prevent camera from moving when in menus
                if (!PlayerHealth.dead && !CutsceneController.cutsceneStopMovement && !MenuManager.inMenu && !PlayerMenuManager.open && DialogController.main?.inDialog == false && !Player.controller.stopMovement && DialogController.main?.pausePlayerMovement == false && !ChangeScene.changingScene
                    && !DevConsole.inConsole) // && not paused(?)
                {
                    if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x < 0)
                        RbFollowVector -= (RbFollowVector - new Vector2(-magCap, 0)) * Time.deltaTime * 2;
                    else if (InputReader.inputs.actions["Move"].ReadValue<Vector2>().x > 0)
                        RbFollowVector -= (RbFollowVector - new Vector2(magCap, 0)) * Time.deltaTime * 2;
                }
                //else //Recenter when not moving
                //    RbFollowVector -= (RbFollowVector - new Vector2(0, 0)) * Time.deltaTime * 2;
                //RbFollowVector = new Vector2(Mathf.Clamp(RbFollowVector.x, -magCap, magCap), Mathf.Clamp(RbFollowVector.y, -magCap, magCap));
                RbFollowVector = Vector2.ClampMagnitude(RbFollowVector, magCap);

                finalTarg += (Vector3)RbFollowVector * 2;

                ////Velocity based
                //if (Mathf.Abs(followrb.velocity.x) > .75f)
                //    RbFollowVector = Vector2.Lerp(new Vector3(followrb.velocity.x, 0, 0), RbFollowVector, .005f) * .45f;
                //finalTarg += (Vector3)RbFollowVector;
            }

            float targZoom = defaultZoom;
            if (Player.controller != null && Player.controller.sprintWindUpPercent >= 0.5 && Player.controller.isSprinting)
            {
                targZoom += SettingsManager.currentSettings.fovChange;
            }

            if (lockX)
                finalTarg.x = lockDetails.x;
            if (lockY)
                finalTarg.y = lockDetails.y;
            if (lockZoom)
                targZoom = lockDetails.z;
            

            //transform.position += (finalTarg - transform.position) * Time.deltaTime * speed;
            rb.MovePosition(speed * Time.deltaTime * (finalTarg - transform.position) + transform.position);
            if (Camera.main.orthographicSize != targZoom)
            {
                Camera.main.orthographicSize -= (Camera.main.orthographicSize - targZoom) * Time.deltaTime;
                if (Mathf.Abs(Camera.main.orthographicSize - targZoom) < .01f)
                    Camera.main.orthographicSize = targZoom;
            }
        }

        rb.velocity = Vector2.zero;

        cldr.size = new Vector3(18, 10) * mainCam.orthographicSize / 5;
    }
}
