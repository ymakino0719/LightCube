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
            pC.Control = false;
        }

        // HowToPlayパネル表示中に左クリック、エンターキーまたはエスケープキーが押された場合、HowToPlayパネルを閉じ、Pausedパネルを開く
        if (openHTP && (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)))
        {
            howToPlayPanel.SetActive(false);
            openHTP = false;

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
        var fade_N_Next = GameObject.Find("FadeCanvas_Normal").GetComponent<Fade>();
        var titUI = GameObject.Find("GameDirector").GetComponent<TitleUI>();

        // 遷移した後の処理
        fade_N_Next.cutoutRange = 1;
        titUI.StageSelectBool = true;
        titUI.ReturnFromStages = true;

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
        pC.Control = true;
    }
    public bool FirstStage
    {
        set { firstStage = value; }
        get { return firstStage; }
    }
}
