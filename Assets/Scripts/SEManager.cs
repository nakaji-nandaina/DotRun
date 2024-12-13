using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance; // シングルトンとして利用
    private Dictionary<string, AudioClip> seDictionary = new Dictionary<string, AudioClip>();

    private AudioSource audioSource;

    [System.Serializable]
    public class SEData
    {
        public string key; // 効果音のキー
        public AudioClip clip; // 効果音クリップ
    }

    public List<SEData> seList = new List<SEData>();

    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも保持
        }
        else
        {
            Destroy(gameObject);
        }

        // 辞書にデータを登録
        foreach (SEData se in seList)
        {
            if (!seDictionary.ContainsKey(se.key))
            {
                seDictionary.Add(se.key, se.clip);
            }
        }
    }
    private void Start()
    {
        if (gameObject.GetComponent<AudioSource>()) audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 指定したキーの効果音を再生
    /// </summary>
    /// <param name="key">再生する効果音のキー</param>
    public void PlaySE(string key)
    {
        if (seDictionary.TryGetValue(key, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"指定されたキー[{key}]に対応する効果音が見つかりません。");
        }
    }
}
