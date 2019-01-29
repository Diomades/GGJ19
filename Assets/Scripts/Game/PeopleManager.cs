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

    Vector3 lineStartPos;
    private List<GameObject> _people = new List<GameObject>();

    public void StartGenerate(GameObject mapRef)
    {
        //Store a refernece to our world map for spawning purposes
        _worldMapMain = mapRef;

        //Spawn the player on the main map
        SpawnPerson(PersonType.Player);

        //Start spawning friends and others
        StartCoroutine(SpawnTimer());
    }

    //Update all of the spawned people at a certain rate
    private void Update()
    {
        if (gameManager.gamePlaying)
        {
            for (int i = 0; i < _people.Count; i++)
            {
                PersonScript person = _people[i].GetComponent<PersonScript>();

                person.UpdateStrength(updateRate);

                if (person.queueKill)
                {
                    //Check if we had a connection to this person
                    if (_people[i].GetComponent<LineRenderer>().enabled)
                    {
                        //Reduce total links as we just lost one
                        connectionManager.totalLinks--;

                        //Check if there's any event to display appropriate text for
                        if (person.thisPersonType == PersonType.Friend)
                        {
                            gameEvents.CheckRunEvent(GameEvent.FriendshipLoss);
                        }
                        else if (person.thisPersonType == PersonType.Lover)
                        {
                            gameEvents.CheckRunEvent(GameEvent.LoveLoss);
                        }
                    }

                    //Remove the line renderer
                    _people[i].GetComponent<LineRenderer>().enabled = false;

                    //Destroy the person and remove them from the lists
                    Destroy(person);
                    _people.RemoveAt(i);

                    i--; //Go back one to account for the removal
                }
            }
        }
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
                lineStartPos = thisPerson.transform.position; //Store the position of Person 1 for distance measuring
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

        //thisScript.InstantiatePerson()

        if (type == PersonType.Player)
        {
            thisPerson.name = "Player";

            lineDraw.StoreReferences(thisPerson);
        }
        else
        {
            //Add the person to the people list
            _people.Add(thisPerson);
            //Tell the WorldMover that it needs to refresh the World Maps
            worldMover.UpdateContinents();
        }
    }

    //Repeated on update and accessed by another function. Simply delegates the DrawLine function twice as appropriate to both groups
    public void OrderDrawLines()
    {
        lineDraw.DrawLines(_people);
    }

    private Vector3 SpawnPoint()
    {
        int attempt = 0;
        float spawnX;
        float spawnY;

        do
        {
            spawnX = Random.Range(worldMapBounds.bounds.min.x, worldMapBounds.bounds.max.x);
            spawnY = Random.Range(worldMapBounds.bounds.min.y, worldMapBounds.bounds.max.y);
            attempt++;
        }
        while (!worldMapCollider.OverlapPoint(new Vector2(spawnX, spawnY)) && attempt <= 100);

        if(attempt >= 100)
        {
            Debug.Log("Failed to spawn!");
        }
        else
        {
            Debug.Log("Spawned at " + spawnX + " " + spawnY + " after " + attempt + " attempts. Does it overlap? Unity thinks " + worldMapCollider.OverlapPoint(new Vector2(spawnX, spawnY)));
        }

        return new Vector3(spawnX, spawnY, 1.5f); //Spawn a character between the Cloud and World layers if successful
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