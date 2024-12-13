using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject ShotPoint;
    private List<float> playerPoints;
    public  List<bool> isPlayerInLane{get;private set; }=new List<bool>();
    private int currentLane;
    private int nextLane;
    private List<Shot> shots;
    [SerializeField]
    private Shot DefShot;
    private float moveSpeed;

    private float invincibilityTime=1f;
    private float invincibilityCounter = 0;
    public enum PlayerState
    {
        Run,
        Move,
    }
    public PlayerState ps { get; private set; }

    void Start()
    {
        
        shots = new List<Shot>();
        AddShot(DefShot);
        initLanePos();
        setCurrentLane(0);
        currentLane = 0;
        nextLane = 0;
        moveSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        onClickMoveButton();
        MoveLane();
        Shots();
        invincibilityCounter -= Time.deltaTime;
        invincibilityCounter = Mathf.Max(0, invincibilityCounter);
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

    private void setCurrentLane(int id)
    {
        for(int i = 0; i < 4; i++)isPlayerInLane[i] = false;
        isPlayerInLane[id] = true;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,playerPoints[id],this.gameObject.transform.position.z);
        currentLane = id;
    }
    
    private void onClickMoveButton()
    {
        if (ps != PlayerState.Run) return;
        if (Input.GetKeyDown("s"))
        {
            if (currentLane == 3) return;
            nextLane = currentLane + 1;
            ChangePS(PlayerState.Move);
            return;
        }
        if (Input.GetKeyDown("w"))
        {
            if (currentLane == 0) return;
            nextLane = currentLane - 1;
            ChangePS(PlayerState.Move);
        }
    }
    private void MoveLane()
    {
        if (currentLane == nextLane|| ps != PlayerState.Move) return;
        if ((currentLane<nextLane&&(this.gameObject.transform.position.y-playerPoints[nextLane]) <= 0.01f)|| (currentLane > nextLane && (this.gameObject.transform.position.y- playerPoints[nextLane]) >= -0.01f))
        {
            setCurrentLane(nextLane);
            ChangePS(PlayerState.Run);
            return;
        }
        if (currentLane < nextLane)this.gameObject.transform.Translate(Vector3.down * Time.deltaTime*moveSpeed);
        else this.gameObject.transform.Translate(Vector3.up * Time.deltaTime*moveSpeed);

    }
    public void TakeDamage(int at)
    {
        if(invincibilityCounter > 0) return;
        invincibilityCounter = invincibilityTime;
        GameManager.Instance.OnPlayerDamage();
    }
}
