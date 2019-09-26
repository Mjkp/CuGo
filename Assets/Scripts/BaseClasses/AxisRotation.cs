using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotation : MonoBehaviour
{
    public bool isRotatingRight;
    public bool isRotatingLeft;
    private float Step = 1;
    public float RotationState;
    private Vector3 axisF;
    private Vector3 origin;
    public bool resetRot;
    public int rotationDir; 
    // Start is called before the first frame update
    void Start()
    {
        RotationState = 0;
        currenAxisnOrigin();

    }

    // Update is called once per frame
    public void Update()
    {
        if (resetRot) totheOrigin();
        if (isRotatingRight) RotateRight();
        if (isRotatingLeft) RotateLeft();

        RotationInterruption();
        currenAxisnOrigin();
    }

    public void currenAxisnOrigin()
    {
        origin = transform.position;
        if (rotationDir == 0)
        {
            axisF = transform.forward;

        }
        else if (rotationDir == 1)
        {
            axisF = transform.right;

        }
    }
    public void RotationInterruption()
    {
        if (isRotatingLeft && isRotatingRight)
        {
            isRotatingLeft = false;
            isRotatingRight = false;
        }
    }

    public void RotateRight()
    {
        if(RotationState>=0 && RotationState<120)
        {
            transform.RotateAround(origin, axisF, Step);
            RotationState += Step;
            if (RotationState==120)
            {
                isRotatingRight = false;

            }
        }
        else if (RotationState>=-120 && RotationState<0)
        {
            transform.RotateAround(origin, axisF, Step);
            RotationState += Step;
            if (RotationState == 0)
            {
                isRotatingRight = false;

            }
        }
        else
        {
            isRotatingRight = false;
        }
    }
    public void RotateLeft()
    {
        if (RotationState <= 0 && RotationState > -120)
        {
            transform.RotateAround(origin, axisF, -Step);
            RotationState -= Step;
            if (RotationState == -120)
            {
                isRotatingLeft = false;

            }
        }
        else if (RotationState <= 120 && RotationState >0)
        {
            transform.RotateAround(origin, axisF,-Step);
            RotationState -= Step;
            if (RotationState == 0)
            {
                isRotatingLeft = false;
            }
        }
        else
        {
            isRotatingLeft = false;
        }

    }

    public void totheOrigin()
    {
        if (RotationState >0)
        {
            isRotatingLeft = true;
        }
        else if (RotationState < 0)
        {
            isRotatingRight = true;
        }
        resetRot = false;
    }

}
