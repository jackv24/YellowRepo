﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlaceTowers : MonoBehaviour
{
    public GameObject placementUI;

    //Can the player place towers?
    public bool isEnabled = false;

    //The layer on which towers can be placed
    public LayerMask layer;

    //The tower prefab to instantiate
    public GameObject[] towerPrefabs;
    private int towerIndex = -1;

    //The current tower, to follow the mouse cursor over the grid for placement
    private GameObject currentTower;

    void Start()
    {
        placementUI.SetActive(isEnabled);
    }

    void Update()
    {
        if (Input.GetButtonDown("ToggleBuild"))
        {
            isEnabled = !isEnabled;

            if (isEnabled)
                ChangeCurrentTower(towerIndex);
            else
                ChangeCurrentTower(-1);

            placementUI.SetActive(isEnabled);
        }

        //If enabled, and If a tower is selected, and If the mouse isn't over a GUI element...
        if (isEnabled && currentTower && !EventSystem.current.IsPointerOverGameObject())
        {
            //Ray to be cast from the mouse outward
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100f, layer))
            {
                GridNode node = hitInfo.collider.gameObject.GetComponent<GridNode>();

                //If the node is not occupied...
                if (!node.isOccupied)
                {
                    //Set the position of the current tower to the grid tile position
                    currentTower.transform.position = hitInfo.transform.position;

                    //When the player clicks the left mouse button...
                    if (Input.GetMouseButton(0))
                    {
                        //Set the node to occupied
                        node.isOccupied = true;
                        //Place the tower
                        PlaceTower(towerPrefabs[towerIndex], hitInfo.transform.position);

                        //Move the current tower out of view
                        currentTower.transform.position = Vector3.up * 100;
                    }
                }
            }
        }
    }

    //Changes the currently selected tower (usually called from the GUI)
    public void ChangeCurrentTower(int index)
    {
        //If a tower is already selected, destroy it (the placement dummy)
        if (currentTower)
            Destroy(currentTower);

        //If there is a prefab with that index...
        if (index < towerPrefabs.Length && index >= 0)
        {
            //Set the current tower index to that index
            towerIndex = index;

            //Spawn the placement tower
            currentTower = (GameObject)Instantiate(towerPrefabs[towerIndex], Vector3.up * 100, Quaternion.identity);
            //Disable script - this is a placement dummy
            currentTower.GetComponent<Turret>().enabled = false;

            //Finds all the mesh renderers in the model (for appearance changes without affecting the material)
            Component[] comps = currentTower.GetComponentsInChildren<MeshRenderer>();

            //Tint the object green
            foreach (Component comp in comps)
            {
                comp.renderer.material.color = Color.green;
            }
        }
    }

    //Spawns the specified tower at a specified position
    void PlaceTower(GameObject towerPrefab, Vector3 position)
    {
        //Spawns the tower
        GameObject tower = (GameObject)Instantiate(towerPrefab, position, Quaternion.identity);

        //Creates a reference to a gameobject
        GameObject towers;

        //If the gameobject doesn't exist, create it
        if (!GameObject.Find("Towers"))
            towers = new GameObject("Towers");
        //If the gameobject does exist, set it's reference
        else
            towers = GameObject.Find("Towers");

        //Parent the tower to the gameobject (for a clean heirarchy)
        tower.transform.parent = towers.transform;
    }
}