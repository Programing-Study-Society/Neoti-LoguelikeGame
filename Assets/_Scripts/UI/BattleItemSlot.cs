using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// バトル画面用アイテムスロット
/// アイテムIDとレアリティを保持し、クリック時にBattleScreenに通知
/// </summary>
public class BattleItemSlot : MonoBehaviour
{
    [Header("表示要素")]
    [SerializeField] private Image itemImage;          // アイテム画像
    [SerializeField] private TextMeshProUGUI itemNameText;  // アイテム名
    [SerializeField] private TextMeshProUGUI countText;      // 所持数テキスト（×2など）
    [SerializeField] private Button itemButton;        // クリック可能なボタン

    [Header("テキスト色設定")]
    [SerializeField] private Color commonTextColor = Color.white;   // Commonテキスト色（白）
    [SerializeField] private Color rareTextColor = new Color(0f, 0.5f, 1f, 1f);      // Rareテキスト色（青）
    [SerializeField] private Color epicTextColor = new Color(0.5f, 0f, 1f, 1f);   // Epicテキスト色（紫）

    // アイテムデータ
    private int itemId;      // アイテムID (0-8)
    private int rarity;       // レアリティ (0=Common, 1=Rare, 2=Epic)
    private int count;        // 所持数

    // BattleScreenへの参照（生成時に設定）
    private BattleScreen battleScreen;

    private void Awake()
    {
        // ボタンが設定されていない場合は自動取得
        if (itemButton == null)
        {
            itemButton = GetComponent<Button>();
        }

        // ボタンクリックイベントを設定
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// アイテムスロットの初期設定
    /// </summary>
    /// <param name="id">アイテムID (0-8)</param>
    /// <param name="rar">レアリティ (0=Common, 1=Rare, 2=Epic)</param>
    /// <param name="itemCount">所持数</param>
    /// <param name="screen">BattleScreenへの参照</param>
    /// <param name="itemData">item.csへの参照（画像・名前取得用）</param>
    public void Setup(int id, int rar, int itemCount, BattleScreen screen, item itemData)
    {
        itemId = id;
        rarity = rar;
        count = itemCount;
        battleScreen = screen;

        if (itemData == null)
        {
            Debug.LogError("BattleItemSlot: itemDataがnullです");
            return;
        }

        // RectTransformの状態を確認
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            Debug.Log($"BattleItemSlot: RectTransform - 位置: {rect.anchoredPosition}, サイズ: {rect.sizeDelta}, アクティブ: {gameObject.activeSelf}");
        }

        // ButtonコンポーネントのImageを確認
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Debug.Log($"BattleItemSlot: Button Image - enabled: {buttonImage.enabled}, 色: {buttonImage.color}, Sprite: {(buttonImage.sprite != null ? buttonImage.sprite.name : "NULL")}");
        }
        else
        {
            Debug.LogWarning("BattleItemSlot: ButtonコンポーネントにImageが設定されていません");
        }

        // アイテム名を設定
        if (itemNameText != null)
        {
            if (itemData.item_name_list.TryGetValue(itemId, out string itemName))
            {
                itemNameText.text = itemName;
                Debug.Log($"BattleItemSlot: アイテム名を設定 - {itemName}");
            }
            else
            {
                itemNameText.text = $"アイテム{itemId}";
                Debug.LogWarning($"BattleItemSlot: アイテム名が見つかりません - ID:{itemId}");
            }
        }
        else
        {
            Debug.LogWarning("BattleItemSlot: itemNameTextがnullです");
        }

        // 所持数を設定
        if (countText != null)
        {
            countText.text = $"×{count}";
            Debug.Log($"BattleItemSlot: 所持数を設定 - ×{count}");
        }
        else
        {
            Debug.LogWarning("BattleItemSlot: countTextがnullです");
        }

        // アイテム画像を読み込む
        LoadItemImage(itemId);

        // レアリティに応じて色を設定
        ApplyRarityColor();
        
        // レアリティに応じてテキストの色を設定
        ApplyRarityTextColor();

        // UI要素の状態を確認
        Debug.Log($"BattleItemSlot: UI要素の状態確認 - ID:{itemId}");
        Debug.Log($"  - itemImage: {(itemImage != null ? "設定済み" : "NULL")}, 色: {(itemImage != null ? itemImage.color.ToString() : "N/A")}, Sprite: {(itemImage != null && itemImage.sprite != null ? itemImage.sprite.name : "NULL")}");
        
