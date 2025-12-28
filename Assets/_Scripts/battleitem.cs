using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleitem : MonoBehaviour
{
    public GameObject hand;
    public item item_L;
    public player player;
    public void use_item(int item_id,int item_class,List<string> object_names)
    {
        //以下のプログラムはオブジェクトからステータス(MAXHPなど)を取得する。
        //オブジェクト名を指定する
        foreach(string object_name in object_names)
        {
     
            GameObject targetObject = GameObject.Find(object_name);
            if (targetObject != null)//オブジェクトが存在するか確認
            {
                //オブジェクトからスクリプトを指定する。
                enemy_L target = targetObject.GetComponent<enemy_L>();
                if (target != null)//スクリプトが存在するか確認する
                {
                    //値を使用
                    if(item_L.item_list[item_id][item_class]>0){
                        switch (item_id)
                        {
                            case 0:
                            //リペアユニット
                                switch (item_class)
                                {
                                    case 0:
                                    player.HP += (int)(player.MAXHP*Random.Range(0.1f, 0.3f));
                                    break;
                                    case 1:
                                    player.HP += (int)(player.MAXHP*Random.Range(0.4f, 0.7f));
                                    break;
                                    case 2:
                                    player.HP += (int)(player.MAXHP*Random.Range(0.8f, 1f));
                                    break;
                                }
                                break;
                            case 1:
                            //ガトリングガン
                                switch (item_class)
                                {
                                    case 0:
                                    for(int i = 0;i<5;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.05f, 0.35f));
                                    }
                                    break;
                                    case 1:
                                    for(int i = 0;i<5;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.25f, 0.55f));
                                    }
                                    break;
                                    case 2:
                                    for(int i = 0;i<5;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.45f, 0.85f));
                                    }
                                    break;
                                }
                                break;
                            case 2:
                            //重装ランチャー
                                switch (item_class)
                                {
                                    case 0:
                                    target.HP -= (int)(player.ATK*Random.Range(1.5f, 1.8f));
                                    break;
                                    case 1:
                                    target.HP -= (int)(player.ATK*Random.Range(2.0f, 2.3f));
                                    break;
                                    case 2:
                                    target.HP -= (int)(player.ATK*Random.Range(3.5f, 3.8f));
                                    break;
                                }
                                break;
                            case 3:
                            //火炎放射器
                                switch (item_class)
                                {
                                    case 0:
                                    for(int i = 0;i<3;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.02f, 0.12f));
                                    }
                                    if(Random.Range(1,100)>15)
                                    {
                                        target.BUFF[0] = 1;
                                    }
                                    break;
                                    case 1:
                                    for(int i = 0;i<3;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.13f, 0.23f));
                                    }
                                    if(Random.Range(1,100)>45)
                                    {
                                        target.BUFF[0] = 2;
                                    }
                                    break;
                                    case 2:
                                    for(int i = 0;i<3;i++)
                                    {
                                        target.HP -= (int)(player.ATK*Random.Range(0.34f, 0.56f));
                                    }
                                    if(Random.Range(1,100)>75)
                                    {
                                        target.BUFF[0] = 3;
                                    }
                                    break;
                                }
                                break;
                            case 4:
                            //サイバーハックモジュール
                                switch (item_class)
                                {
                                    case 0:
                                    target.HP -= (int)(player.ATK*Random.Range(0.1f, 0.3f));
                                    target.BUFF[1] = 1;
                                    target.ATK = (int)(target.ATK * 0.1);
                                    break;
                                    case 1:
                                    target.HP -= (int)(player.ATK*Random.Range(0.3f, 0.6f));
                                    target.BUFF[1] = 2;
                                    target.ATK = (int)(target.ATK * 0.3);
                                    break;
                                    case 2:
                                    target.HP -= (int)(player.ATK*Random.Range(1.2f, 2.2f));
                                    target.BUFF[1] = 3;
                                    target.ATK = (int)(target.ATK * 0.5);
                                    break;
                                }
                                break;
                            case 5:
                            //オーバークロックモジュール
                                switch (item_class)
                                {
                                    case 0:
                                    player.BUFF[2] = 1;
                                    player.ATK = (int)(player.ATK * 1.25);
                                    break;
                                    case 1:
                                    player.BUFF[2] = 2;
                                    player.ATK = (int)(player.ATK * 1.75);
                                    break;
                                    case 2:
                                    player.BUFF[2] = 3;
                                    player.ATK = (int)(player.ATK * 2.5);
                                    break;
                                }
                                break;
                            case 6:
                            //スタンガン
                                switch (item_class)
                                {
                                    case 0:
                                    target.HP -= (int)(player.DEF*Random.Range(0.1f, 0.3f));
                                    target.BUFF[3] = 1;
                                    target.DEF = (int)(target.DEF * 0.1);
                                    break;
                                    case 1:
                                    target.HP -= (int)(player.DEF*Random.Range(0.3f, 0.6f));
                                    target.BUFF[3] = 2;
                                    target.DEF = (int)(target.DEF * 0.3);
                                    break;
                                    case 2:
                                    target.HP -= (int)(player.DEF*Random.Range(1.2f, 2.2f));
                                    target.BUFF[3] = 3;
                                    target.DEF = (int)(target.DEF * 0.5);
                                    break;
                                }
                                break;
                            case 7:
                            //フィールドシールド
                                switch (item_class)
                                {
                                    case 0:
                                    player.BUFF[4] = 1;
                                    player.DEF = (int)(player.DEF * 1.25);
                                    break;
                                    case 1:
                                    player.BUFF[4] = 2;
                                    player.DEF = (int)(player.DEF * 1.75);
                                    break;
                                    case 2:
                                    player.BUFF[4] = 3;
                                    player.DEF = (int)(player.DEF * 2.5);
                                    break;
                                }
                                break;
                            case 8:
                            //EMPパルスキャノン
                                switch (item_class)
                                {
                                    case 0:
                                    target.HP -= (int)(player.ATK*Random.Range(1f, 1.5f));
                                    break;
                                    case 1:
                                    target.HP -= (int)(player.ATK*Random.Range(1.5f, 2f));
                                    break;
                                    case 2:
                                    target.HP -= (int)(player.ATK*Random.Range(2f, 3.5f));
                                    break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
