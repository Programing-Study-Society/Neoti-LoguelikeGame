using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill : MonoBehaviour
{
    void Start()
    {
        //1体目の座標指定
        Vector2 position = Vector2.zero;
        //3体出現させる(3回繰り返す)
        for(int i = 0; i < 3; i++)
        {
            //Instantiateでプレハブを複製
            GameObject cod = Instantiate(hand, position, Quaternion.identity);
            //一つ一つ複製したプレハブに名づけ
            cod.name = "cod" + i.ToString();
            //次の複製体のために間隔をあける
            position.x += 2;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit2D.collider != null)
            {
                skill_push(hit2D.collider.gameObject.name);
            }
        }
            
    }

    public void skill_push(int typ)
    {
        
    }

    public void skill_in(int typ)
    {
        switch (typ)
        {
            case 1:
                
                break;
        }
    }
}
