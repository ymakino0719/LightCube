using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessRegulator : MonoBehaviour
{
    // Materialを入れる
    Material myMaterial;

    // Emissionの最小値
    private float minEmission = 1.0f;
    // Emissionの強度
    private float magEmission = 10.0f;
    // 角度
    private float degree = -1;
    // 発光速度
    private float speed = 0.3f;
    // 面の色
    Color[] faceColor = new Color[3];
    // 乱数値（発光色を決める）
    int rand;

    // Use this for initialization
    void Start()
    {
        // オブジェクトにアタッチしているMaterialを取得
        myMaterial = GetComponent<Renderer>().material;

        // オブジェクトの最初の色を設定
        rand = Random.Range(0, 3);
        myMaterial.SetColor("_EmissionColor", faceColor[rand] * minEmission);

        //
        faceColor[0] = new Color32(1, 1, 5, 1);
        faceColor[1] = new Color32(3, 1, 8, 1);
        faceColor[2] = new Color32(1, 3, 8, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (degree >= 0)
        {
            // 光らせる強度を計算する
            Color emissionColor = faceColor[rand] * (minEmission + Mathf.Sin(degree * Mathf.Deg2Rad) * magEmission);

            // エミッションに色を設定する
            myMaterial.SetColor("_EmissionColor", emissionColor);

            //現在の角度を小さくする
            degree -= speed;
        }
    }

    //衝突時に呼ばれる関数
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Star"))
        {
            
            if (degree < 0)
            {
                degree = 180; //角度を180に設定
                rand = Random.Range(0, 3);
            } 
        }
    }
}
