using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    // オーディオソース
    AudioSource audioSource;
    // inspector上でセットする効果音のリスト
    public AudioClip[] sfxList = new AudioClip[1];
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySFX(int num)
    {
        // 効果音を流す
        audioSource.PlayOneShot(sfxList[num]);
    }
}
