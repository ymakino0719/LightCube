using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject stageSelectPanel;
    GameObject titlePanel;
    
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

        fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        fade_N = GameObject.Find("FadeCanvas_Normal").GetComponent<Fade>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stageSelectPanel.SetActive(false);
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
        fade.FadeIn(fadeTime);
        
        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        // ★仮措置
        //yield return null;

        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene(stageName);
    }
    IEnumerator FadeOutCoroutine()
    {
        Debug.Log("FadeOutStart");

        fade_N.FadeOut(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        Debug.Log("FadeOutEnd");

        // ★仮措置
        //yield return null;
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        // 遷移した後の処理
        fade.cutoutRange = 1;

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
