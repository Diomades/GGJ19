using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Screen { LogoScreen, MenuScreen, GameScreen, PauseScreen };

//ScreenManager manages all tasks to swapping between specific screens
public class ScreenManager : MonoBehaviour
{
    private GameManager _gameManager;

    private List<GameObject> _screens = new List<GameObject>();
    public GameObject gameScreen;
    public GameObject logoScreen;
    public GameObject menuScreen;

    //PauseScreen is treated separately to the other major screens, as it displays over the GameScreen
    public GameObject pauseScreen;

    //Storing references to the current screen
    private GameObject _curScreenObject;
    private Screen _curScreen;

    public void StartScreenManager(GameManager man)
    {
        //Store a reference to the GameManager
        _gameManager = man;

        _screens.Add(gameScreen);
        _screens.Add(logoScreen);
        _screens.Add(menuScreen);

        _curScreenObject = logoScreen;

        ActivateSingleScreen();
    }

    public void ChangeCurrentScreen(Screen target)
    {
        //Store a reference to our current screen
        _curScreen = target;

        //Store a reference to our current screen game object
        switch (target)
        {
            case Screen.GameScreen:
                _curScreenObject = gameScreen;
                break;
            case Screen.LogoScreen:
                _curScreenObject = logoScreen;
                break;
            case Screen.MenuScreen:
                _curScreenObject = menuScreen;
                break;
            case Screen.PauseScreen:
                _curScreenObject = pauseScreen;
                break;
        }

        //Activate the relevant screen
        ActivateSingleScreen();
    }
    
    private void ActivateSingleScreen()
    {
        //Set the current screen active
        _curScreenObject.SetActive(true);

        //Hide all the screens
        foreach (GameObject screen in _screens)
        {
            //Only disable screens that are not our active one
            if (screen != _curScreenObject)
            {
                screen.SetActive(false);
            }
        }
    }
}
