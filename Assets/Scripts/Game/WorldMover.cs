using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    public GameObject worldMap;
    public GameObject continents;
    public GameObject continents2;
    public GameObject clouds;
    public GameObject clouds2;

    public float continentMoveSpeed;
    public float cloudMoveSpeed;

    private bool _moving = false;
    private GameObject _curContinent;
    private GameObject _curClouds;

    public void StartMoving()
    {
        //Set the current continent and clouds to the ones set up in editor
        _curContinent = continents;
        _curClouds = clouds;

        //Instantiate new continents and clouds at the appropriate positions and add them to the list
        InstantiateContinent2();
        InstantiateClouds2();

        _moving = true;
    }

    private void Update()
    {
        if (_moving)
        {
            continents.transform.Translate(continentMoveSpeed * Time.deltaTime, 0, 0);
            continents2.transform.Translate(continentMoveSpeed * Time.deltaTime, 0, 0);
            clouds.transform.Translate(cloudMoveSpeed * Time.deltaTime, 0, 0);
            clouds2.transform.Translate(cloudMoveSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void InstantiateContinent2()
    {
        BoxCollider2D curCollider = continents.transform.GetComponent<BoxCollider2D>(); //Get the box collider

        //Instantiate new object at the current placement, and then relocate it appropriately
        continents2 = Instantiate(continents, worldMap.transform);
        continents2.transform.position = SpawnPosition(continents.transform.position, (curCollider.size.x * continents.transform.localScale.x));
    }

    private void InstantiateClouds2()
    {
        BoxCollider2D curCollider = clouds.transform.GetComponent<BoxCollider2D>(); //Get the box collider

        //Instantiate new object at the current placement, and then relocate it appropriately
        clouds2 = Instantiate(clouds, worldMap.transform);
        clouds2.transform.position = SpawnPosition(clouds.transform.position, (curCollider.size.x * clouds.transform.localScale.x));
    }

    //Takes a Vector3 position and Collider width to decide on a spawn position
    private Vector3 SpawnPosition(Vector3 curPos, float colliderWidth)
    {
        Vector3 spawnPosition = curPos;
        spawnPosition.x -= colliderWidth;

        return spawnPosition;
    }

    //private void MoveObjectBehindCurrent
}
