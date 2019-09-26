using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DHParameters : MonoBehaviour
{
    /*------------------------------------------------------------------------------------------------------
     * setting up DH conventional paramters for forward kinematics
     * 
     * thetha = z axis angle
     * alpha = x axis angle
     * d = z axis offset
     * a = x axis offset 
     *  ----- DH parameter as Modules ------- redundant frames are added for sake of modularity -----------
     *  
     * [thetha, d, alpha, a]  l = edgeLength
     * case 0 (unit rotation = 0)
     * F(n)    = [  thetha +30   ,  0.58 * l                , 0          ,  0.82 * l               ]
     * F(n+1)  = [  -30          ,  0                       , 0           , (squareRoot(6)/4) * l  ]
     * 
     * case 1 ( unit rotation = 90)    
     * F(n)    = [  thetha       ,  0                       , 0           , (squareRoot(2)/2) * l  ]
     * F(n+1)  = [  60           ,  0                       , -19.47      , (squareRoot(2)/4) * l  ]
     * F(n+2)  = [  0            ,  (squareRoot(6)/4) * l   , 90          ,  0                     ]
     * 
     * case 2 ( unit rotation = 180)    
     * F(n)    = [  thetha       ,  0                       , 0           , (squareRoot(2)/2) * l  ]
     * F(n+1)  = [  120          ,  0                       , 19.47       , (squareRoot(2)/4) * l  ]
     * F(n+2)  = [  0            , (squareRoot(6)/4) * l    , 90          ,  0                     ]
     * F(n+3)  = [  60           ,  0                       , 0           ,  0                     ]
     *
     * case 3 ( unit rotation = -90)    
     * F(n)    = [  60 + thetha  ,  0                       , 0           , (squareRoot(2)/2) * l  ]
     * F(n+1)  = [  -60          ,  0                       , 19.47       , (squareRoot(2)/4) * l  ]
     * F(n+2)  = [  0            , (squareRoot(6)/4) * l    , -90         ,  0                     ]
     * F(n+3)  = [  -60          ,  0                       , 0           ,  0                     ]
     * 
     * case 4 
     * last Joint(normally for toolpath) (use case 0)
     *---------------------------------------------------------------------------------------------------*/
    private List<int> RotType;
    public Transform root;
    public List<float[]> ConventionalDHtable;
    private float[] YvalsforUnits;
    private float[] rotVal;
    [HideInInspector] public float edgeLength;
    [HideInInspector] public List<int[]> rotAngle;
    [HideInInspector] public List<Transform> joints;
    [HideInInspector] public Vector3[] endEffectorPos;

    public void Awake()
    {
        edgeLength = 6f;
        setJoinTransformList();
        SetRotType();
        RootVec();
        //SetAngles();
        //PrintDHTable();
    }



    public void SetRotType()
    {
        // look for the rotation difference from consecuting units. and see whether it is rotated 0,90,180,270 degrees
        RotType = new List<int>();
        YvalsforUnits = new float[joints.Count];
        rotVal = new float[YvalsforUnits.Length - 1];

        for (int i = 0; i < YvalsforUnits.Length; i++)
        {
            // set up all the global y rotation values of units
            YvalsforUnits[i] = joints[i].eulerAngles.y;
        }

        for (int i = 1; i <= rotVal.Length; i++)
        {
            // subtract n from n +1 and add that number to the rot Value list
            rotVal[i - 1] = YvalsforUnits[i] - YvalsforUnits[i - 1];
        }
        for (int i = 0; i < rotVal.Length; i++)
        {
            if (((int)rotVal[i] == 90) || ((int)rotVal[i] == -270))
            {
                RotType.Add(1);
            }
            else if ((int)Mathf.Abs(rotVal[i]) == 180)
            {
                RotType.Add(2);
            }
            else if (((int)rotVal[i] == 270) || ((int)rotVal[i] == -90))
            {
                RotType.Add(3);
            }
            else if ((int)rotVal[i] == 0)
            {
                RotType.Add(0);
            }
        }
        // last one for the endEffector pos;
        RotType.Add(0);
    }

    public void SetFullDHtable(int counter)
    {
        ConventionalDHtable = new List<float[]>();
        for (int i = 0; i < RotType.Count; i++)
        {
            dhRowParameters(RotType[i], rotAngle[counter][i], edgeLength, ConventionalDHtable);
        }
    }

 

    public void PrintDHTable()
    {
        foreach (float[] dh in ConventionalDHtable)
        {
            Debug.Log("|"+"thetha: "+dh[0] + "||"+ "d: " + dh[1] + "||" + "alpha: " + dh[2] + "||" + "a: " + dh[3]+"|");
        }
    }
    public void dhRowParameters(int type, int thetha, float edgeL, List<float[]> dhparameter)
    {
        // according to rotation type assign different Frames
        switch (type)
        {
            case 0:
                float[] F00 = { thetha +30f , 0.58f * edgeL, 0f, 0.82f * edgeL }; 
                float[] F01 = { -30f, 0f, 0f, 0 };

                dhparameter.Add(F00);
                dhparameter.Add(F01);
                break;

            case 1:
                float[] F10 = { thetha, 0f, 0f, (Mathf.Sqrt(2) / 2) * edgeL };     
                float[] F11 = { 60f, 0f, -19.471f, (Mathf.Sqrt(2) / 4) * edgeL };  
                float[] F12 = { 0f, (Mathf.Sqrt(6) / 4) * edgeL, 90f, 0f };

                dhparameter.Add(F10);
                dhparameter.Add(F11);
                dhparameter.Add(F12);
                break;

            case 2:
                float[] F20 = { thetha, 0f, 0f, (Mathf.Sqrt(2) / 2) * edgeL };
                float[] F21 = { 120f, 0f, 19.471f, (Mathf.Sqrt(2) / 4) * edgeL };
                float[] F22 = { 0f, (Mathf.Sqrt(6) / 4) * edgeL, 90f, 0f };
                float[] F23 = { 60f, 0f, 0f, 0f };

                dhparameter.Add(F20);
                dhparameter.Add(F21);
                dhparameter.Add(F22);
                dhparameter.Add(F23);
                break;

            case 3:
                float[] F30 = { 60f + thetha, 0f, 0f, (Mathf.Sqrt(2) / 2) * edgeL };
                float[] F31 = { -60f, 0f, 19.471f, (Mathf.Sqrt(2) / 4) * edgeL };
                float[] F32 = { 0f, (Mathf.Sqrt(6) / 4) * edgeL, -90f, 0f };
                float[] F33 = { -60f, 0f, 0f, 0f };

                dhparameter.Add(F30);
                dhparameter.Add(F31);
                dhparameter.Add(F32);
                dhparameter.Add(F33);
                break;
        }
    }

    public void RootVec()
    {
        root.forward = -transform.GetChild(0).GetChild(0).forward;
        root.Rotate(root.forward, 60,Space.World); //NOTES// 60 degrees rotation, calculated from the rhino model
    }

    public void setJoinTransformList()
    {
        joints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            joints.Add(transform.GetChild(i));
        }
    }

}


//public void SetAngles()
//{
//    rotAngle = new List<int[]>();
//    endEffectorPos = new Vector3[(int)Mathf.Pow(3, angleConfig.numberofUnits)];
//    for (int i = 0; i < (int)Mathf.Pow(3, angleConfig.numberofUnits); i++)
//    {
//        int[] anglePatterns = { (int)angleConfig.config[i].x, (int)angleConfig.config[i].y, (int)angleConfig.config[i].z, (int)angleConfig.config[i].w };
//        rotAngle.Add(anglePatterns);
//        SetFullDHtable(i);
//        endEffectorPos[i] = DHForwardKinematics().ExtractPosition();
//        ConventionalDHtable.Clear();
//    }
//}
