using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject ShotPoint;
    private List<float> playerPoints;
    public float BaseY { get; private set; }
    private float JumpHeight,Gravity,JumpVelocity;
    public float JumpY { get; private set; }
    public bool IsJump { get; private set; }



    public  List<bool> isPlayerInLane{get;private set; }=new List<bool>();
    private int currentLane;
    private int nextLane;
    private List<Shot> shots;
    [SerializeField]
    private Shot DefShot;
    private float moveSpeed;

    private float invincibilityTime=1f;
    private float invincibilityCounter = 0;
    private float blinkFre = 0.1f;
    private float blinkCounter = 0;

    public int HP { get; private set; }

    public enum PlayerState
    {
        Run,
        Move,
    }
    public PlayerState ps { get; private set; }

    

    void Start()
    {
        ChangePS(PlayerState.Run);
        shots = new List<Shot>();
        AddShot(DefShot);
        initLanePos();
        setCurrentLane(0);
        currentLane = 0;
        nextLane = 0;
        moveSpeed = 5f;
        JumpY = 0;
        JumpHeight = 1.5f;
        Gravity = -9.8f;
        IsJump = false;

        //Status
        HP = 100;
    }

    // Update is called once per frame
    void Update()
    {
        onClickMoveButton();
        MoveLane();
        Shots();
        Jump();
        Invincible();
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, JumpY + BaseY);
    }
    private void Invincible()
    {
        invincibilityCounter -= Time.deltaTime;
        invincibilityCounter = Mathf.Max(0, invincibilityCounter);
        if (invincibilityCounter > 0)
        {
            blinkCounter += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Sin(blinkCounter * Mathf.PI / blinkFre));
        }
        else
        {
            blinkCounter = 0;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,1);
        }
    }
    private void Jump()
    {
        if (!IsJump&& Input.GetKeyDown(KeyCode.Space))
        {
            JumpVelocity=  Mathf.Sqrt(-2f * Gravity * JumpHeight);
            IsJump = true;
            Debug.Log("Jump");
        }
        if (!IsJump) return;
        JumpY += JumpVelocity * Time.deltaTime; // 速度に基づいて位置を更新
        JumpVelocity += Gravity * Time.deltaTime; // 重力を加える
        if (JumpY <= 0)
        {
            JumpY = 0;
            IsJump = false;
        }
    }

    public void AddShot(Shot shot)
    {
        if(shots.Contains(shot)) return;
        if (shots.Count >= 6) return;
        shot.shotTimer =0;
        shots.Add(shot);
    }

    private void Shots()
    {
        foreach(Shot shot in shots)
        {
            shot.shotTimer -= Time.deltaTime;
            if (shot.shotTimer <= 0)
            {
                shot.shotTimer = shot.shotInterval;
                GameObject bullet= Instantiate(shot.shotPrefab, ShotPoint.transform.position, Quaternion.identity);
                bullet.GetComponent<BulletController>().InitBullet(shot.shotSpeed, shot.shotDamage,isPlayerInLane);
            }
        }
    }

    public void ChangePS(PlayerState nextstate)
    {
        switch (nextstate)
        {
            case PlayerState.Run:
                ps = nextstate;
                break;
            case PlayerState.Move:
                ps = nextstate;
                break;
        }
    }
    

    private void initLanePos()
    {
        playerPoints = new List<float>();
        for(int i = 0; i < 4; i++)playerPoints.Add(-(i + 0.2f));
        
        isPlayerInLane = new List<bool>();
        for(int i = 0; i < 4; i++)isPlayerInLane.Add(false);
        
    }

    private void SetLanes(int id)
    {
        for (int i = 0; i < 4; i++) isPlayerInLane[i] = false;
        isPlayerInLane[id] = true;
    }
    private void setCurrentLane(int id)
    {
        SetLanes(id);
        BaseY = playerPoints[id];
        currentLane = id;
        GetComponent<SpriteRenderer>().sortingOrder = id;
    }
    
    private void onClickMoveButton()
    {
        if (ps != PlayerState.Run) return;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentLane == 3) return;
            nextLane = currentLane + 1;
            ChangePS(PlayerState.Move);
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentLane == 0) return;
            nextLane = currentLane - 1;
            ChangePS(PlayerState.Move);
        }
    }
    private void MoveLane()
    {
        if (currentLane == nextLane|| ps != PlayerState.Move) return;
        if ((currentLane<nextLane&&(BaseY-playerPoints[nextLane]) <= 0.01f)|| (currentLane > nextLane && (BaseY- playerPoints[nextLane]) >= -0.01f))
        {
            setCurrentLane(nextLane);
            ChangePS(PlayerState.Run);
            return;
        }
        if (currentLane < nextLane) BaseY -= Time.deltaTime * moveSpeed;
        else BaseY += Time.deltaTime * moveSpeed;
        if (Mathf.Abs(playerPoints[nextLane] - BaseY) < Mathf.Abs(playerPoints[currentLane] - BaseY)) SetLanes(nextLane);
        else SetLanes(currentLane);
        
    }
    public void TakeDamage(int at)
    {
        if(invincibilityCounter > 0) return;
        invincibilityCounter = invincibilityTime;
        GameManager.Instance.PlayDamageUI(at);
        HP -= at;
        HP = Mathf.Max(0, HP);
        GameManager.Instance.OnPlayerDamage();
        
    }
}
