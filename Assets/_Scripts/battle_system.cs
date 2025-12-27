using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR;

public class battle_system : MonoBehaviour
{
    public GameObject hand;
    private int battlePlayerHP;
    //private int battleEnemyHP;
    public player player;
    private List<enemy_L> enemies = new List<enemy_L>();
    void OnEnable()
    {
        player=GetComponent<player>();
        //1体目の座標指定
        Vector2 position = Vector2.zero;
        //3体出現させる(3回繰り返す)
        for(int i = 0; i < 3; i++)
        {
            //Instantiateでプレハブを複製
            GameObject cod = Instantiate(hand, position, Quaternion.identity);//複製したい敵のPrefab、出現座標、角度
            //一つ一つ複製したプレハブに名づけ
            cod.name = "cod" + i.ToString();
            //次の複製体のために間隔をあける

            //敵ステータス(enemy_L)を取得してリストに追加
            enemy_L enemy = cod.GetComponent<enemy_L>();
            if (enemy != null)
            {
               enemy.HP = enemy.MAXHP;          //HP初期化
               enemies.Add(enemy);              //管理リストに登録
            }
           
            position.x += 2;
        }
        StartBattle();
    }

    void StartBattle()
    {
        // バトル開始時に全回復
        battlePlayerHP = player.MAXHP;

        Debug.Log("バトル開始 プレイヤーHP: " + battlePlayerHP);
        Debug.Log("敵の数: " + enemies.Count);     //確認用ログ
    }

    public void PlayerTakeDamage(int damage)
    {
        int realDamage = Mathf.Max(1, damage - player.DEF);
        battlePlayerHP -= realDamage;

        Debug.Log("プレイヤー被ダメージ: " + realDamage);

        if (battlePlayerHP <= 0)
        {
            Debug.Log("プレイヤー敗北");
        }
    }

      public void AttackEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count) return;

        enemy_L enemy = enemies[index];

        int damage = Mathf.Max(1, player.ATK - enemy.DEF);
        enemy.HP -= damage;

        Debug.Log($"敵に {damage} ダメージ");

        if (enemy.HP <= 0)
        {
            Debug.Log("敵撃破");
            Destroy(enemy.gameObject);
            enemies.RemoveAt(index);

            //敵が全滅したらバトル終了
            if (enemies.Count == 0)
            {
                EndBattle();
                return;
            }
        }
        EnemyTurn();
    }
    public void EnemyTurn()
    {
        Debug.Log("敵のターン開始");

        foreach (enemy_L enemy in enemies)
        {
            if (enemy == null) continue;

            int damage = Mathf.Max(1, enemy.ATK - player.DEF);
            battlePlayerHP -= damage;

            Debug.Log($"敵の攻撃！ プレイヤーに {damage} ダメージ");

            if (battlePlayerHP <= 0)
            {
                Debug.Log("プレイヤー敗北");
                return;
            }
        }

        Debug.Log("敵のターン終了");
    }

    void EndBattle()
    {
        // 何もしなくてOK（次のバトルで自動回復）
        Debug.Log("バトル終了 次のバトルで全回復");
    }
}