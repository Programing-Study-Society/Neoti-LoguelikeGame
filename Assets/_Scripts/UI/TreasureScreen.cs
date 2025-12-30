using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 宝箱ステージのUI管理クラス
/// 宝箱から出現したアイテムを表示する
/// </summary>
public class TreasureScreen : MonoBehaviour
{
    /// <summary>
    /// 惑星の種類（MapScreenと共通）
    /// </summary>
    public enum PlanetType
    {
        Forest_Planet,
        Ice_Planet,
        Old_Empire_Planet,
        Desert_Planet,
        Volcano_Planet
    }


    [Header("宝箱")]
    [SerializeField] private Image treasureChestImage; // 宝箱画像
    [SerializeField] private Sprite treasureChestClosedSprite; // 閉じた宝箱
    [SerializeField] private Sprite treasureChestOpenSprite;   // 開いた宝箱

    [Header("アイテム表示キャンバス")]
    [SerializeField] private GameObject itemDisplayCanvas; // アイテム表示用のパネル（宝箱を覆う）
    [SerializeField] private Transform itemContainer; // アイテムスロットの親オブジェクト（HorizontalLayoutGroup）

    [Header("スロットプレハブ")]
    [SerializeField] private GameObject itemSlotPrefab; // アイテム表示用のプレハブ（Image + Text）
    [SerializeField] private GameObject creditSlotPrefab; // お金・スキルポイント表示用のプレハブ（Image + Text）

    [Header("確認ボタン")]
    [SerializeField] private Button confirmButton;

    [Header("イベント")]
    public UnityEvent onConfirmButtonClicked; // 確認ボタンが押された時のイベント

    [Header("アイテム画像設定")]
    [SerializeField] private Sprite defaultItemSprite; // デフォルトのアイテム画像（画像がない場合）

    [Header("演出設定")]
    [SerializeField] private float chestWaitTime = 1.0f;      // 宝箱表示から開くまでの待機時間
    [SerializeField] private float chestOpenTime = 0.5f;      // 宝箱が開いてからアイテム表示までの待機時間

    [Header("アイテムデータ参照")]
    [SerializeField] private item itemData; // item_L.csへの参照（インスペクターで設定）

    [Header("レアリティ色設定")]
    [SerializeField] private Color commonTextColor = Color.white;   // Commonテキスト色（白）
    [SerializeField] private Color rareTextColor = new Color(0f, 0.5f, 1f, 1f);      // Rareテキスト色（青）
    [SerializeField] private Color epicTextColor = new Color(0.5f, 0f, 1f, 1f);   // Epicテキスト色（紫）

    [Header("表示制限")]
    [SerializeField] private int maxRewardDisplayCount = 5; // 最大表示数（3-5個）

    // アイテムID → 画像ファイル名のマッピング（ローカル管理）
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
    
    // 演出用の一時保存（TreasureReward型）
    private List<TreasureReward> pendingRewards;

    /// <summary>
    /// オブジェクト生成時に自動で初期化
    /// 注意: Awake()は問題ないが、Start()は使わないこと
    /// Start()は一度だけ呼ばれるため、後からGameManagerから呼び出す場合にバグる可能性がある
    /// </summary>
    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// オブジェクトがアクティブになるたびに呼ばれる（SetActive(true)の時）
    /// 2回目以降の表示時にも初期化を確実に実行するため
    /// </summary>
    private void OnEnable()
    {
        // 基本的な初期化を再実行（2回目以降の表示時にも必要）
        Initialize();
    }

    /// <summary>
    /// 宝箱画面の初期化
    /// 2回目以降の表示時にも前のリワード情報をクリアする
    /// </summary>
    public void Initialize()
    {
        // アイテム表示キャンバスを非表示
        if (itemDisplayCanvas != null)
        {
            itemDisplayCanvas.SetActive(false);
        }

        // 宝箱を閉じた状態に
        if (treasureChestImage != null && treasureChestClosedSprite != null)
        {
            treasureChestImage.sprite = treasureChestClosedSprite;
            treasureChestImage.gameObject.SetActive(true);
        }

        // 確認ボタンの設定
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        // 以前のアイテムスロットをクリア（2回目以降の表示時にも前の情報を削除）
        ClearItemSlots();

        // 演出用の一時保存データもクリア
        pendingRewards = null;

        Debug.Log("TreasureScreen: 初期化完了（前のリワード情報をクリア）");
    }

    #region メイン機能（ロジック側から呼び出し）

