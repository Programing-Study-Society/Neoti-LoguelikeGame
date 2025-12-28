using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treasure: MonoBehaviour
{
  System.Random r = new System.Random();
  public item itemTresure;
  public player player;
  public void Choose()
  { 
    int dropRandom = r.Next(3, 6);
    Debug.Log(dropRandom);
    int itemGrade = SelectGrade();
    for(int i = 0; i <= dropRandom; i++) {

        int dropProbability = r.Next(1, 101);
        System.Console.WriteLine(dropProbability);

        if(dropProbability <= 50) {

          int randomItem = r.Next(0, 9);
          Debug.Log(randomItem);

          switch(randomItem) {
            case 0: 
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("リペアキットがドロップしました。"); 
              break;
            case 1:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("ガトリングガンがドロップしました。"); 
              break;
            case 2:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("重装ランチャーがドロップしました。");  
              break;
            case 3:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("火炎放射器がドロップしました。");  
              break;
            case 4:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("サイバーハックモジュールがドロップしました。");  
              break;
            case 5:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("オーバークロックモジュールがドロップしました。"); 
              break;
            case 6:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("スタンガンがドロップしました。"); 
              break;
            case 7:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("フィールドシールドがドロップしました。"); 
              break;
            case 8:  
              itemTresure.item_list[randomItem][itemGrade] += 1;
              Debug.Log("EMPパルスキャノンがドロップしました。"); 
              break;
          }            
        }
        else if(dropProbability <= 75) {
          int randomMoney = r.Next(100, 301);
          int money = randomMoney;
          player.MONEY += money;
          Debug.Log("お金が" + money + "円ドロップしました"); 
        }
        else {
          int randomSkill = r.Next(100, 301);
          int skillPoint = randomSkill;
          player.SKILL += skillPoint;
          Debug.Log("スキルポイントが" + skillPoint + "ポイントドロップしました"); 
        }
    }
  }
  public int SelectGrade() {
    int stageProgress = 2;
    int itemGrade;
    if(stageProgress == 1) {
      itemGrade = 0;
    }
    else if(stageProgress == 2) {
      itemGrade = 1;
    }
    else {
      itemGrade = 2;
    }
    return itemGrade;
  }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
