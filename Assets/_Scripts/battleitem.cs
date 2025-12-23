using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleitem : MonoBehaviour
{
    public GameObject hand;
    public item item_L;
    public player player;
    public void use_item(int item_id,int item_class,string object_name)
    {
        //以下のプログラムはオブジェクトからステータス(MAXHPなど)を取得する。
        //オブジェクト名を指定する
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
                                player.HP += player.MAXHP*Random.Range(0.1f, 0.3f);
                                break;
                                case 1:
                                player.HP += player.MAXHP*Random.Range(0.4f, 0.7f);
                                break;
                                case 2:
                                player.HP += player.MAXHP*Random.Range(0.8f, 1f);
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
                                    target.HP -= player.ATK*Random.Range(0.05f, 0.35f);
                                }
                                break;
                                case 1:
                                for(int i = 0;i<5;i++)
                                {
                                    target.HP -= player.ATK*Random.Range(0.25f, 0.55f);
                                }
                                break;
                                case 2:
                                for(int i = 0;i<5;i++)
                                {
                                    target.HP -= player.ATK*Random.Range(0.45f, 0.85f);
                                }
                                break;
                            }
                            break;
                        case 2:
                        //重装ランチャー
                            switch (item_class)
                            {
                                case 0:
                                target.HP -= player.ATK*Random.Range(1.5f, 1.8f);
                                break;
                                case 1:
                                target.HP -= player.ATK*Random.Range(2.0f, 2.3f);
                                break;
                                case 2:
                                target.HP -= player.ATK*Random.Range(3.5f, 3.8f);
                                break;
                            }
                            break;
                        case 3:
                        //火炎放射器
                            switch (item_class)
                            {
                                case 0:
                                target.HP -= player.ATK*Random.Range(1.5f, 1.8f);
                                break;
                                case 1:
                                target.HP -= player.ATK*Random.Range(2.0f, 2.3f);
                                break;
                                case 2:
                                target.HP -= player.ATK*Random.Range(3.5f, 3.8f);
                                break;
                            }
                            break;
                    }
                }
            }
        }
    }
}
