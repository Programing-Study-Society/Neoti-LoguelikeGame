using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// バトル画面のUI管理クラス
/// ターン制バトルの表示を担当
/// </summary>
public class BattleScreen : MonoBehaviour
{
    /// <summary>
    /// 惑星の種類（TreasureScreenと共通）
    /// </summary>
    public enum PlanetType
    {
        Forest_Planet,
        Ice_Planet,
        Old_Empire_Planet,
        Desert_Planet,
        Volcano_Planet
    }

    #region SerializeField

    [Header("背景設定")]
    [SerializeField] private Image backgroundImage;

    [Header("惑星背景画像")]
    [SerializeField] private Sprite forestPlanetSprite;
    [SerializeField] private Sprite icePlanetSprite;
    [SerializeField] private Sprite oldEmpirePlanetSprite;
    [SerializeField] private Sprite desertPlanetSprite;
    [SerializeField] private Sprite volcanoPlanetSprite;
    [SerializeField] private Sprite defaultBackgroundSprite;

    [Header("プレイヤー表示")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Slider playerHPBar;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private Color playerPlaceholderColor = Color.blue;

    [Header("敵表示")]
    [SerializeField] private Transform enemyContainer; // 敵を配置する親オブジェクト
    [SerializeField] private GameObject enemySlotPrefab; // 敵表示用プレハブ
    [SerializeField] private Color enemyPlaceholderColor = Color.red;

    [Header("HPバー設定")]
    [SerializeField] private Color hpBarColor = Color.red;
    [SerializeField] private Color hpBarBackgroundColor = Color.black;

    [Header("イベント（ロジック側へ通知）")]
    [SerializeField] private UnityEvent onBattleConfirmed; // 戦闘終了確認時

    [Header("データ参照")]
    [SerializeField] private enemy_L enemyData; // enemy_L.csへの参照（画像ファイル名取得用）
    [SerializeField] private item itemData; // item.csへの参照（アイテムデータ取得用）
    [SerializeField] private TestBattleManager testBattleManager; // テスト用（本番ではbattle_systemへの参照）

    [Header("アイテム選択パネル")]
    [SerializeField] private GameObject itemSelectionPanel; // アイテム選択パネル（中央表示）
    [SerializeField] private Transform itemGridContainer; // アイテムグリッドの親（GridLayoutGroup）
    [SerializeField] private GameObject battleItemSlotPrefab; // バトル用アイテムスロットプレハブ
    [SerializeField] private TextMeshProUGUI itemSelectionTitleText; // タイトルテキスト（オプション）

    #endregion

    #region Private Fields

    // 生成された敵スロットのリスト
    private List<EnemySlot> spawnedEnemySlots = new List<EnemySlot>();

    // 現在の惑星
    private PlanetType currentPlanet;

    // プレイヤーのHP情報
    private int playerCurrentHP;
    private int playerMaxHP;

    // 生成されたアイテムスロットのリスト
    private List<BattleItemSlot> spawnedItemSlots = new List<BattleItemSlot>();

    #endregion

    #region Unity Lifecycle

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

    #endregion

    #region Public Methods - 初期化・表示制御

    /// <summary>
    /// バトル画面の初期化
    /// </summary>
    public void Initialize()
    {
        // プレイヤーのプレースホルダー設定
        if (playerImage != null)
        {
            playerImage.color = playerPlaceholderColor;
        }

        // HPバーの色設定
        if (playerHPBar != null)
        {
            SetSliderColors(playerHPBar, hpBarColor, hpBarBackgroundColor);
        }

        // アイテム選択パネルを初期状態で非表示
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }

        Debug.Log("BattleScreen: 初期化完了");
    }

