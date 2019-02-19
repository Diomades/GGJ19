using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class PersonScript : MonoBehaviour
{
    private GameManager _gameManager;
    private float _updateRate;
    private Color _baseColor;
    private Color _fadeColor = Color.grey;

    public string personName; //To track whether or not we've connected to this person
    private float _distance; //The distance of this person from the player
    private float _strength; //The strength of the persons relationship to the player, between 0 and 1
    private float _fadeSpeed; //The speed at which strength wanes, between 0 and 1 / UpdateSpeed
    private float _friendship; //A modifier to FadeSpeed that says we're friends
    public List<Vector3> lineRef = new List<Vector3>(); //The reference to this line
    public PersonType thisPersonType; //The type of person this is

    public bool queueKill = false; //Queue this person to be destroyed

    public void InstantiatePerson(Vector3 playerPos, PersonType type, Color lineColor, Color flashColor, float rate)
    {
        thisPersonType = type; //Save our type for later use
        _updateRate = rate; //Store the update rate

        //If it's not a player, get a distance, calculate strength and fade speed
        if (type != PersonType.Player)
        {
            _baseColor = this.GetComponent<SpriteRenderer>().color; //Store the original color
            _distance = Vector3.Distance(this.transform.position, playerPos);
            _strength = Random.Range(0.6f, 1f); //Do a min of 0.2 to make sure there's time to react
            _fadeSpeed = Random.Range(0f, 1f); //The rate at which the player relationship fades with this person, decrease this by distance?
        }
    }

    private void Update()
    {
        //Only perform updates if GameManager says we're playing and we're not the player
        if (thisPersonType != PersonType.Player)
        {
            //Store a reference to the GameManager if we don't have one yet
            if (_gameManager == null)
            {
                _gameManager = GameObject.Find("MenuScripts").GetComponent<GameManager>();
            }
            else
            {
                if (_gameManager.gamePlaying)
                {
                    UpdateStrength();
                }                
            }
        }        
    }

    //Gradually tweaks our relationship over time
    public void UpdateStrength()
    {
        //Friended fade speed has a modifier because we're friends
        float friendedFadeSpeed = _fadeSpeed - _friendship;
        if (friendedFadeSpeed < 0)
        {
            friendedFadeSpeed = 0;
        }

        _strength = _strength - (friendedFadeSpeed / _updateRate);

        //We've totally faded out
        if (_strength <= 0)
        {
            queueKill = true; //Queue us to be killed            
        }
        else
        {
            //Set the alpha of our color to be tied to our relationship strength
            //Color newColor = this.gameObject.GetComponent<SpriteRenderer>().color;
            //newColor.a = _strength;

            Color newColor = Color.Lerp(_fadeColor, _baseColor, _strength);

            //Update the color
            this.gameObject.GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    //On click, pass details to the Connection Manager and let it handle them
    void OnMouseDown()
    {
        //Store a reference to the connection manager manually
        ConnectionManager connectionManager = GameObject.Find("GameplayScripts").GetComponent<ConnectionManager>();
        Debug.Log(thisPersonType + " was clicked, sending " + this + " to " + connectionManager);
        connectionManager.PersonClicked(thisPersonType, this.gameObject);
    }

    public void MakeRefreshConnection()
    {
        _strength = 1; //Refresh the strength because a connection has been made
        _friendship = 0.35f; //We are friends now, so our fade speed gets a benefit
    }

    //Store a reference to the line to this object
    public void AddLine(List<Vector3> line)
    {
        lineRef = line;
    }
}
