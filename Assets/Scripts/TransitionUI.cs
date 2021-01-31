using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{
    GameObject fadeCanvas, fade_NCanvas;
    Fade fade, fade_N;
    // 星型のフェードを行う時間
    float fadeTime = 2.0f;
    // 星型のフェードを始めてからシーンが遷移するまでの時間
    float transitionTime = 2.1f;
    // 通常のフェードを行う時間
    float fadeTime_N = 3.0f;
    // 通常のフェードを始めてからシーンが遷移するまでの時間
    float transitionTime_N = 1.5f;

    void Awake()
    {
        fadeCanvas = GameObject.Find("FadeCanvas");
        fade_NCanvas = GameObject.Find("FadeCanvas_Normal");
        fade = fadeCanvas.GetComponent<Fade>();
        fade_N = fade_NCanvas.GetComponent<Fade>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeCanvas.SetActive(false);
        fade_NCanvas.SetActive(false);

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
        fade.FadeOut(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        fadeCanvas.SetActive(false);
    }
    IEnumerator RestartFadeInCoroutine()
    {
        fadeCanvas.SetActive(true);
        fade.FadeIn(fadeTime);
        
        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        // ★仮措置
        //yield return null;

        PausedUI pUI = GameObject.Find("UIDirector").GetComponent<PausedUI>();
        pUI.Restart_SceneChange();
    }
    IEnumerator ReturnToStageSelectCoroutine(float fTime, float tTime)
    {
        fade_NCanvas.SetActive(true);
        fade_N.FadeIn(fTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(tTime);

        // ★仮措置
        //yield return null;

        PausedUI pUI = GameObject.Find("UIDirector").GetComponent<PausedUI>();
        pUI.ReturnToStageSelect_SceneChange();
    }
}
