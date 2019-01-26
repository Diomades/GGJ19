﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    private ConnectionManager _connectionManager;

    public string personName; //If we ever want to add names
    private float _distance; //The distance of this person from the player
    private float _strength; //The strength of the persons relationship to the player, between 0 and 1
    private float _fadeSpeed; //The speed at which strength wanes, between 0 and 1 / 10
    private PersonType _thisPerson; //The type of person this is

    public void InstantiatePerson(Vector3 playerPos, PersonType type, ConnectionManager man)
    {
        _thisPerson = type; //Save our type for later use
        _connectionManager = man; //Store a reference to the People Manager

        //If it's not a player, get a distance, calculate strength and fade speed
        if (type != PersonType.Player)
        {
            _distance = Vector3.Distance(this.transform.position, playerPos);
            _strength = Random.Range(0.2f, 1f); //Do a min of 0.2 to make sure there's time to react
            _fadeSpeed = Random.Range(0f, 1f) / 10; //The rate at which the player relationship fades with this person, decrease this by distance
        }

        //We would randomise a name from here if we get that far
    }

    //On click, pass details to the Connection Manager and let it handle them
    void OnMouseUpAsButton()
    {
        Debug.Log(_thisPerson + " was clicked " + _connectionManager);
        _connectionManager.personClicked(_thisPerson, this);
    }

    public void MakeConnection()
    {
        _strength = 1; //Refresh the strength because a connection has been made
        this.GetComponent<LineRenderer>().enabled = true; //Activate the line
    }
}
