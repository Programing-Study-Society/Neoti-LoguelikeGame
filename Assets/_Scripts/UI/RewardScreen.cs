using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// バトル報酬画面UI管理クラス
/// バトル勝利時の報酬を表示（TreasureScreenと同様の構造）
/// </summary>
public class RewardScreen : MonoBehaviour
{
    [Header("報酬表示")]
    [SerializeField] private Transform itemContainer; // アイテムスロットの親オブジェクト（HorizontalLayoutGroup）
    
    [Header("スロットプレハブ")]
    [SerializeField] private GameObject itemSlotPrefab; // アイテム表示用のプレハブ（Image + Text）
    [SerializeField] private GameObject creditSlotPrefab; // お金・スキルポイント表示用のプレハブ（Image + Text）
    
    [Header("確認ボタン")]
    [SerializeField] private Button confirmButton; // 確認ボタン
    
    [Header("アイテム画像設定")]
    [SerializeField] private Sprite defaultItemSprite; // デフォルトのアイテム画像（画像がない場合）
    
    [Header("アイテムデータ参照")]
    [SerializeField] private item itemData; // item.csへの参照（インスペクターで設定）
    
    [Header("レアリティ色設定")]
    [SerializeField] private Color commonTextColor = Color.white;   // Commonテキスト色（白）
    [SerializeField] private Color rareTextColor = new Color(0f, 0.5f, 1f, 1f);      // Rareテキスト色（青）
    [SerializeField] private Color epicTextColor = new Color(0.5f, 0f, 1f, 1f);   // Epicテキスト色（紫）
    
    [Header("イベント")]
    public UnityEvent onConfirmButtonClicked; // 確認ボタンが押された時のイベント
    
    // アイテムID → 画像ファイル名のマッピング（TreasureScreenと共通）
    private static readonly Dictionary<int, string> itemImageMap = new Dictionary<int, string>
    {
        { 0, "repair_unit" },      // リペアユニット
        { 1, "gatling_gun" },      // ガトリングガン
        { 2, "heavy_launcher" },   // 重装ランチャー
        { 3, "firethrower" },      // 火炎放射器（ファイル名: firethrower.png）
        { 4, "cyber_hack" },       // サイバーハックモジュール
        { 5, "overclock" },        // オーバークロックモジュール
        { 6, "stun_gun" },         // スタンガン
        { 7, "field_shield" },     // フィールドシールド
        { 8, "emp_cannon" }        // EMPパルスキャノン
    };
    
