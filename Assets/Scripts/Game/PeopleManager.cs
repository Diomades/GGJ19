using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonType { Friend, Lover, Player };

public class PeopleManager : MonoBehaviour
{
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
    public Bounds worldMapBounds;
    public PolygonCollider2D worldMapCollider;

    private GameObject _continents1Ref;
    private GameObject _continents2Ref;

    Vector3 lineStartPos;
    private List<GameObject> _people1 = new List<GameObject>();
    private List<GameObject> _people2 = new List<GameObject>();

    public void StartGenerate(Bounds bounds, GameObject map1, GameObject map2)
    {
        //Store references
        worldMapBounds = bounds;
        _continents1Ref = map1;
        _continents2Ref = map2;

        //Spawn the player twice - once for each map
        CreateTwo(PersonType.Player);

        //Start spawning friends and others
        StartCoroutine(SpawnTimer());
    }

    //Update all of the spawned people at a certain rate
    private void Update()
    {
        for(int i = 0; i < _people1.Count; i++)
        {
            PersonScript _person1 = _people1[i].GetComponent<PersonScript>();
            PersonScript _person2 = _people2[i].GetComponent<PersonScript>();

            _person1.UpdateStrength(updateRate);
            _person2.UpdateStrength(updateRate);

            if (_person1.queueKill)
            {
                //Check if we had a connection to this person
                if (_people1[i].GetComponent<LineRenderer>().enabled)
                {
                    //Reduce total links as we just lost one
                    connectionManager.totalLinks--;

                    //Check if there's any event to display appropriate text for
                    if(_person1.thisPersonType == PersonType.Friend)
                    {
                        gameEvents.CheckRunEvent(GameEvent.FriendshipLoss);
                    }
                    else if(_person1.thisPersonType == PersonType.Lover)
                    {
                        gameEvents.CheckRunEvent(GameEvent.LoveLoss);
                    }
                }

                //Remove the line renderer
                _people1[i].GetComponent<LineRenderer>().enabled = false;
                _people2[i].GetComponent<LineRenderer>().enabled = false;

                //Destroy the person and remove them from the lists
                Destroy(_person1);
                Destroy(_person2);
                _people1.RemoveAt(i);
                _people2.RemoveAt(i);

                i--; //Go back one to account for the removal
            }
        }
    }

    //Because we always have 2 maps, we always need to generate 2 versions of each character and asset that moves with the map.
    private void CreateTwo(PersonType type)
    {
        //Do one object and then instantiate the same object on the other map
        GameObject person1 = Instantiate(person, _continents1Ref.transform);
        SpriteRenderer person1Sprite = person1.GetComponent<SpriteRenderer>();

        person1.transform.position = SpawnPoint();

        //Colour and change shape of people
        switch (type)
        {
            case PersonType.Player:
                person1Sprite.color = playerColor;
                person1Sprite.sprite = playerSprite;
                lineStartPos = person1.transform.position; //Store the position of Person 1 for distance measuring
                break;
            case PersonType.Friend:
                person1Sprite.color = friendColor;
                person1Sprite.sprite = friendSprite;
                break;
            case PersonType.Lover:
                person1Sprite.color = loverColor;
                person1Sprite.sprite = loverSprite;
                break;
        }

        GameObject person2 = Instantiate(person1, _continents2Ref.transform);

        //Finally, instantiate the person after create a second copy on the other map
        //We don't care about sending the correct playerPos, as this is just used to calculate distance and not stored
        person1.GetComponent<PersonScript>().InstantiatePerson(lineStartPos, type, connectionManager, lineColor, flashColor);
        person2.GetComponent<PersonScript>().InstantiatePerson(lineStartPos, type, connectionManager, lineColor, flashColor);

        if (type == PersonType.Player)
        {
            person1.name = "Player";
            person2.name = "Player";

            lineDraw.StoreReferences(person1, person2);
        }
        else
        {
            //Add these to an appropriate list
            _people1.Add(person1);
            _people2.Add(person2);

            //Store references in both the people to their sister version
            person1.GetComponent<PersonScript>().StoreSisterPerson(person2.GetComponent<PersonScript>());
            person2.GetComponent<PersonScript>().StoreSisterPerson(person1.GetComponent<PersonScript>());
        }
    }

    //Repeated on update and accessed by another function. Simply delegates the DrawLine function twice as appropriate to both groups
    public void OrderDrawLines()
    {
        lineDraw.DrawLines(_people1, true);
        lineDraw.DrawLines(_people2, false);
    }

    private Vector3 SpawnPoint()
    {
        int attempt = 0;
        float spawnX;
        float spawnY;

        do
        {
            spawnX = Random.Range(worldMapBounds.min.x, worldMapBounds.max.x);
            spawnY = Random.Range(worldMapBounds.min.y, worldMapBounds.max.y);
            attempt++;
        }
        while (!worldMapCollider.OverlapPoint(new Vector2(spawnX, spawnY)) && attempt <= 100);

        /*if(attempt == 100)
        {
            Debug.Log("Failed to spawn!");
        }
        else
        {
            Debug.Log("Spawned at " + spawnX + " " + spawnY + " after " + attempt + " attempts.");
        }*/

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
            if (friendOrLover <= 0.9f)
            {
                CreateTwo(PersonType.Friend);
            }
            else
            {
                CreateTwo(PersonType.Lover);
            }
        }
    }
}