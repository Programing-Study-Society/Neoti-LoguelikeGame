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

    [Header("背景設定")]
    [SerializeField] private Image backgroundImage; // 背景画像を表示するImage

    [Header("惑星背景画像")]
    [SerializeField] private Sprite forestPlanetSprite;      // Forest_Planet
    [SerializeField] private Sprite icePlanetSprite;         // Ice_Planet
    [SerializeField] private Sprite oldEmpirePlanetSprite;   // Old_Empire_Planet
    [SerializeField] private Sprite desertPlanetSprite;      // Desert_Planet（未実装）
    [SerializeField] private Sprite volcanoPlanetSprite;     // Volcano_Planet（未実装）
    [SerializeField] private Sprite defaultBackgroundSprite; // デフォルト背景

    [Header("宝箱")]
    [SerializeField] private Image treasureChestImage; // 宝箱画像
    [SerializeField] private Sprite treasureChestClosedSprite; // 閉じた宝箱
    [SerializeField] private Sprite treasureChestOpenSprite;   // 開いた宝箱

    [Header("アイテム表示キャンバス")]
    [SerializeField] private GameObject itemDisplayCanvas; // アイテム表示用のパネル（宝箱を覆う）
    [SerializeField] private Transform itemContainer; // アイテムスロットの親オブジェクト（HorizontalLayoutGroup）

    [Header("アイテムスロットプレハブ")]
    [SerializeField] private GameObject itemSlotPrefab; // アイテム表示用のプレハブ（Image + Text）

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

    // アイテムID → 画像ファイル名のマッピング（ローカル管理）
    private static readonly Dictionary<int, string> itemImageMap = new Dictionary<int, string>
    {
        { 0, "repair_unit" },      // リペアユニット
        { 1, "gatling_gun" },      // ガトリングガン
        { 2, "heavy_launcher" },   // 重装ランチャー
        { 3, "flamethrower" },     // 火炎放射器
        { 4, "cyber_hack" },       // サイバーハックモジュール
        { 5, "overclock" },        // オーバークロックモジュール
        { 6, "stun_gun" },         // スタンガン
        { 7, "field_shield" },     // フィールドシールド
        { 8, "emp_cannon" }        // EMPパルスキャノン
    };

    // 生成されたアイテムスロットのリスト
    private List<GameObject> spawnedItemSlots = new List<GameObject>();
    
    // 現在の惑星
    private PlanetType currentPlanet;
    
    // 演出用の一時保存（int型ID）
    private List<int> pendingItemIds;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// 宝箱画面の初期化
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

        // 以前のアイテムスロットをクリア
        ClearItemSlots();

        Debug.Log("TreasureScreen: 初期化完了");
    }

    #region 惑星背景設定

    /// <summary>
    /// 惑星名（文字列）から背景を設定する
    /// </summary>
    /// <param name="planetName">惑星名（例: "Forest_Planet"）</param>
    public void SetPlanetBackground(string planetName)
    {
        Debug.Log($"TreasureScreen: 惑星背景を設定 - {planetName}");

        if (Enum.TryParse<PlanetType>(planetName, out PlanetType planetType))
        {
            SetPlanetBackground(planetType);
        }
        else
        {
            Debug.LogWarning($"TreasureScreen: 不明な惑星名 '{planetName}'。デフォルト背景を使用します。");
            SetDefaultBackground();
        }
    }

    /// <summary>
    /// 惑星タイプから背景を設定する
    /// </summary>
    /// <param name="planetType">惑星タイプ</param>
    public void SetPlanetBackground(PlanetType planetType)
    {
        currentPlanet = planetType;
        Sprite selectedSprite = GetPlanetSprite(planetType);

        if (selectedSprite != null)
        {
            SetBackgroundSprite(selectedSprite);
            Debug.Log($"TreasureScreen: {planetType} の背景を設定しました");
        }
        else
        {
            Debug.LogWarning($"TreasureScreen: {planetType} のスプライトが設定されていません。デフォルト背景を使用します。");
            SetDefaultBackground();
        }
    }

    /// <summary>
    /// 惑星タイプから対応するスプライトを取得
    /// </summary>
    private Sprite GetPlanetSprite(PlanetType planetType)
    {
        switch (planetType)
        {
            case PlanetType.Forest_Planet:
                return forestPlanetSprite;
            case PlanetType.Ice_Planet:
                return icePlanetSprite;
            case PlanetType.Old_Empire_Planet:
                return oldEmpirePlanetSprite;
            case PlanetType.Desert_Planet:
                return desertPlanetSprite;
            case PlanetType.Volcano_Planet:
                return volcanoPlanetSprite;
            default:
                return null;
        }
    }

    /// <summary>
    /// 背景スプライトを設定
    /// </summary>
    private void SetBackgroundSprite(Sprite sprite)
    {
        if (backgroundImage != null && sprite != null)
        {
            backgroundImage.sprite = sprite;
        }
    }

    /// <summary>
    /// デフォルト背景を設定
    /// </summary>
    private void SetDefaultBackground()
    {
        if (backgroundImage != null && defaultBackgroundSprite != null)
        {
            backgroundImage.sprite = defaultBackgroundSprite;
        }
    }

    /// <summary>
    /// 現在の惑星タイプを取得
    /// </summary>
    public PlanetType GetCurrentPlanet()
    {
        return currentPlanet;
    }

    #endregion

    #region メイン機能（ロジック側から呼び出し）

    /// <summary>
    /// 宝箱画面を表示してアイテムを表示する（演出付き）
    /// ロジック側から呼び出されるメインメソッド
    /// </summary>
    /// <param name="planetName">惑星名（背景画像用）例: "Forest_Planet"</param>
    /// <param name="itemIds">入手したアイテムIDのリスト 例: [0, 1, 2]（int型）</param>
    public void ShowTreasure(string planetName, List<int> itemIds)
    {
        Debug.Log($"TreasureScreen: ShowTreasure呼び出し - 惑星: {planetName}, アイテム数: {itemIds.Count}");

        // 1. 背景を設定
        SetPlanetBackground(planetName);

        // 2. アイテムIDを保存して演出開始
        pendingItemIds = itemIds;
        StartCoroutine(TreasureOpenSequence());
    }

    /// <summary>
    /// 宝箱を開けてアイテムを表示する（演出なし、即時表示）
    /// </summary>
    /// <param name="itemIds">表示するアイテムIDのリスト（int型）</param>
    public void OpenTreasureAndShowItems(List<int> itemIds)
    {
        Debug.Log($"TreasureScreen: 宝箱を開けます（即時）。アイテム数: {itemIds.Count}");

        // 宝箱を開いた状態に
        OpenTreasureChest();

        // アイテムを表示
        ShowItems(itemIds);
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

        // アイテムを表示
        ShowItems(pendingItemIds);

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
    /// アイテムを表示する
    /// </summary>
    /// <param name="itemIds">アイテムIDのリスト（int型: 0-8）</param>
    private void ShowItems(List<int> itemIds)
    {
        // 以前のスロットをクリア
        ClearItemSlots();

        foreach (int itemId in itemIds)
        {
            CreateItemSlot(itemId);
        }

        Debug.Log($"TreasureScreen: {itemIds.Count}個のアイテムを表示");
    }

    /// <summary>
    /// アイテムスロットを生成
    /// </summary>
    /// <param name="itemId">アイテムID（int: 0-8）</param>
    private void CreateItemSlot(int itemId)
    {
        if (itemSlotPrefab == null || itemContainer == null)
        {
            Debug.LogError("TreasureScreen: itemSlotPrefab または itemContainer が設定されていません");
            return;
        }

        // スロットを生成
        GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
        spawnedItemSlots.Add(slot);

        // アイテム画像を設定（itemIdでファイルを読み込む）
        Image itemImage = slot.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            Sprite itemSprite = GetItemSprite(itemId);
            itemImage.sprite = itemSprite != null ? itemSprite : defaultItemSprite;
        }

        // 日本語の表示名を取得（item_L.csから取得）
        string displayName = GetItemDisplayName(itemId);

        // アイテム名テキストを設定（日本語表示名）
        Text itemText = slot.GetComponentInChildren<Text>();
        if (itemText != null)
        {
            itemText.text = displayName;
        }

        // TMPを使用している場合
        TMPro.TextMeshProUGUI itemTMPText = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (itemTMPText != null)
        {
            itemTMPText.text = displayName;
        }

        Debug.Log($"TreasureScreen: アイテムスロット生成 - ID:{itemId} ({displayName})");
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
