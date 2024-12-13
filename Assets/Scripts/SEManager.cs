using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance; // �V���O���g���Ƃ��ė��p
    private Dictionary<string, AudioClip> seDictionary = new Dictionary<string, AudioClip>();

    private AudioSource audioSource;

    [System.Serializable]
    public class SEData
    {
        public string key; // ���ʉ��̃L�[
        public AudioClip clip; // ���ʉ��N���b�v
    }

    public List<SEData> seList = new List<SEData>();

    private void Awake()
    {
        // �V���O���g���̐ݒ�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ł��ێ�
        }
        else
        {
            Destroy(gameObject);
        }

        // �����Ƀf�[�^��o�^
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
    /// �w�肵���L�[�̌��ʉ����Đ�
    /// </summary>
    /// <param name="key">�Đ�������ʉ��̃L�[</param>
    public void PlaySE(string key)
    {
        if (seDictionary.TryGetValue(key, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"�w�肳�ꂽ�L�[[{key}]�ɑΉ�������ʉ���������܂���B");
        }
    }
}
