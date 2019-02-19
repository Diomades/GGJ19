using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ContinentManager : MonoBehaviour
{
    public ConnectionManager connectionManager;
    public GameManager gameManager;
    public LineDraw lineDraw;
    public GameObject player;

    public List<List<Vector3>> lines = new List<List<Vector3>>(); //A list of lists of coordinates
    //public List<GameObject> people = new List<GameObject>();

    private float _updateRate;

    public void InstantiateMainContinent(ConnectionManager conMan, GameManager gameMan, LineDraw lineMan, GameObject man, float rate)
    {
        connectionManager = conMan;
        gameManager = gameMan;
        lineDraw = lineMan;
        player = man;
        _updateRate = rate;
    }

    public void InstantiateNewContinent(List<List<Vector3>> mainLines)
    {
        lines = mainLines;

        foreach (List<Vector3> points in lines)
        {
            lineDraw.DrawLine(points, this.transform);
        }
    }

    public void AddLine(List<Vector3> locations)
    {
        lines.Add(locations);
    }

    public void RemoveLine(List<Vector3> line)
    {
        for(int i = 0; i < lines.Count; i++)
        {
            //If the points of this line match our reference line, remove it
            if(lines[i] == line)
            {
                lines.RemoveAt(i);
                return;
            }
        }
        Debug.Log("FAILED TO FIND LINE");
    }

    public void PingContinent()
    {
        Debug.Log("Confirming things are working on " + this.name + ". Connection Manager: " + connectionManager + " Game Manager: " + gameManager + " Player: " + player);
    }

    /*public void UpdatePeople()
    {
        for (int i = 0; i < people.Count; i++)
        {
            PersonScript person = people[i].GetComponent<PersonScript>();
            person.UpdateStrength(_updateRate); //Update this persons strength

            if (person.queueKill)
            {
                //Erase the line, remove this person from the people list, and destroy them
                RemoveLine(person.lineRef);
                lineDraw.EraseLine(person.lineRef);
                people.RemoveAt(i);
                Destroy(person.transform.gameObject);
                i--; //Go back one so we don't skip the next object
            }
        }
    }*/

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
