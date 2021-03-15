using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject titlePanel;
    GameObject stageSelectPanel;
    GameObject hideUIPanel;

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

    bool returnFromStages = false;

    // 効果音
    SFXPlayer sfx;

    // BGMの音量
    float bgmVol = 1.0f;
    // SFXの音量
    float sfxVol = 1.0f;
    void Awake()
    {
        titlePanel = GameObject.Find("TitlePanel");
        stageSelectPanel = GameObject.Find("StageSelectPanel");
        hideUIPanel = GameObject.Find("HideUIPanel");

        fadeCanvas = GameObject.Find("FadeCanvas");
        fade = fadeCanvas.GetComponent<Fade>();
        fI = fadeCanvas.GetComponent<FadeImage>();

        sfx = GameObject.Find("SFX").GetComponent<SFXPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stageSelectPanel.SetActive(false);
        fadeCanvas.SetActive(false);

        if (!returnFromStages) 
        {
            hideUIPanel.SetActive(false);
        }
        else
        {
            StartCoroutine("FadeOutCoroutine");
            ChangeToStageSelectPanel();
            returnFromStages = false;
        }
    }
    public void ChangeToStageSelectPanel()
    {
        titlePanel.SetActive(false);
        stageSelectPanel.SetActive(true);

        if(!returnFromStages) sfx.PlaySFX(0); // 効果音を鳴らす
    }
    public void ChangeToTitlePanel()
    {
        stageSelectPanel.SetActive(false);
        titlePanel.SetActive(true);

        // 効果音を鳴らす
        sfx.PlaySFX(1);
    }
    public void Stage01_1()
    {
        StartCoroutine(FadeInCoroutine("Stage01-1"));
        SelectStageSFX();
    }
    public void Stage01_2()
    {
        StartCoroutine(FadeInCoroutine("Stage01-2"));
        SelectStageSFX();
    }
    public void Stage01_3()
    {
        StartCoroutine(FadeInCoroutine("Stage01-3"));
        SelectStageSFX();
    }
    public void Stage01_4()
    {
        StartCoroutine(FadeInCoroutine("Stage01-4"));
        SelectStageSFX();
    }
    public void Stage01_5()
    {
        StartCoroutine(FadeInCoroutine("Stage01-5"));
        SelectStageSFX();
    }
    public void Stage01_6()
    {
        StartCoroutine(FadeInCoroutine("Stage01-6"));
        SelectStageSFX();
    }
    public void CursorOnTheButton()
    {
        // 効果音を鳴らす
        sfx.PlaySFX(2);
    }
    void SelectStageSFX()
    {
        // 効果音を鳴らす
        sfx.PlaySFX(3);
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

        hideUIPanel.SetActive(false);
        fadeCanvas.SetActive(false);
    }
    private void SceneLoaded_StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var fade_Next = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        var stageUI = GameObject.Find("UIDirector").GetComponent<StageUI>();
        var slider_BGM_Next = GameObject.FindWithTag("Slider_BGM").GetComponent<Slider>();
        var slider_SFX_Next = GameObject.FindWithTag("Slider_SFX").GetComponent<Slider>();

        // 遷移した後の処理
        fade_Next.cutoutRange = 1;
        stageUI.BgmVol = bgmVol;
        stageUI.SfxVol = sfxVol;
        slider_BGM_Next.value = bgmVol;
        slider_SFX_Next.value = sfxVol;

        // イベントから削除
        SceneManager.sceneLoaded -= SceneLoaded_StageSelect;
    }
    public bool ReturnFromStages
    {
        set { returnFromStages = value; }
        get { return returnFromStages; }
    }
    public float BgmVol
    {
        set { bgmVol = value; }
        get { return bgmVol; }
    }
    public float SfxVol
    {
        set { sfxVol = value; }
        get { return sfxVol; }
    }
}
