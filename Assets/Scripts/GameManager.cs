using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool skipOpening;

    public GameLaunch gameLaunch;
    public ScreenManager screenManager;
    public GameEvents gameEvents;
    public FadeInOut fadeInOut;

    public bool gamePlaying = false;

    //This is the only script that should start anything
    void Start()
    {
        screenManager.StartScreenManager(this);

        if (skipOpening)
        {
            //Hide all the other menu screens to skip straight to gameplay
            screenManager.ChangeCurrentScreen(GameScreens.GameScreen);

            //Start the game music
            gameEvents.RampAudio(MusicEvent.Base);

            //Start moving everything
            gameLaunch.worldMover.StartMoving();
            gamePlaying = true;
        }
        else
        {
            //Start the game music
            gameEvents.RampAudio(MusicEvent.Base);

            //Start everything
            screenManager.StartScreenManager(this);
            gameLaunch.BeginSequence(this);
        }        
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
