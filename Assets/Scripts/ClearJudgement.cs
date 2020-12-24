using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearJudgement : MonoBehaviour
{
    GameObject clearLight;
    Light lighting;
    // ゲームクリア判定
    bool gameOver = false;
    // 現在の鍵の数
    int keyNum = 0;
    // ゲームクリアに必要な鍵の数
    public int clearNum;
    // Rangeの下限
    float lower = 6.0f;
    // Rangeの増幅率
    float ampAmount = 0.2f;
    // Rangeの拡大率
    float magnification = 1.5f;
    // Rangeを動かすスピード
    float speed = 2.5f;
    // 光りはじめ（trueのとき、Range = 0 からlowerまで拡大する）
    bool beginning = true;
    // Rangeの振幅開始時間
    float startTime;
    void Awake()
    {
        clearLight = GameObject.Find("ClearLight");
        lighting = clearLight.GetComponent<Light>();
        lighting.range = 0;
        lighting.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!gameOver)
        {
            JudgeClearConditions();
        }
        else
        {
            ScalingLightRange();
        }
    }

    void JudgeClearConditions()
    {
        if (keyNum == clearNum)
        {
            gameOver = true;
            lighting.enabled = true;
        }
    }

    void ScalingLightRange()
    {
        if (beginning)
        {
            if (lighting.range < lower)
            {
                lighting.range += ampAmount;
            }
            else
            {
                lighting.range = lower;
                startTime = Time.time;
                beginning = false;
            }
        }
        else
        {
            float currentTime = Time.time - startTime;
            lighting.range = lower + Mathf.Sin(currentTime * speed) * magnification;
        }
    }
    public int KeyNum
    {
        set { keyNum = value; }
        get { return keyNum; }
    }
}
