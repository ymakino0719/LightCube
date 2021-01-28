using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedUI : MonoBehaviour
{
    GameObject pausedPanel;
    GameObject howToPlayPanel;
    bool openHTP = false;
    // 最初のステージの開幕に限り、Howtoplayパネルを表示させる
    public bool firstStage = false;
    PlayerController pC;
    void Awake()
    {
        pausedPanel = GameObject.Find("PausedPanel");
        howToPlayPanel = GameObject.Find("HowToPlayPanel");
        pC = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pausedPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    void Update()
    {
        // Pausedに対応するPボタンが押されるか、Escキーが押されたらPausedパネルを開く（ただしプレイヤーが操作可能な状態の時に限り、HowToPlayパネルが表示中も無効）
        if (pC.Control && !openHTP && (Input.GetButtonDown("Paused") || Input.GetKeyDown(KeyCode.Escape)))
        {
            pausedPanel.SetActive(true);
        }

        // HowToPlayパネル表示中にいずれかのボタンが押された場合、HowToPlayパネルを閉じ、Pausedパネルを開く
        if (openHTP && Input.anyKey)
        {
            howToPlayPanel.SetActive(false);
            openHTP = false;

            // 最初のステージの開幕に限り、Pausedパネルを開かない
            if (!firstStage) pausedPanel.SetActive(true);
            else firstStage = false;
        }
    }

    public void Restart()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.RestartFade();
    }
    public void Restart_SceneChange()
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
    public void ReturnToStageSelect()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.ReturnToStageSelect(3.0f, 1.5f);
    }

    public void ReturnToStageSelect_SceneChange()
    {
        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene("Title");
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var titUI = GameObject.Find("GameDirector").GetComponent<TitleUI>();
        //var fade_N = GameObject.Find("FadeCanvas_Normal").GetComponent<Fade>();

        // 遷移した後の処理
        titUI.StageSelectBool = true;
        //fade_N.cutoutRange = 1;
        titUI.GoBackToStageSelect();

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }
    public void HowToPlay()
    {
        // Pausedパネルを閉じ、HowToPlayパネルを開く
        pausedPanel.SetActive(false);
        howToPlayPanel.SetActive(true);

        openHTP = true;
    }
    public void BackToGame()
    {
        pausedPanel.SetActive(false);
    }
    public bool FirstStage
    {
        set { firstStage = value; }
        get { return firstStage; }
    }
}
