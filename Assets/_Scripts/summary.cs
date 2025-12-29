using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class summary : MonoBehaviour
{
    public enum GameState{
        Title,      //タイトル画面中
        Map,        //マップ上で移動
        Battle,     //バトル中
        Result,     //バトル後の報酬
        Treasure,   //宝箱
        Shop        //ショップ
    }

    public GameState currentState;

    void Start(){
        ChangeState(GameState.Title);
    }

    public void ChangeState(GameState newState){
        currentState = newState;

        switch (newState)
        {
            case GameState.Title:
                Title();
                break;
            
            case GameState.Map:
                Map();
                break;

            case GameState.Battle:
                Battle();
                break;

            case GameState.Result:
                Result();
                break;

            case GameState.Treasure:
                Box();
                break;

            case GameState.Shop:
                Shop();
                break;
        }
    }
    

    public void Title(){
        
    }
    public void Map(){
        
    }
    
    /// <summary>
    /// バトルを開始（テスト用：InspectorのContextMenuから実行可能）
    /// </summary>
    [ContextMenu("バトルを開始")]
    public void Battle(){
        // テスト用：アイテムを適当に持たせる
        SetupTestItems();
        
        // battle_systemを取得してバトルを開始
        battle_system battleSystem = FindObjectOfType<battle_system>();
        if (battleSystem != null)
        {
            // battle_systemのStartBattle()を呼ぶ（フラグを立ててOnEnable()経由で開始）
            battleSystem.StartBattle();
            Debug.Log("summary: バトル開始 - battle_system.StartBattle()を呼び出しました");
        }
        else
        {
            Debug.LogError("summary: battle_systemが見つかりません");
        }
    }
    
    /// <summary>
    /// テスト用：アイテムを適当に持たせる
    /// </summary>
    private void SetupTestItems()
    {
        item itemData = FindObjectOfType<item>();
        if (itemData != null && itemData.item_list != null)
        {
            // 適当にアイテムを持たせる（ID, [Common, Rare, Epic]）
            itemData.item_list[0] = new List<int>() { 3, 2, 1 }; // リペアユニット: Common3, Rare2, Epic1
            itemData.item_list[1] = new List<int>() { 2, 1, 0 }; // ガトリングガン: Common2, Rare1, Epic0
            itemData.item_list[2] = new List<int>() { 1, 1, 1 }; // 重装ランチャー: Common1, Rare1, Epic1
            itemData.item_list[3] = new List<int>() { 2, 0, 0 }; // 火炎放射器: Common2, Rare0, Epic0
            itemData.item_list[4] = new List<int>() { 1, 2, 0 }; // サイバーハック: Common1, Rare2, Epic0
            itemData.item_list[5] = new List<int>() { 0, 1, 1 }; // オーバークロック: Common0, Rare1, Epic1
            itemData.item_list[6] = new List<int>() { 1, 0, 0 }; // スタンガン: Common1, Rare0, Epic0
            itemData.item_list[7] = new List<int>() { 0, 0, 1 }; // フィールドシールド: Common0, Rare0, Epic1
            itemData.item_list[8] = new List<int>() { 1, 1, 0 }; // EMPキャノン: Common1, Rare1, Epic0
            
            Debug.Log("summary: テスト用アイテムを設定しました");
        }
        else
        {
            Debug.LogWarning("summary: itemが見つかりません（アイテム設定をスキップ）");
        }
    }
    public void Result(){
        
    }
    public void Box(){
        
    }
    public void Shop(){
        
    }
}
