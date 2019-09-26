using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class HingeRotation_config : MonoBehaviour
{
    ///<summary>
    /// angle configurations
    /// </summary>
    public Vector4[] config;

    /// <summary>
    /// end effector intiger vector values by forward kinematics
    /// </summary>
    public Vector3[] IntfkValue;

    /// <summary>
    /// counter for the all possibilities
    /// </summary>
    [HideInInspector] public int counter = 0;

    [HideInInspector] public int numberofUnits;
    [HideInInspector] public List<HingeJointRotation> joints;
    [HideInInspector] public DHParameters dhkinematics;

    /// <summary>
    /// binary val list to dispatch angle configuration that goes below ground
    /// </summary>
    public int[] underGround;

    public Transform Targets; // parent Gameobject of all targets


    /// <summary>
    /// all targets that are detected
    /// </summary>
    public List<Vector3> allTargets;

    /// <summary>
    /// targets on ground level 1,2,3
    /// </summary>
    public List<Vector3> targetsOnL1;
    public List<Vector3> targetsOnL2;
    public List<Vector3> targetsOnL3;
    public List<Vector3> L2onTopL1;
    public List<Vector3> L3onTopL2;
    private int edgeLength = 6;

    /// <summary>
    /// to visually check the location of endeffector
    /// </summary>
    public Transform virtualObject;

    /// <summary>
    /// dictionary for level1,2,3 configuration 
    /// each of them contains endeffector position as key and angle configuration as value
    /// </summary>
    [HideInInspector] public Dictionary<Vector3, Vector4> L1lookupTable;
    [HideInInspector] public Dictionary<Vector3, Vector4> L2lookupTable;
    [HideInInspector] public Dictionary<Vector3, Vector4> L3lookupTable;
    [HideInInspector] public Dictionary<Vector3, Vector4> FullLookupTable;
    /// <summary>
    /// four angles for four units
    /// incase for 5 units use Vector5
    /// </summary>
    public Vector4 angleVal;
    private UDPClient udpClient;

    /// <summary>
    /// the position where the active units are located on board
    /// </summary>
    public Vector3 CuGoP;


    public void Start()
    {
        edgeLength = 6;
        udpClient = GetComponent<UDPClient>();
        dhkinematics = GetComponent<DHParameters>();
        numberofUnits = transform.childCount;
        SetConfigTable();
        setJointList(0);
        SetAngles();
        SetAvoidList();
        SetFullLookUpTable();

    }

    public bool CuGoRun;

    public void Update()
    {
        ///<summary>
        /// configure the robot arm
        /// </summary>
        /// 
        if (this.name == "CuGo1")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DetectAllTargets();
                SetTargetsInLayers(allTargets.ToArray()); // from this always use targetsOnL1,L2,L3 list
                ObserveStateBeforeMovementandAssign(allTargets.ToArray());
                //PrintLookUpTable(L1lookupTable);
                //PrintLookUpTable(L2lookupTable);
                //PrintLookUpTable(L3lookupTable);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    joints[i].resetRot = true;
                }
                // flip the rotation value and send
                angleVal *= -1;
                udpClient.InitiateTimer(1); // 0 for forward 1 for backword

            }
        }
        else if (this.name == "CuGo2")
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                DetectAllTargets();
                SetTargetsInLayers(allTargets.ToArray()); // from this always use targetsOnL1,L2,L3 list
                ObserveStateBeforeMovementandAssign(allTargets.ToArray());
                //PrintLookUpTable(L1lookupTable);
                //PrintLookUpTable(L2lookupTable);
                //PrintLookUpTable(L3lookupTable);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    joints[i].resetRot = true;
                }
                // flip the rotation value and send
                angleVal *= -1;
                udpClient.InitiateTimer(1); // 0 for forward 1 for backword

            }
        }
        //if (CuGoRun)
        //{
        //    DetectAllTargets();
        //    SetTargetsInLayers(allTargets.ToArray()); // from this always use targetsOnL1,L2,L3 list
        //    ObserveStateBeforeMovementandAssign(allTargets.ToArray());
        //    CuGoRun = !CuGoRun;
        //}

        ///<summary>
        /// re configure to origin position
        /// </summary>




        CuGoP = transform.GetChild(0).position;
    }


    /// <summary>
    /// detecting all targets on the board
    /// </summary>
    public void DetectAllTargets()
    {
        allTargets = new List<Vector3>();
        for ( int i = 0; i < Targets.childCount; i++)
        {
            float xPos = Targets.GetChild(i).position.x - transform.GetChild(0).position.x;
            float yPos = Targets.GetChild(i).position.y  - transform.GetChild(0).position.y; //0.5f is just to adjust with the physical world
            float zPos = Targets.GetChild(i).position.z - transform.GetChild(0).position.z;
            int x = Mathf.RoundToInt(xPos);
            int y = Mathf.RoundToInt(yPos);
            int z = Mathf.RoundToInt(zPos);

            allTargets.Add(new Vector3(roundToMultiple(x,edgeLength), roundToMultiple(y, edgeLength), roundToMultiple(z, edgeLength)));
        }
    }

    /// <summary>
    /// Sets the targets in layers.
    /// </summary>
    /// <param name="targets">array of all targets detected.</param>
    public void SetTargetsInLayers(Vector3[] targets)
    {
        targetsOnL1.Clear();
        targetsOnL2.Clear();
        targetsOnL3.Clear();
        L2onTopL1.Clear();
        L3onTopL2.Clear();

        targetsOnL1 = new List<Vector3>();
        targetsOnL2 = new List<Vector3>();
        targetsOnL3 = new List<Vector3>();
        L2onTopL1 = new List<Vector3>();
        L3onTopL2 = new List<Vector3>();
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (L1lookupTable.ContainsKey(RoundVec(targets[i])))
                {
                    targetsOnL1.Add(RoundVec(targets[i]));
                }
                else if(L2lookupTable.ContainsKey(RoundVec(targets[i])))
                {
                    targetsOnL2.Add(RoundVec(targets[i]));
                }
                else if(L3lookupTable.ContainsKey(RoundVec(targets[i])))
                {
                    targetsOnL3.Add(RoundVec(targets[i]));
                }

                // looking for possible position on top on detected passive units
                Vector3 StagedTargetL2 = new Vector3(targets[i].x, targets[i].y+edgeLength, targets[i].z);
                if(L2lookupTable.ContainsKey(RoundVec(StagedTargetL2)))
                {
                    L2onTopL1.Add(StagedTargetL2);
                }

                // only looking at targets that are on second level
                if(targets[i].y> edgeLength-1)
                {
                    Vector3 StagedTargetL3 = new Vector3(targets[i].x, targets[i].y + edgeLength, targets[i].z);
                    if(L3lookupTable.ContainsKey(RoundVec(StagedTargetL2)))
                    {
                        L3onTopL2.Add(StagedTargetL3);
                    }
                }
            }



        }
        else 
        {
            Debug.Log(this.name + ": there are no targets to search!");
        }

        // we have to remove 0,edgenlength*4,0 from the dictionary orelse when there is a target on top of the endEffector it will not move
        Vector3 TargetonTop = new Vector3(0, edgeLength * transform.childCount, 0);
        Vector3 TargetonBottom = Vector3.zero;

        L3lookupTable.Remove(TargetonTop);
        L1lookupTable.Remove(TargetonBottom);
        FullLookupTable.Remove(TargetonBottom);
        FullLookupTable.Remove(TargetonTop);

        // always use targetsonBounds array
        // i do not care about the targets that are not reachable.
    }

    public void AssignAngles(Vector3 passiveBlock, Dictionary<Vector3, Vector4> whichlookupTable)
    {

        //if (whichlookupTable.ContainsKey(new Vector3(0,edgeLength* transform.childCount, 0)))
        //{
        //    whichlookupTable.Remove(new Vector3(0, edgeLength * transform.childCount, 0));
        //}

        if (whichlookupTable.ContainsKey(passiveBlock))
        {
            //Vector4 angleConfiguration = whichlookupTable[passiveBlock];
            angleVal = whichlookupTable[passiveBlock];
            Debug.Log(this.name+": Found the target pos: " + passiveBlock + "angles" + angleVal.x + "," + angleVal.y + "," + angleVal.z + "," + angleVal.w);
            DecideRotation((int)this.angleVal.w, joints[3]);
            DecideRotation((int)this.angleVal.z, joints[2]);
            DecideRotation((int)this.angleVal.y, joints[1]);
            DecideRotation((int)this.angleVal.x, joints[0]);
            udpClient.InitiateTimer(0);               // 0 for forward 1 for backword
        }
        else 
        {
            // get random index
            List<Vector3> keyList4Hint = new List<Vector3>(whichlookupTable.Keys);
            Vector3 randomVec = keyList4Hint[UnityEngine.Random.Range(0, keyList4Hint.Count)];
            angleVal = whichlookupTable[randomVec];
            Debug.Log(this.name + ": Not Found, suggesting a target pos= " + randomVec + " | angles= " + angleVal.x + "," + angleVal.y + "," + angleVal.z + "," + angleVal.w); 
            DecideRotation((int)this.angleVal.w, joints[3]);
            DecideRotation((int)this.angleVal.z, joints[2]);
            DecideRotation((int)this.angleVal.y, joints[1]);
            DecideRotation((int)this.angleVal.x, joints[0]);
            udpClient.InitiateTimer(0);               // 0 for forward 1 for backword
        }
    }


    /// <summary>
    /// selecting a target among multiple passive units that are reachable
    /// selecting one with most joint rotation, which brings the gravitational point closer to the base unit
    /// </summary>
    /// <returns>The atarget.</returns>
    /// <param name="targetsinRange">list of target to scan through.</param>
    /// <param name="whichlookupTable"> dictionary to search in</param>
    public Vector3 SelectAtarget(Vector3[] targetsinRange, Dictionary<Vector3, Vector4> whichlookupTable)
    {
        Vector3 oneTarget = Vector3.zero;
        if (targetsinRange.Length > 1)
        {
            float mostRotatedVal = 0;
            for (int i = 0; i < targetsinRange.Length; i++)
            {
                Vector4 allJointsAngles = whichlookupTable[RoundVec(targetsinRange[i])];
                float angleSum = Mathf.Abs(allJointsAngles.x) + Mathf.Abs(allJointsAngles.y) + Mathf.Abs(allJointsAngles.z) + Mathf.Abs(allJointsAngles.w);
                if (angleSum > mostRotatedVal)
                {
                    mostRotatedVal = angleSum;
                    oneTarget = targetsinRange[i];
                }
            }

        }
        else if (targetsinRange.Length == 1)
        {
            oneTarget = targetsinRange[0];
        }
        else
        {
            //Debug.Log(this.name + ": there are no targets that are reachable");
        }
        return RoundVec(oneTarget);
    }

    

    /// <summary>
    /// Observes the state of the environment, position of passive units, before movement is assigned.
    /// </summary>
    /// <param name="passiveBlocks">Passive blocks.</param>
    public void ObserveStateBeforeMovementandAssign(Vector3[] passiveBlocks)
    {
        if (passiveBlocks.Length == 0)
        {
            Debug.Log("there are no passive units detected");
        }
        bool GrabbedOne = false;
        for(int i = 0; i < passiveBlocks.Length;i++)
        {

            if (passiveBlocks[i].y >= edgeLength * (transform.childCount -2)
                && Mathf.Abs(passiveBlocks[i].x) < 3f 
                && Mathf.Abs(passiveBlocks[i].z) < 3f) 
            {
                GrabbedOne = true;
                Debug.Log(this.name + ": passive units on top of me");
                break;
            }
        }

        if (GrabbedOne)
        {

            // search towards majority, todo but it should search through all targets. OR todo this should be tranined through RL 
            int TonL1 = 0;
            int TonL2 = 0;
            for (int i = 0; i < passiveBlocks.Length; i++)
            {

                if(Mathf.RoundToInt(passiveBlocks[i].y) == 0)
                {
                    TonL1++;
                }

                // check whether there are targets on top of each other. if found one eliminate one from the TonL1
                for( int j = i+1; j<passiveBlocks.Length; j++)
                {
                    if(roundToMultiple((int)passiveBlocks[i].x,edgeLength) == roundToMultiple((int)passiveBlocks[j].x,edgeLength) 
                    && roundToMultiple((int)passiveBlocks[i].z,edgeLength) == roundToMultiple((int)passiveBlocks[j].z,edgeLength))
                    {
                        TonL2++;
                        TonL1--;

                    }
                }
            }

            if(TonL1>TonL2)
            {
                //2nd level
                Vector3 targetCandidate = (SelectAtarget(L2onTopL1.ToArray(), L2lookupTable)); //TransformGlobalVec2LocalVec(SelectAtarget(targetsOnL2.ToArray(), L2lookupTable))
                Debug.Log(this.name + ": moving to layer 2");
                AssignAngles(targetCandidate, L2lookupTable);
            }
            else
            {
                // 3rd level
                Vector3 targetCandidate = (SelectAtarget(L3onTopL2.ToArray(), L3lookupTable)); //TransformGlobalVec2LocalVec(SelectAtarget(targetsOnL3.ToArray(), L3lookupTable)
                Debug.Log(this.name + ": moving to layer 3");
                AssignAngles(targetCandidate, L3lookupTable);
            }

        }
        else if(!GrabbedOne)
        {

            // all level
            Vector3 targetCandidate =  (SelectAtarget(targetsOnL1.ToArray(), FullLookupTable)); //TransformGlobalVec2LocalVec(SelectAtarget(targetsOnL1.ToArray(), L1lookupTable));
            Debug.Log(this.name + ": trying to grab passive units");
            AssignAngles(targetCandidate, FullLookupTable);
            return;
        }
    }

    public void SetFullLookUpTable()
    {

        L1lookupTable = new Dictionary<Vector3, Vector4>();
        L2lookupTable = new Dictionary<Vector3, Vector4>();
        L3lookupTable = new Dictionary<Vector3, Vector4>();
        FullLookupTable = new Dictionary<Vector3, Vector4>();

        int L1Count = 0;
        int L2Count = 0;
        int L3Count = 0;
        int FullCount = 0; 

        // for loop for dividing the lookup table according to its levels
        for (int i = 0; i < counter; i++)
        {
            if(IntfkValue[i].y >= -edgeLength && underGround[i] == 0)
            {
                if(!L1lookupTable.ContainsKey(IntfkValue[i]) && IntfkValue[i].y == 0)
                {
                    L1lookupTable.Add(IntfkValue[i], config[i]);
                    L1Count++;
                }
                else if(!L2lookupTable.ContainsKey(IntfkValue[i]) && IntfkValue[i].y == edgeLength)
                {
                    L2lookupTable.Add(IntfkValue[i], config[i]);
                    L2Count++;
                }
                else if (!L3lookupTable.ContainsKey(IntfkValue[i]) && IntfkValue[i].y > edgeLength)
                {
                    L3lookupTable.Add(IntfkValue[i], config[i]);
                    L3Count++;
                }
            }
        }

        //TODO adding value to the existing key in the dictionary
        //  public Dictionary<Vector3, List<Vector4>> lookupTable
        //  public void testListDict()
        //  {
        //
        //    // dict
        //    Dictionary <Vector3, List<Vector4>> lut = new Dictionary<Vector3, List<Vector4>>();
        //    Vector3 key = new Vector3(1, 1, 1);
        //    Vector4 config = new Vector4(1, 2, 3, 4);
        //    List<Vector4> result;
        //
        //    // if it's the first time: array doesn't exist yet, we make it
        //
        //    if (!lut.TryGetValue(key, out result))
        //    {
        //        result = new List<Vector4>();
        //    }
        //
        //    if (result != null)
        //    {
        //    // added first value
        //    result.Add(config1);
        //    // put back into LUT
        //    lut.Add(key, result);
        //    }
        //
        //    Vector4 config2 = new Vector4(2, 4, 6, 8);
        //    lut.TryGetValue(key, out result);
        //    if (result != null)
        //    {
        //        // added first value
        //        result.Add(config2);
        //    }
        //    Debug.Log(lut[key][0] + "," + lut[key][1]);
        //}

        //for loop for general lookup table for passive units in all levels
        for (int i = 0; i < counter; i++)
        {
            if (IntfkValue[i].y >= -edgeLength && underGround[i] == 0)
            {
                if (!FullLookupTable.ContainsKey(IntfkValue[i]))
                {
                    FullLookupTable.Add(IntfkValue[i], config[i]);
                    FullCount++;
                }
            }
        }
        //Debug.Log("l1, l2, l3, full: key counts  " + L1Count + "," + L2Count + "," + L3Count+ "," + FullCount);


    }

    public void SetAngles()
    {
        IntfkValue = new Vector3[(int)Mathf.Pow(3, numberofUnits)];
        dhkinematics.rotAngle = new List<int[]>();
        for (int i = 0; i <  counter; i++)
        {
            int[] anglePatterns = { (int)config[i].x, (int)config[i].y, (int)config[i].z, (int)config[i].w };
            dhkinematics.rotAngle.Add(anglePatterns);
            dhkinematics.SetFullDHtable(i);
            IntfkValue[i] = RoundVec(DHForwardKinematics().ExtractPosition());
            dhkinematics.ConventionalDHtable.Clear();
        }
    }



    /// <summary>
    /// where it sends unity physics rotation to rotate
    /// </summary>
    /// <param name="x">the target angle in degrees.</param>
    public void DecideRotation(int x, HingeJointRotation joint)
    {
        switch(x)
        {
            case 120:
                joint.rotRight = true;
                break;
            case -120:
                joint.rotLeft = true;
                break;
            case 0:
                break;
        }
    }

    /// <summary>
    /// generate the endeffector position vector by using conventional dh parameters
    /// </summary>
    /// <returns>The 4x4 Matrix of position.</returns>
    public Matrix4x4 DHForwardKinematics()
    {
        Matrix4x4 T = Matrix4x4.identity;
        for (int i = 0; i < dhkinematics.ConventionalDHtable.Count; i++)
        {
            //TransformationMatrix( rz,  rx,  tx,  tz)
            T *= KinematicsMatrix.TransformationMatrix(dhkinematics.ConventionalDHtable[i][0], dhkinematics.ConventionalDHtable[i][2],
                                                       dhkinematics.ConventionalDHtable[i][3], dhkinematics.ConventionalDHtable[i][1]).transpose;
        }
        Matrix4x4 newT = dhkinematics.root.localToWorldMatrix * T;
        return newT;
    }

    /// <summary>
    /// eliminate configurations that might break the physical robot
    /// </summary>
    public void SetAvoidList()
    {
        underGround = new int[counter];

        for(int i =0; i< counter;i++)
        {
            dhkinematics.SetFullDHtable(i);
            Matrix4x4 CheckT = Matrix4x4.identity;
            for (int j = 0; j < dhkinematics.ConventionalDHtable.Count; j++)
            {
                CheckT *= KinematicsMatrix.TransformationMatrix(dhkinematics.ConventionalDHtable[j][0], dhkinematics.ConventionalDHtable[j][2],
                                                           dhkinematics.ConventionalDHtable[j][3], dhkinematics.ConventionalDHtable[j][1]).transpose;

                //Debug.Log("each frame position"+(dhkinematics.root.localToWorldMatrix ).ExtractPosition().y);
                if ((dhkinematics.root.localToWorldMatrix * CheckT).ExtractPosition().y < -edgeLength/2-0.01f )  
                {
                    //Debug.Log("take me off");
                    underGround[i] = 1;
                    break;
                }
            }
            dhkinematics.ConventionalDHtable.Clear();
        }

    }


    /// <summary>
    /// creating list of joints of active units
    /// </summary>
    /// <param name="x">forward chain is 0, backward chain is 1.</param>
    public void setJointList(int x)
    {
        joints = new List<HingeJointRotation>();
        for (int i = 0; i < transform.childCount; i++)
        {
            joints.Add(transform.GetChild(i).GetComponent<HingeJointRotation>());
        }
    }


    /// <summary>
    /// generate all possible angle configuration
    /// </summary>
    public void SetConfigTable()
    {
        config = new Vector4[(int)Mathf.Pow(3, numberofUnits)];
        for (int i = -120; i < 121; i += 120)
        {
            for (int j = -120; j < 121; j += 120)
            {
                for (int k = -120; k < 121; k += 120)
                {
                    for (int l = -120; l < 121; l += 120)
                    {
                        config[counter] = new Vector4(i, j, k, l); 
                        counter++;
                    }
                }
            }
        }
    }



    static int roundToMultiple(int number, int multiple)
    {
        if (multiple < 0)
            multiple *= -1;
        int prem = number % multiple;     // get remainder
        if (prem < 0)
            prem += multiple;             // turn into positive remainder
        number -= prem;                   // round i toward minus infinity
        if (prem * 2 > multiple)          // round upward 
            return number + multiple;
        if (prem * 2 < multiple)          // round downward
            return number;
        //bitch case - round to even
        if ((number / multiple) % 2 == 0) // is current rounding even?
            return number;
        return number + multiple;
    }

    public Vector3 RoundVec(Vector3 obj)
    {
        Vector3 roundedVec = new Vector3(Mathf.Round(obj.x), Mathf.Round(obj.y), Mathf.Round(obj.z));
        return roundedVec;
    }

    public void PrintLookUpTable(Dictionary<Vector3, Vector4> dictionary)
    {
        Debug.Log("________________________________");
        foreach (KeyValuePair<Vector3, Vector4> dict in dictionary)
        {
            Debug.Log("|" + dict.Key + "|||" + dict.Value.x + "||" + dict.Value.y + "||" + dict.Value.z + "||" + dict.Value.w + "|");
        }
        Debug.Log("________________________________");
    }
}


