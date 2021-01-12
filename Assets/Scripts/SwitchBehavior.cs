using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBehavior : MonoBehaviour
{
    // スイッチがオンかオフか
    bool switchBool = false;
    // スイッチの番号
    public int switchNum = 0;

    // マテリアルのセット
    public Material mat00, mat01, mat02, mat03;

    GameObject b_Main, bHB_ON, bHB_OFF, b_Gate;
    List<MeshRenderer> b_Main_MRList = new List<MeshRenderer>();
    List<BoxCollider> b_Main_BCList = new List<BoxCollider>();
    void Start()
    {
        OpeningSequence();
    }

    void OpeningSequence()
    {
        // 開幕処理
        if (switchNum == 0)
        {
            // スイッチの番号が未入力の場合、エラーメッセージを表示させる
            Debug.Log("Error! SwitchNum does not input");
        }
        if (switchNum == 1)
        {
            b_Main = GameObject.Find("B01_Main");
            bHB_ON = GameObject.Find("B01HB_ON");
            bHB_OFF = GameObject.Find("B01HB_OFF");
            b_Gate = GameObject.Find("Gates01");
        }
        else if (switchNum == 2)
        {
            b_Main = GameObject.Find("B02_Main");
            bHB_ON = GameObject.Find("B02HB_ON");
            bHB_OFF = GameObject.Find("B02HB_OFF");
            b_Gate = GameObject.Find("Gates02");
        }
        else if (switchNum == 3)
        {
            b_Main = GameObject.Find("B03_Main");
            bHB_ON = GameObject.Find("B03HB_ON");
            bHB_OFF = GameObject.Find("B03HB_OFF");
            b_Gate = GameObject.Find("Gates03");
        }
        else
        {
            // スイッチの番号が範囲外の場合、エラーメッセージを表示させる
            Debug.Log("Error! SwitchNum is not correct");
        }

        // スイッチがONのときの当たり判定とゲートを非アクティブにしておく
        bHB_ON.SetActive(false);
        b_Gate.SetActive(false);

        // 橋の歩く部分（Main部）のBoxColliderとMeshRendererのリスト化
        foreach (Transform c1Tra in b_Main.transform)
        {
            BoxCollider bC = c1Tra.gameObject.GetComponent<BoxCollider>();
            // リストに追加
            b_Main_BCList.Add(bC);

            foreach (Transform c2Tra in c1Tra)
            {
                if (c2Tra.gameObject.CompareTag("BridgeColorBox"))
                {
                    MeshRenderer mR = c2Tra.gameObject.GetComponent<MeshRenderer>();
                    // リストに追加
                    b_Main_MRList.Add(mR);
                    // マテリアルの初期化
                    mR.material = mat00;
                }
            }
        }
    }
    public void SwitchONOFF()
    {
        // スイッチの切り替え
        switchBool = !switchBool;

        if(switchBool) // スイッチがONに切り替わったとき
        {
            bHB_ON.SetActive(true);
            bHB_OFF.SetActive(false);
            b_Gate.SetActive(true);

            SwitchONProcess();
        }
        else // スイッチがOFFに切り替わったとき
        {
            bHB_ON.SetActive(false);
            bHB_OFF.SetActive(true);
            b_Gate.SetActive(false);

            SwitchOFFProcess();
        }
    }

    void SwitchONProcess()
    {
        // Materialの変更（色を付ける）
        if (switchNum == 1)
        {
            foreach (MeshRenderer mR in b_Main_MRList)
            {
                mR.material = mat01;
            }
        }
        else if(switchNum == 2)
        {
            foreach (MeshRenderer mR in b_Main_MRList)
            {
                mR.material = mat02;
            }
        }
        else if (switchNum == 3)
        {
            foreach (MeshRenderer mR in b_Main_MRList)
            {
                mR.material = mat03;
            }
        }

        // BoxColliderの切り替え（当たり判定を付ける）
        foreach (BoxCollider bC in b_Main_BCList)
        {
            bC.enabled = true;
        }
    }
    void SwitchOFFProcess()
    {
        // Materialの変更（無色に）
        foreach (MeshRenderer mR in b_Main_MRList)
        {
            mR.material = mat00;
        }

        // BoxColliderの切り替え（当たり判定をなくす）
        foreach (BoxCollider bC in b_Main_BCList)
        {
            bC.enabled = false;
        }
    }
}
