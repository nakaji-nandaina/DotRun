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

    private float damageInterval = 0.1f;
    private float damageConter = 0;
    [SerializeField]
    private float attackInterval = 0.1f;
    private float attackCounter = 0;
    public float heightBuff = 0.2f;

    private float BaseY, JumpY;
    public bool IsDead { get; private set; }

    [SerializeField]
    private float runSpeed=2f;
    [HideInInspector]
    public List<bool> isEnemyInLane { get; private set; }

    [SerializeField]
    private float destroyTime = 10f;
    private float destroyCounter;

    // Start is called before the first frame update
    void Start()
    {
        damageInterval = 0.1f;
        damageConter = 0;
        destroyCounter = destroyTime;
    }

    // Update is called once per frame
    void Update()
    {
        destroyCounter -= Time.deltaTime;
        if (destroyCounter <= 0) Destroy(this.gameObject);
        damageConter -= Time.deltaTime;
        damageConter = Mathf.Max(0, damageConter);
        attackCounter -= Time.deltaTime;
        attackCounter = Mathf.Max(0, attackCounter);
        this.gameObject.transform.Translate(Vector2.left* (GameManager.Instance.scrollSpeed +runSpeed)*Time.deltaTime);
    }

    public void InitEnemy(int raneId)
    {
        isEnemyInLane = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            if (i == raneId) isEnemyInLane.Add(true);
            else isEnemyInLane.Add(false);
        }
        BaseY = this.gameObject.transform.position.y;
        GetComponent<SpriteRenderer>().sortingOrder = raneId;
    }

    public void TakeDamage(int at)
    {
        if (IsDead) return;
        if (damageConter > 0) return;
        damageConter = damageInterval;
        Hp -= at;
        if (Hp <= 0) Dead();

    }
    void Dead()
    {
        IsDead = true;
        Destroy(this.gameObject);
    }

    private void DamagePlayer(PlayerController player)
    {
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
