using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treasure: MonoBehaviour
{
  System.Random r = new System.Random();
  public item itemTresure;
  public player player;
  /// <summary>
  /// 宝箱から報酬を抽選して返す
  /// </summary>
  /// <returns>生成された報酬のリスト（TreasureReward型、最大5個）</returns>
  public List<TreasureReward> Choose()
  { 
    List<TreasureReward> rewards = new List<TreasureReward>();
    
    int dropRandom = r.Next(3, 6);
    Debug.Log($"treasure: ドロップ回数 = {dropRandom}");
    int itemGrade = SelectGrade();
    
    for(int i = 0; i <= dropRandom; i++) {

        int dropProbability = r.Next(1, 101);
        System.Console.WriteLine(dropProbability);

        if(dropProbability <= 50) {
          // アイテムドロップ
          int randomItem = r.Next(0, 9);
          Debug.Log($"treasure: アイテムドロップ - ID:{randomItem}, レアリティ:{itemGrade}");

          // 報酬リストに追加
          rewards.Add(new TreasureReward(randomItem, itemGrade));

          // 実際の所持数も更新
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
          // お金ドロップ
          int randomMoney = r.Next(100, 301);
          int money = randomMoney;
          
          // 報酬リストに追加
          rewards.Add(new TreasureReward(money));
          
          // 実際の所持金も更新
          player.MONEY += money;
          Debug.Log($"お金が{money}円ドロップしました"); 
        }
        else {
          // スキルポイントドロップ
          int randomSkill = r.Next(100, 301);
          int skillPoint = randomSkill;
          
          // 報酬リストに追加
          rewards.Add(TreasureReward.CreateSkillPointReward(skillPoint));
          
          // 実際のスキルポイントも更新
          player.SKILL += skillPoint;
          Debug.Log($"スキルポイントが{skillPoint}ポイントドロップしました"); 
        }
    }
    
    Debug.Log($"treasure: 報酬生成完了 - 合計{rewards.Count}個");
    return rewards;
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
