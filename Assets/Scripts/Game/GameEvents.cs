using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameEvent { Connection, Love, FriendshipLoss, LoveLoss, Peak }
public enum MusicEvent { Base, Connection1, Connection5, Connection10, LoveConnection, LoveBreak }

public class GameEvents : MonoBehaviour
{
    public float musicVolume;
    public float fadeTime;

    public Text sideBar;
    public AudioSource musicPhase1a;
    public AudioSource musicPhase1b;
    public AudioSource musicPhase2;
    private bool _musicPhase2Mute = true;
    public AudioSource musicPhase3;
    private bool _musicPhase3Mute = true;
    public AudioSource musicPhase4;
    private bool _musicPhase4Mute = true;
    public AudioSource musicPhase5;
    private bool _musicPhase5Mute = true;

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

    private void Awake()
    {
        musicPhase1a = AddAudio(atmosHigh);
        musicPhase1b = AddAudio(bassSpoopy);
        musicPhase2 = AddAudio(bassLaser);
        musicPhase3 = AddAudio(melodyBells);
        musicPhase4 = AddAudio(melodyPiano);
        musicPhase5 = AddAudio(melodyChimes);

        musicPhase1a.Play();
        musicPhase1b.Play();
        musicPhase2.Play();
        musicPhase3.Play();
        musicPhase4.Play();
        musicPhase5.Play();
    }

    private void Update()
    {
        //Fade in/out all the appropriate music
        MusicFade(musicPhase2, _musicPhase2Mute);
        MusicFade(musicPhase3, _musicPhase3Mute);
        MusicFade(musicPhase4, _musicPhase4Mute);
        MusicFade(musicPhase5, _musicPhase5Mute);
    }

    private AudioSource AddAudio(AudioClip audio)
    {
        GameObject cam = Camera.main.gameObject;
        AudioSource newSource = cam.AddComponent(typeof(AudioSource)) as AudioSource;
        newSource.clip = audio;
        newSource.loop = true;
        newSource.playOnAwake = true;
        newSource.volume = 0f;

        return newSource;
    }

    public void CheckRunEvent(GameEvent thisEvent)
    {
        switch (thisEvent)
        {
            case GameEvent.Connection:
                if (!_connectionDone)
                {
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

    public void RampAudio(MusicEvent me)
    {
        switch (me)
        {
            case MusicEvent.Base:
                musicPhase1a.volume = musicVolume;
                musicPhase1b.volume = musicVolume;
                _musicPhase2Mute = true;
                _musicPhase3Mute = true;
                _musicPhase4Mute = true;
                break;
            case MusicEvent.Connection1:
                musicPhase1a.volume = musicVolume;
                musicPhase1b.volume = musicVolume;
                _musicPhase2Mute = false;
                _musicPhase3Mute = true;
                _musicPhase4Mute = true;
                break;
            case MusicEvent.Connection5:
                musicPhase1a.volume = musicVolume;
                musicPhase1b.volume = musicVolume;
                _musicPhase2Mute = false;
                _musicPhase3Mute = false;
                _musicPhase4Mute = true;
                break;
            case MusicEvent.Connection10:
                musicPhase1a.volume = musicVolume;
                musicPhase1b.volume = musicVolume;
                _musicPhase2Mute = false;
                _musicPhase3Mute = false;
                _musicPhase4Mute = false;
                break;
            case MusicEvent.LoveConnection:
                _musicPhase5Mute = false;
                break;
            case MusicEvent.LoveBreak:
                _musicPhase5Mute = true;
                break;
        }
    }

    private void MusicFade(AudioSource music, bool mute)
    {
        //If we need to fade out
        if (mute)
        {
            if (music.volume > 0f)
            {
                music.volume = music.volume - (Time.deltaTime / fadeTime);
            }
        }
        else //We need to fade in
        {
            if (music.volume < 1f)
            {
                music.volume = music.volume + (Time.deltaTime / fadeTime);
            }
        }
    }
}
