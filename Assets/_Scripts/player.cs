using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public int MAXHP = 20000; //最大HP
    public int HP = 2000;
    public int ATK = 100; //攻撃力
    public int DEF = 100; //防御力
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
