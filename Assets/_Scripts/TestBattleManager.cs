using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// バトル画面のテスト用マネージャー
/// 実際のゲームではBattleLogicがこの役割を担う
/// </summary>
public class TestBattleManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private BattleScreen battleScreen;
    
    [Header("データ参照")]
    [SerializeField] private enemy_L enemyData; // enemy_L.csへの参照（Inspectorで設定）
    [SerializeField] private item itemData; // item.csへの参照（アイテムデータ設定用）

    [Header("テスト設定")]
    [SerializeField] private string testPlanetName = "Forest_Planet";
    [SerializeField] private int testPlayerHP = 80;
    [SerializeField] private int testPlayerMaxHP = 100;
    [SerializeField] private bool autoStartOnAwake = true;

    [Header("テスト用敵データ（敵IDは0-17）")]
    [Tooltip("⚠️ Inspectorで値を変更したら、右クリックメニューから「バトルを開始」を実行してください")]
    [SerializeField] private List<TestEnemyInfo> testEnemies = new List<TestEnemyInfo>()
    {
        new TestEnemyInfo(0, 60, 60),  // Claw_Loader (雑魚)
        new TestEnemyInfo(1, 50, 50)   // Omni-Roid (雑魚)
    };

    [Header("テスト用アイテム所持数（アイテムID 0-8、レアリティ別）")]
    [Tooltip("⚠️ Inspectorで値を変更したら、右クリックメニューから「バトルを開始」を実行してください")]
    [SerializeField] private List<TestItemInfo> testItems = new List<TestItemInfo>()
    {
        new TestItemInfo(0, 2, 1, 0),  // リペアユニット: Common×2, Rare×1, Epic×0
        new TestItemInfo(1, 0, 1, 0),  // ガトリングガン: Common×0, Rare×1, Epic×0
        new TestItemInfo(2, 0, 0, 1),  // 重装ランチャー: Common×0, Rare×0, Epic×1
    };

    /// <summary>
    /// 注意: Start()はテスト用の自動開始のみに使用
    /// 本番のBattleManagerでは、GameManagerからStartBattle()を直接呼び出す設計にすること
    /// Start()に依存すると、オブジェクトが最初からアクティブな場合にバグる可能性がある
    /// </summary>
    private void Start()
    {
        // テスト用: 自動でバトルを開始する場合のみ
        if (autoStartOnAwake)
        {
            StartCoroutine(AutoStartBattle());
        }
    }

    /// <summary>
    /// 自動でバトルを開始（少し待ってから）
    /// </summary>
    private IEnumerator AutoStartBattle()
    {
        yield return new WaitForSeconds(0.5f);
        StartTestBattle();
    }

    /// <summary>
    /// テスト用バトルを開始
    /// </summary>
    [ContextMenu("バトルを開始")]
    public void StartTestBattle()
    {
        if (battleScreen == null)
        {
            Debug.LogError("TestBattleManager: BattleScreenが設定されていません");
            return;
        }

        // enemy_Lが設定されていない場合はエラー
        if (enemyData == null)
        {
            Debug.LogError("TestBattleManager: enemy_Lが設定されていません。Inspectorで設定してください。");
            return;
        }

        // テスト用敵データをEnemyDataに変換
        List<EnemyData> enemies = new List<EnemyData>();
        
        Debug.Log($"TestBattleManager: 敵データを読み込み開始 - 敵数: {testEnemies.Count}");
        
        foreach (var testEnemy in testEnemies)
        {
            // enemy_Lから敵情報を取得
            string enemyName = enemyData.GetEnemyName(testEnemy.enemyId);
            string imageName = enemyData.GetEnemyImageName(testEnemy.enemyId);
            
            // enemy_LからHPデータを取得（Inspectorで設定されていない場合のフォールバック）
            List<int> enemyStats = enemyData.GetEnemyData(testEnemy.enemyId);
            int maxHPFromData = enemyStats != null && enemyStats.Count > 0 ? enemyStats[0] : testEnemy.maxHP;
            
            // HPが0または未設定の場合は、enemy_Lのデータから取得
            int finalMaxHP = testEnemy.maxHP > 0 ? testEnemy.maxHP : maxHPFromData;
            int finalCurrentHP = testEnemy.currentHP > 0 ? testEnemy.currentHP : finalMaxHP;
            
            Debug.Log($"TestBattleManager: 敵ID {testEnemy.enemyId} - 名前: {enemyName}, HP: {finalCurrentHP}/{finalMaxHP} (設定値: {testEnemy.currentHP}/{testEnemy.maxHP}), 画像: {imageName}");
            
            // Resourcesフォルダから敵画像を読み込む
            // GetEnemyImageName()で取得したファイル名を使用
            Sprite enemySprite = null;
            if (!string.IsNullOrEmpty(imageName))
            {
                enemySprite = Resources.Load<Sprite>($"enemies/{imageName}");
                
                if (enemySprite == null)
                {
                    Debug.LogWarning($"TestBattleManager: 敵画像が見つかりません - Resources/enemies/{imageName} (プレースホルダーを使用)");
                }
                else
                {
                    Debug.Log($"TestBattleManager: 敵画像を読み込み成功 - {imageName}");
                }
            }
            else
            {
                Debug.LogWarning($"TestBattleManager: 敵ID {testEnemy.enemyId} の画像ファイル名が設定されていません");
            }
            
            // EnemyDataを作成（enemyIdは文字列に変換）
            enemies.Add(new EnemyData(
                testEnemy.enemyId.ToString(),
                enemyName,
                finalCurrentHP,
                finalMaxHP,
                enemySprite // 画像を設定（nullの場合はプレースホルダーが表示される）
            ));
        }

        // アイテム所持数を設定
        SetupTestItems();

        // バトル開始
        Debug.Log($"TestBattleManager: バトル開始 - 惑星: {testPlanetName}, プレイヤーHP: {testPlayerHP}/{testPlayerMaxHP}, 敵数: {enemies.Count}");
        battleScreen.StartBattle(testPlanetName, enemies, testPlayerHP, testPlayerMaxHP);
        Debug.Log("TestBattleManager: バトルを開始しました");
    }

    /// <summary>
    /// テスト用アイテム所持数を設定
    /// </summary>
    private void SetupTestItems()
    {
        if (itemData == null)
        {
            Debug.LogWarning("TestBattleManager: itemDataが設定されていません。アイテム選択機能は動作しません。");
            return;
        }

        // すべてのアイテムを0にリセット
        for (int i = 0; i <= 8; i++)
        {
            if (!itemData.item_list.ContainsKey(i))
            {
                itemData.item_list[i] = new List<int>() { 0, 0, 0 };
            }
            else
            {
                itemData.item_list[i] = new List<int>() { 0, 0, 0 };
            }
        }

        Debug.Log($"TestBattleManager: itemDataのインスタンスID: {itemData.GetInstanceID()}");
        Debug.Log($"TestBattleManager: testItemsの数: {testItems.Count}");

        // テスト用アイテム所持数を設定
        foreach (var testItem in testItems)
        {
            if (testItem.itemId >= 0 && testItem.itemId <= 8)
            {
                itemData.item_list[testItem.itemId] = new List<int>()
                {
                    testItem.commonCount,
                    testItem.rareCount,
                    testItem.epicCount
                };
                Debug.Log($"TestBattleManager: アイテムID {testItem.itemId} の所持数を設定 - Common:{testItem.commonCount}, Rare:{testItem.rareCount}, Epic:{testItem.epicCount}");
            }
        }

        // 設定後の確認
        Debug.Log("TestBattleManager: 設定後のitem_listの内容:");
        foreach (var kvp in itemData.item_list)
        {
            Debug.Log($"  ID {kvp.Key}: Common:{kvp.Value[0]}, Rare:{kvp.Value[1]}, Epic:{kvp.Value[2]}");
        }
    }

    /// <summary>
    /// プレイヤーにダメージを与える（テスト用）
    /// </summary>
    [ContextMenu("プレイヤーに10ダメージ")]
    public void DamagePlayer()
    {
        testPlayerHP = Mathf.Max(0, testPlayerHP - 10);
        battleScreen?.UpdatePlayerHP(testPlayerHP, testPlayerMaxHP);
        Debug.Log($"TestBattleManager: プレイヤーに10ダメージ → HP: {testPlayerHP}/{testPlayerMaxHP}");
    }

    /// <summary>
    /// 敵0にダメージを与える（テスト用）
    /// </summary>
    [ContextMenu("敵0に10ダメージ")]
    public void DamageEnemy0()
    {
        if (testEnemies.Count > 0)
        {
            testEnemies[0].currentHP = Mathf.Max(0, testEnemies[0].currentHP - 10);
            battleScreen?.UpdateEnemyHP(0, testEnemies[0].currentHP, testEnemies[0].maxHP);

            if (testEnemies[0].currentHP <= 0)
            {
                battleScreen?.OnEnemyDefeated(0);
            }

            Debug.Log($"TestBattleManager: 敵0に10ダメージ → HP: {testEnemies[0].currentHP}/{testEnemies[0].maxHP}");
        }
    }

    /// <summary>
    /// 敵1にダメージを与える（テスト用）
    /// </summary>
    [ContextMenu("敵1に10ダメージ")]
    public void DamageEnemy1()
    {
        if (testEnemies.Count > 1)
        {
            testEnemies[1].currentHP = Mathf.Max(0, testEnemies[1].currentHP - 10);
            battleScreen?.UpdateEnemyHP(1, testEnemies[1].currentHP, testEnemies[1].maxHP);

            if (testEnemies[1].currentHP <= 0)
            {
                battleScreen?.OnEnemyDefeated(1);
            }

            Debug.Log($"TestBattleManager: 敵1に10ダメージ → HP: {testEnemies[1].currentHP}/{testEnemies[1].maxHP}");
        }
    }

    /// <summary>
    /// 惑星を変更してバトルを再開始（テスト用）
    /// </summary>
    [ContextMenu("惑星をIce_Planetに変更")]
    public void ChangePlanetToIce()
    {
        testPlanetName = "Ice_Planet";
        StartTestBattle();
    }

    /// <summary>
    /// 惑星を変更してバトルを再開始（テスト用）
    /// </summary>
    [ContextMenu("惑星をOld_Empire_Planetに変更")]
    public void ChangePlanetToOldEmpire()
    {
        testPlanetName = "Old_Empire_Planet";
        StartTestBattle();
    }

    /// <summary>
    /// バトルをリセット（テスト用）
    /// </summary>
    [ContextMenu("バトルをリセット")]
    public void ResetBattle()
    {
        testPlayerHP = testPlayerMaxHP;
        foreach (var enemy in testEnemies)
        {
            enemy.currentHP = enemy.maxHP;
        }
        StartTestBattle();
        Debug.Log("TestBattleManager: バトルをリセットしました");
    }

    /// <summary>
    /// 敵0の攻撃アニメーションをテスト（テスト用）
    /// </summary>
    [ContextMenu("敵0の攻撃アニメーション")]
    public void TestEnemy0Attack()
    {
        battleScreen?.PlayEnemyAttackAnimation(0);
        Debug.Log("TestBattleManager: 敵0の攻撃アニメーションを実行");
    }

    /// <summary>
    /// 敵1の攻撃アニメーションをテスト（テスト用）
    /// </summary>
    [ContextMenu("敵1の攻撃アニメーション")]
    public void TestEnemy1Attack()
    {
        battleScreen?.PlayEnemyAttackAnimation(1);
        Debug.Log("TestBattleManager: 敵1の攻撃アニメーションを実行");
    }

    /// <summary>
    /// プレイヤーターンを表示（アイテム選択画面を表示）
    /// </summary>
    [ContextMenu("プレイヤーターン（アイテム選択）")]
    public void ShowPlayerTurn()
    {
        battleScreen?.ShowPlayerTurn();
        Debug.Log("TestBattleManager: プレイヤーターンを表示（アイテム選択画面を表示）");
    }

    /// <summary>
    /// アイテムが選択された時の処理（BattleScreenから呼ばれる）
    /// </summary>
    /// <param name="itemId">アイテムID (0-8)</param>
    /// <param name="rarity">レアリティ (0=Common, 1=Rare, 2=Epic)</param>
    public void OnItemSelected(int itemId, int rarity)
    {
        if (itemData == null)
        {
            Debug.LogWarning("TestBattleManager: itemDataが設定されていません");
            return;
        }

        // アイテム名を取得
        string itemName = "不明なアイテム";
        if (itemData.item_name_list.TryGetValue(itemId, out string name))
        {
            itemName = name;
        }

        string rarityName = rarity == 0 ? "Common" : (rarity == 1 ? "Rare" : "Epic");

        Debug.Log($"TestBattleManager: アイテム選択検出 - ID:{itemId} ({itemName}), レアリティ:{rarity} ({rarityName})");

        // TODO: アイテム使用処理を実装
        // 例: 所持数を減らす、アイテム効果を適用するなど
    }
}

