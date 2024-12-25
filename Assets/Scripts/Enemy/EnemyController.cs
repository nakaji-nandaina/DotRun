using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private int Hp = 100;

    [SerializeField]
    private int At=10;

    private float damageInterval = 0.5f;
    private float damageCounter = 0;

    private float InvincibleTime = 0.1f;
    private float InvincibleCounter = 0f;

    [SerializeField]
    private float knockBackTime = 0.5f;
    private float knockBackCounter = 0f;
    private float knockBackFource=10f;
    private float knockBackHeight = 0.8f;
    [SerializeField]
    private float DeadTime = 1.2f;
    private float DeadCounter = 0f;

    private float blinkCounter = 0;
    private float blinkFre = 0.12f;

    [SerializeField]
    private float attackInterval = 0.1f;
    private float attackCounter = 0;
    public float heightBuff = 0.2f;
    
    protected float BaseY, JumpY,KnockY;

    protected int currentLane;
    public bool IsDead { get; private set; }

    [SerializeField]
    private float runSpeed=2f;
    [HideInInspector]
    public List<bool> isEnemyInLane { get; protected set; }

    protected List<float> enemyPoints;

    [SerializeField]
    private float destroyTime = 10f;
    private float destroyCounter;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InvincibleCounter = 0;
        destroyCounter = destroyTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsDead)
        {
            DeadCounter -= Time.deltaTime;
            DeadCounter = Mathf.Max(0, DeadCounter);
            blinkCounter += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Sin(blinkCounter * Mathf.PI / blinkFre)*0.7f+0.3f);
            KnockY =Mathf.Sin(Mathf.PI * DeadCounter / DeadTime) * knockBackHeight*1.2f;
            if (DeadCounter > 0) this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x - (GameManager.Instance.scrollSpeed - knockBackFource*1.2f) * Time.deltaTime, BaseY + KnockY);
            else Destroy(gameObject);
            return;
        }
        destroyCounter -= Time.deltaTime;
        if (destroyCounter <= 0) Destroy(this.gameObject);
        damageCounter -= Time.deltaTime;
        damageCounter = Mathf.Max(0, damageCounter);
        attackCounter -= Time.deltaTime;
        attackCounter = Mathf.Max(0, attackCounter);
        InvincibleCounter -= Time.deltaTime;
        InvincibleCounter = Mathf.Max(0, InvincibleCounter);
        knockBackCounter -= Time.deltaTime;
        knockBackCounter = Mathf.Max(0, knockBackCounter);
        KnockY = Mathf.Sin(Mathf.PI * knockBackCounter/knockBackTime)*knockBackHeight;
        if (knockBackCounter > 0) this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x - (GameManager.Instance.scrollSpeed - knockBackFource) * Time.deltaTime, BaseY + KnockY);
        else this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x - (GameManager.Instance.scrollSpeed + runSpeed) * Time.deltaTime, BaseY);
        if (damageCounter > 0)
        {
            blinkCounter += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Sin(blinkCounter * Mathf.PI / blinkFre)*0.7f+0.3f);
        }
        else
        {
            blinkCounter = 0;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,1);
        }
    }
    private void InitLanes()
    {
        enemyPoints = new List<float>();
        for (int i = 0; i < 4; i++)
        {
            enemyPoints.Add(-i-heightBuff);
        }
    }
    public void InitEnemy(int raneId)
    {
        isEnemyInLane = new List<bool>();
        InitLanes();
        for (int i = 0; i < 4; i++)
        {
            if (i == raneId) isEnemyInLane.Add(true);
            else isEnemyInLane.Add(false);
        }
        currentLane = raneId;
        BaseY = enemyPoints[currentLane];
        this.gameObject.transform.position = new Vector2(this.gameObject.transform.position.x, BaseY);
        GetComponent<SpriteRenderer>().sortingOrder = raneId;
    }

    public void TakeDamage(int at)
    {
        if (IsDead) return;
        if (InvincibleCounter > 0) return;
        knockBackCounter = knockBackTime;
        damageCounter = damageInterval;
        InvincibleCounter = InvincibleTime;
        Hp -= at;
        if (Hp <= 0) Dead();

    }
    void Dead()
    {
        if (IsDead) return;
        IsDead = true;
        DeadCounter = DeadTime;
    }

    private void DamagePlayer(PlayerController player)
    {
        if (IsDead) return;
        if (attackCounter > 0) return;
        bool isAt = false;
        for (int i = 0; i < 4; i++) if (player.isPlayerInLane[i] && isEnemyInLane[i])isAt = true;
        if (player.JumpY > GetComponent<BoxCollider2D>().offset.y+GetComponent<BoxCollider2D>().size.y/2) isAt = false;
        Debug.Log("Jump"+player.JumpY.ToString());
        Debug.Log("Enemy"+(GetComponent<BoxCollider2D>().offset.y + GetComponent<BoxCollider2D>().size.y / 2).ToString() );
        if (!isAt) return;
        player.TakeDamage(At);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") DamagePlayer(collision.gameObject.GetComponent<PlayerController>());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") DamagePlayer(collision.gameObject.GetComponent<PlayerController>());
        
    }
}
