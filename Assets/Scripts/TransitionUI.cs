using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    GameObject fadeCanvas;
    Fade fade;
    FadeImage fI;

    // テクスチャのセット
    public Texture tex_S, tex_N;

    // 星型のフェードを行う時間
    float fadeTime = 2.0f;
    // 星型のフェードを始めてからシーンが遷移するまでの時間
    float transitionTime = 2.1f;

    void Awake()
    {
        fadeCanvas = GameObject.Find("FadeCanvas");
        fade = fadeCanvas.GetComponent<Fade>();
        fI = fadeCanvas.GetComponent<FadeImage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeCanvas.SetActive(false);

        StartCoroutine("FadeOutCoroutine");
    }

    public void RestartFade()
    {
        StartCoroutine("RestartFadeInCoroutine");
    }
    public void ReturnToStageSelect(float fTime, float tTime)
    {
        StartCoroutine(ReturnToStageSelectCoroutine(fTime, tTime));
    }

    IEnumerator FadeOutCoroutine()
    {
        fadeCanvas.SetActive(true);
        fI.maskTexture = tex_S;
        fade.FadeOut(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        fadeCanvas.SetActive(false);
    }
    IEnumerator RestartFadeInCoroutine()
    {
        fadeCanvas.SetActive(true);
        fI.maskTexture = tex_S;
        fade.FadeIn(fadeTime);
        
        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        StageUI sUI = GameObject.Find("UIDirector").GetComponent<StageUI>();
        sUI.Restart_SceneChange();
    }
    IEnumerator ReturnToStageSelectCoroutine(float fTime, float tTime)
    {
        fadeCanvas.SetActive(true);
        fI.maskTexture = tex_N;
        fade.FadeIn(fTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(tTime);

        StageUI sUI = GameObject.Find("UIDirector").GetComponent<StageUI>();
        sUI.ReturnToStageSelect_SceneChange();
    }
}
