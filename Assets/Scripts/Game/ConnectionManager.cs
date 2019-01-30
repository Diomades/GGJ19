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
                //Make a connection for this person
                PersonScript thisPerson = clicked.GetComponent<PersonScript>();
                thisPerson.MakeConnection();

                //Increase the number of our links. If they are more than the required to run the Peak event, run it.
                totalLinks++;
                if (totalLinks >= gameEvents.peakReq)
                {
                    gameEvents.CheckRunEvent(GameEvent.Peak);
                }
                else if(totalLinks == 1) //If there's only 1 link, see if it's our first
                {
                    gameEvents.CheckRunEvent(GameEvent.Connection);
                }

                if(thisPerson.thisPersonType == PersonType.Lover)
                {
                    gameEvents.CheckRunEvent(GameEvent.Love);
                }                

                //Disable the line renderer going to the mouse pointer
                currentPlayer.GetComponent<LineRenderer>().enabled = false; //Disable the line renderer
                playerSelected = false;

                //Render the line between the two clicked objects
                lineDraw.DrawLine(clicked);
            }
        }
    }


}
