using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // �V���O���g���Ƃ��ė��p

    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();

    private AudioSource audioSource; // BGM�Đ��p��AudioSource

    [System.Serializable]
    public class BGMData
    {
        public string key; // BGM�̃L�[
        public AudioClip clip; // BGM�N���b�v
    }

    public List<BGMData> bgmList = new List<BGMData>();

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // �V���O���g���̐ݒ�
        if (Instance == null)
        {
            if (gameObject.GetComponent<AudioSource>()) audioSource = gameObject.AddComponent<AudioSource>();
            else audioSource = gameObject.GetComponent<AudioSource>();
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ł��ێ�
        }
        else
        {
            Destroy(gameObject);
        }

        // �����Ƀf�[�^��o�^
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
            Debug.LogWarning($"�w�肳�ꂽ�L�[[{key}]�ɑΉ�����BGM��������܂���B");
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
        audioSource.volume = 1f; // ���̍Đ��ɔ����Č��̉��ʂɖ߂�
    }
}