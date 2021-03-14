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
    // 面の色の配列
    Color[] faceColor = new Color[3];
    // 各面の色の指定
    public Color32 color01 = new Color32(1, 1, 5, 1);
    public Color32 color02 = new Color32(3, 1, 8, 1);
    public Color32 color03 = new Color32(1, 3, 8, 1);
    // 乱数値（発光色を決める）
    int rand;

    // Use this for initialization
    void Start()
    {
        // オブジェクトにアタッチしているMaterialを取得
        myMaterial = GetComponent<Renderer>().material;

        // faceColorの設定
        faceColor[0] = color01;
        faceColor[1] = color02;
        faceColor[2] = color03;

        // オブジェクトの最初の色を設定
        rand = Random.Range(0, 3);
        myMaterial.SetColor("_EmissionColor", faceColor[rand] * minEmission);
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
