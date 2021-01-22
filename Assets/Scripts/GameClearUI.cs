﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    GameObject gameClearPanel;

    // Start is called before the first frame update
    void Awake()
    {
        gameClearPanel = GameObject.Find("GameClearPanel");
    }
    void Start()
    {
        gameClearPanel.SetActive(false);
    }

    public void DisplayGameClear()
    {
        gameClearPanel.SetActive(true);
    }
}
