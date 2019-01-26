using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public PeopleManager peopleManager;
    public WorldMover worldMover;

    public bool playerSelected = false; //Tracks whether or not a player has been selected
    public GameObject currentPlayer; //Gets updated by PeopleManager and WorldMover with the currently selected player WE MAY NOT NEED THIS UNLESS WE DO THE MOUSE POINTER LINE

    public void personClicked(PersonType person, PersonScript clicked)
    {
        if(person == PersonType.Player)
        {
            playerSelected = true;
        }
        else
        {
            //If the player was selected first, make the connection and deselect the player
            if (playerSelected)
            {
                clicked.MakeConnection();
                currentPlayer.GetComponent<LineRenderer>().enabled = false; //Disable the line renderer
                playerSelected = false;
            }
        }
    }
}
