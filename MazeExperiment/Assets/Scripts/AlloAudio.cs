﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AlloAudio : MonoBehaviour
{
    //Dir path of audio files
    private string audioDir = Directory.GetCurrentDirectory() + "/Assets/Audio/Ego";
    //Audio source
    private AudioSource Source;
    //Current sound playing
    private AudioClip Clip;

    //List of AudioClips
    List<AudioClip> Clips = new List<AudioClip>();

    private int randomNum;

    bool waitActive = false;

    int counter = 0;

    public int WaitDelay = 4;
    public int MinMoves = 3;
    public int MaxMoves = 6;

    // Start is called before the first frame update
    void Start()
    {
        randomNum = Random.Range(MinMoves, MaxMoves);

        Source = GetComponent<AudioSource>();

        //Grabs all files from audioDir
        string[] files;
        files = Directory.GetFiles(audioDir);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".ogg"))
            {

                StartCoroutine(GetAudioClip(files[i]));
            }

        }
    }

    void Play(int index)
    {

        Clip = Clips[index];
        Source.clip = Clip;
        Source.Play();

    }

    // Update is called once per frame
    void Update()
    {

        if (!waitActive)
        {

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                counter += 1;
            }

            if (counter == randomNum)
            {

                int randomFileIndex = (int)Random.Range(0, Clips.Count);
                Play(randomFileIndex);
                StartCoroutine(Wait((int)Clips[randomFileIndex].length));
                randomNum = (int)Random.Range(MinMoves, MaxMoves);
                counter = 0;

            }
        }

    }

    IEnumerator Wait(int time)
    {

        waitActive = true;
        yield return new WaitForSeconds(time + WaitDelay);
        waitActive = false;

    }

    IEnumerator GetAudioClip(string fileName)
    {

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(new System.Uri(fileName).AbsoluteUri, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                Clips.Add(myClip);
            }
        }
    }

}
