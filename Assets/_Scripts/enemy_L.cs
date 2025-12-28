using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_L : MonoBehaviour
{
    public int MAXHP = 0 ; //最大HP
    public int HP = 0;//リアルタイムHP
    public int ATK = 0; //攻撃力
    public int DEF = 0; //防御力
    public List<int> BUFF = new List<int>() {
    0,//持続ダメージ(デバフ)
    0,//ダメージダウン(デバフ)
    0,//ダメージアップ(バフ)
    0,//防御力ダウン(デバフ)
    0//防御力アップ(バフ)
    };

}
