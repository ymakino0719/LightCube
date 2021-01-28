using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearJudgement : MonoBehaviour
{
    GameObject clearLight;
    Light lighting;

    // ゲームクリア判定
    bool gameOver = false;
    // ゲームクリア判定: 遷移01
    bool gameOver01 = false;
    // ゲームクリア判定: 遷移02
    bool gameOver02 = false;
    // ゲームクリア判定: 遷移03
    bool gameOver03 = false;
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
    bool startingOperation01 = true;
    // キャラクターのStarLight方面への振り向き
    bool beginning01 = true;
    // キャラクターのVictory画面
    bool beginning02 = true;
    // エンドムービーの待機時間
    bool beginning03 = true;
    // Rangeの振幅開始時間
    float startTime;
    // カメラ制御
    CameraControll cC;
    // アニメーション制御
    AnimationController aC;
    // プレイヤー制御
    PlayerController pC;

    void Awake()
    {
        clearLight = GameObject.Find("ClearLight");
        lighting = clearLight.GetComponent<Light>();
        lighting.range = 0;
        lighting.enabled = false;

        cC = GameObject.Find("Camera").GetComponent<CameraControll>();
        aC = GameObject.Find("Yagikun3D").GetComponent<AnimationController>();
        pC = GameObject.Find("Player").GetComponent<PlayerController>();
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
            if(gameOver01)
            {
                GameOver01_Behavior();
            }
            else if(gameOver02)
            {
                GameOver02_Behavior();
            }
            else if(gameOver03)
            {
                GameOver03_Behavior();
            }

            if (startingOperation01)
            {
                // StarLight
                ScalingLightBeginningRange();
            }
            else
            {
                // StarLight
                ScalingLightAfterRange();
            }
        }
    }

    void GameOver01_Behavior()
    {
        if (beginning01)
        {
            // TurnToStarLight
            pC.LookAtStarLightSmoothly_Beginning();

            beginning01 = false;
        }
    }
    void GameOver02_Behavior()
    {
        if (beginning02)
        {
            // VictoryAnimation
            aC.VictoryAnimation();
            
            beginning02 = false;
        }
    }
    void GameOver03_Behavior()
    {
        if (beginning03)
        {
            StartCoroutine("GameOverScreenDisplayTime");
        }
        else
        {
            ReturnToStageSelect();
        }
    }

    IEnumerator GameOverScreenDisplayTime()
    {
        // 勝利画面でn秒間待機する
        yield return new WaitForSeconds(4);

        beginning03 = false;
    }

    void JudgeClearConditions()
    {
        if (keyNum == clearNum)
        {
            gameOver = true;
            gameOver01 = true;
            lighting.enabled = true;
        }
    }
    void ReturnToStageSelect()
    {
        if (Input.anyKey)
        {
            TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
            traUI.ReturnToStageSelect(5.0f, 3.0f);
        }
    }

    void ScalingLightBeginningRange()
    {
        if (lighting.range < lower)
        {
            lighting.range += ampAmount;
        }
        else
        {
            lighting.range = lower;
            startTime = Time.time;
            startingOperation01 = false;
        }
    }
    void ScalingLightAfterRange()
    {
        float currentTime = Time.time - startTime;
        lighting.range = lower + Mathf.Sin(currentTime * speed) * magnification;
    }
    public int KeyNum
    {
        set { keyNum = value; }
        get { return keyNum; }
    }
    public bool GameOver01
    {
        set { gameOver01 = value; }
        get { return gameOver01; }
    }
    public bool GameOver02
    {
        set { gameOver02 = value; }
        get { return gameOver02; }
    }
    public bool GameOver03
    {
        set { gameOver03 = value; }
        get { return gameOver03; }
    }
}
