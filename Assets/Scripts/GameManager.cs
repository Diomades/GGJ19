using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameLaunch gameLaunch;
    public ScreenManager screenManager;
    public FadeInOut fadeInOut;

    public bool gamePlaying = false;

    //This is the only script that should start anything
    void Start()
    {
        //Start everything
        screenManager.StartScreenManager(this);
        gameLaunch.BeginSequence(this);
    }

    //The StartGame button activates this function
    public void StartGameButton()
    {
        //This launches instantly
        //screenManager.ChangeCurrentScreen(Screen.GameScreen);

        //This launches with a fade
        gameLaunch.StartGame();
        gamePlaying = true;
    }
}
