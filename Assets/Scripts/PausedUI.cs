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
        SceneManager.LoadScene("Stage01-1");
    }
    public void ReturnToStageSelect()
    {
        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene("Title");
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var tUI = GameObject.Find("GameDirector").GetComponent<TitleUI>();

        // 遷移した後の処理
        tUI.StageSelectBool = true;

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
