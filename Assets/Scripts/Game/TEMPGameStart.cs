using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script can be deleted later and only exists to generate the circumstances of a game start from menu
public class TEMPGameStart : MonoBehaviour
{
    public WorldMover worldMover;

    void Start()
    {
        worldMover.StartMoving();
    }
}
