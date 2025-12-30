using UnityEngine;

/// <summary>
/// 宝箱報酬の種類
/// </summary>
public enum TreasureRewardType
{
    Item,        // アイテム
    Money,       // お金
    SkillPoint   // スキルポイント
}

/// <summary>
/// 宝箱報酬データ構造体
/// </summary>
[System.Serializable]
public class TreasureReward
{
    public TreasureRewardType type;      // 報酬の種類
    public int itemId;                   // アイテムID（typeがItemの場合のみ有効、0-8）
    public int rarity;                   // レアリティ（typeがItemの場合のみ有効、0=Common, 1=Rare, 2=Epic）
    public int amount;                   // 数量（お金の金額、スキルポイントの量、アイテムの場合は常に1）

    /// <summary>
    /// アイテム報酬のコンストラクタ
    /// </summary>
    public TreasureReward(int itemId, int rarity)
    {
        this.type = TreasureRewardType.Item;
        this.itemId = itemId;
        this.rarity = rarity;
        this.amount = 1;
    }

    /// <summary>
    /// お金報酬のコンストラクタ
    /// </summary>
    public TreasureReward(int moneyAmount)
    {
        this.type = TreasureRewardType.Money;
        this.itemId = -1;
        this.rarity = -1;
        this.amount = moneyAmount;
    }

    /// <summary>
    /// スキルポイント報酬を作成するstaticメソッド
    /// </summary>
    public static TreasureReward CreateSkillPointReward(int skillPointAmount)
    {
        TreasureReward reward = new TreasureReward();
        reward.type = TreasureRewardType.SkillPoint;
        reward.itemId = -1;
        reward.rarity = -1;
        reward.amount = skillPointAmount;
        return reward;
    }

    /// <summary>
    /// デフォルトコンストラクタ（シリアライズ用）
    /// </summary>
    public TreasureReward()
    {
        this.type = TreasureRewardType.Item;
        this.itemId = -1;
        this.rarity = -1;
        this.amount = 0;
    }

    /// <summary>
    /// 報酬の表示名を取得
    /// </summary>
    public string GetDisplayName(item itemData)
    {
        switch (type)
        {
            case TreasureRewardType.Item:
                if (itemData != null && itemData.item_name_list != null)
                {
                    if (itemData.item_name_list.TryGetValue(itemId, out string itemName))
                    {
                        return itemName;
                    }
                }
                return $"アイテム{itemId}";

            case TreasureRewardType.Money:
                return $"クレジット:{amount}";

            case TreasureRewardType.SkillPoint:
                return $"クレジット:{amount}";

            default:
                return "不明な報酬";
        }
    }
}

