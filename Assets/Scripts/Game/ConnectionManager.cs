using System.Collections;
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
                PersonScript thisPerson = clicked.GetComponent<PersonScript>();
                if (!PersonIsSelected(clicked.name))
                {
                    //Add the person to the list
                    linkedPeople.Add(clicked.name);

                    //Make a connection for this person                
                    thisPerson.MakeRefreshConnection();

                    //Increase the number of our links. If they are more than the required to run the Peak event, run it.
                    totalLinks++;
                    if (totalLinks >= gameEvents.peakReq)
                    {
                        gameEvents.CheckRunEvent(GameEvent.Peak);
                    }
                    else if (totalLinks == 1) //If there's only 1 link, see if it's our first
                    {
                        gameEvents.CheckRunEvent(GameEvent.Connection);
                    }

                    if (thisPerson.thisPersonType == PersonType.Lover)
                    {
                        gameEvents.CheckRunEvent(GameEvent.Love);
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
