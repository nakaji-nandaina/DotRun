using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    public float damage { get; private set; }
    public List<bool> currentLane=new List<bool>();
    private float breakTime = 5f;

    public void InitBullet(float sp,int dm,List<bool> lane)
    {
        speed = sp;
        damage = dm;
        currentLane = lane;
        int order = 0;
        for (int i=0;i<4;i++)
        {
            if (currentLane[i]) order = i;
        }

        this.GetComponent<SpriteRenderer>().sortingLayerName = "Lane";
        this.GetComponent<SpriteRenderer>().sortingOrder = order;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);
        breakTime -= Time.deltaTime;
        if(breakTime <= 0)Destroy(this.gameObject);
        
    }
}
