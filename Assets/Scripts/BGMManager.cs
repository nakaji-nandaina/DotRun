using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // シングルトンとして利用

    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();

    private AudioSource audioSource; // BGM再生用のAudioSource

    [System.Serializable]
    public class BGMData
    {
        public string key; // BGMのキー
        public AudioClip clip; // BGMクリップ
    }

    public List<BGMData> bgmList = new List<BGMData>();

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            if (gameObject.GetComponent<AudioSource>()) audioSource = gameObject.AddComponent<AudioSource>();
            else audioSource = gameObject.GetComponent<AudioSource>();
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも保持
        }
        else
        {
            Destroy(gameObject);
        }

        // 辞書にデータを登録
        foreach (var bgm in bgmList)
        {
            if (!bgmDictionary.ContainsKey(bgm.key))
            {
                bgmDictionary.Add(bgm.key, bgm.clip);
            }
        }
    }
    
    
    public void PlayBGM(string key, float fadeDuration = 1.0f)
    {
        if (bgmDictionary.TryGetValue(key, out AudioClip clip))
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn(clip, fadeDuration));
        }
        else
        {
            Debug.LogWarning($"指定されたキー[{key}]に対応するBGMが見つかりません。");
        }
    }

    
    public void StopBGM(float fadeDuration = 1.0f)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeIn(AudioClip clip, float duration)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.volume = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = audioSource.volume;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f; // 次の再生に備えて元の音量に戻す
    }
}