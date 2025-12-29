using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickStatasVieu : MonoBehaviour
{
    public ChangeRarity changeRarity;
    public Text statasVieuText;

    private string[] itemNames = new string[]
    {
        "リペアユニット",
        "ガトリングガン",
        "重装ロケットランチャー",
        "火炎放射器",
        "サイバーハックモジュール",
        "オーバークロックモジュール",
        "スタンガン",
        "フィールドシールド",
        "EMPパルスキャノン"
    };

    private string[] itemStatas = new string[]
    {
        "効果：\nHPを回復する。\n回復量:\nコモン:5000\nレア:8000\nエピック:15000",
        "効果：\n1～6回の多段攻撃を行う。\nダメージ倍率:\nコモン:100%\nレア:150%\nエピック:300%",
        "効果：\n単体に強力な一撃を与える。\nダメージ倍率:\nコモン:400%\nレア:600%\nエピック:1200%",
        "効果：\n敵全体に3ターン継続の火炎ダメージを与える。\n毎ターンダメージ:\nコモン:250%\nレア:375%\nエピック:750%",
        "効果：\n敵単体の与ダメージを減少させる（デバフ、3ターン）。\n減少量:\nコモン:10%\nレア:30%\nエピック:60%",
        "効果：\n味方単体の与ダメージを増加させる（バフ、3ターン）。\n増加量:\nコモン:40%\nレア:70%\nエピック:100%",
        "効果：\n敵単体の防御力を下げる（デバフ、3ターン）。\n減少量:\nコモン:10%\nレア:30%\nエピック:60%",
        "効果：\n味方全体の防御力を上げる（バフ、3ターン）。\n増加量:\nコモン:40%\nレア:70%\nエピック:100%",
        "効果：\n敵全体に範囲攻撃を行う。\nダメージ倍率:\nコモン:300%\nレア:450%\nエピック:900%"
    };




    public void OnClickStatasVieu(int itemNumber)
    {
        statasVieuText.text = "名前: \n" + itemNames[itemNumber] + "\n" + itemStatas[itemNumber];
    }
}