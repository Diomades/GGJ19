using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class LineDraw : MonoBehaviour
{
    public ConnectionManager connectionManager;
    public PeopleManager peopleManager;

    public List<LineRenderer> renderLines = new List<LineRenderer>();

    public void DrawLine(GameObject target)
    {
        //From the target, we get the name and a Vector3 position
        string tarName = target.name;
        Vector3 tarPos = target.transform.position;

        //Get the CURRENT Player pos and form a Vector3 relative to this position
        Vector3 relPos = connectionManager.currentPlayer.transform.position - tarPos;

        //Apply this relative pos with the original player position
        relPos += peopleManager.originalPlayer.transform.position;
        Vector3 startPos;

        LineRenderer tarLine;

        //Find the original version of this person and get the line renderer attached to them
        foreach (GameObject person in peopleManager.people)
        {
            if(person.name == target.name)
            {
                tarLine = person.GetComponent<LineRenderer>();                
                startPos = person.transform.position;

                //Draw the line appropriately
                tarLine.enabled = true;
                var points = new Vector3[2];
                points[0] = startPos;
                points[1] = relPos;
                tarLine.SetPositions(points);

                //Add the line to our list to be rendered
                renderLines.Add(tarLine);
            }
        }
    }

    //Draw the lines we want to draw
    public void DrawLines(float xOffset)
    {
        if (renderLines.Count != 0)
        {
            foreach (LineRenderer line in renderLines)
            {
                //Store the points
                var points = new Vector3[2];
                points[0] = line.GetPosition(0);
                points[1] = line.GetPosition(1);

                //Offset our points appropriately
                points[0].x += xOffset;
                points[1].x += xOffset;
            }
        }        

        //With all that done, check to see if we need to draw a point to the mouse pointer
        DrawLineToMousePointer();
    }

    private void DrawLineToMousePointer()
    {
        if (connectionManager.playerSelected)
        {
            //Get the mouse location relative to the screen
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);

            //Get the current player object and reactivate its line renderer
            GameObject curPlayer = connectionManager.currentPlayer;
            LineRenderer lineRenderer = curPlayer.GetComponent<LineRenderer>();
            lineRenderer.enabled = true;

            //Draw the line
            var points = new Vector3[2];
            points[0] = curPlayer.transform.position;
            points[1] = mouseWorld;
            lineRenderer.SetPositions(points);
        }
    }
}
