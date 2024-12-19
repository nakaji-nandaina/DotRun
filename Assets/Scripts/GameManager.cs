using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scrolling Settings")]
    public float originalScrollSpeed = 6f;          // 通常のスクロール速度
    [HideInInspector]
    public float scrollSpeed { get; private set; }      // 現在のスクロール速度
    [SerializeField]
    private float damageScrollSpeed = 1f;
    public float speedChangeDuration = 4f;  // 元の速度に戻るまでの時間


    [Header("Background Settings")]
    public GameObject stagegroundPrefab;     // 背景のプレハブ
    public GameObject backgroundPrefab;

    [Header("UI")]
    public Slider HPSlider;
    public Canvas WorldCanvas;
    public GameObject DamageUI;
    public Text ScoreText;

    [Header("Enemys")]
    public List<GameObject> enemyPrefabs;

    private float spawnIntervalMax = 3f;
    private float spawnIntervalMin = 0.5f;
    private float spawnCounter = 0;

    [HideInInspector]
    public float stagegroundWidth { get; private set; } // ステージの幅
    [HideInInspector]
    public float backgroundWidth {get; private set;}     // 背景の幅

    public GameObject Grid;
    private GameObject parentGrid;
    private List<GameObject> stagegrounds = new List<GameObject>();
    private List<GameObject> backgrounds = new List<GameObject>();

    [HideInInspector]
    public PlayerController player;

    public float Score { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
        scrollSpeed = damageScrollSpeed;
        BGMManager.Instance.PlayBGM("Stage1");
        // 最初の背景を配置
        SpawnInitialGrounds();
    }

    private void Update()
    {
        
        // 背景をスクロール
        Scrollgrounds();
        //スクロールスピードの上昇
        UpScrollSpeed();
        //敵の生成
        SpawnEnemy();
        Score += Time.deltaTime * scrollSpeed;
        ScoreText.text = ((int)Score).ToString()+"m";
    }
    private void SpawnEnemy()
    {

        spawnCounter -= Time.deltaTime;
        if (spawnCounter > 0)return;
        spawnCounter = Random.Range(spawnIntervalMin, spawnIntervalMax);
        //ランダムで敵を生成
        int enemyIndex = Random.Range(0, enemyPrefabs.Count);
        int spawnLane = Random.Range(0, 4);
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex],new Vector2(12,-spawnLane- enemyPrefabs[enemyIndex].GetComponent<EnemyController>().heightBuff), Quaternion.identity);
        enemy.GetComponent<SpriteRenderer>().sortingOrder = spawnLane;
        enemy.GetComponent<EnemyController>().InitEnemy(spawnLane);
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
        scrollSpeed += ((originalScrollSpeed-damageScrollSpeed)/ speedChangeDuration) * Time.deltaTime;
        if (scrollSpeed > originalScrollSpeed) scrollSpeed = originalScrollSpeed;
    }

    private void SpawnStageground()
    {
        Vector3 newPos = new Vector3(stagegrounds[stagegrounds.Count - 1].transform.position.x + stagegroundWidth, 0, 0);
        GameObject newBg = Instantiate(stagegroundPrefab, newPos, Quaternion.identity);
        newBg.transform.SetParent(parentGrid.transform);
        stagegrounds.Add(newBg);
    }
    private void SpawnBackground()
    {
        Vector3 newPos = new Vector3(backgrounds[backgrounds.Count-1].transform.position.x+ backgroundWidth, 0, 0);
        GameObject newBg = Instantiate(backgroundPrefab, newPos, Quaternion.identity);
        newBg.transform.SetParent(parentGrid.transform);
        backgrounds.Add(newBg);
        //Debug.Log("BGAdd");
    }
    public void OnPlayerDamage()
    {
        scrollSpeed = damageScrollSpeed;
        HPSlider.value = player.HP;
    }
    public void PlayDamageUI(int damage)
    {
        GameObject damageUI = Instantiate(DamageUI);
        damageUI.transform.SetParent(WorldCanvas.transform);
        damageUI.GetComponent<UpUI>().InitPos(new Vector2(player.transform.position.x,player.transform.position.y+0.4f));
        damageUI.GetComponent<Text>().text = damage.ToString();
        Destroy(damageUI, 0.5f);
    }
}
    
