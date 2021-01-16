using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    GameObject stageSelectPanel;
    GameObject titlePanel;

    bool stageSelectBool = false;
    void Awake()
    {
        titlePanel = GameObject.Find("TitlePanel");
        stageSelectPanel = GameObject.Find("StageSelectPanel");
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
        SceneManager.LoadScene("Stage01-1");
    }
    public void Stage01_2()
    {
        SceneManager.LoadScene("Stage01-2");
    }
    public void Stage01_3()
    {
        SceneManager.LoadScene("Stage01-3");
    }
    public void Stage01_4()
    {
        SceneManager.LoadScene("Stage01-4");
    }
    public bool StageSelectBool
    {
        set { stageSelectBool = value; }
        get { return stageSelectBool; }
    }
}
