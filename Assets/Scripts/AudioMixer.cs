using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMixer : MonoBehaviour
{
    public UnityEngine.Audio.AudioMixer audioMixer;

    Slider slider_BGM, slider_SFX;

    void Awake()
    {
        slider_BGM = GameObject.FindWithTag("Slider_BGM").GetComponent<Slider>();
        slider_SFX = GameObject.FindWithTag("Slider_SFX").GetComponent<Slider>();
    }

    // SEのVolumeを設定したい時に呼ぶメソッド
    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", ConvertVolumeToDecibel(slider_SFX.value));
    }

    public void ChangeBGMVolume()
    {
        audioMixer.SetFloat("BGMVolume", ConvertVolumeToDecibel(slider_BGM.value));
    }
    
    private float ConvertVolumeToDecibel(float value)// 0 ～ 1 の値をdB単位( -80 ～ 0 )に変換
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1.0f);
        float volumeDB = (float)(20.0d * Mathf.Log10(volume));
        return volumeDB;
    }
}
