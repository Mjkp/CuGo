using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPhysics : MonoBehaviour
{
    /// <summary>
    /// The generate jointat start.
    /// </summary>
    public bool generateJointAtStart;

    /// <summary>
    /// list of gameobjects, hingejoints, and fixed joints
    /// to manipulate the states in flexible way
    /// </summary>
    public List<GameObject> GroupStates;
    public List<GameObject> HalfcubeStates;
    public List<HingeJoint> blueHingeJoints;
    public List<FixedJoint> redFixedJoints;

    /// <summary>
    /// list of tranform and vectors to access position data
    /// </summary>
    public List<Transform> GroupTransformStates;
    public List<Transform> HalfcubeTransformStates;
    public List<Vector3> groupPositions;
    public List<Vector3> halfcubePositions;


    public List<Quaternion> groupRotations;
    public List<Quaternion> halfcubeRotations;
    public List<Rigidbody> rigidbodies;

    public GameObject plane;



    public float softJointStrength;
    public Vector3 anchorVector;
    public Vector3 RotaionAxis;
    public Vector3 connectedAnchor;

    public float rigidBodyMass;

    

    void Awake()
    {
        GetGameObjectStates();
        GetTransformStates();
    }

    private void Start()
    {
        if (generateJointAtStart)
        {
            SetJoints();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetJoints();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetStates();
        }
    }

    public void GetGameObjectStates()
    {
        GroupStates = new List<GameObject>();
        HalfcubeStates = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject group = transform.GetChild(i).gameObject;
            GroupStates.Add(group);

            for (int j = 0; j < group.transform.childCount; j++)
            {
                GameObject halfcube = group.transform.GetChild(j).gameObject;
                HalfcubeStates.Add(halfcube);
            }
        }
    }

    public void GetTransformStates()
    {
        GroupTransformStates = new List<Transform>();
        HalfcubeTransformStates = new List<Transform>();

        groupPositions = new List<Vector3>();
        halfcubePositions = new List<Vector3>();

        groupRotations = new List<Quaternion>();
        halfcubeRotations = new List<Quaternion>();

        rigidbodies = new List<Rigidbody>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform group = transform.GetChild(i);
            GroupTransformStates.Add(group);
            groupPositions.Add(group.position);
            groupRotations.Add(group.rotation);

            for (int j = 0; j < group.childCount; j++)
            {
                Transform halfcube = group.GetChild(j);
                HalfcubeTransformStates.Add(halfcube);
                halfcubePositions.Add(halfcube.position);
                halfcubeRotations.Add(halfcube.rotation);
            }
        }
    }



    public void SetStates()
    {
        //plane fixed joint destroy
        Destroy(plane.GetComponent<FixedJoint>());

        //destroy fixed joint between units
        for (int i = 0; i < redFixedJoints.Count; i++)
        {
            Destroy(redFixedJoints[i]);
        }
        //destroy hinge joints
        for (int i = 0; i < blueHingeJoints.Count; i++)
        {
            Destroy(blueHingeJoints[i]);
        }
        // destroy rigidbodies after all the joints
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            Destroy(rigidbodies[i]);
        }

        for (int i = 0; i < HalfcubeStates.Count; i++)
        {
            HalfcubeTransformStates[i].SetPositionAndRotation(halfcubePositions[i], halfcubeRotations[i]);
        }
        for (int i = 0; i < GroupStates.Count; i++)
        {
            GroupTransformStates[i].SetPositionAndRotation(groupPositions[i], groupRotations[i]);
        }
    }



    public void SetJoints()
    {
    
        Debug.Log("resetPhysics");
        //add the rigid bodies first
        for (int i = 0; i < HalfcubeStates.Count; i++)
        {
            HalfcubeStates[i].AddComponent<Rigidbody>();
        }

        //add hinge joint
        for (int i = 0; i < GroupStates.Count; i++)
        {
            GameObject blue = GroupStates[i].transform.GetChild(0).gameObject;
            Rigidbody red = GroupStates[i].transform.GetChild(1).GetComponent<Rigidbody>();
            red.mass = rigidBodyMass;

            AddHinge(blue, red);
        }

        //add fixed joint
        for (int i = 0; i < GroupStates.Count -1; i++)
        {
            GameObject thisUnit = GroupStates[i].transform.GetChild(1).gameObject;
            Rigidbody connectedUnit = GroupStates[i+1].transform.GetChild(0).GetComponent<Rigidbody>();
            connectedUnit.mass = rigidBodyMass;
            AddSoftFixed(thisUnit,connectedUnit);
        }

        //re make the rigidbody list and joint list
        GetRigidBody();
        GetJointStates();
        // fixjoint with the plane and the first unit of the halfcubes
        AddHardFixed(plane, HalfcubeStates[0].GetComponent<Rigidbody>());
    }

    
    public void GetJointStates()
    {
        blueHingeJoints = new List<HingeJoint>();
        redFixedJoints = new List<FixedJoint>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject group = transform.GetChild(i).gameObject;
            blueHingeJoints.Add(group.transform.GetChild(0).GetComponent<HingeJoint>());
        }
        for (int i = 0; i < transform.childCount -1; i++)
        {
            GameObject group = transform.GetChild(i).gameObject;
            redFixedJoints.Add(group.transform.GetChild(1).GetComponent<FixedJoint>());
        }
    }

    public void GetRigidBody()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform group = transform.GetChild(i);
            for (int j = 0; j < group.childCount; j++)
            {
                Transform halfcube = group.GetChild(j);
                rigidbodies.Add(halfcube.GetComponent<Rigidbody>());
            }
        }
    }

    public void AddHinge(GameObject jointBody,Rigidbody connectedRigidBody)
    {
        HingeJoint connection = jointBody.AddComponent<HingeJoint>();
        connection.connectedBody = connectedRigidBody;
        //connection.anchor = new Vector3(-0.5f, 0.5f, -0.5f);
        //connection.axis = new Vector3(-1, 1, -1);
        //connection.connectedAnchor = new Vector3(-0.5f, 0.5f, -0.5f);
        connection.anchor = anchorVector;
        connection.axis = RotaionAxis;
        connection.connectedAnchor = connectedAnchor;
        connection.useMotor = true;
        connection.useLimits = true;
        connection.breakForce = Mathf.Infinity;
        connection.breakTorque = Mathf.Infinity;
        connection.autoConfigureConnectedAnchor = false;
        JointLimits limits = connection.limits;
        limits.bounceMinVelocity = 0;
        limits.bounciness = 0;
        connection.limits = limits;

    }

    public void AddSoftFixed(GameObject jointBody, Rigidbody connectedRigidBody)
    {
        FixedJoint connection = jointBody.AddComponent<FixedJoint>();
        connection.connectedBody = connectedRigidBody;
        connection.breakForce = softJointStrength;
        connection.breakTorque = softJointStrength;
    }

    public void AddHardFixed(GameObject jointBody, Rigidbody connectedRigidBody)
    {
        FixedJoint connection = jointBody.AddComponent<FixedJoint>();
        connection.connectedBody = connectedRigidBody;
        connection.breakForce = Mathf.Infinity;
        connection.breakTorque = Mathf.Infinity;
    }

    public void AssignYRotation(int defaultRot,int[] rotationVal)
    {
        for( int i = 0; i < transform.childCount; i ++)
        {
            transform.GetChild(i).localEulerAngles = new Vector3(0, defaultRot+rotationVal[i], 0);
        }
    }
}
