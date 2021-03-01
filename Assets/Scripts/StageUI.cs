using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageUI : MonoBehaviour
{
    GameObject pausedPanel;
    GameObject howToPlayPanel01;
    GameObject howToPlayPanel02;
    GameObject howToPlayPanel03;
    GameObject gameClearPanel;
    GameObject stageUIButtons;
    GameObject camType01, camType02, camType03;
    GameObject helpButton;
    GameObject hideStageUIPanel;

    bool openHTP01 = false;
    bool openHTP02 = false;
    bool openHTP03 = false;
    // HowToPlayパネルの現在のページ番号（０から）
    int pageHTP_Current = 0;
    // HowToPlayPanel01～03のページ枚数
    int pageNum01 = 0;
    int pageNum02 = 0;
    int pageNum03 = 0;
    // 現在開いているHowToPlayPanelのページ枚数
    int openingHTPPageNum = 0;
    // 最初のステージの開幕に限り、HowToPlayパネルを表示させる
    public bool firstStage = false;
    PlayerController pC;
    CameraController cC;
    void Awake()
    {
        pausedPanel = GameObject.Find("PausedPanel");
        howToPlayPanel01 = GameObject.Find("HowToPlayPanel01");
        howToPlayPanel02 = GameObject.Find("HowToPlayPanel02");
        howToPlayPanel03 = GameObject.Find("HowToPlayPanel03");
        gameClearPanel = GameObject.Find("GameClearPanel");
        stageUIButtons = GameObject.Find("StageUIButtons");
        camType01 = GameObject.Find("CamType01");
        camType02 = GameObject.Find("CamType02");
        camType03 = GameObject.Find("CamType03");
        helpButton = GameObject.Find("HelpButton");
        hideStageUIPanel = GameObject.Find("HideStageUIPanel");

        pC = GameObject.Find("Player").GetComponent<PlayerController>();
        cC = GameObject.Find("Camera").GetComponent<CameraController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // HowToPlayPanel01～3の子スライドのページ枚数を数えておく
        foreach (Transform child in howToPlayPanel01.transform) pageNum01++;
        foreach (Transform child in howToPlayPanel02.transform) pageNum02++;
        foreach (Transform child in howToPlayPanel03.transform) pageNum03++;

        // 全て非表示にしておく
        pausedPanel.SetActive(false);
        howToPlayPanel01.SetActive(false);
        howToPlayPanel02.SetActive(false);
        howToPlayPanel03.SetActive(false);
        gameClearPanel.SetActive(false);
        camType02.SetActive(false);
        camType03.SetActive(false);
        helpButton.SetActive(false);
    }

    void Update()
    {
        // Playerが静止状態かつ着地している場合のみ、画面上のUI操作が可能となる
        if(pC.Stopping) hideStageUIPanel.SetActive(false);
        else hideStageUIPanel.SetActive(true);

        // Pausedに対応するPボタンが押されるか、Escキーが押されたらPausedパネルを開く（ただしプレイヤーが操作可能な状態で停止している場合に限り、HowToPlayパネルが表示中も無効）
        if (pC.Control && pC.Stopping && (!openHTP01 && !openHTP02 && !openHTP03) && (Input.GetButtonDown("Paused") || Input.GetKeyDown(KeyCode.Escape)))
        {
            OpenPausedPanel();
        }

        // HowToPlayパネル表示中に左クリックまたはエンターキーが押された場合、ページを１枚目めくる、またはHowToPlayパネルを閉じる
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
        {
            TurnPagesORCloseHTP();
        } 
    }
    public void OpenPausedPanel()
    {
        pausedPanel.SetActive(true);
        // カメラのコントロールを無効にする
        cC.CamControl = false;
        // プレイヤーのコントロールを無効にする（※カメラがSatelliteモード、FirstPersonモードの時は既に無効になっているが念のため）
        pC.Control = false;
    }
    void TurnPagesORCloseHTP()
    {
        if (openHTP01) TurnPagesORCloseHTP01();
        else if (openHTP02) TurnPagesORCloseHTP02();
        else if (openHTP03) TurnPagesORCloseHTP03();
    }
    void TurnPagesORCloseHTP01()
    {
        pageHTP_Current++;

        // 現在のページを非表示（透過）にする
        howToPlayPanel01.transform.GetChild(pageHTP_Current - 1).GetComponent<Image>().enabled = false;

        // 最終ページで押されたとき、HowToPlayパネルを閉じ、Pausedパネルを開く。
        if (pageHTP_Current == pageNum01)
        {
            howToPlayPanel01.SetActive(false);
            pageHTP_Current = 0;
            openHTP01 = false;

            // 最初のステージの開幕に限り、Pausedパネルを開かない
            // また、プレイヤーのコントロールも有効にする
            if (!firstStage)
            {
                pausedPanel.SetActive(true);
            }
            else
            {
                firstStage = false;
                pC.Control = true;
            }
        }
        else howToPlayPanel01.transform.GetChild(pageHTP_Current).GetComponent<Image>().enabled = true; // 最終ページではないときは、次のページを表示する
    }
    void TurnPagesORCloseHTP02()
    {
        pageHTP_Current++;

        // 現在のページを非表示（透過）にする
        howToPlayPanel02.transform.GetChild(pageHTP_Current - 1).GetComponent<Image>().enabled = false;

        // 最終ページで押されたとき、HowToPlayパネルを閉じる
        if (pageHTP_Current == pageNum02)
        {
            howToPlayPanel02.SetActive(false);
            pageHTP_Current = 0;
            cC.CamControl = true;

            openHTP02 = false;
        }
        else howToPlayPanel02.transform.GetChild(pageHTP_Current).GetComponent<Image>().enabled = true; // 最終ページではないときは、次のページを表示する
    }
    void TurnPagesORCloseHTP03()
    {
        pageHTP_Current++;

        // 現在のページを非表示（透過）にする
        howToPlayPanel03.transform.GetChild(pageHTP_Current - 1).GetComponent<Image>().enabled = false;

        // 最終ページで押されたとき、HowToPlayパネルを閉じる
        if (pageHTP_Current == pageNum03)
        {
            howToPlayPanel03.SetActive(false);
            pageHTP_Current = 0;
            cC.CamControl = true;

            openHTP03 = false;
        }
        else howToPlayPanel03.transform.GetChild(pageHTP_Current).GetComponent<Image>().enabled = true; // 最終ページではないときは、次のページを表示する
    }

    public void Restart()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.RestartFade();
    }
    public void Restart_SceneChange()
    {
        string stage = SceneManager.GetActiveScene().name;

        SceneManager.sceneLoaded += SceneLoaded_Restart;
        SceneManager.LoadScene(stage);
    }

    private void SceneLoaded_Restart(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var fade_Next = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        // 遷移した後の処理
        fade_Next.cutoutRange = 1;

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }

    public void ReturnToStageSelect()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.ReturnToStageSelect(1.5f, 1.6f);
    }

    public void ReturnToStageSelect_SceneChange()
    {
        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene("Title");
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var fade_Next = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        var titUI = GameObject.Find("GameDirector").GetComponent<TitleUI>();

        // 遷移した後の処理
        fade_Next.cutoutRange = 1;
        titUI.StageSelectBool = true;
        titUI.ReturnFromStages = true;

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }
    public void HowToPlay01()
    {
        // Pausedパネルを閉じ、HowToPlayパネルを開く
        pausedPanel.SetActive(false);
        howToPlayPanel01.SetActive(true);

        // １枚目のスライドの表示
        howToPlayPanel01.transform.GetChild(0).GetComponent<Image>().enabled = true;

        // 開くHowToPlayパネルの全枚数（openingHTPPageNum）の更新
        openingHTPPageNum = pageNum01;

        openHTP01 = true;
    }
    public void HowToPlay02or03()
    {
        cC.CamControl = false;

        if (cC.Satellite) // Satelliteモードの時
        {
            // HowToPlayPanel02を開く
            howToPlayPanel02.SetActive(true);
            // １枚目のスライドの表示
            howToPlayPanel02.transform.GetChild(0).GetComponent<Image>().enabled = true;
            // 開くHowToPlayパネルの全枚数（openingHTPPageNum）の更新
            openingHTPPageNum = pageNum02;

            openHTP02 = true;
        }
        else if (cC.FirstPerson) // FirstPersonモードの時
        {
            // HowToPlayPanel03を開く
            howToPlayPanel03.SetActive(true);
            // １枚目のスライドの表示
            howToPlayPanel03.transform.GetChild(0).GetComponent<Image>().enabled = true;
            // 開くHowToPlayパネルの全枚数（openingHTPPageNum）の更新
            openingHTPPageNum = pageNum03;

            openHTP03 = true;
        } 
    }
    public void BackToGame()
    {
        pausedPanel.SetActive(false);
        // カメラのコントロールを有効に戻す
        cC.CamControl = true;
        // カメラが通常カメラモードの場合のみ、プレイヤーのコントロールを有効に戻す
        if(!cC.Satellite && !cC.FirstPerson) pC.Control = true;
    }
    public void DisplayGameClear()
    {
        gameClearPanel.SetActive(true);
    }
    public void SwitchToSatelliteCamMode_PressedButton()
    {
        pC.SwitchToSatelliteCamMode_PressedButton();
    }
    public void SwitchToFirstPersonCamMode_PressedButton()
    {
        cC.SwitchToFirstPersonCamMode_PressedButton();
    }
    public void SwitchToNormalCamMode_PressedButton()
    {
        cC.SwitchToNormalCamMode_PressedButton();
    }
    public void SwitchCamTypeUI_ToSatellite()
    {
        camType01.SetActive(false);
        camType02.SetActive(true);
    }
    public void SwitchCamTypeUI_ToFirstPerson()
    {
        camType02.SetActive(false);
        camType03.SetActive(true);
    }
    public void SwitchCamTypeUI_ToNormal()
    {
        camType03.SetActive(false);
        camType01.SetActive(true);
    }
    public void DisplayHelpButton()
    {
        helpButton.SetActive(true);
    }
    public void HideHelpButton()
    {
        helpButton.SetActive(false);
    }
    public void HideStageUIButtons()
    {
        stageUIButtons.SetActive(false);
    }
    public bool FirstStage
    {
        set { firstStage = value; }
        get { return firstStage; }
    }
}
