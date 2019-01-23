using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//FadeInOut manages a screen fade effect
public class FadeInOut : MonoBehaviour
{
    public GameObject blackScreen;
    public float fadeTime;

    public void FadeScreen(bool fadeAway)
    {
        StartCoroutine(Fade(fadeAway));
    }

    IEnumerator Fade(bool fadeAway)
    {
        //Store the image component of the Black Screen
        Image img = blackScreen.GetComponent<Image>();
        Color imgColor = img.color;

        //Fade out
        if (fadeAway)
        {
            //If the target screen is active
            if (blackScreen.activeInHierarchy)
            {
                //Loop over fadeTime seconds
                for (float i = fadeTime; i > 0; i -= Time.deltaTime)
                {
                    //Create a percentage and use this as an alpha
                    float percent = i / fadeTime;
                    imgColor.a = percent;
                    img.color = imgColor;
                    yield return null;
                }
                //Deactivate the screen now that it's faded out
                blackScreen.SetActive(false);
            }
            //Else the target screen is inactive, throw an error
            else
            {
                Debug.Log("BlackScreen is not active to fade out!");
            }
        }

        //Fade in
        else if (!fadeAway)
        {
            //If the target screen is active
            if (blackScreen.activeInHierarchy)
            {
                Debug.Log("BlackScreen was already active for fade in!");
            }
            else
            {
                //Make the target screen active
                blackScreen.SetActive(true);

                //Loop over fadeTime seconds
                for (float i = 0; i <= fadeTime; i += Time.deltaTime)
                {
                    //Create a percentage and use this as an alpha
                    float percent = i / fadeTime;
                    imgColor.a = percent;
                    img.color = imgColor;
                    yield return null;
                }
            }            
        }
    }

}
