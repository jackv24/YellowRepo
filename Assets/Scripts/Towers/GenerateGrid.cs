﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GenerateGrid : MonoBehaviour
{
    //The node prefab to be instantiated in a grid
    public GameObject nodePrefab;
    //Where the grid starts
    public Vector3 origin;
    //The size of the grid (in node amounts)
    public Vector2 size;
    //The layer for rays to be cast to determine node height
    public LayerMask layer;

    //Should a grid be generated?
    public bool generate = false;

    void Start()
    {
        generate = false;
    }

    void Update()
    {
        //If a grid is to be generated...
        if (generate)
        {
            Debug.Log("Generating Grid");
            generate = false;

            //Get the size of the node's box collider
            float sizeX = nodePrefab.transform.localScale.x;
            float sizeY = nodePrefab.transform.localScale.z;

            if (GameObject.Find("Nodes"))
                DestroyImmediate(GameObject.Find("Nodes"));

            GameObject nodes = new GameObject("Nodes");

            //Create the grid. For every row...
            for (int i = 0; i < size.x; i++)
            {
                //Generate a column
                for (int j = 0; j < size.y; j++)
                {
                    //Create the vector at which the node should be placed
                    Vector3 placeVector = new Vector3(i * sizeX + origin.x, 500, j * sizeY + origin.z);

                    RaycastHit hitInfo;
                    //Cast a ray down onto layer
                    if (Physics.Raycast(placeVector, -Vector3.up, out hitInfo, 1000, layer))
                    {
                        //Placement height is where ray hit
                        placeVector.y = hitInfo.point.y;

                        //Instantiate node
                        GameObject obj = (GameObject)Instantiate(nodePrefab, placeVector, Quaternion.identity);
                        //Set node as child of grid
                        obj.transform.parent = nodes.transform;
                    }
                }
            }
        }
    }
}
