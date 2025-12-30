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
    [SerializeField] private item itemData; // item.csへの参照（アイテムデータ取得用、名前・画像取得用）
    [SerializeField] private battle_system battleSystem; // バトルシステム（ロジック側、A案：ロジックが正）

    [Header("アイテム選択パネル")]
    [SerializeField] private GameObject itemSelectionPanel; // アイテム選択パネル（中央表示）
    [SerializeField] private Transform itemGridContainer; // アイテムグリッドの親（GridLayoutGroup）
    [SerializeField] private GameObject battleItemSlotPrefab; // バトル用アイテムスロットプレハブ
    [SerializeField] private TextMeshProUGUI itemSelectionTitleText; // タイトルテキスト（オプション）
    
    [Header("攻撃ボタン")]
    [SerializeField] private GameObject attackButtonPanel; // 攻撃ボタンパネル（アイテム選択後に表示）
    [SerializeField] private Transform attackButtonContainer; // 攻撃ボタンの親（HorizontalLayoutGroupなど）
    [SerializeField] private GameObject attackButtonPrefab; // 攻撃ボタンプレハブ

    [Header("ダメージ表示")]
    [SerializeField] private GameObject damageTextPrefab; // ダメージ表示用プレハブ（TextMeshProUGUI）
    [SerializeField] private Transform damageTextParent; // ダメージテキストの親（BattleScreen直下推奨、未設定時はBattleScreen自身を使用）
    [SerializeField] private float damageTextDuration = 1.5f; // ダメージ表示時間（秒）
    [SerializeField] private float damageTextMoveDistance = 100f; // ダメージテキストの移動距離（ピクセル）
    [SerializeField] private Color playerDamageColor = Color.red; // プレイヤー被ダメージの色
    [SerializeField] private Color enemyDamageColor = Color.yellow; // 敵被ダメージの色

    [Header("敵遭遇表示")]
    [SerializeField] private GameObject encounterPanel; // 敵遭遇表示パネル
    [SerializeField] private TextMeshProUGUI encounterTitleText; // 「敵に遭遇！」テキスト
    [SerializeField] private Transform encounterEnemyContainer; // 敵表示用コンテナ（HorizontalLayoutGroupなど）
    [SerializeField] private GameObject encounterEnemySlotPrefab; // 敵遭遇表示用スロットプレハブ（Image + TextMeshProUGUI）
    [SerializeField] private float encounterDisplayDuration = 3f; // 敵遭遇表示時間（秒）

    #endregion

    #region Private Fields

    // 生成された敵スロットのリスト
    private List<EnemySlot> spawnedEnemySlots = new List<EnemySlot>();

    // プレイヤーのHP情報
    private int playerCurrentHP;
    private int playerMaxHP;

    // 生成されたアイテムスロットのリスト
    private List<BattleItemSlot> spawnedItemSlots = new List<BattleItemSlot>();
    
    // 選択されたアイテム情報
    private int selectedItemId = -1; // 選択されたアイテムID（-1は未選択）
    private int selectedItemRarity = -1; // 選択されたアイテムのレアリティ（-1は未選択）
    
    // 生成された攻撃ボタンのリスト
    private List<GameObject> spawnedAttackButtons = new List<GameObject>();
    
    // 遭遇メッセージ表示中かどうか
    private bool isShowingEncounterMessage = false;
    
    // 遭遇メッセージ非表示後にアイテム選択を表示する必要があるか
    private bool shouldShowItemSelectionAfterEncounter = false;

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
        
        // 攻撃ボタンパネルを初期状態で非表示
        if (attackButtonPanel != null)
        {
            attackButtonPanel.SetActive(false);
        }
        
        // 敵遭遇メッセージを初期状態で非表示
        try
        {
            if (encounterPanel != null && encounterPanel.gameObject != null)
            {
                encounterPanel.SetActive(false);
            }
            if (encounterTitleText != null && encounterTitleText.gameObject != null)
            {
                encounterTitleText.gameObject.SetActive(false);
            }
        }
        catch (MissingReferenceException)
        {
            Debug.LogWarning("BattleScreen: 遭遇メッセージオブジェクトが破棄されています（Initialize時）");
        }
        
        // 選択状態をリセット
        selectedItemId = -1;
        selectedItemRarity = -1;

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

        // 背景設定は削除（summary.csの起動時に一度だけ設定される）

        // プレイヤーHP設定
        UpdatePlayerHP(playerHP, playerMaxHP);

        // 敵を生成
        SpawnEnemies(enemies);

        // 画面表示
        Show();

        // 敵遭遇メッセージを表示
        ShowEncounterMessage();
    }

    /// <summary>
    /// 敵遭遇メッセージを表示（数秒後に自動で非表示）
    /// </summary>
    private void ShowEncounterMessage()
    {
        isShowingEncounterMessage = true;
        
        // encounterPanelまたはencounterTitleTextが設定されている場合
        if (encounterPanel != null)
        {
            encounterPanel.SetActive(true);
            Debug.Log("BattleScreen: 敵遭遇メッセージを表示");
            
            // 数秒後に非表示にする
            StartCoroutine(HideEncounterMessageAfterDelay());
        }
        else if (encounterTitleText != null)
        {
            encounterTitleText.gameObject.SetActive(true);
            encounterTitleText.text = "敵に遭遇！";
            Debug.Log("BattleScreen: 敵遭遇メッセージを表示");
            
            // 数秒後に非表示にする
            StartCoroutine(HideEncounterMessageAfterDelay());
        }
        else
        {
            Debug.LogWarning("BattleScreen: encounterPanelまたはencounterTitleTextが設定されていません");
            isShowingEncounterMessage = false;
        }
    }

    /// <summary>
    /// 敵遭遇メッセージを指定時間後に非表示にするコルーチン
    /// </summary>
    private IEnumerator HideEncounterMessageAfterDelay()
    {
        yield return new WaitForSeconds(encounterDisplayDuration);
        
        if (encounterPanel != null)
        {
            encounterPanel.SetActive(false);
            Debug.Log("BattleScreen: 敵遭遇メッセージを非表示");
        }
        else if (encounterTitleText != null)
        {
            encounterTitleText.gameObject.SetActive(false);
            Debug.Log("BattleScreen: 敵遭遇メッセージを非表示");
        }
        
        // 遭遇メッセージ表示終了
        isShowingEncounterMessage = false;
        
        // 遭遇メッセージ非表示後にアイテム選択を表示する必要がある場合
        if (shouldShowItemSelectionAfterEncounter)
        {
            shouldShowItemSelectionAfterEncounter = false;
            ShowItemSelection();
            Debug.Log("BattleScreen: 遭遇メッセージ非表示後、アイテム選択画面を表示");
        }
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
            // 撃破アニメーションを実行
            spawnedEnemySlots[enemyIndex].OnDefeated();
            
            // スロットを削除（後でDestroy）
            EnemySlot defeatedSlot = spawnedEnemySlots[enemyIndex];
            spawnedEnemySlots.RemoveAt(enemyIndex);
            
            // スロットのGameObjectを破棄
            if (defeatedSlot != null && defeatedSlot.gameObject != null)
            {
                Destroy(defeatedSlot.gameObject);
            }
            
            Debug.Log($"BattleScreen: 敵{enemyIndex}を撃破してスロットを削除 - 残り敵数: {spawnedEnemySlots.Count}");
        }
        else
        {
            Debug.LogWarning($"BattleScreen: 無効な敵インデックス: {enemyIndex} (スロット数: {spawnedEnemySlots.Count})");
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
        
        // 遭遇メッセージが表示中の場合は、非表示後にアイテム選択を表示
        if (isShowingEncounterMessage)
        {
            shouldShowItemSelectionAfterEncounter = true;
            Debug.Log("BattleScreen: 遭遇メッセージ表示中。非表示後にアイテム選択画面を表示します");
        }
        else
        {
            // 遭遇メッセージが表示されていない場合は即座にアイテム選択パネルを表示
            ShowItemSelection();
        }
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

    /// <summary>
    /// プレイヤーへのダメージを表示
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void ShowPlayerDamage(int damage)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogWarning("BattleScreen: damageTextPrefabが設定されていません");
            return;
        }

        Vector3 position = GetPlayerPosition();
        ShowDamageText(damage, position, playerDamageColor);
        Debug.Log($"BattleScreen: プレイヤーへのダメージ表示 - {damage}");
    }

    /// <summary>
    /// 敵へのダメージを表示
    /// </summary>
    /// <param name="enemyIndex">敵のインデックス</param>
    /// <param name="damage">ダメージ量</param>
    public void ShowEnemyDamage(int enemyIndex, int damage)
    {
        if (enemyIndex < 0 || enemyIndex >= spawnedEnemySlots.Count)
        {
            Debug.LogWarning($"BattleScreen: 無効な敵インデックス: {enemyIndex}");
            return;
        }

        if (damageTextPrefab == null)
        {
            Debug.LogWarning("BattleScreen: damageTextPrefabが設定されていません");
            return;
        }

        // 敵の位置を取得
        EnemySlot enemySlot = spawnedEnemySlots[enemyIndex];
        if (enemySlot == null)
        {
            Debug.LogWarning($"BattleScreen: 敵スロットが見つかりません - インデックス: {enemyIndex}");
            return;
        }

        RectTransform enemyRect = enemySlot.GetComponent<RectTransform>();
        if (enemyRect == null)
        {
            Debug.LogWarning($"BattleScreen: 敵のRectTransformが見つかりません - インデックス: {enemyIndex}");
            return;
        }

        Vector3 position = enemyRect.anchoredPosition;
        ShowDamageText(damage, position, enemyDamageColor);
        Debug.Log($"BattleScreen: 敵{enemyIndex}へのダメージ表示 - {damage}");
    }

    /// <summary>
    /// ダメージテキストを表示（アニメーション付き）
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="position">表示位置</param>
    /// <param name="color">テキストの色</param>
    private void ShowDamageText(int damage, Vector3 position, Color color)
    {
        if (damageTextPrefab == null) return;

        // 親を決定（damageTextParentが設定されていればそれを使用、なければBattleScreen自身を使用）
        Transform parent = damageTextParent;
        if (parent == null)
        {
            // BattleScreenオブジェクトを親として使用（バトル画面専用のUIなので）
            parent = transform;
        }

        // ダメージテキストを生成
        GameObject damageObj = Instantiate(damageTextPrefab, parent);
        if (damageObj == null)
        {
            Debug.LogError("BattleScreen: ダメージテキストの生成に失敗しました");
            return;
        }

        // RectTransformを取得して位置を設定
        RectTransform damageRect = damageObj.GetComponent<RectTransform>();
        if (damageRect != null)
        {
            damageRect.anchoredPosition = position;
        }

        // TextMeshProUGUIを取得してテキストと色を設定
        TextMeshProUGUI damageText = damageObj.GetComponent<TextMeshProUGUI>();
        if (damageText == null)
        {
            damageText = damageObj.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (damageText != null)
        {
            damageText.text = $"-{damage}";
            damageText.color = color;
        }
        else
        {
            Debug.LogWarning("BattleScreen: ダメージテキストにTextMeshProUGUIが見つかりません");
        }

        // アニメーションを開始
        StartCoroutine(DamageTextAnimation(damageObj, damageRect, damageText));
    }

    /// <summary>
    /// ダメージテキストのアニメーション（フェードアウト＋上に移動）
    /// </summary>
    private IEnumerator DamageTextAnimation(GameObject damageObj, RectTransform damageRect, TextMeshProUGUI damageText)
    {
        if (damageObj == null || damageRect == null || damageText == null) yield break;

        Vector3 startPosition = damageRect.anchoredPosition;
        Vector3 endPosition = startPosition + new Vector3(0f, damageTextMoveDistance, 0f);
        Color startColor = damageText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;
        while (elapsed < damageTextDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / damageTextDuration;

            // 位置を上に移動
            damageRect.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);

            // フェードアウト
            if (damageText != null)
            {
                damageText.color = Color.Lerp(startColor, endColor, t);
            }

            yield return null;
        }

        // アニメーション終了後に削除
        if (damageObj != null)
        {
            Destroy(damageObj);
        }
    }

    #endregion

    #region Private Methods - 背景設定


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
        
        // パネルの状態を確認
        Debug.Log($"BattleScreen: アイテム選択パネルを表示 - Active: {itemSelectionPanel.activeSelf}, ActiveInHierarchy: {itemSelectionPanel.activeInHierarchy}");

        // 所持アイテムを表示
        DisplayOwnedItems();
        
        // ScrollRectを有効化して、クリックしなくてもスクロールできるようにする
        EnableScrollRectInteraction();

        Debug.Log("BattleScreen: アイテム選択パネルを表示完了");
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
        
        // 攻撃ボタンもクリア
        ClearAttackButtons();
        
        // 攻撃ボタンパネルを非表示
        if (attackButtonPanel != null)
        {
            attackButtonPanel.SetActive(false);
        }
        
        // 選択状態をリセット
        selectedItemId = -1;
        selectedItemRarity = -1;

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

        // 選択されたアイテム情報を保存
        selectedItemId = itemId;
        selectedItemRarity = rarity;

        // アイテム選択パネル（スクロール可能なパネル全体）を非表示
        if (itemSelectionPanel != null)
        {
            itemSelectionPanel.SetActive(false);
            Debug.Log("BattleScreen: アイテム選択パネル（スクロール可能なパネル全体）を非表示");
        }
        else
        {
            // itemSelectionPanelがnullの場合、itemGridContainerから親のScrollRectを探して非表示にする
            if (itemGridContainer != null)
            {
                UnityEngine.UI.ScrollRect scrollRect = itemGridContainer.GetComponentInParent<UnityEngine.UI.ScrollRect>();
                if (scrollRect != null)
                {
                    scrollRect.gameObject.SetActive(false);
                    Debug.Log("BattleScreen: ScrollRectを非表示（itemSelectionPanelがnullのため）");
                }
            }
        }

        // 攻撃ボタンを表示
        ShowAttackButtons();
    }
    
    /// <summary>
    /// 攻撃ボタンを表示（アイテムの種類に応じて動的に生成）
    /// </summary>
    private void ShowAttackButtons()
    {
        if (attackButtonPanel == null)
        {
            Debug.LogWarning("BattleScreen: attackButtonPanelが設定されていません");
            return;
        }

        if (attackButtonContainer == null)
        {
            Debug.LogWarning("BattleScreen: attackButtonContainerが設定されていません");
            return;
        }

        if (attackButtonPrefab == null)
        {
            Debug.LogWarning("BattleScreen: attackButtonPrefabが設定されていません");
            return;
        }

        // 既存の攻撃ボタンをクリア
        ClearAttackButtons();

        // 選択されたアイテムの種類を判定
        if (selectedItemId < 0)
        {
            Debug.LogWarning("BattleScreen: アイテムが選択されていません");
            return;
        }

        // アイテムの種類に応じてボタンを生成
        // 自分へのバフ・回復（Self）: アイテム0（Heal）、5（BuffATK）、7（BuffDEF）
        if (selectedItemId == 0 || selectedItemId == 5 || selectedItemId == 7)
        {
            // 「アイテムを使用する」ボタン1つ（enemyIndex = -1で自分に使用）
            CreateAttackButton(-1, "アイテムを使用する");
        }
        // 範囲攻撃（OpponentAll）: アイテム8（AoEDamage）
        else if (selectedItemId == 8)
        {
            // 「攻撃する」ボタン1つ（enemyIndex = -1で全敵に攻撃）
            CreateAttackButton(-1, "攻撃する");
        }
        // 単体攻撃・デバフ（OpponentSingle）: アイテム1,2,3,4,6
        else
        {
            // 敵の数を取得
            int enemyCount = spawnedEnemySlots.Count;

            if (enemyCount == 0)
            {
                Debug.LogWarning("BattleScreen: 敵が存在しません");
                return;
            }

            // 攻撃ボタンを生成（現在の設定のまま）
            if (enemyCount == 1)
            {
                // 敵が1体の場合：「攻撃する」ボタン1つ
                CreateAttackButton(0, "攻撃する");
            }
            else
            {
                // 敵が2体以上の場合：各敵ごとにボタン
                for (int i = 0; i < enemyCount; i++)
                {
                    string buttonText = GetEnemyButtonText(i, enemyCount);
                    CreateAttackButton(i, buttonText);
                }
            }
        }

        // 攻撃ボタンパネルを表示
        attackButtonPanel.SetActive(true);
        Debug.Log($"BattleScreen: 攻撃ボタンを表示 - アイテムID: {selectedItemId}");
    }

    /// <summary>
    /// 敵のインデックスからボタンテキストを取得
    /// </summary>
    private string GetEnemyButtonText(int enemyIndex, int totalEnemies)
    {
        if (totalEnemies == 1)
        {
            return "攻撃する";
        }
        else if (totalEnemies == 2)
        {
            return enemyIndex == 0 ? "左の敵に攻撃" : "右の敵に攻撃";
        }
        else
        {
            // 3体以上の場合
            if (enemyIndex == 0)
            {
                return "左の敵に攻撃";
            }
            else if (enemyIndex == totalEnemies - 1)
            {
                return "右の敵に攻撃";
            }
            else
            {
                return $"敵{enemyIndex + 1}に攻撃";
            }
        }
    }

    /// <summary>
    /// 攻撃ボタンを生成
    /// </summary>
    private void CreateAttackButton(int enemyIndex, string buttonText)
    {
        GameObject buttonObj = Instantiate(attackButtonPrefab, attackButtonContainer);
        
        if (buttonObj == null)
        {
            Debug.LogError("BattleScreen: 攻撃ボタンの生成に失敗しました");
            return;
        }

        // ボタンのテキストを設定
        UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            TextMeshProUGUI buttonTextComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonTextComponent != null)
            {
                buttonTextComponent.text = buttonText;
            }

            // ボタンクリックイベントを設定
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAttackButtonClicked(enemyIndex));
        }

        spawnedAttackButtons.Add(buttonObj);
        Debug.Log($"BattleScreen: 攻撃ボタンを生成 - 敵インデックス: {enemyIndex}, テキスト: {buttonText}");
    }

    /// <summary>
    /// 攻撃ボタンがクリックされた時の処理
    /// </summary>
    private void OnAttackButtonClicked(int enemyIndex)
    {
        if (selectedItemId < 0 || selectedItemRarity < 0)
        {
            Debug.LogWarning("BattleScreen: アイテムが選択されていません");
            return;
        }

        // enemyIndex = -1 の場合は自分に使用または全敵に攻撃（範囲攻撃）
        // enemyIndex >= 0 の場合は単体の敵に攻撃
        if (enemyIndex >= 0 && enemyIndex >= spawnedEnemySlots.Count)
        {
            Debug.LogWarning($"BattleScreen: 無効な敵インデックス: {enemyIndex}");
            return;
        }

        // HideItemSelection() 内で selectedItemId / selectedItemRarity がリセットされるため、
        // ここでローカル変数に退避してから使用する
        int usedItemId = selectedItemId;
        int usedItemRarity = selectedItemRarity;

        Debug.Log($"BattleScreen: 攻撃ボタンクリック - アイテムID: {usedItemId}, レアリティ: {usedItemRarity}, 敵インデックス: {enemyIndex}");

        // 攻撃ボタンパネルを非表示
        if (attackButtonPanel != null)
        {
            attackButtonPanel.SetActive(false);
        }

        // アイテム選択パネルも非表示（念のため）
        HideItemSelection();

        // battle_systemに通知（本番用）
        // enemyIndex = -1 の場合は自分に使用または全敵に攻撃
        // enemyIndex >= 0 の場合は単体の敵に攻撃
        if (battleSystem != null)
        {
            battleSystem.OnItemUsed(usedItemId, usedItemRarity, enemyIndex);
        }

        // 選択状態をリセット
        selectedItemId = -1;
        selectedItemRarity = -1;
    }

    /// <summary>
    /// 攻撃ボタンをすべて削除
    /// </summary>
    private void ClearAttackButtons()
    {
        foreach (var button in spawnedAttackButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        spawnedAttackButtons.Clear();
    }

    #endregion

    #region Private Methods - アイテム表示

    /// <summary>
    /// 所持アイテムを表示
    /// A案：ロジックが正で、UIはbattle_systemから取得
    /// </summary>
    private void DisplayOwnedItems()
    {
        Debug.Log("BattleScreen: DisplayOwnedItems() 開始");
        
        // デバッグ: 各参照の状態を確認
        Debug.Log($"BattleScreen: DisplayOwnedItems - battleSystem: {(battleSystem != null ? "設定済み" : "NULL")}");
        Debug.Log($"BattleScreen: DisplayOwnedItems - itemData: {(itemData != null ? "設定済み" : "NULL")}");
        Debug.Log($"BattleScreen: DisplayOwnedItems - itemGridContainer: {(itemGridContainer != null ? "設定済み" : "NULL")}");
        Debug.Log($"BattleScreen: DisplayOwnedItems - battleItemSlotPrefab: {(battleItemSlotPrefab != null ? "設定済み" : "NULL")}");

        // battle_systemからアイテム所持数を取得（A案）
        Dictionary<int, List<int>> itemCounts = null;
        if (battleSystem != null)
        {
            itemCounts = battleSystem.GetBattleItemCounts();
            Debug.Log($"BattleScreen: battle_systemからアイテム所持数を取得 - {itemCounts?.Count ?? 0}種類");
        }
        else if (itemData != null && itemData.item_list != null)
        {
            // フォールバック：itemDataから直接取得（テスト用）
            itemCounts = itemData.item_list;
            Debug.LogWarning("BattleScreen: battleSystemが設定されていないため、itemDataから直接取得しました");
        }
        else
        {
            Debug.LogError("BattleScreen: battleSystemもitemDataも設定されていません");
            return;
        }

        if (itemData == null)
        {
            Debug.LogError("BattleScreen: itemDataが設定されていません（アイテム名・画像取得用）");
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
        
        // デバッグ: itemCountsの内容を確認
        if (itemCounts == null || itemCounts.Count == 0)
        {
            Debug.LogWarning("BattleScreen: itemCountsがnullまたは空です。アイテムが表示されません。");
            return;
        }
        
        Debug.Log($"BattleScreen: itemCountsの要素数: {itemCounts.Count}");

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

        // battle_systemから取得したアイテム所持数をループして表示
        foreach (var kvp in itemCounts)
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

    /// <summary>
    /// ScrollRectを有効化して、クリックしなくてもスクロールできるようにする
    /// </summary>
    private void EnableScrollRectInteraction()
    {
        if (itemGridContainer == null) return;
        
        // ScrollRectを取得
        UnityEngine.UI.ScrollRect scrollRect = itemGridContainer.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scrollRect != null)
        {
            // ScrollRectを有効化
            scrollRect.enabled = true;
            
            // ScrollRectのGameObjectを有効化
            if (scrollRect.gameObject != null)
            {
                scrollRect.gameObject.SetActive(true);
            }
            
            // ScrollRectのImageコンポーネントのraycastTargetを有効化（マウスイベントを受け取るため）
            UnityEngine.UI.Image scrollRectImage = scrollRect.GetComponent<UnityEngine.UI.Image>();
            if (scrollRectImage != null)
            {
                scrollRectImage.raycastTarget = true;
            }
            
            // ViewportのImageコンポーネントのraycastTargetも有効化
            if (scrollRect.viewport != null)
            {
                UnityEngine.UI.Image viewportImage = scrollRect.viewport.GetComponent<UnityEngine.UI.Image>();
                if (viewportImage != null)
                {
                    viewportImage.raycastTarget = true;
                }
            }
            
            Debug.Log("BattleScreen: ScrollRectを有効化しました - クリックしなくてもスクロール可能");
        }
        else
        {
            Debug.LogWarning("BattleScreen: ScrollRectが見つかりません");
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