    // 生成されたアイテムスロットのリスト
    private List<GameObject> spawnedItemSlots = new List<GameObject>();
    
    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        Initialize();
    }
    
    private void OnEnable()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        // 確認ボタンの設定
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        
        // 以前のアイテムスロットをクリア
        ClearItemSlots();
        
        Debug.Log("RewardScreen: 初期化完了");
    }
    
    /// <summary>
    /// 報酬画面を表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        Debug.Log("RewardScreen: 表示");
    }
    
    /// <summary>
    /// 報酬画面を非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        Debug.Log("RewardScreen: 非表示");
    }
    
    /// <summary>
    /// 報酬を表示
    /// </summary>
    public void ShowRewards(int gold, int skillPoint, 
        int itemSmallId, int itemSmallCount,
        int itemMiddleId, int itemMiddleCount,
        int itemLargeId, int itemLargeCount)
    {
        // 以前のスロットをクリア
        ClearItemSlots();
        
        if (itemContainer == null)
        {
            Debug.LogError("RewardScreen: itemContainerが設定されていません");
            return;
        }
        
        // TreasureReward型のリストに変換
        List<TreasureReward> rewards = new List<TreasureReward>();
        
        // ゴールド報酬
        if (gold > 0)
        {
            rewards.Add(new TreasureReward(gold));
        }
        
        // スキルポイント報酬
        if (skillPoint > 0)
        {
            rewards.Add(TreasureReward.CreateSkillPointReward(skillPoint));
        }
        
        // アイテム報酬（小=Common）
        if (itemSmallCount > 0 && itemSmallId >= 0)
        {
            for (int i = 0; i < itemSmallCount; i++)
            {
                rewards.Add(new TreasureReward(itemSmallId, 0)); // Common
            }
        }
        
        // アイテム報酬（中=Rare）
        if (itemMiddleCount > 0 && itemMiddleId >= 0)
        {
            for (int i = 0; i < itemMiddleCount; i++)
            {
                rewards.Add(new TreasureReward(itemMiddleId, 1)); // Rare
            }
        }
        
        // アイテム報酬（大=Epic）
        if (itemLargeCount > 0 && itemLargeId >= 0)
        {
            for (int i = 0; i < itemLargeCount; i++)
            {
                rewards.Add(new TreasureReward(itemLargeId, 2)); // Epic
            }
        }
        
        // 報酬を表示
        ShowRewards(rewards);
        
        Debug.Log($"RewardScreen: 報酬を表示 - Gold:{gold}, Skill:{skillPoint}, Items:{itemSmallCount + itemMiddleCount + itemLargeCount}");
    }
    
    /// <summary>
    /// 報酬を表示（TreasureReward型のリスト）
    /// </summary>
    private void ShowRewards(List<TreasureReward> rewards)
    {
        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("RewardScreen: 表示する報酬がありません");
            return;
        }
        
        // 各報酬を個別に表示
        foreach (var reward in rewards)
        {
            CreateRewardSlot(reward);
        }
        
        Debug.Log($"RewardScreen: {rewards.Count}個の報酬を表示");
    }
    
    /// <summary>
    /// 報酬スロットを生成（TreasureScreenと同様の実装）
    /// </summary>
    private void CreateRewardSlot(TreasureReward reward)
    {
        if (itemContainer == null)
        {
            Debug.LogError("RewardScreen: itemContainer が設定されていません");
            return;
        }

        if (reward == null)
        {
            Debug.LogWarning("RewardScreen: 報酬データがnullです");
            return;
        }

        // 報酬の種類に応じてプレハブを選択
        GameObject prefabToUse = null;
        if (reward.type == TreasureRewardType.Item)
        {
            prefabToUse = itemSlotPrefab;
            if (prefabToUse == null)
            {
                Debug.LogError("RewardScreen: itemSlotPrefab が設定されていません");
                return;
            }
        }
        else
        {
            // お金・スキルポイント用
            prefabToUse = creditSlotPrefab;
            if (prefabToUse == null)
            {
                Debug.LogWarning("RewardScreen: creditSlotPrefab が設定されていません。itemSlotPrefab を使用します。");
                prefabToUse = itemSlotPrefab; // フォールバック
                if (prefabToUse == null)
                {
                    Debug.LogError("RewardScreen: itemSlotPrefab も設定されていません");
                    return;
                }
            }
        }

        // スロットを生成
        GameObject slot = Instantiate(prefabToUse, itemContainer);
        spawnedItemSlots.Add(slot);

        // 画像を設定（ItemImageまたはCreditImageという名前の子オブジェクトを探す）
        Transform itemImageTransform = slot.transform.Find("ItemImage");
        if (itemImageTransform == null)
        {
            itemImageTransform = slot.transform.Find("CreditImage");
        }
        
        if (itemImageTransform != null)
        {
            Image rewardImage = itemImageTransform.GetComponent<Image>();
            if (rewardImage != null)
            {
                Sprite rewardSprite = GetRewardSprite(reward);
                rewardImage.sprite = rewardSprite != null ? rewardSprite : defaultItemSprite;
            }
        }
        else
        {
            // ItemImage/CreditImageが見つからない場合はGetComponentInChildrenで検索
            Image rewardImage = slot.GetComponentInChildren<Image>();
            if (rewardImage != null)
            {
                Sprite rewardSprite = GetRewardSprite(reward);
                rewardImage.sprite = rewardSprite != null ? rewardSprite : defaultItemSprite;
            }
        }

        // 表示名を取得
        string displayName = reward.GetDisplayName(itemData);
        
        // アイテムの場合、個数が1より大きい場合は個数を追加
        if (reward.type == TreasureRewardType.Item && reward.amount > 1)
        {
            displayName += $" x{reward.amount}";
        }

        // テキストを設定（ItemNameTextまたはCreditNameTextという名前の子オブジェクトを探す）
        Transform itemNameTextTransform = slot.transform.Find("ItemNameText");
        if (itemNameTextTransform == null)
        {
            itemNameTextTransform = slot.transform.Find("CreditNameText");
        }
        
        if (itemNameTextTransform != null)
        {
            // TextMeshProUGUIを優先
            TMPro.TextMeshProUGUI itemTMPText = itemNameTextTransform.GetComponent<TMPro.TextMeshProUGUI>();
            if (itemTMPText != null)
            {
                itemTMPText.text = displayName;
                
                // レアリティ色を適用（アイテムの場合のみ）
                if (reward.type == TreasureRewardType.Item)
                {
                    ApplyRarityColor(itemTMPText, reward.rarity);
                }
                else
                {
                    // お金・スキルポイントは白
                    itemTMPText.color = Color.white;
                }
            }
            else
            {
                // TextMeshProUGUIがない場合はUnityEngine.UI.Textを試す
                UnityEngine.UI.Text itemText = itemNameTextTransform.GetComponent<UnityEngine.UI.Text>();
                if (itemText != null)
                {
                    itemText.text = displayName;
                    
                    // レアリティ色を適用（アイテムの場合のみ）
                    if (reward.type == TreasureRewardType.Item)
                    {
                        ApplyRarityColor(itemText, reward.rarity);
                    }
                    else
                    {
                        // お金・スキルポイントは白
                        itemText.color = Color.white;
                    }
                }
            }
        }
        else
        {
            // ItemNameText/CreditNameTextが見つからない場合はGetComponentInChildrenで検索
            TMPro.TextMeshProUGUI itemTMPText = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (itemTMPText != null)
            {
                itemTMPText.text = displayName;
                
                // レアリティ色を適用（アイテムの場合のみ）
                if (reward.type == TreasureRewardType.Item)
                {
                    ApplyRarityColor(itemTMPText, reward.rarity);
                }
                else
                {
                    // お金・スキルポイントは白
                    itemTMPText.color = Color.white;
                }
            }
            else
            {
                // TMPがない場合はTextを使用
                UnityEngine.UI.Text itemText = slot.GetComponentInChildren<UnityEngine.UI.Text>();
                if (itemText != null)
                {
                    itemText.text = displayName;
                    
                    // レアリティ色を適用（アイテムの場合のみ）
                    if (reward.type == TreasureRewardType.Item)
                    {
                        ApplyRarityColor(itemText, reward.rarity);
                    }
                    else
                    {
                        // お金・スキルポイントは白
                        itemText.color = Color.white;
                    }
                }
            }
        }

        Debug.Log($"RewardScreen: 報酬スロット生成 - Type:{reward.type}, ItemId:{reward.itemId}, Rarity:{reward.rarity}, Amount:{reward.amount}");
    }
    
    /// <summary>
    /// 報酬のスプライトを取得（TreasureScreenと同様）
    /// </summary>
    private Sprite GetRewardSprite(TreasureReward reward)
    {
        if (reward == null) return null;
        
        switch (reward.type)
        {
            case TreasureRewardType.Item:
                return GetItemSprite(reward.itemId);

            case TreasureRewardType.Money:
                return Resources.Load<Sprite>("itemphoto/Monney_credit");

            case TreasureRewardType.SkillPoint:
                return Resources.Load<Sprite>("itemphoto/skill_credit");

            default:
                return null;
        }
    }
    
    /// <summary>
    /// アイテムスプライトを読み込む
    /// </summary>
    private Sprite LoadItemSprite(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) return null;
        
        // Resources/itemphoto/ から読み込む
        Sprite sprite = Resources.Load<Sprite>($"itemphoto/{imageName}");
        if (sprite == null)
        {
            Debug.LogWarning($"RewardScreen: アイテム画像 '{imageName}' が見つかりません");
        }
        
        return sprite;
    }
    
    /// <summary>
    /// アイテムIDからスプライトを取得（TreasureScreenと同様）
    /// </summary>
    private Sprite GetItemSprite(int itemId)
    {
        if (itemImageMap.TryGetValue(itemId, out string imageName))
        {
            return LoadItemSprite(imageName);
        }
        
        return null;
    }
    
    /// <summary>
    /// レアリティ色を適用（TextMeshProUGUI用）
    /// </summary>
    private void ApplyRarityColor(TMPro.TextMeshProUGUI text, int rarity)
    {
        if (text == null) return;
        
        Color color = GetRarityColor(rarity);
        text.color = color;
    }
    
    /// <summary>
    /// レアリティ色を適用（UnityEngine.UI.Text用）
    /// </summary>
    private void ApplyRarityColor(UnityEngine.UI.Text text, int rarity)
    {
        if (text == null) return;
        
        Color color = GetRarityColor(rarity);
        text.color = color;
    }
    
    /// <summary>
    /// レアリティ色を取得
    /// </summary>
    private Color GetRarityColor(int rarity)
    {
        return rarity switch
        {
            0 => commonTextColor,  // Common
            1 => rareTextColor,     // Rare
            2 => epicTextColor,     // Epic
            _ => commonTextColor    // デフォルト
        };
    }
    
    /// <summary>
    /// アイテムスロットをクリア
    /// </summary>
    private void ClearItemSlots()
    {
        foreach (var slot in spawnedItemSlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        spawnedItemSlots.Clear();
    }
    
    /// <summary>
    /// 確認ボタンが押された時の処理
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        Debug.Log("RewardScreen: 確認ボタンが押されました");
        onConfirmButtonClicked?.Invoke();
    }
}

