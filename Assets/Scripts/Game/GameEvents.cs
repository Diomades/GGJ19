using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameEvent {Connection, Love, FriendshipLoss, LoveLoss, Peak}

public class GameEvents : MonoBehaviour
{
    public Text sideBar;

    public AudioClip atmosHigh;
    public AudioClip bassLaser;
    public AudioClip bassSpoopy;
    public AudioClip melodyBells;
    public AudioClip melodyChimes;
    public AudioClip melodyPiano;

    [TextArea(8,15)]
    public string connectionEvent;
    private bool _connectionDone = false;

    [TextArea(8, 15)]
    public string loveEvent;
    private bool _loveDone = false;

    [TextArea(8, 15)]
    public string friendshipLossEvent;
    private bool _friendshipDone = false;

    [TextArea(8, 15)]
    public string loveLossEvent;
    private bool _loveLossDone = false;

    [TextArea(8, 15)]
    public string peakEvent;
    private bool _peakDone = false;
    public int peakReq; //Number of links required to triger the peakEvent

    public void CheckRunEvent(GameEvent thisEvent)
    {
        switch (thisEvent)
        {
            case GameEvent.Connection:
                if (!_connectionDone)
                {
                    Debug.Log("Connection made!");
                    sideBar.text = connectionEvent;
                    _connectionDone = true;
                }
                return;
            case GameEvent.Love:
                if (!_loveDone)
                {
                    sideBar.text = loveEvent;
                    _loveDone = true;
                }
                return;
            case GameEvent.FriendshipLoss:
                if (!_friendshipDone)
                {
                    sideBar.text = friendshipLossEvent;
                    _friendshipDone = true;
                }
                return;
            case GameEvent.LoveLoss:
                if (!_loveLossDone)
                {
                    sideBar.text = loveLossEvent;
                    _loveLossDone = true;
                }
                return;
            case GameEvent.Peak:
                if (!_peakDone)
                {
                    sideBar.text = peakEvent;
                    _peakDone = true;
                }
                return;
        }
    }
}