    /// <summary>
    /// 宝箱画面を表示して報酬を表示する（演出付き）
    /// ロジック側から呼び出されるメインメソッド
    /// </summary>
    /// <param name="planetName">惑星名（背景画像用）例: "Forest_Planet"</param>
    /// <param name="rewards">入手した報酬のリスト（TreasureReward型、最大5個）</param>
    public void ShowTreasure(string planetName, List<TreasureReward> rewards)
    {
        Debug.Log($"TreasureScreen: ShowTreasure呼び出し - 惑星: {planetName}, 報酬数: {rewards?.Count ?? 0}");

        // 最大表示数を超えている場合は警告
        if (rewards != null && rewards.Count > maxRewardDisplayCount)
        {
            Debug.LogWarning($"TreasureScreen: 報酬数が最大表示数({maxRewardDisplayCount})を超えています。最初の{maxRewardDisplayCount}個のみ表示します。");
        }

        // 背景設定は削除（summary.csの起動時に一度だけ設定される）

        // 報酬データを保存して演出開始
        pendingRewards = rewards;
        StartCoroutine(TreasureOpenSequence());
    }

    /// <summary>
    /// 宝箱を開けて報酬を表示する（演出なし、即時表示）
    /// </summary>
    /// <param name="rewards">表示する報酬のリスト（TreasureReward型）</param>
    public void OpenTreasureAndShowItems(List<TreasureReward> rewards)
    {
        Debug.Log($"TreasureScreen: 宝箱を開けます（即時）。報酬数: {rewards?.Count ?? 0}");

        // 宝箱を開いた状態に
        OpenTreasureChest();

        // 報酬を表示
        ShowRewards(rewards);
    }

    /// <summary>
    /// 宝箱演出シーケンス（コルーチン）
    /// </summary>
    private IEnumerator TreasureOpenSequence()
    {
        Debug.Log("TreasureScreen: 演出開始 - 宝箱表示中...");

        // 確認ボタンを非表示
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
        }

        // 1. 宝箱（閉じた状態）を表示
        if (treasureChestImage != null)
        {
            treasureChestImage.gameObject.SetActive(true);
            if (treasureChestClosedSprite != null)
            {
                treasureChestImage.sprite = treasureChestClosedSprite;
            }
        }

        // アイテムキャンバスは非表示
        if (itemDisplayCanvas != null)
        {
            itemDisplayCanvas.SetActive(false);
        }

        // 2. 待機（宝箱を見せる時間）
        yield return new WaitForSeconds(chestWaitTime);

        Debug.Log("TreasureScreen: 宝箱を開く...");

        // 3. 宝箱を開いた画像に変更
        if (treasureChestImage != null && treasureChestOpenSprite != null)
        {
            treasureChestImage.sprite = treasureChestOpenSprite;
        }

        // 4. 少し待機（開いた宝箱を見せる時間）
        yield return new WaitForSeconds(chestOpenTime);

        Debug.Log("TreasureScreen: アイテム表示！");

        // 5. 宝箱を非表示にしてアイテムを表示
        if (treasureChestImage != null)
        {
            treasureChestImage.gameObject.SetActive(false);
        }

        if (itemDisplayCanvas != null)
        {
            itemDisplayCanvas.SetActive(true);
        }

        // 報酬を表示
        ShowRewards(pendingRewards);

