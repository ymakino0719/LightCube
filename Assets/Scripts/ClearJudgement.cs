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
    public float lower = 6.0f;
    // Rangeの増幅率
    float ampAmount = 0.2f;
    // Rangeの拡大率
    public float magnification = 1.5f;
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

    // ClearLightのAudioSource
    AudioSource audio_CL;
    // 音量の加算率
    float addVol = 0.02f;
    // 音量の減算率
    float subVol = 0.01f;
    // 最大音量
    float maxVol = 0.1f;
    // 最終的な設定音量
    float endVol = 0.02f;
    // ゲームオーバー後の遷移時の音量のフェードアウト
    bool fadeOutSounds = false;

    void Awake()
    {
        clearLight = GameObject.Find("ClearLight");
        lighting = clearLight.GetComponent<Light>();
        lighting.range = 0;
        lighting.enabled = false;
        audio_CL = clearLight.GetComponent<AudioSource>();

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

            if (fadeOutSounds) FadeOutAllSounds();
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

            ////////////////////
            /////// SFX ////////
            ////////////////////

            // StarLightが輝く効果音を鳴らす
            audio_CL.Play();

            MusicPlayer mP = GameObject.FindWithTag("Music").GetComponent<MusicPlayer>();
            // Musicにフェードアウトをかける
            mP.FadeOutV = true;
            // Musicの通常曲の再生を止める
            mP.GameOver = true;

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

            ////////////////////
            /////// SFX ////////
            ////////////////////

            // StarLightが輝く効果音を徐々に大きくする
            if (audio_CL.volume + Time.deltaTime * addVol < maxVol) audio_CL.volume += Time.deltaTime * addVol;
            else audio_CL.volume = maxVol;
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

            ////////////////////
            /////// SFX ////////
            ////////////////////

            // ゲームオーバーのジングルをかける
            GameObject.FindWithTag("Music").GetComponent<MusicPlayer>().PlayJingle(0);

            // StarLightが輝く効果音の大きさを変更する
            audio_CL.volume = endVol;

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
        yield return new WaitForSeconds(5);

        // AnyKeyの入力受付開始
        startAcceptingInput_AnyKey = true;
    }
    void ReturnToStageSelect()
    {
        if (Input.anyKey)
        {
            startAcceptingInput_AnyKey = false;
            fadeOutSounds = true;

            TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
            traUI.ReturnToStageSelect(3.0f, 3.1f);
        }
    }
    void FadeOutAllSounds()
    {
        // StarLightが輝く効果音を徐々に小さくする
        if (audio_CL.volume - Time.deltaTime * subVol > 0) audio_CL.volume -= Time.deltaTime * subVol;
        else audio_CL.volume = 0;
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
