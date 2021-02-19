using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject stageSelectPanel;
    GameObject titlePanel;

    GameObject fadeCanvas;
    Fade fade;
    FadeImage fI;

    // マテリアルのセット
    public Material mat_S, mat_N;
    // テクスチャのセット
    public Texture tex_S, tex_N;

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
        fade = fadeCanvas.GetComponent<Fade>();
        fI = fadeCanvas.GetComponent<FadeImage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stageSelectPanel.SetActive(false);

        fadeCanvas.SetActive(false);

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
        fI.material = mat_S;
        fI.maskTexture = tex_S;
        fade.FadeIn(fadeTime);
        
        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        SceneManager.sceneLoaded += SceneLoaded_StageSelect;
        SceneManager.LoadScene(stageName);
    }
    IEnumerator FadeOutCoroutine()
    {
        fadeCanvas.SetActive(true);
        fI.material = mat_N;
        fI.maskTexture = tex_N;
        fade.FadeOut(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        fadeCanvas.SetActive(false);
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
