using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{

    private List<float> playerPoints;
    private  List<bool> isPlayerInLane;
    private int currentLane;
    private int nextLane;

    private float moveSpeed;
    // Start is called before the first frame update

    public enum PlayerState
    {
        Run,
        Move,
    }
    public PlayerState ps { get; private set; }

    void Start()
    {
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
        for(int i = 0; i < 4; i++)
        {
            playerPoints.Add(-(i + 0.2f));
        }
        isPlayerInLane = new List<bool>();
        for(int i = 0; i < 4; i++)
        {
            isPlayerInLane.Add(false);
        }
    }

    private void setCurrentLane(int id)
    {
        for(int i = 0; i < 4; i++)
        {
            isPlayerInLane[i] = false;
        }
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
}
