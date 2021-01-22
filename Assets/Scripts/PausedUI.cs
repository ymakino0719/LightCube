using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedUI : MonoBehaviour
{
    GameObject pausedPanel;
    bool paused = false;
    // Start is called before the first frame update
    void Awake()
    {
        pausedPanel = GameObject.Find("PausedPanel");
    }

    // Start is called before the first frame update
    void Start()
    {
        pausedPanel.SetActive(false);
    }

    void Update()
    {
        // Pausedに対応するPボタンが押されるか、Escキーが押されたらPausedパネルを開く
        paused = Input.GetButtonDown("Paused");
        if (Input.GetKeyDown(KeyCode.Escape)) paused = true;

        if(paused) pausedPanel.SetActive(true);
    }

    public void Restart()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.RestartFade();
    }
    public void GoBackToStageSelect_FromPausedUI()
    {
        TransitionUI traUI = GameObject.Find("UIDirector").GetComponent<TransitionUI>();
        traUI.GoBackToStageSelect(3.0f, 1.5f);
    }

    public void Restart_SceneChange()
    {
        string scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
    public void ReturnToStageSelect()
    {
        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene("Title");
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var titUI = GameObject.Find("GameDirector").GetComponent<TitleUI>();
        var fade_N = GameObject.Find("FadeCanvas_Normal").GetComponent<Fade>();

        // 遷移した後の処理
        titUI.StageSelectBool = true;
        fade_N.cutoutRange = 1;
        titUI.GoBackToStageSelect();

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }
    public void HowToMove()
    {
        
    }
    public void BackToGame()
    {
        pausedPanel.SetActive(false);
        paused = false;
    }
}