    /// <summary>
    /// 画面を表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        Debug.Log("BattleScreen: 表示");
    }

    /// <summary>
    /// 画面を非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        Debug.Log("BattleScreen: 非表示");
    }

    #endregion

    #region Public Methods - 戦闘開始

    /// <summary>
    /// 戦闘を開始する（ロジック側から呼び出し）
    /// </summary>
    /// <param name="planetName">惑星名（例: "Forest_Planet"）</param>
    /// <param name="enemies">敵情報のリスト</param>
    /// <param name="playerHP">プレイヤーの現在HP</param>
    /// <param name="playerMaxHP">プレイヤーの最大HP</param>
    public void StartBattle(string planetName, List<EnemyData> enemies, int playerHP, int playerMaxHP)
    {
        Debug.Log($"BattleScreen: 戦闘開始 - 惑星: {planetName}, 敵数: {enemies.Count}");

        // 背景設定
        SetPlanetBackground(planetName);

        // プレイヤーHP設定
        UpdatePlayerHP(playerHP, playerMaxHP);

        // 敵を生成
        SpawnEnemies(enemies);

        // 画面表示
        Show();
    }

    #endregion

    #region Public Methods - HP更新

    /// <summary>
    /// プレイヤーのHPを更新（ロジック側から呼び出し）
    /// </summary>
    public void UpdatePlayerHP(int currentHP, int maxHP)
    {
        playerCurrentHP = currentHP;
        playerMaxHP = maxHP;

        if (playerHPBar != null)
        {
            playerHPBar.maxValue = maxHP;
            playerHPBar.value = currentHP;
        }

        if (playerHPText != null)
        {
            playerHPText.text = $"{currentHP}/{maxHP}";
        }

        Debug.Log($"BattleScreen: プレイヤーHP更新 {currentHP}/{maxHP}");
    }

    /// <summary>
    /// 敵のHPを更新（ロジック側から呼び出し）
    /// </summary>
    /// <param name="enemyIndex">敵のインデックス（0から）</param>
    /// <param name="currentHP">現在HP</param>
    /// <param name="maxHP">最大HP</param>
    public void UpdateEnemyHP(int enemyIndex, int currentHP, int maxHP)
    {
        if (enemyIndex >= 0 && enemyIndex < spawnedEnemySlots.Count)
        {
            spawnedEnemySlots[enemyIndex].UpdateHP(currentHP, maxHP);
            Debug.Log($"BattleScreen: 敵{enemyIndex}のHP更新 {currentHP}/{maxHP}");
        }
        else
        {
            Debug.LogWarning($"BattleScreen: 敵インデックス {enemyIndex} が範囲外です");
        }
    }

    #endregion

    #region Public Methods - 敵撃破

    /// <summary>
    /// 敵が撃破された時の処理（ロジック側から呼び出し）
    /// </summary>
    public void OnEnemyDefeated(int enemyIndex)
    {
        if (enemyIndex >= 0 && enemyIndex < spawnedEnemySlots.Count)
        {
            spawnedEnemySlots[enemyIndex].OnDefeated();
            Debug.Log($"BattleScreen: 敵{enemyIndex}を撃破");
        }
    }

    #endregion

    #region Public Methods - ターン表示

    /// <summary>
    /// プレイヤーターンを表示
    /// </summary>
    public void ShowPlayerTurn()
    {
        Debug.Log("BattleScreen: プレイヤーターン");
        // アイテム選択パネルを表示
        ShowItemSelection();
    }

    /// <summary>
    /// 敵ターンを表示
    /// </summary>
    public void ShowEnemyTurn()
    {
        Debug.Log("BattleScreen: 敵ターン");
        // TODO: ターン表示UI（Phase 2以降で実装）
    }

    #endregion

    #region Public Methods - 敵攻撃アニメーション

    /// <summary>
    /// 敵の攻撃アニメーションを実行（ロジック側から呼び出し）
    /// </summary>
    /// <param name="enemyIndex">攻撃する敵のインデックス</param>
    public void PlayEnemyAttackAnimation(int enemyIndex)
    {
        if (enemyIndex >= 0 && enemyIndex < spawnedEnemySlots.Count)
        {
            // プレイヤーの位置を取得
            Vector3 playerPosition = GetPlayerPosition();
            
            // 敵の攻撃アニメーションを実行
            spawnedEnemySlots[enemyIndex].PlayAttackAnimation(playerPosition);
            Debug.Log($"BattleScreen: 敵{enemyIndex}の攻撃アニメーション開始");
        }
        else
        {
            Debug.LogWarning($"BattleScreen: 敵インデックス {enemyIndex} が範囲外です");
        }
    }

    /// <summary>
    /// プレイヤーの位置を取得（敵の攻撃目標として使用）
    /// </summary>
    private Vector3 GetPlayerPosition()
    {
        if (playerImage != null)
        {
            RectTransform playerRect = playerImage.GetComponent<RectTransform>();
            if (playerRect != null)
            {
                return playerRect.anchoredPosition;
            }
        }
        
        // フォールバック：画面左下の位置を返す
        return new Vector3(250f, 200f, 0f);
    }

    #endregion

    #region Private Methods - 背景設定

    /// <summary>
    /// 惑星名から背景を設定
    /// </summary>
    private void SetPlanetBackground(string planetName)
    {
        if (backgroundImage == null)
        {
            Debug.LogWarning("BattleScreen: backgroundImageが設定されていません");
            return;
        }

        // 文字列からEnumに変換
        if (System.Enum.TryParse<PlanetType>(planetName, out PlanetType planetType))
        {
            SetPlanetBackground(planetType);
        }
        else
        {
            Debug.LogWarning($"BattleScreen: 不明な惑星名 '{planetName}' - デフォルト背景を使用");
            SetDefaultBackground();
        }
    }

    /// <summary>
    /// 惑星タイプから背景を設定
    /// </summary>
    private void SetPlanetBackground(PlanetType planetType)
    {
        currentPlanet = planetType;
        Sprite targetSprite = null;

        switch (planetType)
        {
            case PlanetType.Forest_Planet:
                targetSprite = forestPlanetSprite;
                break;
            case PlanetType.Ice_Planet:
                targetSprite = icePlanetSprite;
                break;
            case PlanetType.Old_Empire_Planet:
                targetSprite = oldEmpirePlanetSprite;
                break;
            case PlanetType.Desert_Planet:
                targetSprite = desertPlanetSprite;
                break;
            case PlanetType.Volcano_Planet:
                targetSprite = volcanoPlanetSprite;
                break;
        }

        if (targetSprite != null)
        {
            backgroundImage.sprite = targetSprite;
            Debug.Log($"BattleScreen: 背景を {planetType} に設定");
        }
        else
        {
            SetDefaultBackground();
        }
    }

    /// <summary>
    /// デフォルト背景を設定
    /// </summary>
    private void SetDefaultBackground()
    {
        if (defaultBackgroundSprite != null)
        {
            backgroundImage.sprite = defaultBackgroundSprite;
        }
        Debug.Log("BattleScreen: デフォルト背景を設定");
    }

    #endregion

    #region Private Methods - 敵生成

    /// <summary>
    /// 敵を生成して表示
    /// </summary>
    private void SpawnEnemies(List<EnemyData> enemies)
    {
        // 既存の敵スロットをクリア
        ClearEnemySlots();

        if (enemyContainer == null || enemySlotPrefab == null)
        {
            Debug.LogWarning("BattleScreen: enemyContainerまたはenemySlotPrefabが設定されていません");
            return;
        }

        foreach (var enemy in enemies)
        {
            // 画像がnullの場合、enemy_Lからファイル名を取得して読み込む
            if (enemy.enemySprite == null && enemyData != null)
            {
                // enemyIdが数値の場合、enemy_Lから画像ファイル名を取得
                if (int.TryParse(enemy.enemyId, out int enemyId))
                {
                    string imageName = enemyData.GetEnemyImageName(enemyId);
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        Sprite loadedSprite = Resources.Load<Sprite>($"enemies/{imageName}");
                        if (loadedSprite == null)
                        {
                            Debug.LogWarning($"BattleScreen: 敵画像が見つかりません - Resources/enemies/{imageName}");
                        }
                        else
                        {
                            // EnemyDataのSpriteを直接更新
                            enemy.enemySprite = loadedSprite;
                        }
                    }
                }
            }

            GameObject slotObj = Instantiate(enemySlotPrefab, enemyContainer);
            EnemySlot slot = slotObj.GetComponent<EnemySlot>();

            if (slot != null)
            {
                Debug.Log($"BattleScreen: 敵スロットを生成 - 敵ID: {enemy.enemyId}, 名前: {enemy.enemyName}, HP: {enemy.currentHP}/{enemy.maxHP}");
                slot.Setup(enemy, hpBarColor, hpBarBackgroundColor, enemyPlaceholderColor);
                spawnedEnemySlots.Add(slot);
            }
            else
            {
                Debug.LogError("BattleScreen: EnemySlotコンポーネントがプレハブに見つかりません");
            }
        }

        Debug.Log($"BattleScreen: {enemies.Count}体の敵を生成");
    }

    /// <summary>
    /// 敵スロットをすべて削除
    /// </summary>
    private void ClearEnemySlots()
    {
        foreach (var slot in spawnedEnemySlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        spawnedEnemySlots.Clear();
    }

    #endregion

    #region Private Methods - HPバー設定

    /// <summary>
    /// スライダーの色を設定
    /// </summary>
    private void SetSliderColors(Slider slider, Color fillColor, Color bgColor)
    {
        // Fill部分の色を設定
        Transform fill = slider.fillRect;
        if (fill != null)
        {
            Image fillImage = fill.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = fillColor;
            }
        }

        // Background部分の色を設定
        Transform background = slider.transform.Find("Background");
        if (background != null)
        {
            Image bgImage = background.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = bgColor;
            }
        }
    }

    #endregion

    #region Public Methods - アイテム選択

    /// <summary>
    /// アイテム選択パネルを表示（プレイヤーターン開始時に呼ばれる）
    /// </summary>
    public void ShowItemSelection()
    {
        if (itemSelectionPanel == null)
        {
            Debug.LogWarning("BattleScreen: itemSelectionPanelが設定されていません");
            return;
        }

        if (itemData == null)
        {
            Debug.LogWarning("BattleScreen: itemDataが設定されていません");
            return;
        }

        // パネルを表示
        itemSelectionPanel.SetActive(true);

        // 所持アイテムを表示
        DisplayOwnedItems();

        Debug.Log("BattleScreen: アイテム選択パネルを表示");
    }

    /// <summary>
    /// アイテム選択パネルを非表示
    /// </summary>
    public void HideItemSelection()
    {
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
        }

        // アイテムスロットをクリア
        ClearItemSlots();

        Debug.Log("BattleScreen: アイテム選択パネルを非表示");
    }

    /// <summary>
    /// アイテムが選択された時の処理
    /// </summary>
    /// <param name="itemId">アイテムID (0-8)</param>
    /// <param name="rarity">レアリティ (0=Common, 1=Rare, 2=Epic)</param>
    public void OnItemSelected(int itemId, int rarity)
    {
        Debug.Log($"BattleScreen: アイテム選択 - ID:{itemId}, レアリティ:{rarity}");

        // パネルを閉じる
        HideItemSelection();

        // TestBattleManagerに通知（テスト用）
        if (testBattleManager != null)
        {
            testBattleManager.OnItemSelected(itemId, rarity);
        }

        // TODO: battle_systemに通知する処理を追加（本番用）
        // battleSystem?.OnItemUsed(itemId, rarity);
    }

    #endregion

    #region Private Methods - アイテム表示

    /// <summary>
    /// 所持アイテムを表示
    /// </summary>
    private void DisplayOwnedItems()
    {
        Debug.Log("BattleScreen: DisplayOwnedItems() 開始");

        if (itemData == null)
        {
            Debug.LogError("BattleScreen: itemDataが設定されていません");
            return;
        }

        if (itemGridContainer == null)
        {
            Debug.LogError("BattleScreen: itemGridContainerが設定されていません");
            return;
        }

        if (battleItemSlotPrefab == null)
        {
            Debug.LogError("BattleScreen: battleItemSlotPrefabが設定されていません");
            return;
        }

        // 既存のスロットをクリア
        ClearItemSlots();
        
        // Content Size Fitterを再有効化（スロット生成前に）
        UnityEngine.UI.ScrollRect scrollRectForFitter = itemGridContainer?.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scrollRectForFitter != null && scrollRectForFitter.content != null)
        {
            UnityEngine.UI.ContentSizeFitter contentSizeFitter = scrollRectForFitter.content.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            if (contentSizeFitter != null)
            {
                contentSizeFitter.enabled = true; // 再有効化
                Debug.Log("BattleScreen: DisplayOwnedItems - ContentSizeFitterを再有効化");
            }
            else
            {
                // Content Size Fitterがない場合は追加
                contentSizeFitter = scrollRectForFitter.content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                contentSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                Debug.Log("BattleScreen: DisplayOwnedItems - ContentSizeFitterを追加しました");
            }
        }
        
        Debug.Log($"BattleScreen: item_listの要素数: {itemData.item_list.Count}");
        Debug.Log($"BattleScreen: itemDataのインスタンスID: {itemData.GetInstanceID()}");

        int totalItemsFound = 0;

        // item_listから所持アイテムを取得して表示
        foreach (var kvp in itemData.item_list)
        {
            int itemId = kvp.Key;
            List<int> counts = kvp.Value; // [common, rare, epic]

            if (counts == null || counts.Count < 3)
            {
                Debug.LogWarning($"BattleScreen: アイテムID {itemId} のcountsが不正です - counts: {counts}");
                continue;
            }

            Debug.Log($"BattleScreen: アイテムID {itemId} - Common:{counts[0]}, Rare:{counts[1]}, Epic:{counts[2]}");

            // 各レアリティの所持数をチェック
            for (int rarity = 0; rarity < counts.Count && rarity < 3; rarity++)
            {
                int count = counts[rarity];
                if (count > 0)
                {
                    // 所持数が0より大きい場合のみ表示
                    Debug.Log($"BattleScreen: アイテムスロットを生成 - ID:{itemId}, レアリティ:{rarity}, 所持数:{count}");
                    CreateItemSlot(itemId, rarity, count);
                    totalItemsFound++;
                }
            }
        }

        Debug.Log($"BattleScreen: {spawnedItemSlots.Count}個のアイテムを表示（検出数: {totalItemsFound}）");
        
        // itemGridContainerの状態を確認
        if (itemGridContainer != null)
        {
            RectTransform containerRect = itemGridContainer.GetComponent<RectTransform>();
            if (containerRect != null)
            {
                Debug.Log($"BattleScreen: itemGridContainer - 位置: {containerRect.anchoredPosition}, サイズ: {containerRect.sizeDelta}, アクティブ: {itemGridContainer.gameObject.activeSelf}");
            }
            
            // GridLayoutGroupの設定を確認
            UnityEngine.UI.GridLayoutGroup grid = itemGridContainer.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            if (grid != null)
            {
                Debug.Log($"BattleScreen: GridLayoutGroup - Cell Size: {grid.cellSize}, Spacing: {grid.spacing}, Constraint: {grid.constraint}, Constraint Count: {grid.constraintCount}");
                
                // レイアウトを強制的に更新
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
                Debug.Log("BattleScreen: GridLayoutGroupのレイアウトを強制更新しました");
            }
            else
            {
                Debug.LogWarning("BattleScreen: itemGridContainerにGridLayoutGroupが設定されていません");
            }
            
            // 子要素の数を確認
            Debug.Log($"BattleScreen: itemGridContainerの子要素数: {itemGridContainer.childCount}");
            
            // 各子要素の状態を確認（レイアウト更新後）
            for (int i = 0; i < itemGridContainer.childCount; i++)
            {
                Transform child = itemGridContainer.GetChild(i);
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    Debug.Log($"BattleScreen: 子要素[{i}] - 名前: {child.name}, 位置: {childRect.anchoredPosition}, サイズ: {childRect.sizeDelta}, アクティブ: {child.gameObject.activeSelf}");
                }
            }
            
            // ScrollRectを使っている場合、Contentのサイズを手動で計算
            UnityEngine.UI.ScrollRect scrollRect = itemGridContainer.GetComponentInParent<UnityEngine.UI.ScrollRect>();
            if (scrollRect != null && scrollRect.content != null)
            {
                Debug.Log("BattleScreen: ScrollRectを検出 - Contentのサイズを手動で計算します");
                
                // スクロール速度を上げる
                scrollRect.scrollSensitivity = 50f; // デフォルトは1、大きくすると速くなる
                Debug.Log($"BattleScreen: ScrollRectのスクロール速度を設定 - Sensitivity: {scrollRect.scrollSensitivity}");
                
                // Content Size Fitterを無効化（手動計算に切り替え）
                UnityEngine.UI.ContentSizeFitter contentSizeFitter = scrollRect.content.GetComponent<UnityEngine.UI.ContentSizeFitter>();
                if (contentSizeFitter != null)
                {
                    contentSizeFitter.enabled = false;
                    Debug.Log("BattleScreen: ContentSizeFitterを無効化（手動計算に切り替え）");
                }
                
                // GridLayoutGroupからContentのHeightを計算（既に取得したgrid変数を使用）
                if (grid != null && itemGridContainer.childCount > 0)
                {
                    // 行数を計算（3列固定）
                    int rowCount = Mathf.CeilToInt((float)itemGridContainer.childCount / grid.constraintCount);
                    
                    // ContentのHeight = (行数 × セル高さ) + ((行数 - 1) × 行間隔) + 余白
                    float contentHeight = (rowCount * grid.cellSize.y) + ((rowCount - 1) * grid.spacing.y);
                    
                    // ContentのRectTransformを取得
                    RectTransform contentRect = scrollRect.content;
                    contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
                    
                    Debug.Log($"BattleScreen: ContentのHeightを手動計算 - 行数: {rowCount}, Height: {contentHeight}");
                }
                else
                {
                    Debug.LogWarning("BattleScreen: GridLayoutGroupが見つからないか、子要素がありません");
                }
                
                // 1フレーム待ってから再度更新（レイアウト計算が完了するまで待つ）
                StartCoroutine(UpdateContentSizeDelayed(scrollRect.content));
                
                Debug.Log($"BattleScreen: Contentのサイズ更新後 - Height: {scrollRect.content.rect.height}");
            }
        }
    }

    /// <summary>
    /// Contentのサイズを遅延更新（レイアウト計算が完了するまで待つ）
    /// </summary>
    private IEnumerator UpdateContentSizeDelayed(RectTransform content)
    {
        yield return null; // 1フレーム待つ
        
        if (content != null && itemGridContainer != null)
        {
            // GridLayoutGroupからContentのHeightを再計算
            UnityEngine.UI.GridLayoutGroup grid = itemGridContainer.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            if (grid != null && itemGridContainer.childCount > 0)
            {
                // 行数を計算（3列固定）
                int rowCount = Mathf.CeilToInt((float)itemGridContainer.childCount / grid.constraintCount);
                
                // ContentのHeight = (行数 × セル高さ) + ((行数 - 1) × 行間隔)
                float contentHeight = (rowCount * grid.cellSize.y) + ((rowCount - 1) * grid.spacing.y);
                
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
                Debug.Log($"BattleScreen: Contentのサイズを遅延更新 - 行数: {rowCount}, Height: {contentHeight}");
            }
        }
    }

    /// <summary>
    /// アイテムスロットを生成
    /// </summary>
    /// <param name="itemId">アイテムID (0-8)</param>
    /// <param name="rarity">レアリティ (0=Common, 1=Rare, 2=Epic)</param>
    /// <param name="count">所持数</param>
    private void CreateItemSlot(int itemId, int rarity, int count)
    {
        if (battleItemSlotPrefab == null)
        {
            Debug.LogError("BattleScreen: battleItemSlotPrefabがnullです");
            return;
        }

        if (itemGridContainer == null)
        {
            Debug.LogError("BattleScreen: itemGridContainerがnullです");
            return;
        }

        GameObject slotObj = Instantiate(battleItemSlotPrefab, itemGridContainer);
        
        if (slotObj == null)
        {
            Debug.LogError("BattleScreen: プレハブのインスタンス化に失敗しました");
            return;
        }

        // RectTransformの状態を確認
        RectTransform slotRect = slotObj.GetComponent<RectTransform>();
        if (slotRect != null)
        {
            Debug.Log($"BattleScreen: スロットRectTransform - 位置: {slotRect.anchoredPosition}, サイズ: {slotRect.sizeDelta}, アクティブ: {slotObj.activeSelf}, 親: {slotRect.parent?.name}");
            
            // スロットが正しく親に設定されているか確認
            if (slotRect.parent != itemGridContainer)
            {
                Debug.LogWarning($"BattleScreen: スロットの親が正しく設定されていません - 親: {slotRect.parent?.name}, 期待: {itemGridContainer.name}");
            }
        }
        else
        {
            Debug.LogError("BattleScreen: スロットにRectTransformがありません");
        }

        BattleItemSlot slot = slotObj.GetComponent<BattleItemSlot>();

        if (slot != null)
        {
            slot.Setup(itemId, rarity, count, this, itemData);
            spawnedItemSlots.Add(slot);
            Debug.Log($"BattleScreen: アイテムスロット生成成功 - ID:{itemId}, レアリティ:{rarity}, 所持数:{count}, 親: {itemGridContainer.name}");
        }
        else
        {
            Debug.LogError("BattleScreen: BattleItemSlotコンポーネントがプレハブに見つかりません");
            Destroy(slotObj);
        }
    }

    /// <summary>
    /// アイテムスロットをすべて削除
    /// </summary>
    private void ClearItemSlots()
    {
        // Content Size Fitterを一時的に無効化（スロット削除時にサイズが0になるのを防ぐ）
        UnityEngine.UI.ScrollRect scrollRect = itemGridContainer?.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        UnityEngine.UI.ContentSizeFitter contentSizeFitter = null;
        bool wasEnabled = false;
        
        if (scrollRect != null && scrollRect.content != null)
        {
            contentSizeFitter = scrollRect.content.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            if (contentSizeFitter != null)
            {
                wasEnabled = contentSizeFitter.enabled;
                contentSizeFitter.enabled = false; // 一時的に無効化
                Debug.Log("BattleScreen: ClearItemSlots - ContentSizeFitterを一時的に無効化");
            }
        }
        
        foreach (var slot in spawnedItemSlots)
        {
            if (slot != null && slot.gameObject != null)
            {
                Destroy(slot.gameObject);
            }
        }
        spawnedItemSlots.Clear();
        
        // Content Size Fitterを再有効化（新しいスロットが生成されるまで待つ）
        if (contentSizeFitter != null && wasEnabled)
        {
            // スロット生成後に再有効化される（DisplayOwnedItems内で）
            Debug.Log("BattleScreen: ClearItemSlots - ContentSizeFitterはDisplayOwnedItems内で再有効化されます");
        }
    }

    #endregion
}

/// <summary>
/// 敵のデータ（ロジック側から渡される）
/// </summary>
[System.Serializable]
public class EnemyData
{
    public string enemyId;      // 敵のID
    public string enemyName;    // 敵の表示名
    public int currentHP;       // 現在HP
    public int maxHP;           // 最大HP
    public Sprite enemySprite;  // 敵の画像（nullならプレースホルダー）

    public EnemyData(string id, string name, int hp, int maxHp, Sprite sprite = null)
    {
        enemyId = id;
        enemyName = name;
        currentHP = hp;
        maxHP = maxHp;
        enemySprite = sprite;
    }
}

