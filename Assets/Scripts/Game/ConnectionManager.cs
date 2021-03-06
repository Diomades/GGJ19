﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public PeopleManager peopleManager;
    public WorldMover worldMover;
    public GameEvents gameEvents;
    public LineDraw lineDraw;

    public bool playerSelected = false; //Tracks whether or not a player has been selected
    public GameObject currentPlayer; //Gets updated by PeopleManager and WorldMover with the currently selected player

    public int totalLinks = 0;
    public int totalLovers = 0;

    private List<string> linkedPeople = new List<string>(); //A list of all the names of the people we've linked to

    public void PersonClicked(PersonType person, GameObject clicked)
    {
        if(person == PersonType.Player)
        {
            Debug.Log("Player Selected!");
            playerSelected = true;
        }
        else
        {
            //If the player was selected first, make the connection and deselect the player
            if (playerSelected)
            {
                //Check that this person hasn't been linked to already
                PersonScript thisPerson = worldMover.continentsMain.transform.Find(clicked.name).GetComponent<PersonScript>();
                if (!PersonIsSelected(clicked.name))
                {
                    //Add the person to the list
                    linkedPeople.Add(clicked.name);

                    //Make or refresh the connection for this person by telling the Main version of it
                    thisPerson.MakeRefreshConnection();

                    //Increase the number of our links. If they are more than the required to run the Peak event, run it.
                    totalLinks++;
                    if (totalLinks >= gameEvents.peakReq)
                    {
                        gameEvents.CheckRunEvent(GameEvent.Peak);
                    }
                    else if (totalLinks == 1) //If there's only 1 link, see if it's our first and add to the music
                    {
                        gameEvents.CheckRunEvent(GameEvent.Connection);
                    }

                    //The music queue If statements
                    if (totalLinks <= 4)
                    {
                        gameEvents.RampAudio(MusicEvent.Connection1);
                    }
                    else if (totalLinks <= 9)
                    {
                        gameEvents.RampAudio(MusicEvent.Connection5);
                    }
                    else if (totalLinks >= 10)
                    {
                        gameEvents.RampAudio(MusicEvent.Connection10);
                    }

                    //Specific events for lover connections
                    if (thisPerson.thisPersonType == PersonType.Lover)
                    {
                        totalLovers++;
                        gameEvents.CheckRunEvent(GameEvent.Love);
                        gameEvents.RampAudio(MusicEvent.LoveConnection);
                    }

                    //Disable the line renderer going to the mouse pointer
                    currentPlayer.GetComponent<LineRenderer>().enabled = false; //Disable the line renderer
                    playerSelected = false;

                    //Render the line between the two clicked objects
                    lineDraw.GenerateLine(clicked);
                    //Since we just drew a new line, update the maps!
                    worldMover.UpdateContinents();
                }
                else //This character has already been linked to, and should at least get the benefits
                {
                    Debug.Log("This character is already linked!");

                    //Refresh the connection for this person                
                    thisPerson.MakeRefreshConnection();

                    //Disable the line renderer going to the mouse pointer
                    currentPlayer.GetComponent<LineRenderer>().enabled = false; //Disable the line renderer
                    playerSelected = false;
                }
            }
        }
    }

    //For when we have a lover lost
    public void RemoveLover()
    {
        totalLovers--;
        if(totalLovers == 0)
        {
            gameEvents.CheckRunEvent(GameEvent.LoveLoss);
            gameEvents.RampAudio(MusicEvent.LoveBreak);
        }
    }

    //Loop through a list of all the selected people to check if we've already connected to this one
    public bool PersonIsSelected(string name)
    {
        foreach(string linkedPerson in linkedPeople)
        {
            if(name == linkedPerson)
            {
                return true;
            }
        }

        return false;
    }
}
