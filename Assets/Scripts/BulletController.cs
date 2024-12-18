using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed;
    public int damage { get; private set; }
    public List<bool> currentLane=new List<bool>();
    private float breakTime = 5f;
    private float destTime = 0.05f;
    private bool isBreak = false;
    [SerializeField]
    private string shotSE="Shot";
    [SerializeField]
    private string hitSE = "Damaged";
    public float BaseY { get; private set; }
    public void InitBullet(float sp,int dm,List<bool> lane)
    {
        speed = sp;
        damage = dm;
        currentLane = new List<bool>(lane);
        int order = 0;
        for (int i=0;i<4;i++)
        {
            if (currentLane[i]) order = i;
        }
        SEManager.Instance.PlaySE(shotSE);
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Lane";
        this.GetComponent<SpriteRenderer>().sortingOrder = order;
        BaseY=GameManager.Instance.player.BaseY;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBreak) return;
        this.gameObject.transform.Translate(Vector2.right * (speed-GameManager.Instance.scrollSpeed) * Time.deltaTime);
        breakTime -= Time.deltaTime;
        if(breakTime <= 0)Destroy(this.gameObject);
    }

    private void AttackEnemy(EnemyController enemy)
    {
        bool isAt = false;
        for (int i = 0; i < 4; i++)if (enemy.isEnemyInLane[i] && currentLane[i]) isAt = true;
        if (!isAt) return;
        enemy.TakeDamage(damage);
        SEManager.Instance.PlaySE(hitSE);
        isBreak = true;
        StartCoroutine(BreakBullet());
    }

    IEnumerator BreakBullet()
    {
        yield return new WaitForSeconds(destTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            AttackEnemy(collision.gameObject.GetComponent<EnemyController>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            AttackEnemy(collision.gameObject.GetComponent<EnemyController>());
        }
    }
}
