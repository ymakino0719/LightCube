using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject stageSelectPanel;
    GameObject titlePanel;

    GameObject fadeCanvas, fade_NCanvas;
    Fade fade, fade_N;
    // フェードを行う時間
    float fadeTime = 1.5f;
    // フェードを始めてからシーンが遷移するまでの時間（fadeTimeと同じかそれ以上の長さにする）
    float transitionTime = 1.6f;

    bool stageSelectBool = false;

    bool returnFromStages = false;
    void Awake()
    {
        titlePanel = GameObject.Find("TitlePanel");
        stageSelectPanel = GameObject.Find("StageSelectPanel");

        fadeCanvas = GameObject.Find("FadeCanvas");
        fade_NCanvas = GameObject.Find("FadeCanvas_Normal");
        fade = fadeCanvas.GetComponent<Fade>();
        fade_N = fade_NCanvas.GetComponent<Fade>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stageSelectPanel.SetActive(false);

        fadeCanvas.SetActive(false);
        fade_NCanvas.SetActive(false);

        if (returnFromStages) { StartCoroutine("FadeOutCoroutine"); }
    }
    void Update()
    {
        if(stageSelectBool)
        {
            ChangeToStageSelectPanel();
            stageSelectBool = false;
        }
    }

    public void ChangeToStageSelectPanel()
    {
        titlePanel.SetActive(false);
        stageSelectPanel.SetActive(true);
    }
    public void ChangeToTitlePanel()
    {
        stageSelectPanel.SetActive(false);
        titlePanel.SetActive(true);
    }
    public void Stage01_1()
    {
        StartCoroutine(FadeInCoroutine("Stage01-1"));
    }
    public void Stage01_2()
    {
        StartCoroutine(FadeInCoroutine("Stage01-2"));
    }
    public void Stage01_3()
    {
        StartCoroutine(FadeInCoroutine("Stage01-3"));
    }
    public void Stage01_4()
    {
        StartCoroutine(FadeInCoroutine("Stage01-4"));
    }
    public void Stage01_5()
    {
        StartCoroutine(FadeInCoroutine("Stage01-5"));
    }
    public void Stage01_6()
    {
        StartCoroutine(FadeInCoroutine("Stage01-6"));
    }

    IEnumerator FadeInCoroutine(string stageName)
    {
        fadeCanvas.SetActive(true);
        fade.FadeIn(fadeTime);
        
        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene(stageName);
    }
    IEnumerator FadeOutCoroutine()
    {
        fade_NCanvas.SetActive(true);
        fade_N.FadeOut(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        fade_NCanvas.SetActive(false);
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var fade_Next = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        // 遷移した後の処理
        fade_Next.cutoutRange = 1;

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }

    public bool StageSelectBool
    {
        set { stageSelectBool = value; }
        get { return stageSelectBool; }
    }
    public bool ReturnFromStages
    {
        set { returnFromStages = value; }
        get { return returnFromStages; }
    }
}
