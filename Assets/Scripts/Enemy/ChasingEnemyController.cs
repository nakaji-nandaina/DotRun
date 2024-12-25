using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemyController : EnemyController
{
    [SerializeField]
    private float laneChangeInterval = 2f; // レーン変更の間隔
    private float laneChangeCounter;

    [SerializeField]
    private float laneMoveSpeed = 5f; // レーン変更時の移動速度

    private int targetLane;
    private bool isMovingLane = false;

    protected override void Start()
    {
        base.Start();
        laneChangeCounter = 1;
        targetLane = currentLane;
    }

    protected override void Update()
    {
        if (IsDead)
        {
            base.Update();
            return;
        }

        base.Update();

        laneChangeCounter -= Time.deltaTime;
        if (laneChangeCounter <= 0 && !isMovingLane)UpdateTargetLane();
        

        if (isMovingLane) MoveTowardsTargetLane();
    }


    private void UpdateTargetLane()
    {
        int playerLane = GameManager.Instance.player.currentLane;
        if (playerLane != currentLane)
        {
            targetLane = playerLane<currentLane?currentLane-1:currentLane+1;
            laneChangeCounter = laneChangeInterval;
            isMovingLane = true;
            UpdateLaneFlags(targetLane);
        }
    }

    // レーンのフラグを更新
    private void UpdateLaneFlags(int newLane)
    {
        for (int i = 0; i < isEnemyInLane.Count; i++)
        {
            isEnemyInLane[i] = (i == newLane);
        }
    }

    private void MoveTowardsTargetLane()
    {
        float step = laneMoveSpeed * Time.deltaTime;
        float targetY = enemyPoints[targetLane];
        BaseY = Mathf.MoveTowards(BaseY, targetY, step);

        if (Mathf.Abs(BaseY - targetY) < 0.01f)
        {
            BaseY = targetY;
            currentLane = targetLane;
            isMovingLane = false;
        }
    }
}
