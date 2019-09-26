using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeJointRotation : MonoBehaviour
{
    [HideInInspector] public HingeJoint InnerJoint;
    private JointMotor InnerMotor;
    [HideInInspector] public JointLimits limits;
    public bool rotRight, rotLeft;
    [HideInInspector] public bool connect, disconnect;
    [HideInInspector] public bool isJoint;
    private float minVal, maxVal;
    public float speed;
    public bool resetRot;
    public bool running;
    [HideInInspector] public bool onGround;
    [HideInInspector] public float groundY = 3.0f;
    public bool angleLimitation;



    public void Update()
    {
        if (rotRight)
        {
            if (limits.max >= 120)
            {
                rotRight = false;
                angleLimitation = true;
            }
            else
            {
                angleLimitation = false;
                RotateUnitRight(114, 26);
            }

        }
        if (rotLeft)
        {
            if (limits.min <= -120)
            {
                rotLeft = false;
                angleLimitation = true;

            }
            else
            {
                angleLimitation = false;
                RotateUnitLeft(114, 26);
            }
        }

        if(resetRot)
        {
            ResetRotation();
        }

    }

    public void ResetRotation()
    {
        if (transform.GetChild(0).GetComponent<HingeJoint>().angle >10 )
        {
            rotLeft = true;
        }
        if (transform.GetChild(0).GetComponent<HingeJoint>().angle <-10)
        {
            rotRight = true;
        }





        resetRot = false;
    }

    public void limitFalse()
    {
        angleLimitation = false;
    }

    public void RotateUnitRight(int RotationVel, int forceval)
    {

        running = true;
        InnerJoint = transform.GetChild(0).GetComponent<HingeJoint>();
        InnerMotor = InnerJoint.motor;
        InnerJoint.useMotor = true;
        InnerMotor.force = forceval;
        InnerJoint.motor = InnerMotor;
        InnerJoint.useLimits = true;
        limits = InnerJoint.limits;
        limits.bounciness = 0;
        limits.bounceMinVelocity = 0;
        if (limits.max >= 0 && limits.max <= 120 - speed && limits.min >= -0.1 && limits.min <= 120 - speed)
        {
            InnerMotor.targetVelocity = -RotationVel;
            maxVal += speed;
            minVal = maxVal - speed;
            limits.min = minVal;
            limits.max = maxVal;
            InnerJoint.limits = limits;
            InnerJoint.motor = InnerMotor;
            if (limits.max <= 120f && limits.max > 120 - speed)
            {
                rotRight = false;
                running = false;

            }
        }
        else if (limits.min < 0 && limits.min >= -120f && limits.max < speed && limits.max >= -120 + speed)
        {
            InnerMotor.targetVelocity = -RotationVel;
            minVal += speed;
            maxVal = minVal + speed;
            limits.max = minVal;
            limits.min = maxVal;
            InnerJoint.limits = limits;
            InnerJoint.motor = InnerMotor;
            if (limits.min == 0f)
            {
                rotRight = false;
                running = false;

            }

        }


        if (!rotRight)
        {
            InnerMotor.force = 0;
            InnerJoint.motor = InnerMotor;
            //transform.GetChild(0).GetComponent<Rigidbody>().mass = rigitBody;
        }
    }



    public void RotateUnitLeft(int RotationVel, int forceval)
    {
        running = true;

        InnerJoint = transform.GetChild(0).GetComponent<HingeJoint>();
        InnerMotor = InnerJoint.motor;
        InnerJoint.useMotor = true;
        InnerMotor.force = forceval;
        InnerJoint.motor = InnerMotor;
        InnerJoint.useLimits = true;
        limits = InnerJoint.limits;
        limits.bounciness = 0;
        limits.bounceMinVelocity = 0;
        if (limits.max > 0 && limits.max <= 120f && limits.min > -speed && limits.min <= 120 - speed)
        {
            InnerMotor.targetVelocity = RotationVel;
            maxVal -= speed;
            minVal = maxVal - speed;
            limits.min = minVal;
            limits.max = maxVal;
            InnerJoint.limits = limits;
            InnerJoint.motor = InnerMotor;
            if (limits.max == 0)
            {
                rotLeft = false;
                running = false;

            }
        }
        else if (limits.min <= 0 && limits.min > -120f && limits.max <= speed && limits.max > -120 + speed)
        {
            InnerMotor.targetVelocity = RotationVel;
            minVal -= speed;
            maxVal = minVal + speed;
            limits.max = maxVal;
            limits.min = minVal;
            InnerJoint.limits = limits;
            InnerJoint.motor = InnerMotor;
            if (limits.min >= -120f && limits.min < -120 + speed)
            {
                rotLeft = false;
                running = false;

            }
        }

        if (!rotLeft)
        {
            InnerMotor.force = 0;
            InnerJoint.motor = InnerMotor;
            //transform.GetChild(0).GetComponent<Rigidbody>().mass = rigitBody;
        }
    }
}
