using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // inspector上でセットする曲のリスト
    public AudioClip[] musicList = new AudioClip[4];

    // 曲の合計本数
    int totalNum = 0;

    // 現在流している曲の番号
    int musicNum = 0;

    // 前の曲が終わってから次の曲を流し始めるまでの時間
    float interval = 10.0f;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        // 曲の合計本数を代入
        totalNum = musicList.Length;
        // 先頭の曲をセットする
        audioSource.clip = musicList[0];
    }
    public void PlayMusic()
    {
        StartCoroutine("SwitchToNextMusicCoroutine");
    }
    IEnumerator SwitchToNextMusicCoroutine()
    {
        // 曲を流す
        audioSource.Play();

        // セットされた曲の再生が終わるまで待つ
        yield return new WaitForSeconds(audioSource.clip.length);

        // 更に次の曲を流すまでの間隔を設ける
        yield return new WaitForSeconds(interval);

        // 次の曲をセットする
        musicNum = (musicNum < totalNum - 1) ? musicNum + 1 : 0;
        audioSource.clip = musicList[musicNum];

        // 次の曲を流す
        PlayMusic();
    }
}
