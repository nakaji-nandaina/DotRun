using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpUI : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    void Update()
    {
        gameObject.transform.position=new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + speed * Time.deltaTime);
    }
    public void InitPos(Vector2 inipos)
    {
        gameObject.transform.position = inipos;
    }
}
