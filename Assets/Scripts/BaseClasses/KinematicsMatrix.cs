using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
//TODO how to make this as namespace
public static class KinematicsMatrix
{
    /*---------------------------------------------------------------------------------------------
     * ROTATION MATRIX
     * 
     *Rot(axis-x, alpha)  = [ 1             0              0             0   ]
     *                      [ 0             cos(alpha)    -sin(alpha)    0   ]
     *                      [ 0             sin(alpha)     cos(alpha)    0   ]
     *                      [ 0             0              0             1   ]
     * 
     *Rot(axis-z, thetha) = [ cos(thetha)   -sin(thetha)   0             0   ]
     *                      [ sin(thetha)   cos(thetha)    0             0   ]
     *                      [ 0             0              1             0   ]
     *                      [ 0             0              0             1   ]
     * 
     *Trans(axis-x, a)    = [ 1             0              0             a   ]
     *                      [ 0             1              0             0   ]
     *                      [ 0             0              1             0   ]
     *                      [ 0             0              0             1   ]
     * 
     *Trans(axis-z, d)    = [ 1             0              0             0   ]
     *                      [ 0             1              0             0   ]
     *                      [ 0             0              1             d   ]
     *                      [ 0             0              0             1   ]
     * 
     * A(i) = Rot(axis-z, thetha) * Trans(axis-z, d) * Rot(axis-x, alpha) * Trans(axis-x, a) 
     * 
     * = [  cos(thetha)    -sin(thetha)cos(alpha)    sin(thetha)sin(alpha)  a*cos(thetha)  ]
     *   [  sin(thetha)     cos(thetha)cos(alpha)   -cos(thetha)sin(alpha)  a*sin(thetha)  ]   
     *   [  0               sin(alpha)               cos(alpha)             d              ]
     *   [  0               0                        0                      1              ]   
     * 
     * ..always rotate z-axis first and x-axis second..
     * ..z-axis should always corresponds to the joint axis..
     * IMPORTANT!!!! USE UNITY MATRIX FORMAT   
     * ---------------------------------------------------------------------------------------------*/

        // angles are in degrees for input, but converted to radians for trig functions
    public static Matrix4x4 TransformationMatrix(float rz, float rx, float tx, float tz)
    {

        Matrix4x4 A = new Matrix4x4();
        A.m00 = Mathf.Cos(rz * Mathf.Deg2Rad);
        A.m10 = -Mathf.Sin(rz * Mathf.Deg2Rad) * Mathf.Cos(rx * Mathf.Deg2Rad);
        A.m20 = Mathf.Sin(rz * Mathf.Deg2Rad) * Mathf.Sin(rx * Mathf.Deg2Rad);
        A.m30 = tx * Mathf.Cos(rz * Mathf.Deg2Rad);

        A.m01 = Mathf.Sin(rz * Mathf.Deg2Rad);
        A.m11 = Mathf.Cos(rz * Mathf.Deg2Rad) * Mathf.Cos(rx * Mathf.Deg2Rad);
        A.m21 = -Mathf.Cos(rz * Mathf.Deg2Rad) * Mathf.Sin(rx * Mathf.Deg2Rad);
        A.m31 = tx * Mathf.Sin(rz * Mathf.Deg2Rad);

        A.m02 = 0;
        A.m12 = Mathf.Sin(rx * Mathf.Deg2Rad);
        A.m22 = Mathf.Cos(rx * Mathf.Deg2Rad);
        A.m32 = tz;

        A.m03 = 0;
        A.m13 = 0;
        A.m23 = 0;
        A.m33 = 1;

        return A;
    }
}