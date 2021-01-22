using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject stageSelectPanel;
    GameObject titlePanel;
    
    Fade fade;
    // フェードを行う時間
    float fadeTime = 1.5f;
    // フェードを始めてからシーンが遷移するまでの時間（fadeTimeと同じかそれ以上の長さにする）
    float transitionTime = 1.8f;

    bool stageSelectBool = false;
    void Awake()
    {
        titlePanel = GameObject.Find("TitlePanel");
        stageSelectPanel = GameObject.Find("StageSelectPanel");
        fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
    }

    // Start is called before the first frame update
    void Start()
    {
        stageSelectPanel.SetActive(false);
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
        StartCoroutine(FadeCoroutine("Stage01-1"));
    }
    public void Stage01_2()
    {
        StartCoroutine(FadeCoroutine("Stage01-2"));
    }
    public void Stage01_3()
    {
        StartCoroutine(FadeCoroutine("Stage01-3"));
    }
    public void Stage01_4()
    {
        StartCoroutine(FadeCoroutine("Stage01-4"));
    }

    IEnumerator FadeCoroutine(string stageName)
    {
        fade.FadeIn(fadeTime);

        // フェード時間中の時間を止める
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(stageName);
    }

    public bool StageSelectBool
    {
        set { stageSelectBool = value; }
        get { return stageSelectBool; }
    }
}
