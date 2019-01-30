using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentManager : MonoBehaviour
{
    public ConnectionManager connectionManager;
    public GameManager gameManager;
    //public List<GameObject> originalPeople = new List<GameObject>();
    public GameObject player;

    public void InstantiateContinent(ConnectionManager conMan, GameManager gameMan, GameObject man)
    {
        connectionManager = conMan;
        gameManager = gameMan;
        player = man;
    }

    public void PingContinent()
    {
        Debug.Log("Confirming things are working on " + this.name + ". Connection Manager: " + connectionManager + " Game Manager: " + gameManager + " Player: " + player);
    }


    //Update all of the spawned people at a certain rate
    /*private void Update()
    {
        if (gameManager.gamePlaying)
        {
            for (int i = 0; i < _people.Count; i++)
            {
                PersonScript person = _people[i].GetComponent<PersonScript>();

                person.UpdateStrength(updateRate);

                if (person.queueKill)
                {
                    //Check if we had a connection to this person
                    if (_people[i].GetComponent<LineRenderer>().enabled)
                    {
                        //Reduce total links as we just lost one
                        connectionManager.totalLinks--;

                        //Check if there's any event to display appropriate text for
                        if (person.thisPersonType == PersonType.Friend)
                        {
                            gameEvents.CheckRunEvent(GameEvent.FriendshipLoss);
                        }
                        else if (person.thisPersonType == PersonType.Lover)
                        {
                            gameEvents.CheckRunEvent(GameEvent.LoveLoss);
                        }
                    }

                    //Remove the line renderer
                    _people[i].GetComponent<LineRenderer>().enabled = false;

                    //Destroy the person and remove them from the lists
                    Destroy(person);
                    _people.RemoveAt(i);

                    i--; //Go back one to account for the removal
                }
            }
        }
    }*/
}
