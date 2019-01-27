using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    private ConnectionManager _connectionManager;

    public string personName; //If we ever want to add names
    private float _distance; //The distance of this person from the player
    private float _strength; //The strength of the persons relationship to the player, between 0 and 1
    private float _fadeSpeed; //The speed at which strength wanes, between 0 and 1 / UpdateSpeed
    private float _friendship; //A modifier to FadeSpeed that says we're friends
    private PersonType _thisPerson; //The type of person this is

    private PersonScript _sister; //The other version of this person elsewhere

    public bool queueKill = false; //Queue this person to be destroyed

    /*public bool flashLine = false; //Whether or not the line needs to flash because we're in danger of no longer being friends
    private bool _flashOn = false; //Whether the flash is currently on
    private Color _lineColor; //The default line color
    private Color _flashColor; //The flashing line color*/

    public void InstantiatePerson(Vector3 playerPos, PersonType type, ConnectionManager man, Color lineColor, Color flashColor)
    {
        _thisPerson = type; //Save our type for later use
        _connectionManager = man; //Store a reference to the People Manager

        //If it's not a player, get a distance, calculate strength and fade speed
        if (type != PersonType.Player)
        {
            _distance = Vector3.Distance(this.transform.position, playerPos);
            _strength = Random.Range(0.6f, 1f); //Do a min of 0.2 to make sure there's time to react
            //_strength = 1;
            _fadeSpeed = Random.Range(0f, 1f); //The rate at which the player relationship fades with this person, decrease this by distance?
            //_fadeSpeed = 1;
        }

        //_lineColor = lineColor; //Not used right now
        //_flashColor = flashColor; //Not used right now
    }

    //Is called by PeopleManager and gradually tweaks this relationship until it finally fades
    public void UpdateStrength(float _updateSpeed)
    {
        //Friended fade speed has a modifier because we're friends
        float friendedFadeSpeed = _fadeSpeed - _friendship;
        if (friendedFadeSpeed < 0)
        {
            friendedFadeSpeed = 0;
        }

        _strength = _strength - (friendedFadeSpeed / _updateSpeed);

        //We've totally faded out
        if (_strength <= 0)
        {
            queueKill = true;
        }
        else
        {
            //Set the alpha of our color to be tied to our relationship strength
            Color newColor = this.gameObject.GetComponent<SpriteRenderer>().color;
            newColor.a = _strength;

            //Update the color
            this.gameObject.GetComponent<SpriteRenderer>().color = newColor;
        }        
        
        //None of this works, don't bother. Line renderer is not a great solution for this.
        /*//If our strength is below a certain level, flash the line
        if (_strength <= 0.4f && !flashLine)
        {
            Debug.Log("Turning on flash!");
            this.GetComponent<LineRenderer>().colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(_flashColor, 0f), new GradientColorKey(_flashColor, 1f) };
            //flashLine = true;
            //StartCoroutine(Flash());
        }
        else if (_strength > 0.4f)
        {
            Debug.Log("Turning off flash");
            flashLine = false;
        }*/
    }

    //Store a reference to this person's sister object
    public void StoreSisterPerson(PersonScript sister)
    {
        _sister = sister;
    }

    //On click, pass details to the Connection Manager and let it handle them
    void OnMouseUpAsButton()
    {
        //Debug.Log(_thisPerson + " was clicked " + _connectionManager);
        _connectionManager.personClicked(_thisPerson, this);
    }

    public void MakeConnection()
    {
        _strength = 1; //Refresh the strength because a connection has been made

        _friendship = 0.35f; //We are friends now, so our fade speed gets a benefit

        this.GetComponent<LineRenderer>().enabled = true; //Activate the line
    }

    //Tell the sister to update separately
    public void ConnectSister()
    {
        _sister.MakeConnection();
    }

    /*IEnumerator Flash()
    {
        //We need to create a GradientColorKey for the line
        //GradientColorKey curColor = new GradientColorKey[];
        Color curColor;

        while (flashLine) //If we wanted this to not flash when paused we could do a "While Playing" bool
        {
            _flashOn = !_flashOn;
            if (_flashOn)
            {
                //Store the flash color on the gradient and apply it to the variable
                curColor = _flashColor;
                Debug.Log("Flash On!");
                //gradient[0] = curColor;
                
            }
            else
            {
                //Store the line color on the gradient and apply it to the variable
                curColor = _lineColor;
                Debug.Log("Flash Off!");
                //gradient[0] = curColor;
            }

            //Apply the color
            //this.GetComponent<LineRenderer>().colorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(curColor,0f) };
            this.GetComponent<LineRenderer>().material.color = curColor;

            //Wait and do it again
            yield return new WaitForSeconds(1f);
        }

        //We've broken out of the while loop and can go back to the standard color
        /*curColor.color = _lineColor;
        gradient[0] = curColor;
        this.GetComponent<LineRenderer>().colorGradient.colorKeys = gradient;*/
    //}
}
