﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DesignPattern;

public class GameUIManager : Singleton<GameUIManager>
{
    public GameObject Player;

    public GameObject TimeText;
    public GameObject LifeText;
    public GameObject StoneText;

    public GameObject PauseBackground;
	
	void Update ()
	{
	    TimeSpan timeSpan = GameManager.Instance.Chronometer;
        TimeText.GetComponent<Text>().text = string.Format("Time : {0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	    LifeText.GetComponent<Text>().text = string.Format("x{0}", Player.GetComponent<PlayerController>().Lifes);
	    StoneText.GetComponent<Text>().text = string.Format("x{0}", Player.GetComponent<PlayerController>().Rocks);
    }
}
