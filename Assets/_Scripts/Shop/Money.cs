using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Money : MonoBehaviour
{
    [HideInInspector]
    public player player;
    public ChangeRarity changeRarity;

    public Text text;

    public int bayMoney;


    void Awake()//playerスクリプト取得
    {
        player = FindObjectOfType<player>();

        if (player == null)
        {
            Debug.LogError("Player がシーン内に存在しません");
        }
    }

    void OnEnable()
    {
        MoneyTextUpdate();
    }

    public void TradeItem()
    {
        if (player.MONEY >= bayMoney)
        {
            player.MONEY -= bayMoney; 
            for (int i = 0; i < changeRarity.bayItemStacks.Count; i++)
            {
                changeRarity.bayItemStacks[i].BayItem();
            }
            bayMoney = 0;
            MoneyTextUpdate();
        }
        
    }

    public void MoneyTextUpdate()
    {
        text.text = "所持金:￥" + player.MONEY.ToString() + "\n"
                        + "合計金額:￥" + bayMoney.ToString();
    }
}