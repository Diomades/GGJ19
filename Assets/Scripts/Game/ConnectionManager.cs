﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public PeopleManager peopleManager;
    public WorldMover worldMover;

    private bool _playerSelected = false; //Tracks whether or not a player has been selected
    public GameObject currentPlayer; //Gets updated by PeopleManager and WorldMover with the currently selected player WE MAY NOT NEED THIS UNLESS WE DO THE MOUSE POINTER LINE

    public void personClicked(PersonType person, PersonScript clicked)
    {
        if(person == PersonType.Player)
        {
            _playerSelected = true;
        }
        else
        {
            //If the player was selected first, make the connection and deselect the player
            if (_playerSelected)
            {
                clicked.MakeConnection();
                _playerSelected = false;
            }
        }
    }
}
