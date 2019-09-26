// ===============================
// AUTHOR          : George Adamopoulos
// CREATE DATE     : 11th of February 2019
// PURPOSE         : A class for mimicking Unity's Parent-Child behaviour without actually changing the scene hierarchy. A sort-of "soft" hierarchy.
// SPECIAL NOTES   : This code was created by George Adamopoulos. Please mention the author if you use part or the totality of the code.
// ===============================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class FakeParentingBehaviour : MonoBehaviour
{
    // Here we establish a reference to the virtual parent object
    public Transform parent;

    // This represents our default relation to the parent
    public Matrix4x4 localTransform;

    // A toggle to enable/disable this parent-child relatonship
    public bool AffectedByParent;
    public bool prevAffectedByParent;

    private Color activeColor;


    private void OnEnable()
    {
    }


    private void Update()
    {

        // Whenever we enable, OR disable the connection...
        if (prevAffectedByParent != AffectedByParent)
        {
            // We set the current relative P,S,R with respect to the "parent", as our default.
            localTransform = parent.localToWorldMatrix.inverse * transform.localToWorldMatrix;

            // Update the preview color
            activeColor = AffectedByParent ? Color.blue : Color.grey;

            prevAffectedByParent = AffectedByParent;
        }

        // If the connection is active...
        if (AffectedByParent)
        {
            // We transform the current default P,S,R in relation to the "virtual" parent
            Matrix4x4 transformation = parent.localToWorldMatrix * localTransform;

            // And we update the current actual Transform of our object
            transform.SetPositionAndRotation(transformation.ExtractPosition(), transformation.ExtractRotation());
            transform.localScale = transformation.ExtractScale();

        }
    }



    private void LateUpdate()
    {
        if (parent != null)
        {
            // Just for visualization
            DrawConnection(transform.position, parent.position, activeColor);
        }

    }

    private void DrawConnection(Vector3 A, Vector3 B, Color col)
    {
        Vector3 mid = (A + B) * 0.5f;
        Vector3 tan = A - B;

        Vector3 norm = Vector3.Cross(tan.normalized, Vector3.up).normalized;
        Vector3 angled = (tan.normalized + norm).normalized;

        norm = Vector3.Cross(-tan.normalized, Vector3.up).normalized;
        Vector3 angled2 = (tan.normalized + norm).normalized;

        Debug.DrawRay(mid, angled * tan.magnitude * 0.1f, col);
        Debug.DrawRay(mid, angled2 * tan.magnitude * 0.1f, col);
        Debug.DrawLine(A, B, col);
    }
}
