using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public int MAXHP = 200 ; //最大HP
    public int HP;
    public int ATK = 10; //攻撃力
    public int DEF = 5; //防御力
    public int SKILL = 0; //スキルポイント
    public int MONEY = 0; //お金
    public List<int> BUFF = new List<int>() {
        0,//持続ダメージ(デバフ)
        0,//ダメージダウン(デバフ)
        0,//ダメージアップ(バフ)
        0,//防御力ダウン(デバフ)
        0//防御力アップ(バフ)
        };
}
