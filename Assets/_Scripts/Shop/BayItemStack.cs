using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BayItemStack : MonoBehaviour
{
    public Text stacktext;
    public Text stackPriceText;
    public ChangeRarity changeRarity;

    [HideInInspector]
    public item item_L;
    public Money money;

    public int ItemNumber;

    [HideInInspector]
    public List<int> stack = new List<int>() { 0, 0, 0 };


    void Awake()//itemスクリプト取得
    {
        item_L = FindObjectOfType<item>();

        if (item_L == null)
        {
            Debug.LogError("Item がシーン内に存在しません");
        }
    }

    void OnEnable()//アクティブになったら更新
    {
        stack = new List<int>() { 0, 0, 0 };
        RarityStackText();
        StackPriceText();
    }

    public void OnClickStackUp()//1:normal 2:rare 3:epicごとに買うスタック数増加
    {
        switch (changeRarity.nowRarity)
        {
            case 1:
                money.bayMoney += 500;
                break;
            case 2:
                money.bayMoney += 1000;
                break;
            case 3:
                money.bayMoney += 1500;
                break;
        }
        stack[changeRarity.nowRarity - 1] += 1;
        RarityStackText();
        money.MoneyTextUpdate();
    }

    public void OnClickStackDown()//1:normal 2:rare 3:epicごとに買うスタック数減少
    {
        if (stack[changeRarity.nowRarity - 1] > 0)
        {
            stack[changeRarity.nowRarity - 1] -= 1;
            switch (changeRarity.nowRarity)
            {
                case 1:
                    money.bayMoney -= 500;
                    break;
                case 2:
                    money.bayMoney -= 1000;
                    break;
                case 3:
                    money.bayMoney -= 1500;
                    break;
            }
        }
        RarityStackText();
        money.MoneyTextUpdate();
    }

    public void RarityStackText()//1:normal 2:rare 3:epicごとに買うスタック数表示
    {
        stacktext.text = stack[changeRarity.nowRarity - 1].ToString();
    }

    public void StackPriceText()
    {
        switch (changeRarity.nowRarity)
        {
            case 1:
                stackPriceText.text = "価格:500" + "\n" +
                                "所持数:" + item_L.item_list[ItemNumber][changeRarity.nowRarity - 1].ToString();
                break;
            case 2:
                stackPriceText.text = "価格:1000" + "\n" +
                                "所持数:" + item_L.item_list[ItemNumber][changeRarity.nowRarity - 1].ToString();
                break;
            case 3:
                stackPriceText.text = "価格:1500" + "\n" +
                                "所持数:" + item_L.item_list[ItemNumber][changeRarity.nowRarity - 1].ToString();
                break;
        }
    }

    public void BayItem()//スタック分購入
    {
        for (int i = 0; i < 3; i++)
        {
            item_L.item_list[ItemNumber][i] += stack[i];
            stack[i] = 0;
        }
        RarityStackText();
        StackPriceText();
    }
}
