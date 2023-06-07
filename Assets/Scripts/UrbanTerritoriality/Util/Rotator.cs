using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * _XForm_Camera is the moving camera
 * _XForm_CameraPivot is the point that the camera moves around (empty object)
 * Press t for switching between top and side view 
 * 
 */

public class Rotator : MonoBehaviour
{
    //For zoom
    private float curZoomPos; // Current zoom position
    private float zoomTo; // Keep track of updated zoom
    private float zoomFrom; //Start position for fieldOfView value of camera


    //FROM: https://www.youtube.com/watch?v=bVo0YLLO43s
    public Camera _XForm_Camera; //Camera that is moving
    public Transform _XForm_CameraPivot; //The empty object you want camera to pivot around

    private Vector3 _LocalRotation;


    public float MouseSensitivity = 4f;
    public float ScrollSensitvity = 5f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;
    public float xAxisSpeed = 1f;
    public float yAxisSpeed = 4f;
    public float _CameraDistance = 50f;

    public bool CameraDisabled = false;

    public bool moveToTheLeft = false;
    public bool moveToTheRight = false;
    public bool topView = false;



    private void Start()
    {
        _LocalRotation.y = 20f;
    }

    void Update()
    { 
        CheckForInput();

        if (!CameraDisabled)
        {
            if (topView)
            {
                if(_LocalRotation.y < 90f)
                {
                    _LocalRotation.y += yAxisSpeed;
                }
                else
                {
                    _LocalRotation.y = 90f;
                }
            }
            else
            {
                if (_LocalRotation.y > 20f)
                {
                    _LocalRotation.y -= yAxisSpeed;
                }
                else
                {
                    _LocalRotation.y = 20f;
                }
            }
            //Automaticly move to the left
            if (moveToTheLeft)
            {
                //Debug.Log("moveToTheLeft");
                _LocalRotation.x += xAxisSpeed;
            }
            //Automaticly move to the left
            else if (moveToTheRight)
            {
                //Debug.Log("moveToTheRight");
                _LocalRotation.x -= xAxisSpeed;
            }
            else
            {
                //Debug.Log("Zoom");
                //Zoom
                Zoom();

                //Moving
                //Detect when left mouse button is pressed
                if (Input.GetMouseButton(0)) //0 == Left mouse button
                {
                    //startX = Input.mousePosition.x;
                    //startY = Input.mousePosition.y;
                    if (Input.GetAxis("Mouse X") != 0)
                    {
                        _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                    }
                }
            }
        }
        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        this._XForm_CameraPivot.rotation = Quaternion.Lerp(this._XForm_CameraPivot.rotation, QT, Time.deltaTime * OrbitDampening);
        if (this._XForm_Camera.transform.localPosition.z != this._CameraDistance * -1f)
        {
            this._XForm_Camera.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.transform.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
        }
    }


    void Zoom(){
        // Attaches the float y to scrollwheel up or down
        float y = Input.mouseScrollDelta.y;
        zoomTo = 0;

        // If the wheel goes up it, decrement ScrollSensitvity from "zoomTo"
        if (y > 0)
        {
            zoomTo -= ScrollSensitvity;
            //Debug.Log("Zoomed In");
        }

        // If the wheel goes down, increment ScrollSensitvity to "zoomTo"
        else if (y < 0)
        {
            zoomTo += ScrollSensitvity;
            //Debug.Log("Zoomed Out");
        }

        // creates a value to raise and lower the camera's field of view
        curZoomPos = _XForm_Camera.fieldOfView + zoomTo;

        _XForm_Camera.fieldOfView = curZoomPos;
    }

    void CheckForInput(){
        if (Input.GetKeyDown("l"))
        {
            //fixX = fixX != true;
            moveToTheLeft = moveToTheLeft != true;
            moveToTheRight &= !moveToTheLeft;
        }
        if (Input.GetKeyDown("r"))
        {
            //fixY = fixY != true;
            moveToTheRight = moveToTheRight != true; //Flip the moveToTheRight
            moveToTheLeft &= !moveToTheLeft;
        }
        if (Input.GetKeyDown("space"))
        {
            CameraDisabled = CameraDisabled != true;
        }
        if(Input.GetKeyDown("t"))
        {
            topView = topView != true;
        }
    }
}