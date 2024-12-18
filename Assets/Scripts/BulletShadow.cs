using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShadow : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position=new Vector2(gameObject.transform.parent.position.x,gameObject.transform.parent.GetComponent<BulletController>().BaseY-0.4f);
    }
}
