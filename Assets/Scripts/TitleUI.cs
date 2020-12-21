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
    public void Stage01()
    {
        SceneManager.LoadScene("Stage01-1");
    }
    public bool StageSelectBool
    {
        set { stageSelectBool = value; }
        get { return stageSelectBool; }
    }
}
