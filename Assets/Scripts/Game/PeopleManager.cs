using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PersonType { Friend, Lover, Player };

public class PeopleManager : MonoBehaviour
{
    public Sprite playerSprite;
    public Color playerColor;
    public Sprite friendSprite;
    public Color friendColor;
    public Sprite loverSprite;
    public Color loverColor;

    public GameObject person;
    public Bounds worldMapBounds;
    public PolygonCollider2D worldMapCollider;

    private GameObject _continents1Ref;
    private GameObject _continents2Ref;

    private GameObject _player1;
    private GameObject _player2;
    private List<GameObject> _people1 = new List<GameObject>();
    private List<GameObject> _people2 = new List<GameObject>();

    //Generate a random point, check it hits on the WorldMapCollider, place a person there

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
                person1.GetComponent<LineRenderer>().enabled = false; //Disable the Line Renderer
                _player1 = person1; //Set Player 1 up for reference later
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

        //Get a distance between the player and the person.


        //Finally, instantiate the person and create a second copy on the other map
        //We don't care about sending the correct playerPos, as this is just used to calculate distance and not stored
        person1.GetComponent<PersonScript>().InstantiatePerson(_player1.transform.position, type);

        GameObject person2 = Instantiate(person1, _continents2Ref.transform);

        if (type == PersonType.Player)
        {
            //Store a reference for the other map for line drawing purposes
            _player2 = person2;
        }
        else
        {
            //Add these to an appropriate list
            _people1.Add(person1);
            _people2.Add(person2);
        }
    }

    //Repeated on update and accessed by another function. Simply delegates the DrawLine function twice as appropriate to both groups
    public void OrderDrawLines()
    {
        DrawLines(_people1,true);
        DrawLines(_people2, false);
    }

    private void DrawLines(List<GameObject> people, bool people1)
    {
        GameObject refPlayer;
        if (people1)
        {
            refPlayer = _player1;
        }
        else
        {
            refPlayer = _player2;
        }

        foreach(GameObject person in people)
        {
            LineRenderer lineRenderer = person.GetComponent<LineRenderer>();
            var points = new Vector3[2];
            points[0] = person.transform.position;
            points[1] = refPlayer.transform.position;
            lineRenderer.SetPositions(points);
        }        
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

        if(attempt == 100)
        {
            Debug.Log("Failed to spawn!");
        }
        else
        {
            Debug.Log("Spawned at " + spawnX + " " + spawnY + " after " + attempt + " attempts.");
        }

        return new Vector3(spawnX, spawnY, 1.5f); //Spawn a character between the Cloud and World layers if successful
    }

    //Every 3-6 seconds, spawn a new person
    //At the start, only spawn fairly close to the Player position, and gradually expand out?
    IEnumerator SpawnTimer()
    {
        while (true)
        {
            //yield return new WaitForSeconds(Random.Range(3f, 6f));
            yield return new WaitForSeconds(1f);

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