        // itemImageの詳細を確認
        if (itemImage != null)
        {
            RectTransform imageRect = itemImage.GetComponent<RectTransform>();
            if (imageRect != null)
            {
                Debug.Log($"  - itemImage RectTransform - 位置: {imageRect.anchoredPosition}, サイズ: {imageRect.sizeDelta}, アクティブ: {itemImage.gameObject.activeSelf}");
            }
            Debug.Log($"  - itemImage enabled: {itemImage.enabled}, raycastTarget: {itemImage.raycastTarget}");
        }
        
        Debug.Log($"  - itemNameText: {(itemNameText != null ? "設定済み" : "NULL")}, テキスト: {(itemNameText != null ? itemNameText.text : "N/A")}, 色: {(itemNameText != null ? itemNameText.color.ToString() : "N/A")}");
        Debug.Log($"  - countText: {(countText != null ? "設定済み" : "NULL")}, テキスト: {(countText != null ? countText.text : "N/A")}, 色: {(countText != null ? countText.color.ToString() : "N/A")}");
        Debug.Log($"  - itemButton: {(itemButton != null ? "設定済み" : "NULL")}, アクティブ: {(itemButton != null ? itemButton.gameObject.activeSelf.ToString() : "N/A")}");

        Debug.Log($"BattleItemSlot: 設定完了 - ID:{itemId}, レアリティ:{rarity}, 所持数:{count}");
    }

    /// <summary>
    /// アイテム画像を読み込む
    /// </summary>
    private void LoadItemImage(int id)
    {
        if (itemImage == null) return;

        // TreasureScreenと同じ画像マッピングを使用
        Dictionary<int, string> itemImageMap = new Dictionary<int, string>
        {
            { 0, "repair_unit" },
            { 1, "gatling_gun" },
            { 2, "heavy_launcher" },
            { 3, "flamethrower" },
            { 4, "cyber_hack" },
            { 5, "overclock" },
            { 6, "stun_gun" },
            { 7, "field_shield" },
            { 8, "emp_cannon" }
        };

        if (itemImageMap.TryGetValue(id, out string imageName))
        {
            Sprite itemSprite = Resources.Load<Sprite>($"itemphoto/{imageName}");
            if (itemSprite != null)
            {
                itemImage.sprite = itemSprite;
                itemImage.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"BattleItemSlot: アイテム画像が見つかりません - Resources/itemphoto/{imageName}");
            }
        }
    }

    /// <summary>
    /// レアリティに応じて色を設定（画像は常に白）
    /// </summary>
    private void ApplyRarityColor()
    {
        if (itemImage == null) return;

        // 画像の色は常に白（レアリティによる色変更はしない）
        itemImage.color = Color.white;
    }

    /// <summary>
    /// レアリティに応じてテキストの色を設定
    /// </summary>
    private void ApplyRarityTextColor()
    {
        Color targetTextColor = commonTextColor;
        switch (rarity)
        {
            case 0: // Common
                targetTextColor = commonTextColor;
                break;
            case 1: // Rare
                targetTextColor = rareTextColor;
                break;
            case 2: // Epic
                targetTextColor = epicTextColor;
                break;
        }

        // アイテム名テキストの色を変更
        if (itemNameText != null)
        {
            itemNameText.color = targetTextColor;
        }

        // 所持数テキストの色を変更
        if (countText != null)
        {
            countText.color = targetTextColor;
        }
    }

    /// <summary>
    /// ボタンがクリックされた時の処理
    /// </summary>
    private void OnButtonClicked()
    {
        if (battleScreen != null)
        {
            battleScreen.OnItemSelected(itemId, rarity);
        }
        else
        {
            Debug.LogWarning("BattleItemSlot: BattleScreenへの参照が設定されていません");
        }
    }

    /// <summary>
    /// アイテムIDを取得（外部から参照用）
    /// </summary>
    public int GetItemId()
    {
        return itemId;
    }

    /// <summary>
    /// レアリティを取得（外部から参照用）
    /// </summary>
    public int GetRarity()
    {
        return rarity;
    }
}

