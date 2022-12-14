using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region Variables

    [SerializeField] private float dampTime = 0.2f;         //The time over which the camera will be moved
    [SerializeField] private float screenEdgeBuffer = 4f;   //Defines the distance the edge of the screen needs to be kept from the targets
    [SerializeField] private float minSize = 6.5f;          //The minimum size of the camera

    [SerializeField] private Transform[] targets;    //The array of camera targets that need to be kept on screen

    private Camera cam;                 //The camera being controlled
    private float zoomSpeed;            //Stores the zoom speed for calculations
    private Vector3 moveVelocity;       //Stores the move speed for calculations
    private Vector3 desiredPosition;    //Stores the calcu

    #endregion //end Variables

    #region Unity Control Methods

    //Awake is called before Start before the first frame update
    void Awake()
    {
        //Get and store the camera component found in this object's children
        cam = GetComponentInChildren<Camera>();
    }//end Awake

    //Start is called before the first frame update
    void Start()
    {
        
    }//end Start

    //Update is called once per frame
    void Update()
    {
        
    }//end Update

    //FixedUpdate is called a set number of times every second
    private void FixedUpdate()
    {
        //Move and Zoom the camera so that all the targets are on screen
        Move();
        Zoom();
    }//end FixedUpdate

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Calculate the desired position for the camera rig and smoothly move it to that position
    /// </summary>
    private void Move()
    {
        //Calculate the desired position for the camera rig
        FindAveragePosition();

        //Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }//end Move

    /// <summary>
    /// Check all valid camera targets, calculate their average position, and set that to be the camera's desired position
    /// </summary>
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        //Loop through all the targets
        for (int i = 0; i < targets.Length; i++)
        {
            //If this target is not active, skip it
            if(!targets[i].gameObject.activeSelf)
            {
                continue;
            }

            //Add this target's position to the average and increment the counter of valid targets
            averagePos += targets[i].position;
            numTargets++;
        }

        //If there is 1 or more active targets, take the average of the targets' positions
        if(numTargets > 0)
        {
            averagePos /= numTargets;
        }

        //Ensure the average y position is the one we want for the camera rig
        averagePos.y = transform.position.y;

        //Set the camera's desired position to be the calculated average position
        desiredPosition = averagePos;
    }//end FindAveragePosition

    /// <summary>
    /// Calculate the required camera size and smoothly transition to it
    /// </summary>
    private void Zoom()
    {
        //Calculate the required camera size and smoothly transition to it
        float requiredSize = FindRequiredSize();
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
    }//end Zoom

    /// <summary>
    /// Calculate and return the required size for the camera to fit all the targets on screen
    /// </summary>
    /// <returns>Returns the required size for the camera to fit all the targets on screen</returns>
    private float FindRequiredSize()
    {
        //Get the camera's desired position within the camera rig's local space
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        float size = 0f;

        //Loop through all the targets
        for (int i = 0; i < targets.Length; i++)
        {
            //If this target is not active, skip it
            if (!targets[i].gameObject.activeSelf)
            {
                continue;
            }

            //Get this target's position within the camera rig's local space
            Vector3 targetLocalPos = transform.InverseTransformPoint(targets[i].position);

            //Get the direction from the desired position to this target's position within the camera rig's local space
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            //Use the calculated direction to determine the size
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / cam.aspect);
        }

        //Add the buffer zone to the camera size
        size += screenEdgeBuffer;

        //Ensure the size is at least the minimum size
        size = Mathf.Max(size, minSize);

        //Return the calculated camera size
        return size;
    }//end FindRequiredSize

    /// <summary>
    /// Calculate the desired position and size for the camera, and set the camera values to these
    /// </summary>
    public void SetStartPositionAndSize()
    {
        //Calculate the desired position for the camera rig
        FindAveragePosition();

        //Set the position to be the desired camera position
        transform.position = desiredPosition;

        //Calculate the required size for the camera and set the camera's size to match this
        cam.orthographicSize = FindRequiredSize();
    }//end SetStartPositionAndSize

    #endregion
}
