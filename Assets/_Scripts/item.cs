using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public Dictionary<int, List<int>> item_list = new Dictionary<int, List<int>>(){
    {0, new List<int>() {0,0,0}},
    {1, new List<int>() {0,0,0}},
    {2, new List<int>() {0,0,0}},
    {3, new List<int>() {0,0,0}},
    {4, new List<int>() {0,0,0}},
    {5, new List<int>() {0,0,0}},
    {6, new List<int>() {0,0,0}},
    {7, new List<int>() {0,0,0}},
    {8, new List<int>() {0,0,0}}
    };
    public Dictionary<int, string> item_name_list = new Dictionary<int, string>(){
    {0,"リペアユニット"},
    {1,"ガトリングガン"},
    {2,"重装ランチャー"},
    {3,"火炎放射器"},
    {4,"サイバーハックモジュール"},
    {5,"オーバークロックモジュール"},
    {6,"スタンガン"},
    {7,"フィールドシールド"},
    {8,"EMPパルスキャノン"}
    };
    
    /// <summary>
    /// アイテムの所持数を設定（バトル終了時に使用）
    /// </summary>
    /// <param name="itemId">アイテムID</param>
    /// <param name="count">新しい所持数（レアリティごとの合計）</param>
    public void SetCount(int itemId, int count)
    {
        if (!item_list.ContainsKey(itemId))
        {
            Debug.LogWarning($"item: アイテムID {itemId} が存在しません");
            return;
        }
        
        // 現在のレアリティごとの所持数を取得
        List<int> currentCounts = item_list[itemId];
        
        // 合計所持数を計算
        int totalCount = 0;
        foreach (int c in currentCounts)
        {
            totalCount += c;
        }
        
        // 差分を計算
        int difference = count - totalCount;
        
        // 差分をCommonに追加（簡易実装）
        // TODO: より高度な分配ロジックが必要な場合は実装
        if (currentCounts.Count > 0)
        {
            currentCounts[0] = Mathf.Max(0, currentCounts[0] + difference);
        }
        
        Debug.Log($"item: アイテムID {itemId} の所持数を設定 - 合計: {count} (差分: {difference})");
    }
}
