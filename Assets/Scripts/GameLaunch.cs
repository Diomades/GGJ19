using System.Collections;
using UnityEngine;

//GameLaunch manages the sequence of events as the game first boots
public class GameLaunch : MonoBehaviour
{
    public WorldMover worldMover;
    private GameManager _gameManager;

    public GameObject splashScreen;
    public GameObject blackScreen;
    public float launchWaitTime;
    public float logoWaitTime;

    //_fadeTime is a reference to the FadeInOut script
    private float _fadeTime;

    public void BeginSequence(GameManager man)
    {
        //Store a reference to the Game Manager
        _gameManager = man;
        _fadeTime = _gameManager.fadeInOut.fadeTime;

        //Make sure the BlackScreen is active
        blackScreen.SetActive(true);

        //Disable all screens not relevant during start up
        _gameManager.screenManager.ChangeCurrentScreen(GameScreens.LogoScreen);

        //Start the sequence
        StartCoroutine(OpeningSequence());
    }

    public void StartGame()
    {
        StartCoroutine(StartGameSequence());
    }

    IEnumerator OpeningSequence()
    {
        //Wait for a predefined time
        yield return new WaitForSeconds(launchWaitTime);
        //Fade the black screen out to the team logo
        splashScreen.GetComponentInChildren<Animation>().Play();
        //logoShadow.GetComponent<Animation>().Play();
        _gameManager.fadeInOut.FadeScreen(true);
        yield return new WaitForSeconds(logoWaitTime);

        //Fade back out again and hide the logo screen and wait for the appropriate time
        _gameManager.fadeInOut.FadeScreen(false);
        yield return new WaitForSeconds(_fadeTime);
        
        //Display the Menu Screen and start fading out to the menu
        _gameManager.screenManager.ChangeCurrentScreen(GameScreens.MenuScreen);
        _gameManager.fadeInOut.FadeScreen(true);
    }

    //This is for a fading game start, NOT CURRENTLY USED
    IEnumerator StartGameSequence()
    {
        //Fade from the menu and wait
        _gameManager.fadeInOut.FadeScreen(false);
        yield return new WaitForSeconds(_fadeTime);
        //Activate the game and fade to it
        _gameManager.screenManager.ChangeCurrentScreen(GameScreens.GameScreen);
        worldMover.StartMoving();
        _gameManager.fadeInOut.FadeScreen(true);
    }
}
