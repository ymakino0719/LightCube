using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageUI : MonoBehaviour
{
    GameObject pausedPanel;
    GameObject howToPlayPanel;
    GameObject gameClearPanel;

    bool openHTP = false;
    // HowToPlayパネルの現在のページ番号（０から）
    int pageHTP_Current = 0;
    // HowToPlayパネルの最大ページ番号（不変値）
    int pageHTP_Max = 2;
    // 最初のステージの開幕に限り、HowToPlayパネルを表示させる
    public bool firstStage = false;
    PlayerController pC;
    void Awake()
    {
        pausedPanel = GameObject.Find("PausedPanel");
        howToPlayPanel = GameObject.Find("HowToPlayPanel");
        gameClearPanel = GameObject.Find("GameClearPanel");
        pC = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // HowToPlayパネルの子スライドを全て非表示にしておく
        foreach (Transform child in howToPlayPanel.transform)
        {
            child.gameObject.GetComponent<Image>().enabled = false;
        }

        // 全て非表示にしておく
        pausedPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        gameClearPanel.SetActive(false);
    }

    void Update()
    {
        // Pausedに対応するPボタンが押されるか、Escキーが押されたらPausedパネルを開く（ただしプレイヤーが操作可能な状態で停止している場合に限り、HowToPlayパネルが表示中も無効）
        if (pC.Control && pC.Stopping && !openHTP && (Input.GetButtonDown("Paused") || Input.GetKeyDown(KeyCode.Escape)))
        {
            pausedPanel.SetActive(true);
            pC.Control = false;
        }

        // HowToPlayパネル表示中に左クリックまたはエンターキーが押された場合、ページを１枚目めくる
        if (openHTP && (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Return)))
        {
            pageHTP_Current++;

            // 現在のページを非表示（透過）にする
            howToPlayPanel.transform.GetChild(pageHTP_Current - 1).GetComponent<Image>().enabled = false;

            // 最終ページで押されたとき、HowToPlayパネルを閉じ、Pausedパネルを開く。そうでないときは、次のページを表示する
            if (pageHTP_Current == pageHTP_Max) CloseHowToPlay();
            else howToPlayPanel.transform.GetChild(pageHTP_Current).GetComponent<Image>().enabled = true;
        } 
    }
    void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
        pageHTP_Current = 0;
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
    public void HowToPlay()
    {
        // Pausedパネルを閉じ、HowToPlayパネルを開く
        pausedPanel.SetActive(false);
        howToPlayPanel.SetActive(true);

        // １枚目のスライドの表示
        howToPlayPanel.transform.GetChild(0).GetComponent<Image>().enabled = true;

        openHTP = true;
    }
    public void BackToGame()
    {
        pausedPanel.SetActive(false);
        pC.Control = true;
    }
    public void DisplayGameClear()
    {
        gameClearPanel.SetActive(true);
    }
    public bool FirstStage
    {
        set { firstStage = value; }
        get { return firstStage; }
    }
}
