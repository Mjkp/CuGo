using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    private int spacing = 6;
    public int row;
    public int column;
    private int totalCell; // row x column 12x12 = 144
    public Transform cell;
    public Transform subReference; 
    // Start is called before the first frame update
    void Start()
    {
        totalCell = row * column;
        for( int i = 0; i< totalCell; i++)
        {
            //(Object original, Vector3 position, Quaternion rotation, Transform parent);
            //GameObject aboard = Instantiate(cell, new Vector3(transform.position.x + (i%column * spacing), -3.5f, transform.position.z+ (i /row * spacing)), Quaternion.identity, transform);
            //aboard.transform.localScale = new Vector3(1, 1, 1);ß
            Transform aboard = Instantiate(cell,transform);
            aboard.position = new Vector3(transform.position.x + (i % column * spacing), -3.5f, transform.position.z + (i / row * spacing));
            aboard.localScale = new Vector3(0.95f, 0.05f, 0.95f);
            aboard.GetComponent<MeshRenderer>().enabled = true;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
