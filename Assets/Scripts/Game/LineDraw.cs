using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class LineDraw : MonoBehaviour
{
    public ConnectionManager connectionManager;
    public PeopleManager peopleManager;
    public WorldMover worldMover;

    private int id = 0;

    public void GenerateLine(GameObject target)
    {
        //From the target, we get the name and a Vector3 position
        string tarName = target.name;
        PersonScript person = worldMover.continentsMain.transform.Find(tarName).GetComponent<PersonScript>();

        //Get the local version of the positions
        Vector3 localTar = worldMover.curContinent.transform.InverseTransformPoint(target.transform.position);
        Vector3 localPlayer = worldMover.curContinent.transform.InverseTransformPoint(connectionManager.currentPlayer.transform.position);

        //Add local version of the points to a list and a reference on the relevant person
        List<Vector3> localPoints = new List<Vector3>() { localTar, localPlayer };
        worldMover.continentsMain.GetComponent<ContinentManager>().AddLine(localPoints);
        person.AddLine(localPoints);
    }

    public void DrawLine(List<Vector3> points, Transform parent)
    {
        //First, offset by our relative position and add to a new list
        List<Vector3> pointsOffset = new List<Vector3>() { parent.TransformPoint(points[0]), parent.TransformPoint(points[1]) };

        //Name the line with an ID number so we can find it later
        id++;
        VectorLine newLine = new VectorLine("Line" + id, pointsOffset, 2.6f);

        newLine.color = peopleManager.connectionColor;
        newLine.Draw3D();
        newLine.drawTransform = parent;
        newLine.rectTransform.SetParent(parent);
    }

    public void DrawVectorLine(VectorLine line)
    {
        line.Draw3D();      
    }

    public void EraseLine(VectorLine line)
    {
        VectorLine.Destroy(ref line);
    }

    //Erase all lines whenever we're deleting a continent
    public void EraseLines(List<VectorLine> lines)
    {
        VectorLine.Destroy(lines);
    }

    //We draw the line to the mouse pointer using LineRenderer (for now?)
    public void DrawLineToMousePointer()
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
