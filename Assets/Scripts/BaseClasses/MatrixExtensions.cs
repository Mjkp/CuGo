// ===============================
// AUTHOR          : George Adamopoulos
// CREATE DATE     : 11th of February 2019
// SPECIAL NOTES   : The extensions for the Matrix4x4 class were obtained from this discussion: https://forum.unity.com/threads/how-to-assign-matrix4x4-to-transform.121966/#post-819280
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FOR EASE OF USE WE EXTEND THE Matrix4x4 class with 2 methods for extracting position and rotation out of a matrix.
public static class MatrixExtensions
{
    public static Quaternion ExtractRotation(this Matrix4x4 m)
    {
        /* PURE MATHEMATICAL VERSION FOR REFERENCE. WILD QUATERNION MATH INVOLVED
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
        */

        // UNITY FRIENDLY VERSION. EASIER TO SEE WHAT'S GOING ON
        Vector3 forward;
        forward.x = m.m02;
        forward.y = m.m12;
        forward.z = m.m22;

        Vector3 upwards;
        upwards.x = m.m01;
        upwards.y = m.m11;
        upwards.z = m.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 m)
    {
        Vector3 position;
        position.x = m.m03;
        position.y = m.m13;
        position.z = m.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}