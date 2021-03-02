using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // 曲の開始にイントロがあるかどうか
    public bool intro = false;

    AudioSource audioSource_Intro, audioSource_Loop;

    // Start is called before the first frame update
    void Awake()
    {
        if(intro) audioSource_Intro = transform.Find("Music_Intro").GetComponent<AudioSource>();
        audioSource_Loop = transform.Find("Music_Loop").GetComponent<AudioSource>();
    }
    public void PlayMusic()
    {
        if(!intro) audioSource_Loop.Play(); // ループ曲を再生し続ける 
        else PlayWithIntro(); // music01を一回だけ再生し、以降はmusic02をループ再生する
    }
    void PlayWithIntro()
    {
        //イントロ部分の再生開始
        audioSource_Intro.PlayScheduled(AudioSettings.dspTime);

        //イントロ終了後にループ部分の再生を開始
        audioSource_Loop.PlayScheduled(AudioSettings.dspTime + ((float)audioSource_Intro.clip.samples / (float)audioSource_Intro.clip.frequency));
    }
}