        // 確認ボタンを表示
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(true);
        }

        Debug.Log("TreasureScreen: 演出完了");
    }

    #endregion

    #region 宝箱・アイテム表示

    /// <summary>
    /// 宝箱を開く
    /// </summary>
    private void OpenTreasureChest()
    {
        if (treasureChestImage != null && treasureChestOpenSprite != null)
        {
            treasureChestImage.sprite = treasureChestOpenSprite;
        }

        // 宝箱を非表示にしてアイテムキャンバスを表示
        if (treasureChestImage != null)
        {
            treasureChestImage.gameObject.SetActive(false);
        }

        if (itemDisplayCanvas != null)
        {
            itemDisplayCanvas.SetActive(true);
        }

        Debug.Log("TreasureScreen: 宝箱を開きました");
    }

    /// <summary>
    /// 報酬を表示する（最大5個まで）
    /// </summary>
    /// <param name="rewards">報酬のリスト（TreasureReward型）</param>
    private void ShowRewards(List<TreasureReward> rewards)
    {
        // 以前のスロットをクリア
        ClearItemSlots();

        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("TreasureScreen: 表示する報酬がありません");
            return;
        }

        // 最大表示数を超えている場合は制限
        int displayCount = Mathf.Min(rewards.Count, maxRewardDisplayCount);
        
        for (int i = 0; i < displayCount; i++)
        {
            CreateRewardSlot(rewards[i]);
        }

        Debug.Log($"TreasureScreen: {displayCount}個の報酬を表示（最大{maxRewardDisplayCount}個まで）");
    }

    /// <summary>
    /// 報酬スロットを生成
    /// </summary>
    /// <param name="reward">報酬データ（TreasureReward型）</param>
    private void CreateRewardSlot(TreasureReward reward)
    {
        if (itemContainer == null)
        {
            Debug.LogError("TreasureScreen: itemContainer が設定されていません");
            return;
        }

        if (reward == null)
        {
            Debug.LogWarning("TreasureScreen: 報酬データがnullです");
            return;
        }

        // 報酬の種類に応じてプレハブを選択
        GameObject prefabToUse = null;
        if (reward.type == TreasureRewardType.Item)
        {
            prefabToUse = itemSlotPrefab;
            if (prefabToUse == null)
            {
                Debug.LogError("TreasureScreen: itemSlotPrefab が設定されていません");
                return;
            }
        }
        else
        {
            // お金・スキルポイント用
            prefabToUse = creditSlotPrefab;
            if (prefabToUse == null)
            {
                Debug.LogWarning("TreasureScreen: creditSlotPrefab が設定されていません。itemSlotPrefab を使用します。");
                prefabToUse = itemSlotPrefab; // フォールバック
                if (prefabToUse == null)
                {
                    Debug.LogError("TreasureScreen: itemSlotPrefab も設定されていません");
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
                // TextMeshProUGUIがない場合はTextを使用
                Text itemText = itemNameTextTransform.GetComponent<Text>();
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
            // ItemNameTextが見つからない場合はGetComponentInChildrenで検索（フォールバック）
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
                Text itemText = slot.GetComponentInChildren<Text>();
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

        Debug.Log($"TreasureScreen: 報酬スロット生成 - 種類:{reward.type}, 表示名:{displayName}");
    }

    /// <summary>
    /// 報酬のスプライトを取得
    /// </summary>
    private Sprite GetRewardSprite(TreasureReward reward)
    {
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
    /// レアリティに応じてテキストの色を設定（バトルシーンと同じ色）
    /// </summary>
    private void ApplyRarityColor(TMPro.TextMeshProUGUI text, int rarity)
    {
        if (text == null) return;

        Color targetColor = commonTextColor;
        switch (rarity)
        {
            case 0: // Common
                targetColor = commonTextColor;
                break;
            case 1: // Rare
                targetColor = rareTextColor;
                break;
            case 2: // Epic
                targetColor = epicTextColor;
                break;
        }

        text.color = targetColor;
    }

    /// <summary>
    /// レアリティに応じてテキストの色を設定（Text用）
    /// </summary>
    private void ApplyRarityColor(Text text, int rarity)
    {
        if (text == null) return;

        Color targetColor = commonTextColor;
        switch (rarity)
        {
            case 0: // Common
                targetColor = commonTextColor;
                break;
            case 1: // Rare
                targetColor = rareTextColor;
                break;
            case 2: // Epic
                targetColor = epicTextColor;
                break;
        }

        text.color = targetColor;
    }

    /// <summary>
    /// アイテムIDから日本語表示名を取得
    /// item_L.csのitem_name_listから取得
    /// </summary>
    /// <param name="itemId">アイテムID（0-8）</param>
    /// <returns>日本語表示名</returns>
    private string GetItemDisplayName(int itemId)
    {
        // item_L.csへの参照がある場合はそこから取得
        if (itemData != null && itemData.item_name_list.TryGetValue(itemId, out string name))
        {
            return name;
        }

        // 参照がない場合はフォールバック
        Debug.LogWarning($"TreasureScreen: itemDataが設定されていないか、アイテムID {itemId} が見つかりません");
        return $"アイテム{itemId}";
    }

    /// <summary>
    /// アイテムIDから対応するスプライトを取得
    /// Resources/itemphoto/ フォルダから読み込む
    /// </summary>
    /// <param name="itemId">アイテムID（int: 0-8）</param>
    /// <returns>対応するSprite</returns>
    private Sprite GetItemSprite(int itemId)
    {
        // 画像ファイル名を取得
        if (!itemImageMap.TryGetValue(itemId, out string imageFileName))
        {
            Debug.LogWarning($"TreasureScreen: アイテムID {itemId} の画像ファイル名が登録されていません");
            return null;
        }

        // Resourcesフォルダから読み込む
        // 例: Resources/itemphoto/repair_unit.png → Resources.Load<Sprite>("itemphoto/repair_unit")
        Sprite sprite = Resources.Load<Sprite>($"itemphoto/{imageFileName}");
        
        if (sprite == null)
        {
            Debug.LogWarning($"TreasureScreen: アイテム画像が見つかりません - Resources/itemphoto/{imageFileName}");
        }

        return sprite;
    }

    /// <summary>
    /// 生成したアイテムスロットをすべて削除
    /// </summary>
    private void ClearItemSlots()
    {
        foreach (GameObject slot in spawnedItemSlots)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        spawnedItemSlots.Clear();
    }

    #endregion

    #region ボタン処理

    /// <summary>
    /// 確認ボタンが押された時の処理
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        Debug.Log("TreasureScreen: 確認ボタンがクリックされました");
        onConfirmButtonClicked?.Invoke();
    }

    #endregion

    #region 表示制御

    /// <summary>
    /// 宝箱画面の表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        Initialize(); // 表示時に初期化
        Debug.Log("TreasureScreen: 表示");
    }

    /// <summary>
    /// 宝箱画面の非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        Debug.Log("TreasureScreen: 非表示");
    }

    #endregion
}
