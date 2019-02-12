using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonType { Friend, Lover, Player };

public class PeopleManager : MonoBehaviour
{
    public GameManager gameManager;
    public WorldMover worldMover;
    public ConnectionManager connectionManager;
    public LineDraw lineDraw;
    public GameEvents gameEvents;

    public float updateRate;

    public Sprite playerSprite;
    public Color playerColor;
    public Sprite friendSprite;
    public Color friendColor;
    public Sprite loverSprite;
    public Color loverColor;

    public Color lineColor; //Not used right now
    public Color flashColor; //Not used right now

    public GameObject person;
    public Collider2D worldMapBounds;
    public PolygonCollider2D worldMapCollider;

    private GameObject _worldMapMain;

    public GameObject originalPlayer;
    public GameObject player1;
    public GameObject player2;
    public bool player1Active;
    private int _peopleID = 0;
    public List<GameObject> people = new List<GameObject>();

    public void StartGenerate(GameObject mapRef)
    {
        //Store a refernece to our world map for spawning purposes
        _worldMapMain = mapRef;

        //Spawn the player on the main map
        SpawnPerson(PersonType.Player);

        //Start spawning friends and others
        StartCoroutine(SpawnTimer());
    }

    private void SpawnPerson(PersonType type)
    {
        GameObject thisPerson = Instantiate(person, _worldMapMain.transform);
        PersonScript thisScript = thisPerson.GetComponent<PersonScript>();
        thisPerson.transform.position = SpawnPoint();

        SpriteRenderer thisPersonSprite = thisPerson.GetComponent<SpriteRenderer>();

        //Change the shape and colour of icons
        switch (type)
        {
            case PersonType.Player:
                thisPersonSprite.color = playerColor;
                thisPersonSprite.sprite = playerSprite;
                originalPlayer = thisPerson; //Store the original player
                thisPerson.name = "Player";
                break;
            case PersonType.Friend:
                thisPersonSprite.color = friendColor;
                thisPersonSprite.sprite = friendSprite;
                break;
            case PersonType.Lover:
                thisPersonSprite.color = loverColor;
                thisPersonSprite.sprite = loverSprite;
                break;
        }

        //Instantiate the person!
        thisScript.InstantiatePerson(originalPlayer.transform.position, type, lineColor, flashColor);

        if (type != PersonType.Player)
        {
            //Give this person a name and ID
            _peopleID++;
            thisPerson.name = "Person" + _peopleID;
            //Add the person to the people list
            people.Add(thisPerson);
            //Tell the WorldMover that it needs to refresh the World Maps
            worldMover.UpdateContinents();
        }
    }

    public void UpdatePeople()
    {
        for(int i = 0; i < people.Count; i++)
        {
            PersonScript person = people[i].GetComponent<PersonScript>();
            person.UpdateStrength(updateRate); //Update this persons strength

            if (person.queueKill)
            {
                //Erase the line, remove this person from the people list, and destroy them
                lineDraw.EraseLine(person.lineRef);
                people.RemoveAt(i);
                Destroy(person.transform.gameObject);
                i--; //Go back one so we don't skip the next object
            }
        }
    }

    //Check if the current player has moved far enough off the screen to swap to the other player
    public void SwapPlayer(GameObject cont1, GameObject cont2)
    {
        float player1Dist = Vector3.Distance(player1.transform.position, Camera.main.transform.position);
        float player2Dist = Vector3.Distance(player2.transform.position, Camera.main.transform.position);

        //Check the relevant player and see if their distance from the camera transform is further than the other point
        if (player1Active & player1Dist > player2Dist)
        {
            //Deactivate the old line
            connectionManager.currentPlayer.GetComponent<LineRenderer>().enabled = false;

            //Set the new currentPlayer
            connectionManager.currentPlayer = cont2.transform.Find("Player").gameObject;
            Debug.Log("Swapped to player 2 at " + connectionManager.currentPlayer);
            player1Active = false;
        }
        else if (!player1Active & player2Dist > player1Dist)
        {
            //Deactivate the old line
            connectionManager.currentPlayer.GetComponent<LineRenderer>().enabled = false;

            //Set the new currentPlayer
            connectionManager.currentPlayer = cont1.transform.Find("Player").gameObject;
            Debug.Log("Swapped to player 1 at " + connectionManager.currentPlayer);
            player1Active = true;
        }

        //We just swapped players, so tell LineDraw to do its thing if necessary
        lineDraw.DrawLineToMousePointer();
    }

    private Vector3 SpawnPoint()
    {
        int attempt = 0;
        float spawnX;
        float spawnY;
        float spawnZ = worldMapCollider.transform.position.z - 0.5f;

        do
        {
            spawnX = Random.Range(worldMapBounds.bounds.min.x, worldMapBounds.bounds.max.x);
            spawnY = Random.Range(worldMapBounds.bounds.min.y, worldMapBounds.bounds.max.y);
            attempt++;
        }
        while (!worldMapCollider.OverlapPoint(new Vector2(spawnX, spawnY)) && attempt <= 100);

        /*if(attempt >= 100)
        {
            Debug.Log("Failed to spawn!");
        }
        else
        {
            Debug.Log("Spawned at " + spawnX + " " + spawnY + " after " + attempt + " attempts. Does it overlap? Unity thinks " + worldMapCollider.OverlapPoint(new Vector2(spawnX, spawnY)));
        }*/

        return new Vector3(spawnX, spawnY, spawnZ); //Spawn a character between the Cloud and World layers if successful
    }

    //Every 3-6 seconds, spawn a new person
    //At the start, only spawn fairly close to the Player position, and gradually expand out?
    IEnumerator SpawnTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            float friendOrLover = Random.Range(0f, 1f);
            if (friendOrLover <= 0.96f)
            {
                SpawnPerson(PersonType.Friend);
            }
            else
            {
                SpawnPerson(PersonType.Lover);
            }
        }
    }
}