/// <summary>
/// テスト用敵情報（Inspectorで編集可能）
/// enemy_L.csの敵ID（0-17）を使用
/// </summary>
[System.Serializable]
public class TestEnemyInfo
{
    [Tooltip("敵ID (0-17): 0,1=雑魚 / 2-4=Volcano / 5-7=Ice / 8-10=Forest / 11-13=Old_Empire / 14-16=Desert / 17=最終ボス")]
    public int enemyId;
    public int currentHP;
    public int maxHP;

    public TestEnemyInfo(int id, int hp, int max)
    {
        enemyId = id;
        currentHP = hp;
        maxHP = max;
    }
}

/// <summary>
/// テスト用アイテム情報（Inspectorで編集可能）
/// item.csのアイテムID（0-8）を使用
/// </summary>
[System.Serializable]
public class TestItemInfo
{
    [Tooltip("アイテムID (0-8): 0=リペアユニット / 1=ガトリングガン / 2=重装ランチャー / 3=火炎放射器 / 4=サイバーハック / 5=オーバークロック / 6=スタンガン / 7=フィールドシールド / 8=EMPパルスキャノン")]
    public int itemId;
    [Tooltip("Common所持数")]
    public int commonCount;
    [Tooltip("Rare所持数")]
    public int rareCount;
    [Tooltip("Epic所持数")]
    public int epicCount;

    public TestItemInfo(int id, int common, int rare, int epic)
    {
        itemId = id;
        commonCount = common;
        rareCount = rare;
        epicCount = epic;
    }
}

