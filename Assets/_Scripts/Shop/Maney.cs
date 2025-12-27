using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class money : MonoBehaviour
{

    public Text text;

    const int Mega = 1000000;

    int oldMoney;
    int oldMegaMoney;

    public SoundVolume soundVolume;

    // Start is called before the first frame update
    void Start()
    {
        oldMoney = globalValue.money;
        oldMegaMoney = globalValue.gigaMoney;
        Abbreviation();
    }

    // Update is called once per frame
    void Update()
    {
        Abbreviation();
        addMoney();
    }

    public void addMoney()
    {
        if (oldMegaMoney < globalValue.gigaMoney)
        {
            soundVolume.seVolume[9].Play();
            oldMoney = globalValue.money;
            oldMegaMoney = globalValue.gigaMoney;
        }
        if (oldMegaMoney < globalValue.gigaMoney)
        {
            soundVolume.seVolume[9].Play();
            oldMoney = globalValue.money;
            oldMegaMoney = globalValue.gigaMoney;
        }
        oldMoney = globalValue.money;
        oldMegaMoney = globalValue.gigaMoney;
    }

    public void Abbreviation()
    {
        if (globalValue.money >= Mega)
        {
            globalValue.gigaMoney  += globalValue.money / Mega;
            globalValue.money = globalValue.money % Mega;
        }
        if (globalValue.money < 0)
        {
            globalValue.gigaMoney -= 1;
            globalValue.money = Mega + globalValue.money;
        }
        
        if(globalValue.gigaMoney == 0)
        {
            text.text = "￥" + globalValue.money.ToString();
        }
        else
        {
            int money = globalValue.money / (Mega / 10);
            text.text = "￥" + globalValue.gigaMoney.ToString() + "." + money.ToString() + "M";
        }
    }
}