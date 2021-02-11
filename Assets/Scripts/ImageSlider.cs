using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{
    private Image image;

    public Sprite slide00;
    public Sprite slide01;
    public Sprite slide02;
    public Sprite slide03;

    private int slideNum = 0;
    private float time = 0.0f;
    private float changeTime = 0.4f; 

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        if(time > changeTime)
        {
            if(slideNum != 3) { slideNum++; }
            else { slideNum = 1; } // 0は使わない

            if (slideNum == 0) { image.sprite = slide00; } // 0は使わない
            else if (slideNum == 1) { image.sprite = slide01; }
            else if (slideNum == 2) { image.sprite = slide02; }
            else if (slideNum == 3) { image.sprite = slide03; }

            time = 0.0f;
        }
    }
}
