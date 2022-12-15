using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool useRelativeRotation = true;   //Enables whether or not this script should control the rotation

    [SerializeField] private Transform cameraRig;   //Used to determine the offset needed caused by the camera angle
    [SerializeField] private Transform tank;        //Used to determine the offset needed caused by the tank's rotation throughout the game

    private float startingAngle;    //Stores the angle offset gathered from the camera angle

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        startingAngle = -cameraRig.eulerAngles.y;
    }//end Start

    // Update is called once per frame
    void Update()
    {
        if(useRelativeRotation)
        {
            float newAngle = startingAngle + tank.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }//end Update

    #endregion //end Unity Control Methods

    public void SetCameraRig(Transform rig)
    {
        cameraRig = rig;
    }
}
