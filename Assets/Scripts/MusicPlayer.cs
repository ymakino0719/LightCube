using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // inspector上でセットする曲のリスト
    public AudioClip[] musicList = new AudioClip[4];

    // inspector上でセットするジングルのリスト
    public AudioClip[] jingleList = new AudioClip[1];

    // 曲の合計本数
    int totalNum = 0;

    // 現在流している曲の番号
    int musicNum = 0;

    // 開幕の曲を流し始めるまでの時間
    float initial = 2.0f;
    // 前の曲が終わってから次の曲を流し始めるまでの時間
    float interval = 10.0f;

    AudioSource audioSource;

    // 初期の音量
    float initialVol;
    // 音量の低減率
    float reduction = 0.015f;
    // 音量を徐々に小さくする（※0まで）
    bool fadeOutV;
    // 最低音量（※Paused画面中など）
    float minVol;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        initialVol = audioSource.volume;
    }
    void Start()
    {
        // 曲の合計本数を代入
        totalNum = musicList.Length;
        // 先頭の曲をセットする
        audioSource.clip = musicList[0];
        // 最低音量の代入
        minVol = initialVol / 3.0f;
    }
    void FixedUpdate()
    {
        if (fadeOutV) FadeOutMusicVolume();
    }
    public void InitialPlayMusic()
    {
        StartCoroutine("InitialPlayMusicCoroutine");
    }
    IEnumerator InitialPlayMusicCoroutine()
    {
        // 開幕の曲を流すまでの間隔を設ける
        yield return new WaitForSeconds(initial);

        PlayMusic();
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
    void FadeOutMusicVolume()
    {
        float vol = audioSource.volume - Time.deltaTime * reduction;
        if (vol > 0) audioSource.volume = vol;
        else
        {
            audioSource.volume = 0;
            fadeOutV = false;
        }
    }
    public void TurnDownMusicVolume()
    {
        audioSource.volume = minVol;
    }
    public void RestoreMusicVolume()
    {
        audioSource.volume = initialVol;
    }
    public void PlayJingle(int num)
    {
        audioSource.clip = jingleList[num];
        // 音量を元に戻す
        RestoreMusicVolume();
        // ジングルを流す
        audioSource.Play();
    }
    public bool FadeOutV
    {
        set { fadeOutV = value; }
        get { return fadeOutV; }
    }
}
