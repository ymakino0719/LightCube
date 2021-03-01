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
    // ステージセレクト画面へ戻るためのAnyKeyの入力受付開始
    bool startAcceptingInput_AnyKey = false;
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
    float speed = 0.8f;
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
    CameraController cC;
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

        cC = GameObject.Find("Camera").GetComponent<CameraController>();
        aC = GameObject.Find("Yagikun3D").GetComponent<AnimationController>();
        pC = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!gameOver) JudgeClearConditions();
        else
        {
            if     (gameOver01) GameOver01_Behavior();
            else if(gameOver02) GameOver02_Behavior();
            else if(gameOver03) GameOver03_Behavior();

            if (startingOperation01) ScalingLightBeginningRange();
            else                     ScalingLightAfterRange();
        }
    }
    void Update()
    {
        if(startAcceptingInput_AnyKey) ReturnToStageSelect();
    }
    void JudgeClearConditions()
    {
        if (keyNum == clearNum)
        {
            gameOver = true;
            gameOver01 = true;
            cC.GameOver = true;
            lighting.enabled = true;
        }
    }

    void GameOver01_Behavior()
    {
        if (beginning01)
        {
            ////////////////////
            ////// Player //////
            ////////////////////

            // yagikun3DをStarLightの方向にゆっくり回転させるための開幕処理
            pC.LookAtStarLightSmoothly_Beginning();

            ////////////////////
            ////// Camera //////
            ////////////////////

            cC.StartingOperation01();

            beginning01 = false;
        }
        else
        {
            ////////////////////
            ////// Player //////
            ////////////////////

            // yagikun3DをStarLightの方向にゆっくり回転させる
            pC.LookAtStarLightSmoothly_Processing();

            ////////////////////
            ////// Camera //////
            ////////////////////

            // ゆっくりStarLightの方を見る
            cC.TurnAroundToStarLight();
            // ゆっくりStarLightにカメラを近づける
            cC.MoveCloserToStarLight();
        }
    }
    void GameOver02_Behavior()
    {
        if (beginning02)
        {
            ////////////////////
            ////// Player //////
            ////////////////////

            // yagikun3DをStarLightの方向にゆっくり回転させる処理の強制終了
            pC.LookAtStarLightSmoothly_End();

            // VictoryAnimation
            aC.VictoryAnimation();

            ////////////////////
            ////// Camera //////
            ////////////////////

            // StarLightが背景になるようにカメラ移動する
            cC.Move_StarLightBecomesBackground();

            beginning02 = false;
        }
    }
    void GameOver03_Behavior()
    {
        ////////////////////
        //////// UI ////////
        ////////////////////

        if (beginning03)
        {
            StartCoroutine("GameOverScreenDisplayTime");
        }
    }

    IEnumerator GameOverScreenDisplayTime()
    {
        beginning03 = false;

        // 勝利画面でn秒間待機する
        yield return new WaitForSeconds(4);

        // AnyKeyの入力受付開始
        startAcceptingInput_AnyKey = true;
    }
    void ReturnToStageSelect()
    {
        if (Input.anyKey)
        {
            startAcceptingInput_AnyKey = false;

            TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
            traUI.ReturnToStageSelect(3.0f, 3.1f);
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
        lighting.range = lower + Mathf.Sin(currentTime * speed * Mathf.PI) * magnification;
    }
    public int KeyNum
    {
        set { keyNum = value; }
        get { return keyNum; }
    }
    public bool GameOver
    {
        set { gameOver = value; }
        get { return gameOver; }
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
