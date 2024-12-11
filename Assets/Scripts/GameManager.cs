using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scrolling Settings")]
    public float originalScrollSpeed = 5f;          // 通常のスクロール速度
    [HideInInspector]
    public float scrollSpeed { get; private set; }      // 現在のスクロール速度
    [SerializeField]
    private float damageScrollSpeed = 2f;
    public float speedChangeDuration = 2f;  // 元の速度に戻るまでの時間


    [Header("Background Settings")]
    public GameObject stagegroundPrefab;     // 背景のプレハブ
    public GameObject backgroundPrefab;
    [HideInInspector]
    public float stagegroundWidth { get; private set; } // ステージの幅
    [HideInInspector]
    public float backgroundWidth {get; private set;}     // 背景の幅

    public GameObject Grid;
    private GameObject parentGrid;
    private List<GameObject> stagegrounds = new List<GameObject>();
    private List<GameObject> backgrounds = new List<GameObject>();


    private void Awake()
    {
        // シングルトンの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //背景サイズの計算
        CalculateGroundsWidth();
        scrollSpeed = originalScrollSpeed;

        // 最初の背景を配置
        SpawnInitialGrounds();
    }

    private void Update()
    {
        
        // 背景をスクロール
        Scrollgrounds();
        //スクロールスピードの上昇
        UpScrollSpeed();

    }
    private void CalculateGroundsWidth()
    {
        //StageGroundの設定
        Tilemap tilemap = stagegroundPrefab.GetComponent<Tilemap>();
        float size = tilemap.size.x;
        float tileSize = tilemap.cellSize.x;
        stagegroundWidth = size * tileSize;
        stagegroundWidth /= 2;
        //BackGroundの設定
        tilemap = backgroundPrefab.GetComponent<Tilemap>();
        size = tilemap.size.x;
        tileSize = tilemap.cellSize.x;
        backgroundWidth = size * tileSize;
        //backgroundWidth /= 2;
    }

    private void SpawnInitialGrounds()
    {
        parentGrid = Instantiate(Grid, Vector3.zero, Quaternion.identity);
        GameObject firstSt = Instantiate(stagegroundPrefab, Vector3.zero, Quaternion.identity);
        firstSt.transform.SetParent(parentGrid.transform);
        stagegrounds.Add(firstSt);
        float lastX = firstSt.transform.position.x;

        for (int i = 1; i < 3; i++)
        {
            Vector3 newPos = new Vector3(lastX + stagegroundWidth, 0, 0);
            firstSt = Instantiate(stagegroundPrefab, newPos, Quaternion.identity);
            firstSt.transform.SetParent(parentGrid.transform);
            stagegrounds.Add(firstSt);
            lastX = firstSt.transform.position.x;
        }

        GameObject firstBg = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity);
        firstBg.transform.SetParent(parentGrid.transform);
        backgrounds.Add(firstBg);
        lastX = firstBg.transform.position.x+backgroundWidth;
        for (int i = 1; i < 3; i++)
        {
            Vector3 newPos = new Vector3(lastX, 0, 0);
            firstBg = Instantiate(backgroundPrefab, newPos, Quaternion.identity);
            firstBg.transform.SetParent(parentGrid.transform);
            backgrounds.Add(firstBg);
            lastX += backgroundWidth;
        }


    }

    private void Scrollgrounds()
    {
        foreach (GameObject bg in stagegrounds)
        {
            bg.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        }
        foreach (GameObject bg in backgrounds)
        {
            bg.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        }


        GameObject firstStToRemove = stagegrounds[0];
        if (firstStToRemove.transform.position.x + stagegroundWidth < Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect))
        {
            stagegrounds.RemoveAt(0);
            Destroy(firstStToRemove);
            SpawnStageground();
        }
        GameObject firstBgToRemove = backgrounds[0];
        if (firstBgToRemove.transform.position.x + backgroundWidth < Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect))
        {
            backgrounds.RemoveAt(0);
            Destroy(firstBgToRemove);
            SpawnBackground();
        }

    }
    private void UpScrollSpeed()
    {
        // スクロール速度を徐々に元に戻す
        if (scrollSpeed >= originalScrollSpeed) return;
        scrollSpeed += (originalScrollSpeed / speedChangeDuration) * Time.deltaTime;
        if (scrollSpeed > originalScrollSpeed) scrollSpeed = originalScrollSpeed;
    }

    private void SpawnStageground()
    {
        Vector3 newPos = new Vector3(2* stagegroundWidth, 0, 0);
        GameObject newBg = Instantiate(stagegroundPrefab, newPos, Quaternion.identity);
        newBg.transform.SetParent(parentGrid.transform);
        stagegrounds.Add(newBg);
    }
    private void SpawnBackground()
    {
        Vector3 newPos = new Vector3(2.8f * stagegroundWidth, 0, 0);
        GameObject newBg = Instantiate(backgroundPrefab, newPos, Quaternion.identity);
        newBg.transform.SetParent(parentGrid.transform);
        backgrounds.Add(newBg);
        Debug.Log("BGAdd");
    }
    public void OnPlayerDamage()
    {
        scrollSpeed = damageScrollSpeed;
    }
}
    
