using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    public GameManager gameManager;
    public PeopleManager peopleManager;
    public ConnectionManager connectionManager;
    public LineDraw lineDraw;

    public Transform worldMapParent;

    public GameObject continentsMain;
    private GameObject _continents1;
    private GameObject _continents2;

    public GameObject clouds1;
    public GameObject clouds2;

    public float continentMoveSpeed;
    public float cloudMoveSpeed;

    private bool _moving = false;

    private bool _curCont1;
    public GameObject curContinent;
    private float _continentWidth;
    private float _continentRespawnPosX;
    public Vector3 mapOffset = new Vector3();

    private GameObject _curClouds;
    private float _cloudsWidth;
    private float _cloudsRespawnPosX;

    public void StartMoving()
    {
        //Set the current continent and clouds to the ones set up in editor
        //_curContinent = continents1;

        //Set up the clouds and store the current cloud position
        _curClouds = clouds1;
        SetUpClouds();

        //Tell PeopleManager to start spawning and feed it the continents reference
        peopleManager.StartGenerate(continentsMain);

        //Set up the continents
        SetUpContinents();

        //Store our initial reference to the current player position
        //connectionManager.currentPlayer = _continents1.transform.Find("Player").gameObject;

        //Move the camera appropriately to the player spawn point
        MoveStartCamera(connectionManager.currentPlayer);

        _moving = true;
    }


    //We only do this if absolutely necessary
    public void UpdateContinents()
    {
        //Whenever anything about the maps change, we delete these copies and instantiate new versions in their place
        Vector3 cont1Pos = _continents1.transform.position;
        Vector3 cont2Pos = _continents2.transform.position;

        //Destroy the old versions, starting with lines first
        //lineDraw.EraseLines(_continents1.GetComponent<ContinentManager>().lines);
        //lineDraw.EraseLines(_continents2.GetComponent<ContinentManager>().lines);
        Destroy(_continents1);
        Destroy(_continents2);

        //Instantiate the new versions and move them into position
        _continents1 = Instantiate(continentsMain, worldMapParent);
        _continents2 = Instantiate(continentsMain, worldMapParent);
        _continents1.transform.position = cont1Pos;
        _continents2.transform.position = cont2Pos;

        //Instantiate the continents and see if we need to swap players
        _continents1.GetComponent<ContinentManager>().InstantiateNewContinent(continentsMain.GetComponent<ContinentManager>().lines);
        _continents2.GetComponent<ContinentManager>().InstantiateNewContinent(continentsMain.GetComponent<ContinentManager>().lines);
        float farPoint = continentsMain.GetComponent<BoxCollider2D>().bounds.center.x + Camera.main.transform.position.x; //This defines how far from the centre of the screen we're allowed to be
        peopleManager.SwapPlayer(_continents1, _continents2, farPoint); //See if we can swap players

        //Set our current continent
        if (_curCont1)
        {
            curContinent = _continents1;
        }
        else
        {
            curContinent = _continents2;
        }

        //Send a reference of the current player to the Connection Manager
        //connectionManager.currentPlayer = curContinent.GetComponent<ContinentManager>().player;

        //Send references of our active players to PeopleManager
        peopleManager.player1 = _continents1.transform.Find("Player").gameObject;
        peopleManager.player2 = _continents2.transform.Find("Player").gameObject;
    }

    public void MoveStartCamera(GameObject player)
    {
        //BoxCollider2D curCollider = continents.transform.GetComponent<BoxCollider2D>();        
        float centerDistance = player.transform.position.x - Camera.main.transform.position.x;

        if (centerDistance < 0)
        {
            centerDistance = centerDistance * -1;

            Vector3 newPos = _continents1.transform.position;
            newPos.x += centerDistance;
            _continents1.transform.position = newPos;

            //Repeat the same steps for the other map
            newPos = _continents2.transform.position;
            newPos.x += centerDistance;
            _continents2.transform.position = newPos;
        }
    }

    void Update()
    {
        if (gameManager.gamePlaying)
        {
            if (_moving)
            {
                _continents1.transform.Translate(continentMoveSpeed * Time.deltaTime, 0, 0);
                _continents2.transform.Translate(continentMoveSpeed * Time.deltaTime, 0, 0);
                clouds1.transform.Translate(cloudMoveSpeed * Time.deltaTime, 0, 0);
                clouds2.transform.Translate(cloudMoveSpeed * Time.deltaTime, 0, 0);

                //Check to see if we need to relocate anything
                if (curContinent.transform.position.x >= _continentRespawnPosX)
                {
                    if (_curCont1)
                    {
                        curContinent.transform.position = SpawnPosition(_continents2.transform.position, _continentWidth);

                        //Change the current continent and update the player that is our main
                        curContinent = _continents2;
                        _curCont1 = false;
                    }
                    else
                    {
                        curContinent.transform.position = SpawnPosition(_continents1.transform.position, _continentWidth);

                        //Change the current continent and player that is our main
                        curContinent = _continents1;
                        _curCont1 = true;

                        //If the player is selected, deactivate the current line
                        if (connectionManager.playerSelected)
                        {
                            connectionManager.currentPlayer.GetComponent<LineRenderer>().enabled = false;
                        }

                        connectionManager.currentPlayer = _continents1.transform.Find("Player").gameObject;
                    }
                }

                if (_curClouds.transform.position.x >= _cloudsRespawnPosX)
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
                        _curClouds.transform.position = SpawnPosition(clouds1.transform.position, _cloudsWidth);
                        _curClouds = clouds1;
                    }
                }

                mapOffset = continentsMain.transform.position - curContinent.transform.position; //Store the offset of the main continents position to our current continents position

                //If the player has been selected
                if (connectionManager.playerSelected)
                {
                    lineDraw.DrawLineToMousePointer(); //Update the mouse pointer line
                }                
            }
        }
    }

    //Update the lines?
    private void LateUpdate()
    {
        //lineDraw.UpdateLines();
    }

    private void SetUpContinents()
    {
        //Store information about the main continent before we do anything
        ContinentManager contMan = continentsMain.GetComponent<ContinentManager>();
        contMan.InstantiateMainContinent(connectionManager, gameManager, lineDraw, continentsMain.transform.Find("Player").gameObject);

        //Store information about the respawn details for the continents
        BoxCollider2D curCollider = continentsMain.transform.GetComponent<BoxCollider2D>(); //Get the box collider
        _continentWidth = curCollider.size.x * continentsMain.transform.localScale.x;

        _continentRespawnPosX = continentsMain.transform.position.x + _continentWidth;

        //We generate copies of the current continent map and move the main version side
        _continents1 = Instantiate(continentsMain, worldMapParent);
        _continents2 = Instantiate(continentsMain, worldMapParent);

        connectionManager.currentPlayer = _continents1.GetComponent<ContinentManager>().player;

        //Change the position of the second set of continents and set the first continent as our main
        _continents2.transform.position = SpawnPosition(continentsMain.transform.position, _continentWidth);
        curContinent = _continents1;
        _curCont1 = true;

        //Move the main continent out of the way
        Vector3 holdPos = continentsMain.transform.position;
        holdPos.y += 50; //Offset out of the way by 50.
        continentsMain.transform.position = holdPos;

        //Send references of our active players to PeopleManager
        peopleManager.player1 = _continents1.transform.Find("Player").gameObject;
        peopleManager.player2 = _continents2.transform.Find("Player").gameObject;
        //Set Player1 as the active player
        peopleManager.player1Active = true;
    }

    private void SetUpClouds()
    {
        BoxCollider2D curCollider = clouds1.transform.GetComponent<BoxCollider2D>(); //Get the box collider
        _cloudsWidth = curCollider.size.x * clouds1.transform.localScale.x;

        clouds2.transform.position = SpawnPosition(clouds1.transform.position, _cloudsWidth);

        _cloudsRespawnPosX = clouds1.transform.position.x + _cloudsWidth;
    }

    //Takes a Vector3 position and Collider width to decide on a spawn position
    private Vector3 SpawnPosition(Vector3 curPos, float colliderWidth)
    {
        Vector3 spawnPosition = curPos;
        spawnPosition.x -= colliderWidth;

        return spawnPosition;
    }
}
