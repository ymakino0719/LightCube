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
    private float magEmission = 15.0f;
    // 角度
    private int degree = 0;
    //発光速度
    private int speed = 1;
    // ターゲットのデフォルトの色
    [ColorUsage(false, true)] public Color defaultColor;

    // Use this for initialization
    void Start()
    {
        //オブジェクトにアタッチしているMaterialを取得
        myMaterial = GetComponent<Renderer>().material;

        //オブジェクトの最初の色を設定
        myMaterial.SetColor("_EmissionColor", defaultColor * minEmission);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (degree >= 0)
        {
            // 光らせる強度を計算する
            Color emissionColor = defaultColor * (minEmission + Mathf.Sin(degree * Mathf.Deg2Rad) * magEmission);

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
            //角度を180に設定
            degree = 180;
        }
    }
}
