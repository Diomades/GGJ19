using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    public PeopleManager peopleManager;
    public ConnectionManager connectionManager;

    public GameObject worldMap;

    public GameObject continents;
    public GameObject continents2;

    public GameObject clouds;
    public GameObject clouds2;

    public float continentMoveSpeed;
    public float cloudMoveSpeed;

    private bool _moving = false;

    private GameObject _curContinent;
    private float _continentWidth;
    private float _continentRespawnPosX;

    private GameObject _curClouds;
    private float _cloudsWidth;
    private float _cloudsRespawnPosX;

    public void StartMoving()
    {
        //Set the current continent and clouds to the ones set up in editor
        _curContinent = continents;
        _curClouds = clouds;

        //Instantiate new continents and clouds at the appropriate positions and add them to the list
        InstantiateContinent2();
        InstantiateClouds2();

        //Tell PeopleManager to start spawning
        peopleManager.StartGenerate(continents.GetComponent<BoxCollider2D>().bounds, continents, continents2);

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

            //Check to see if we need to relocate anything
            if (_curContinent.transform.position.x >= _continentRespawnPosX)
            {
                if (_curContinent.name == "WorldMap")
                {
                    Debug.Log("Moving WorldMapBig");
                    _curContinent.transform.position = SpawnPosition(continents2.transform.position, _continentWidth);

                    //Change the current continent and update the player that is our main
                    _curContinent = continents2;
                    connectionManager.currentPlayer = continents2.transform.Find("Player").gameObject;
                }
                else
                {
                    Debug.Log("Moving WorldMapBig(Clone)");
                    _curContinent.transform.position = SpawnPosition(continents.transform.position, _continentWidth);

                    //Change the current continent and player that is our main
                    _curContinent = continents;
                    connectionManager.currentPlayer = continents.transform.Find("Player").gameObject;
                }
            }

            if(_curClouds.transform.position.x >= _cloudsRespawnPosX)
            {
                if (_curClouds.name == "Clouds")
                {
                    Debug.Log("Moving Clouds");
                    _curClouds.transform.position = SpawnPosition(clouds2.transform.position, _cloudsWidth);
                    _curClouds = clouds2;
                }
                else
                {
                    Debug.Log("Moving Clouds(Clone)");
                    _curClouds.transform.position = SpawnPosition(clouds.transform.position, _cloudsWidth);
                    _curClouds = clouds;
                }
            }

            peopleManager.OrderDrawLines(); //Update any drawn lines
        }
    }

    private void InstantiateContinent2()
    {
        BoxCollider2D curCollider = continents.transform.GetComponent<BoxCollider2D>(); //Get the box collider
        _continentWidth = curCollider.size.x * continents.transform.localScale.x;

        //Instantiate new object at the current placement, and then relocate it appropriately
        continents2 = Instantiate(continents, worldMap.transform);
        continents2.transform.position = SpawnPosition(continents.transform.position, _continentWidth);

        //While we are here, let's also set the Continent respawn position
        _continentRespawnPosX = continents.transform.position.x + _continentWidth;
    }

    private void InstantiateClouds2()
    {
        BoxCollider2D curCollider = clouds.transform.GetComponent<BoxCollider2D>(); //Get the box collider
        _cloudsWidth = curCollider.size.x * clouds.transform.localScale.x;

        //Instantiate new object at the current placement, and then relocate it appropriately
        clouds2 = Instantiate(clouds, worldMap.transform);
        clouds2.transform.position = SpawnPosition(clouds.transform.position, _cloudsWidth);

        _cloudsRespawnPosX = clouds.transform.position.x + _cloudsWidth;
    }

    //Takes a Vector3 position and Collider width to decide on a spawn position
    private Vector3 SpawnPosition(Vector3 curPos, float colliderWidth)
    {
        Vector3 spawnPosition = curPos;
        spawnPosition.x -= colliderWidth;

        return spawnPosition;
    }
}
