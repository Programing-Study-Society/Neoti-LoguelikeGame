using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR;

/// <summary>
/// バトルシステム管理クラス
/// 戦闘ロジックと敵生成を管理
/// </summary>
public class battle_system : MonoBehaviour
{
    [Header("プレハブ")]
    [SerializeField] private GameObject enemyPrefab; // 敵ロジック用プレハブ（enemy_Lコンポーネント付き）

    [Header("データ参照")]
    [SerializeField] private EnemySpawnManager enemySpawnManager; // 敵出現管理
    [SerializeField] private enemy_L enemyDataFile; // 敵データファイル（enemy_L.LoadFromDataFile用）
    [SerializeField] private stage_data stageData; // ステージ進行度データ（惑星名取得用）
    [SerializeField] private item itemMaster; // アイテムマスター（所持数管理用）

    [Header("UI参照")]
    [SerializeField] private BattleScreen battleScreen; // バトル画面UI

    [Header("プレイヤー")]
    public player player;

    [Header("敵配置設定")]
    [SerializeField] private float enemySpacing = 2f; // 敵同士の間隔

    // =========================
    // アイテム定義
    // =========================
    public enum ItemEffectType
    {
        Heal,
        Damage,
        MultiHit,
        DamageOverTime,
        BuffATK,
        BuffDEF,
        DebuffATK,
        DebuffDEF,
        AoEDamage
    }

    public enum ItemTarget
    {
        Self,
        OpponentSingle,
        OpponentAll
    }

    [System.Serializable]
    public class BattleItemData
    {
        public ItemEffectType effectType;
        public ItemTarget target;
        public int power;
        public int hitCount;
        public int duration;
    }

    private Dictionary<int, BattleItemData> itemDatabase;

    // =========================
    // プレイヤーバフ
    // =========================
    private int playerBuffDEF = 0;
    private int playerBuffTurn = 0;
    private int playerBuffATKPercent = 0;
    private int playerBuffATKTurn = 0;

    // =========================
    // 回復（Heal）設定（固定値）
    // =========================
    [Header("Heal Settings")]
    [SerializeField] private int healSmallAmount = 5000;
    [SerializeField] private int healMiddleAmount = 8000;
    [SerializeField] private int healLargeAmount = 15000;
    
    // =========================
    // マルチヒット調整（倍率は後から変更できるように）
    // =========================
    [Header("MultiHit Settings")]
    [SerializeField] private float multiHitSmallAtkRate = 1.0f;   // 小：ATK100%（固定でOK）
    [SerializeField] private float multiHitMiddleAtkRate = 1.5f;  // 中：あとで調整
    [SerializeField] private float multiHitLargeAtkRate = 3.0f;   // 大：あとで調整
    [SerializeField] private int multiHitMin = 1;
    [SerializeField] private int multiHitMax = 6;

    // =========================
    // 持続ダメージ（DOT）設定（倍率＝1ターンあたり）
    // =========================
    [Header("DOT Settings")]
    [SerializeField] private int dotDurationTurns = 3;      // 継続ターン
    [SerializeField] private float dotSmallAtkRate = 2.5f;  // 小：1ターン250%
    [SerializeField] private float dotMiddleAtkRate = 3.75f;// 中：1ターン375%
    [SerializeField] private float dotLargeAtkRate = 7.5f;  // 大：1ターン750%

    // =========================
    // 強攻撃（Damage）設定（倍率＝1発あたり）
    // =========================
    [Header("Strong Attack Settings")]
    [SerializeField] private float strongSmallAtkRate = 4.0f;   // 小：400%
    [SerializeField] private float strongMiddleAtkRate = 6.0f;  // 中：600%
    [SerializeField] private float strongLargeAtkRate = 12.0f;  // 大：1200%

    [Header("Strong Attack Hidden Effects")]
    [SerializeField] private int strongFailDenominator = 20;    // 1/20 失敗
    [SerializeField] private float strongFailMultiplier = 0.5f; // 失敗時：半分
    [SerializeField] private int strongLuckyDenominator = 30;   // 1/30 上振れ
    [SerializeField] private float strongLuckyMultiplier = 3.0f;// 上振れ：+200%→合計×3

    // =========================
    // ダメージアップ（ATK Buff）設定（%）
    // =========================
    [Header("Player ATK Buff Settings (Item 5)")]
    [SerializeField] private int atkBuffDurationTurns = 4;
    [SerializeField] private int atkBuffSmallPercent = 40;
    [SerializeField] private int atkBuffMiddlePercent = 70;
    [SerializeField] private int atkBuffLargePercent = 100;

    // =========================
    // 防御力ダウン（Enemy DEF Debuff）設定（%）
    // =========================
    [Header("Enemy DEF Debuff Settings (Item 6)")]
    [SerializeField] private int defDebuffDurationTurns = 4;
    [SerializeField] private int defDebuffSmallPercent = 40;  // 小：40%
    [SerializeField] private int defDebuffMiddlePercent = 70; // 中：70%
    [SerializeField] private int defDebuffLargePercent = 100; // 大：100%

    // =========================
    // 防御力アップ（Player DEF Buff）設定（%）
    // =========================
    [Header("Player DEF Buff Settings (Item 7)")]
    [SerializeField] private int defBuffDurationTurns = 4;
    [SerializeField] private int defBuffSmallPercent = 40;  // 小：40%
    [SerializeField] private int defBuffMiddlePercent = 70; // 中：70%
    [SerializeField] private int defBuffLargePercent = 100; // 大：100%

    private int battlePlayerHP;
    private List<enemy_L> enemies = new List<enemy_L>();
    
    // =========================
    // 敵ステータス管理
    // =========================
    class EnemyStatus
    {
        public int dotDamage;
        public int dotTurn;
        public int debuffATKPercent;        // 敵ATKデバフ（%）
        public int debuffATKTurn;          // 敵ATKデバフ残りターン
        public int debuffDEFPercent;       // 敵DEFデバフ（%）
        public int debuffDEFTurn;          // 敵DEFデバフ残りターン
    }

    private Dictionary<enemy_L, EnemyStatus> enemyStatus
        = new Dictionary<enemy_L, EnemyStatus>();

    // =========================
    // バトル中アイテム管理（レアリティごとに管理）
    // =========================
    private Dictionary<int, List<int>> battleItemCounts
        = new Dictionary<int, List<int>>();

    private Dictionary<int, List<int>> battleStartItemCounts
        = new Dictionary<int, List<int>>();
    
    // ターン管理
    private enum BattleTurn { Player, Enemy, End }
    private BattleTurn currentTurn = BattleTurn.Player;
    private bool isActionProcessing = false;
    
    // バトル開始フラグ（summary.Battle()が呼ばれた時だけtrueにする）
    private bool shouldStartBattle = false;

    // =========================
    // 報酬計算＆付与
    // =========================
    private int battleStartEnemyCount = 0; // バトル開始時の敵の数
    private int rewardGold = 0; // 報酬ゴールド
    private int rewardSkillPoint = 0; // 報酬スキルポイント
    private int rewardItemSmall = 0; // 報酬アイテム（小）の数
    private int rewardItemMiddle = 0; // 報酬アイテム（中）の数
    private int rewardItemLarge = 0; // 報酬アイテム（大）の数
    private int rewardItemSmallId = 0; // 報酬アイテム（小）のID
    private int rewardItemMiddleId = 0; // 報酬アイテム（中）のID
    private int rewardItemLargeId = 0; // 報酬アイテム（大）のID


    void OnEnable()
    {
        // shouldStartBattleがfalseの場合は何もしない（起動時の自動開始を防ぐ）
        if (!shouldStartBattle)
        {
            Debug.Log("battle_system: OnEnable()が呼ばれましたが、shouldStartBattle=falseのためバトルを開始しません");
            return;
        }
        
        // フラグをリセット（次回のバトル開始に備える）
        shouldStartBattle = false;
        
        // バトル開始処理を実行
        StartBattleInternal();
    }
    
    /// <summary>
    /// バトル開始処理（内部用）
    /// </summary>
    private void StartBattleInternal()
    {
        // プレイヤー参照を取得（Inspector優先、nullなら自動検索）
        if (player == null)
        {
            player = GetComponent<player>();
            if (player == null)
            {
                player = FindObjectOfType<player>();
            }
        }

        if (player == null)
        {
            Debug.LogError("battle_system: playerが見つかりません。Inspectorで設定するか、シーン内にplayerコンポーネントを配置してください。");
            return;
        }
        
        // 既存の敵をクリア
        ClearEnemies();
        
        // アイテムDBを初期化
        InitItemDatabase();

        // EnemySpawnManagerから敵IDリストを取得
        if (enemySpawnManager == null)
        {
            Debug.LogError("battle_system: EnemySpawnManagerが設定されていません");
            return;
        }

        List<int> enemyIds = enemySpawnManager.GenerateEnemyIds();
        
        if (enemyIds == null || enemyIds.Count == 0)
        {
            Debug.LogWarning("battle_system: 敵IDリストが空です");
            return;
        }

        // 敵を生成
        SpawnEnemies(enemyIds);
        
        // バトル開始時の敵の数を記録（報酬計算用）
        battleStartEnemyCount = enemies.Count;
        
        // バトル開始処理（アイテム管理の初期化）
        InitializeBattleItems();
        
        // バトル開始時に全回復
        battlePlayerHP = player.MAXHP;
        
        // ターンをプレイヤーに設定
        currentTurn = BattleTurn.Player;

        Debug.Log($"battle_system: バトル開始 - プレイヤーHP: {battlePlayerHP}/{player.MAXHP}, 敵の数: {enemies.Count}");

        // UIにバトル開始を通知
        NotifyBattleStartToUI();
        
        // プレイヤーターン開始を通知
        NotifyPlayerTurnStart();
    }

    /// <summary>
    /// 敵を生成する
    /// </summary>
    /// <param name="enemyIds">敵IDのリスト</param>
    private void SpawnEnemies(List<int> enemyIds)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("battle_system: enemyPrefabが設定されていません");
            return;
        }

        if (enemyDataFile == null)
        {
            Debug.LogError("battle_system: enemyDataFileが設定されていません");
            return;
        }

        int enemyCount = enemyIds.Count;
        
        // 敵を中央配置するための開始位置を計算
        float totalWidth = (enemyCount - 1) * enemySpacing;
        Vector2 startPosition = new Vector2(-totalWidth / 2f, 0f);

        for (int i = 0; i < enemyCount; i++)
        {
            int enemyId = enemyIds[i];
            
            // 座標計算
            Vector2 position = startPosition + new Vector2(i * enemySpacing, 0f);
            
            // プレハブを生成
            GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemyObj.name = $"Enemy_{enemyId}_{i}";
            
            // 非表示にする（ロジック用なので見た目は不要）
            enemyObj.SetActive(false);
            
            // enemy_Lコンポーネントを取得
            enemy_L enemy = enemyObj.GetComponent<enemy_L>();
            if (enemy == null)
            {
                Debug.LogError($"battle_system: 敵プレハブにenemy_Lコンポーネントがありません - EnemyID: {enemyId}");
                Destroy(enemyObj);
                continue;
            }

            // 敵データファイルへの参照を設定
            enemy.dataFile = enemyDataFile;
            
            // 敵IDからデータを読み込む
            enemy.LoadFromDataFile(enemyId);
            
            // HPを初期化
            enemy.HP = enemy.MAXHP;
            
            // 管理リストに追加
            enemies.Add(enemy);
            enemyStatus[enemy] = new EnemyStatus();
            
            Debug.Log($"battle_system: 敵生成 - ID: {enemyId}, 名前: {enemyDataFile.GetEnemyName(enemyId)}, HP: {enemy.HP}/{enemy.MAXHP}, ATK: {enemy.ATK}, DEF: {enemy.DEF}");
        }

        Debug.Log($"battle_system: {enemyCount}体の敵を生成しました");
    }

    /// <summary>
    /// 既存の敵をクリアする
    /// </summary>
    private void ClearEnemies()
    {
        foreach (enemy_L enemy in enemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemies.Clear();
        enemyStatus.Clear();
    }
    
    // =========================
    // アイテムDB
    // =========================
    void InitItemDatabase()
    {
        itemDatabase = new Dictionary<int, BattleItemData>()
        {
            {0,new BattleItemData{effectType=ItemEffectType.Heal,target=ItemTarget.Self,power=30}},
            {1,new BattleItemData{effectType=ItemEffectType.MultiHit,target=ItemTarget.OpponentSingle,power=5,hitCount=5}},
            {2,new BattleItemData{effectType=ItemEffectType.Damage,target=ItemTarget.OpponentSingle,power=25}},
            {3,new BattleItemData{effectType=ItemEffectType.DamageOverTime,target=ItemTarget.OpponentSingle,power=5,duration=3}},
            {4,new BattleItemData{effectType=ItemEffectType.DebuffATK,target=ItemTarget.OpponentSingle,power=5,duration=4}},
            {5,new BattleItemData{effectType=ItemEffectType.BuffATK,target=ItemTarget.Self,power=0,duration=4}},
            {6,new BattleItemData{effectType=ItemEffectType.DebuffDEF,target=ItemTarget.OpponentSingle,power=0,duration=4}},
            {7,new BattleItemData{effectType = ItemEffectType.BuffDEF,target = ItemTarget.Self,power = 0,duration = 4}},
            {8,new BattleItemData{effectType=ItemEffectType.AoEDamage,target=ItemTarget.OpponentAll,power=15}},
        };
    }
    
    /// <summary>
    /// バトル開始時のアイテム管理初期化
    /// </summary>
    void InitializeBattleItems()
    {
        battleItemCounts.Clear();
        battleStartItemCounts.Clear();

        if (itemMaster != null && itemMaster.item_list != null)
        {
            foreach (var p in itemMaster.item_list)
            {
                // レアリティごとの所持数をコピー
                battleItemCounts[p.Key] = new List<int>(p.Value);
                battleStartItemCounts[p.Key] = new List<int>(p.Value);
            }
        }
    }

    /// <summary>
    /// バトルを開始する（外部から呼ばれる）
    /// </summary>
    public void StartBattle()
    {
        // フラグを立てて、OnEnable()経由で開始する
        shouldStartBattle = true;
        
        // 既にアクティブの場合は、一度非アクティブ→アクティブにしてOnEnable()を呼ぶ
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
    }
    
    // =========================
    // レベル補正とアイテム効果計算
    // =========================
    float GetLevelMultiplier(ItemLevel level)
    {
        return level switch
        {
            ItemLevel.Small => 0.8f,
            ItemLevel.Middle => 1.0f,
            ItemLevel.Large => 1.2f,
            _ => 1.0f
        };
    }

    BattleItemData CreateAdjustedItem(BattleItemData baseItem, ItemLevel level)
    {
        if (baseItem.effectType == ItemEffectType.Heal)
        {
            int heal = level switch
            {
                ItemLevel.Small => healSmallAmount,
                ItemLevel.Middle => healMiddleAmount,
                ItemLevel.Large => healLargeAmount,
                _ => healSmallAmount
            };

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = baseItem.duration,
                power = Mathf.Max(1, heal)
            };
        }
        if (baseItem.effectType == ItemEffectType.MultiHit)
        {
            int hits = Random.Range(multiHitMin, multiHitMax + 1);

            float multiRate = level switch
            {
                ItemLevel.Small => multiHitSmallAtkRate,
                ItemLevel.Middle => multiHitMiddleAtkRate,
                ItemLevel.Large => multiHitLargeAtkRate,
                _ => 1.0f
            };

            int atk = GetPlayerATK();
            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = hits,
                duration = baseItem.duration,
                power = Mathf.Max(1, Mathf.RoundToInt(atk * multiRate))
            };
        }
        if (baseItem.effectType == ItemEffectType.DamageOverTime)
        {
            float dotRate = level switch
            {
                ItemLevel.Small => dotSmallAtkRate,
                ItemLevel.Middle => dotMiddleAtkRate,
                ItemLevel.Large => dotLargeAtkRate,
                _ => 1.0f
            };

            int atk = GetPlayerATK();

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = dotDurationTurns,
                power = Mathf.Max(1, Mathf.RoundToInt(atk * dotRate))
            };
        }
        if (baseItem.effectType == ItemEffectType.Damage)
        {
            float strongRate = level switch
            {
                ItemLevel.Small => strongSmallAtkRate,
                ItemLevel.Middle => strongMiddleAtkRate,
                ItemLevel.Large => strongLargeAtkRate,
                _ => 1.0f
            };

            int atk = GetPlayerATK();
            int basePower = Mathf.Max(1, Mathf.RoundToInt(atk * strongRate));

            // 隠し効果（失敗→上振れの順で判定。どちらか一方のみ）
            int finalPower = basePower;

            bool failed = (strongFailDenominator > 0) && (Random.Range(0, strongFailDenominator) == 0);
            if (failed)
            {
                finalPower = Mathf.Max(1, Mathf.RoundToInt(basePower * strongFailMultiplier));
            }
            else
            {
                bool lucky = (strongLuckyDenominator > 0) && (Random.Range(0, strongLuckyDenominator) == 0);
                if (lucky)
                {
                    finalPower = Mathf.Max(1, Mathf.RoundToInt(basePower * strongLuckyMultiplier));
                }
            }

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = baseItem.duration,
                power = finalPower
            };
        }
        if (baseItem.effectType == ItemEffectType.DebuffATK)
        {
            int percent = level switch
            {
                ItemLevel.Small => 10,
                ItemLevel.Middle => 30,
                ItemLevel.Large => 60,
                _ => 10
            };

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = 4,
                power = percent
            };
        }
        if (baseItem.effectType == ItemEffectType.DebuffDEF)
        {
            int percent = level switch
            {
                ItemLevel.Small => defDebuffSmallPercent,
                ItemLevel.Middle => defDebuffMiddlePercent,
                ItemLevel.Large => defDebuffLargePercent,
                _ => defDebuffSmallPercent
            };

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = defDebuffDurationTurns,
                power = Mathf.Clamp(percent, 0, 100)
            };
        }
        if (baseItem.effectType == ItemEffectType.BuffDEF)
        {
            int percent = level switch
            {
                ItemLevel.Small => defBuffSmallPercent,
                ItemLevel.Middle => defBuffMiddlePercent,
                ItemLevel.Large => defBuffLargePercent,
                _ => defBuffSmallPercent
            };

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = defBuffDurationTurns,
                power = Mathf.Clamp(percent, 0, 100)
            };
        }
        if (baseItem.effectType == ItemEffectType.BuffATK)
        {
            int percent = level switch
            {
                ItemLevel.Small => atkBuffSmallPercent,
                ItemLevel.Middle => atkBuffMiddlePercent,
                ItemLevel.Large => atkBuffLargePercent,
                _ => atkBuffSmallPercent
            };

            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = atkBuffDurationTurns,
                power = Mathf.Clamp(percent, 0, 100)
            };
        }

        // AoEダメージ（EMPキャノン）の場合はプレイヤーATKをベースに計算
        if (baseItem.effectType == ItemEffectType.AoEDamage)
        {
            float aoeRate = GetLevelMultiplier(level);
            int atk = GetPlayerATK();
            int power = Mathf.RoundToInt(atk * aoeRate);
            
            return new BattleItemData
            {
                effectType = baseItem.effectType,
                target = baseItem.target,
                hitCount = baseItem.hitCount,
                duration = baseItem.duration,
                power = Mathf.Max(1, power)
            };
        }

        float rate = GetLevelMultiplier(level);

        return new BattleItemData
        {
            effectType = baseItem.effectType,
            target = baseItem.target,
            hitCount = baseItem.hitCount,
            duration = baseItem.duration,
            power = Mathf.RoundToInt(baseItem.power * rate)
        };
    }

    // =========================
    // プレイヤー攻撃力取得（ATKバフ込み）
    // =========================
    int GetPlayerATK()
    {
        int atk = player.ATK;

        if (playerBuffATKTurn > 0)
        {
            atk = Mathf.RoundToInt(atk * (1f + playerBuffATKPercent / 100f));
        }

        return Mathf.Max(1, atk);
    }
    
    /// <summary>
    /// UIにバトル開始を通知（enemy_L→EnemyData変換）
    /// </summary>
    private void NotifyBattleStartToUI()
    {
        if (battleScreen == null)
        {
            Debug.LogWarning("battle_system: BattleScreenが設定されていません");
            return;
        }

        if (stageData == null)
        {
            Debug.LogWarning("battle_system: stage_dataが設定されていません（惑星名が取得できません）");
            return;
        }

        if (enemyDataFile == null)
        {
            Debug.LogWarning("battle_system: enemyDataFileが設定されていません");
            return;
        }

        // 惑星名を取得
        string planetName = stageData.GetCurrentPlanetName();

        // enemy_LリストからEnemyDataリストに変換
        List<EnemyData> enemyDataList = new List<EnemyData>();
        for (int i = 0; i < enemies.Count; i++)
        {
            enemy_L enemy = enemies[i];
            if (enemy == null || enemy.gameObject == null) continue;

            // 敵名を取得
            string enemyName = enemyDataFile.GetEnemyName(enemy.enemyId);
            
            // 画像を読み込む
            string imageName = enemyDataFile.GetEnemyImageName(enemy.enemyId);
            Sprite enemySprite = null;
            if (!string.IsNullOrEmpty(imageName))
            {
                enemySprite = Resources.Load<Sprite>($"enemies/{imageName}");
                if (enemySprite == null)
                {
                    Debug.LogWarning($"battle_system: 敵画像が見つかりません - Resources/enemies/{imageName}");
                }
            }

            // EnemyDataを作成
            EnemyData enemyData = new EnemyData(
                enemy.enemyId.ToString(),
                enemyName,
                enemy.HP,
                enemy.MAXHP,
                enemySprite
            );

            enemyDataList.Add(enemyData);
        }

        // BattleScreenに通知
        battleScreen.StartBattle(planetName, enemyDataList, battlePlayerHP, player.MAXHP);
        
        Debug.Log($"battle_system: UIにバトル開始を通知 - 惑星: {planetName}, 敵数: {enemyDataList.Count}");
    }
    
    /// <summary>
    /// プレイヤーターン開始をUIに通知
    /// </summary>
    private void NotifyPlayerTurnStart()
    {
        if (battleScreen != null)
        {
            battleScreen.ShowPlayerTurn();
            Debug.Log("battle_system: プレイヤーターン開始をUIに通知");
        }
    }
    
    /// <summary>
    /// 敵ターン開始をUIに通知
    /// </summary>
    private void NotifyEnemyTurnStart()
    {
        if (battleScreen != null)
        {
            battleScreen.ShowEnemyTurn();
            Debug.Log("battle_system: 敵ターン開始をUIに通知");
        }
    }

    public void PlayerTakeDamage(int damage)
    {
        int realDamage = Mathf.Max(1, damage - player.DEF);
        battlePlayerHP -= realDamage;

        Debug.Log("プレイヤー被ダメージ: " + realDamage);

        // UIにHP更新を通知
        NotifyPlayerHPUpdate();

        if (battlePlayerHP <= 0)
        {
            Debug.Log("プレイヤー敗北");
            EndBattle(false);
        }
    }
    
    /// <summary>
    /// プレイヤーHP更新をUIに通知
    /// </summary>
    private void NotifyPlayerHPUpdate()
    {
        if (battleScreen != null)
        {
            battleScreen.UpdatePlayerHP(battlePlayerHP, player.MAXHP);
        }
    }
    
    /// <summary>
    /// 敵HP更新をUIに通知
    /// </summary>
    private void NotifyEnemyHPUpdate(int enemyIndex)
    {
        // battleScreenが有効で、インデックスが有効な場合のみ通知
        if (battleScreen == null) return;
        if (battleScreen.gameObject == null) return;
        if (!battleScreen.gameObject.activeInHierarchy) return;
        
        if (enemyIndex < 0 || enemyIndex >= enemies.Count) return;
        
        enemy_L enemy = enemies[enemyIndex];
        if (enemy == null) return;
        if (enemy.gameObject == null) return;
        
        battleScreen.UpdateEnemyHP(enemyIndex, enemy.HP, enemy.MAXHP);
    }

    // =========================
    // プレイヤー行動
    // =========================
    public void PlayerUseItem(int itemId, ItemLevel level, int enemyIndex)
    {
        if (currentTurn != BattleTurn.Player) return;
        if (isActionProcessing) return;
        if (!battleItemCounts.ContainsKey(itemId)) return;
        
        // レアリティごとの所持数をチェック（使用するレアリティの所持数が0以上か確認）
        int rarityIndex = level switch
        {
            ItemLevel.Small => 0,
            ItemLevel.Middle => 1,
            ItemLevel.Large => 2,
            _ => 1
        };
        
        if (battleItemCounts[itemId][rarityIndex] <= 0) return;

        if (!itemDatabase.ContainsKey(itemId)) return;

        // 所持数を減らす
        battleItemCounts[itemId][rarityIndex]--;
        
        BattleItemData adjusted = CreateAdjustedItem(itemDatabase[itemId], level);

        StartCoroutine(PlayerActionRoutine(adjusted, enemyIndex));
    }

    IEnumerator PlayerActionRoutine(BattleItemData item, int enemyIndex)
    {
        isActionProcessing = true;

        ApplyItem(item, enemyIndex);
        
        // 敵が全滅した場合はバトル終了（ApplyItem内でEndBattleが呼ばれる可能性がある）
        if (enemies.Count == 0)
        {
            isActionProcessing = false;
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        if (currentTurn == BattleTurn.End) yield break;

        currentTurn = BattleTurn.Enemy;
        yield return EnemyTurnRoutine();

        if (currentTurn != BattleTurn.End)
            currentTurn = BattleTurn.Player;

        isActionProcessing = false;
        
        // プレイヤーターンに戻ったらUIに通知
        NotifyPlayerTurnStart();
    }

    // =========================
    // アイテム効果
    // =========================
    void ApplyItem(BattleItemData item, int enemyIndex)
    {
        if (item.target == ItemTarget.Self)
            ApplyItemToPlayer(item);
        else if (item.target == ItemTarget.OpponentSingle)
            ApplyItemToEnemy(item, enemyIndex);
        else
            ApplyItemToAllEnemies(item);

        if (enemies.Count == 0)
            EndBattle(true);
    }

    void ApplyItemToPlayer(BattleItemData item)
    {
        if (item.effectType == ItemEffectType.Heal)
        {
            battlePlayerHP = Mathf.Min(battlePlayerHP + item.power, player.MAXHP);
            NotifyPlayerHPUpdate();
        }
        else if (item.effectType == ItemEffectType.BuffDEF)
        {
            playerBuffDEF = item.power;
            playerBuffTurn = item.duration;
        }
        else if (item.effectType == ItemEffectType.BuffATK)
        {
            playerBuffATKPercent = item.power;
            playerBuffATKTurn = item.duration;
        }
    }

    void ApplyItemToEnemy(BattleItemData item, int index)
    {
        if (index < 0 || index >= enemies.Count) return;

        if (item.effectType == ItemEffectType.Damage)
        {
            // item.powerはCreateAdjustedItem()で既に計算済み（プレイヤーATK × 倍率）
            DealDamage(enemies[index], index, item.power);
        }
        else if (item.effectType == ItemEffectType.MultiHit)
        {
            // item.powerはCreateAdjustedItem()で既に計算済み（プレイヤーATK × 倍率）
            for (int i = 0; i < item.hitCount; i++)
            {
                if (index >= enemies.Count) break;
                DealDamage(enemies[index], index, item.power);
            }
        }
        else
        {
            enemy_L enemy = enemies[index];
            if (!enemyStatus.ContainsKey(enemy)) return;
            
            EnemyStatus st = enemyStatus[enemy];

            if (item.effectType == ItemEffectType.DamageOverTime)
            {
                st.dotDamage = item.power;
                st.dotTurn = item.duration;
            }
            else if (item.effectType == ItemEffectType.DebuffATK)
            {
                st.debuffATKPercent = item.power;
                st.debuffATKTurn = item.duration;
            }
            else if (item.effectType == ItemEffectType.DebuffDEF)
            {
                st.debuffDEFPercent = item.power;
                st.debuffDEFTurn = item.duration;
            }
        }
    }

    void ApplyItemToAllEnemies(BattleItemData item)
    {
        // item.powerはCreateAdjustedItemで既にプレイヤーATK × レアリティ倍率で計算されている
        int power = item.power;
        
        // 後ろから順に処理（インデックスがずれないように）
        // 敵を削除するとenemies.Countが減るので、ループの条件を動的にチェック
        while (enemies.Count > 0)
        {
            int i = enemies.Count - 1; // 最後の敵から処理
            
            // 敵がnullまたは破棄されている場合は削除してスキップ
            if (enemies[i] == null || enemies[i].gameObject == null)
            {
                enemies.RemoveAt(i);
                continue;
            }
            
            // ダメージを与える
            DealDamage(enemies[i], i, power);
            
            // 敵が全滅した場合は終了（DealDamage内でEndBattleが呼ばれる可能性がある）
            if (enemies.Count == 0)
            {
                break;
            }
            
            // バトルが終了した場合は終了
            if (currentTurn == BattleTurn.End)
            {
                break;
            }
        }
    }

    // =========================
    // 敵ターン
    // =========================
    IEnumerator EnemyTurnRoutine()
    {
        // buff（プレイヤー側）
        if (playerBuffTurn > 0 && --playerBuffTurn == 0)
            playerBuffDEF = 0;

        if (playerBuffATKTurn > 0 && --playerBuffATKTurn == 0)
            playerBuffATKPercent = 0;
        
        // UIに敵ターン開始を通知
        NotifyEnemyTurnStart();
        
        Debug.Log("battle_system: 敵のターン開始");
        
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (i >= enemies.Count) continue;
            
            enemy_L enemy = enemies[i];
            if (enemy == null || enemy.gameObject == null)
            {
                enemies.RemoveAt(i);
                continue;
            }
            
            if (!enemyStatus.ContainsKey(enemy))
            {
                enemyStatus[enemy] = new EnemyStatus();
            }
            
            EnemyStatus st = enemyStatus[enemy];

            // DOT処理
            if (st.dotTurn > 0)
            {
                enemy.HP -= st.dotDamage;
                st.dotTurn--;
                
                // UIに敵HP更新を通知（敵がまだ存在する場合のみ）
                if (i >= 0 && i < enemies.Count && enemies[i] == enemy)
                {
                    NotifyEnemyHPUpdate(i);
                }
                
                // HPが0以下になった場合は削除
                if (enemy.HP <= 0)
                {
                    string dotEnemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
                    Debug.Log($"battle_system: {dotEnemyName} がDOTで撃破 (インデックス: {i})");
                    
                    // UIに敵撃破を通知（削除前のインデックスを渡す）
                    if (battleScreen != null && battleScreen.gameObject != null && battleScreen.gameObject.activeInHierarchy)
                    {
                        battleScreen.OnEnemyDefeated(i);
                    }
                    
                    RemoveEnemy(enemy, i);
                    
                    // 敵が全滅した場合は終了
                    if (enemies.Count == 0)
                    {
                        EndBattle(true);
                        yield break;
                    }
                    
                    continue;
                }
            }

            // 敵の攻撃アニメーションを実行（battleScreenが有効な場合のみ）
            if (battleScreen != null && battleScreen.gameObject != null && battleScreen.gameObject.activeInHierarchy)
            {
                battleScreen.PlayEnemyAttackAnimation(i);
                // アニメーションが完了するまで待機（攻撃アニメーションの時間分）
                yield return new WaitForSeconds(0.5f);
            }

            // 敵の攻撃力計算（デバフ考慮）
            int atk = enemy.ATK;
            if (st.debuffATKTurn > 0)
            {
                atk = Mathf.RoundToInt(atk * (1f - st.debuffATKPercent / 100f));
            }
            atk = Mathf.Max(1, atk);
            
            // プレイヤーの防御力計算（バフ考慮）
            int def = player.DEF;
            if (playerBuffTurn > 0)
            {
                def = Mathf.RoundToInt(def * (1f + playerBuffDEF / 100f));
            }

            int damage = Mathf.Max(1, atk - def);
            battlePlayerHP -= damage;
            
            // HPが0以下にならないように制限
            battlePlayerHP = Mathf.Max(0, battlePlayerHP);
            
            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName}の攻撃！ プレイヤーに {damage} ダメージ - 残りHP: {battlePlayerHP}/{player.MAXHP}");

            // UIにダメージ表示を通知（battleScreenが有効な場合のみ）
            if (battleScreen != null && battleScreen.gameObject != null && battleScreen.gameObject.activeInHierarchy)
            {
                battleScreen.ShowPlayerDamage(damage);
            }

            // UIにプレイヤーHP更新を通知
            NotifyPlayerHPUpdate();

            if (battlePlayerHP <= 0)
            {
                EndBattle(false);
                yield break;
            }
            
            // debuff（敵側）のターン減少
            if (st.debuffATKTurn > 0 && --st.debuffATKTurn == 0)
                st.debuffATKPercent = 0;

            if (st.debuffDEFTurn > 0 && --st.debuffDEFTurn == 0)
                st.debuffDEFPercent = 0;

            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("battle_system: 敵のターン終了");
    }

    // =========================
    // 共通処理
    // =========================
    void DealDamage(enemy_L enemy, int index, int power)
    {
        // nullチェック（敵が既に削除されている場合は処理しない）
        if (enemy == null) return;
        if (enemy.gameObject == null) return;
        
        // インデックスが有効かチェック
        if (index < 0 || index >= enemies.Count) return;
        if (enemies[index] != enemy) return; // インデックスと敵が一致しない場合は処理しない
        
        int def = enemy.DEF;

        // 防御デバフ反映（%）
        if (enemyStatus.TryGetValue(enemy, out var st))
        {
            if (st.debuffDEFTurn > 0)
            {
                float rate = Mathf.Clamp01(1f - (st.debuffDEFPercent / 100f));
                def = Mathf.RoundToInt(def * rate);
            }
        }

        int realDamage = Mathf.Max(1, power - def);
        enemy.HP -= realDamage;

        Debug.Log($"battle_system: 敵に {realDamage} ダメージ - 残りHP: {enemy.HP}/{enemy.MAXHP}");

        // UIにダメージ表示を通知（battleScreenが有効な場合のみ）
        if (battleScreen != null && battleScreen.gameObject != null && battleScreen.gameObject.activeInHierarchy)
        {
            battleScreen.ShowEnemyDamage(index, realDamage);
        }

        // UIに敵HP更新を通知（敵がまだ存在する場合のみ）
        if (index >= 0 && index < enemies.Count && enemies[index] == enemy)
        {
            NotifyEnemyHPUpdate(index);
        }

        // HPが0以下になった場合は削除
        if (enemy.HP <= 0)
        {
            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName} 撃破 (インデックス: {index})");
            
            // UIに敵撃破を通知（削除前のインデックスを渡す）
            if (battleScreen != null && battleScreen.gameObject != null && battleScreen.gameObject.activeInHierarchy)
            {
                battleScreen.OnEnemyDefeated(index);
            }
            
            // ロジック側から敵を削除
            RemoveEnemy(enemy, index);
            
            // 敵が全滅したらバトル終了
            if (enemies.Count == 0)
            {
                EndBattle(true);
                return; // バトル終了後は処理を終了
            }
        }
    }

    void RemoveEnemy(enemy_L enemy, int index)
    {
        // enemyStatusから削除
        enemyStatus.Remove(enemy);
        
        // 敵オブジェクトを破棄
        if (enemy != null && enemy.gameObject != null)
        {
            Destroy(enemy.gameObject);
        }
        
        // enemiesリストから削除（インデックスが有効な場合のみ）
        if (index >= 0 && index < enemies.Count)
        {
            enemies.RemoveAt(index);
        }
        else
        {
            // インデックスが無効な場合は、enemyオブジェクトで検索して削除
            enemies.Remove(enemy);
        }
        
        Debug.Log($"battle_system: 敵を削除 - 残り敵数: {enemies.Count}");
    }

    void EndBattle(bool win)
    {
        currentTurn = BattleTurn.End;

        ReflectBattleItems();

        if (win)
        {
            // 報酬を計算
            CalculateBattleRewards();
            
            // 報酬を付与
            GiveBattleRewards();
            
            // 報酬画面を表示（summary.cs経由）
            if (battleScreen != null)
            {
                battleScreen.gameObject.SetActive(false);
                Debug.Log("battle_system: 勝利によりBattleScreenを非アクティブ化しました");
            }
            
            // summary.csに報酬表示を通知
            summary gameManager = FindObjectOfType<summary>();
            if (gameManager != null)
            {
                gameManager.ShowBattleReward(rewardGold, rewardSkillPoint, 
                    rewardItemSmallId, rewardItemSmall,
                    rewardItemMiddleId, rewardItemMiddle,
                    rewardItemLargeId, rewardItemLarge);
            }
        }
        else
        {
            if (battleScreen != null)
            {
                battleScreen.gameObject.SetActive(false);
            }
        }

        Debug.Log(win ? "勝利" : "敗北");
    }
    void ReflectBattleItems()
    {
        if (itemMaster == null) return;
        
        foreach (var kv in battleItemCounts)
        {
            if (itemMaster.item_list.ContainsKey(kv.Key))
            {
                // レアリティごとの所持数を更新
                itemMaster.item_list[kv.Key] = new List<int>(kv.Value);
            }
        }
    }

    /// <summary>
    /// 現在の敵リストを取得（外部から参照用）
    /// </summary>
    /// <returns>敵のリスト</returns>
    public List<enemy_L> GetEnemies()
    {
        return enemies;
    }

    /// <summary>
    /// 現在のプレイヤーHPを取得
    /// </summary>
    /// <returns>プレイヤーの現在HP</returns>
    public int GetPlayerHP()
    {
        return battlePlayerHP;
    }
    
    /// <summary>
    /// アイテム所持数を取得（UI用）
    /// A案：ロジックが正で、UIはbattle_systemから取得
    /// </summary>
    /// <returns>アイテムIDとレアリティごとの所持数の辞書</returns>
    public Dictionary<int, List<int>> GetBattleItemCounts()
    {
        // バトル中のアイテム所持数を返す（レアリティごとに管理）
        if (battleItemCounts != null && battleItemCounts.Count > 0)
        {
            return battleItemCounts;
        }
        
        // フォールバック：itemMasterから直接取得
        if (itemMaster != null && itemMaster.item_list != null)
        {
            Debug.LogWarning("battle_system: battleItemCountsが空のため、itemMasterから直接取得しました");
            return itemMaster.item_list;
        }
        
        Debug.LogWarning("battle_system: itemMasterが設定されていません");
        return new Dictionary<int, List<int>>();
    }
    
    /// <summary>
    /// アイテム使用（UIから呼ばれる）
    /// </summary>
    /// <param name="itemId">アイテムID (0-8)</param>
    /// <param name="rarity">レアリティ (0=Common=Small, 1=Rare=Middle, 2=Epic=Large)</param>
    /// <param name="enemyIndex">敵インデックス（-1の場合は自分に使用）</param>
    public void OnItemUsed(int itemId, int rarity, int enemyIndex)
    {
        if (currentTurn != BattleTurn.Player)
        {
            Debug.LogWarning("battle_system: プレイヤーターンではありません");
            return;
        }

        if (isActionProcessing)
        {
            Debug.LogWarning("battle_system: アクション処理中です");
            return;
        }

        // レアリティ→ItemLevel変換
        ItemLevel level = rarity switch
        {
            0 => ItemLevel.Small,   // Common
            1 => ItemLevel.Middle,  // Rare
            2 => ItemLevel.Large,   // Epic
            _ => ItemLevel.Middle
        };

        // アイテムを使用
        PlayerUseItem(itemId, level, enemyIndex);
        
        Debug.Log($"battle_system: アイテム使用 - ID:{itemId}, レアリティ:{rarity}→ItemLevel:{level}, 敵インデックス:{enemyIndex}");
    }
    
    /// <summary>
    /// アイテムレベル（アイテムシステム実装時に使用）
    /// </summary>
    public enum ItemLevel
    {
        Small,   // Common
        Middle,  // Rare
        Large    // Epic
    }
    
    // =========================
    // UI向け：バトル状況スナップショット
    // =========================
    [System.Serializable]
    public struct BattleUIState
    {
        public int playerHP;
        public int playerMaxHP;

        public int playerBuffDEFPercent;
        public int playerBuffDEFTurn;
        public int playerBuffATKPercent;
        public int playerBuffATKTurn;

        public int enemyCount;

        // 敵ごとの表示用（必要最低限）
        public int[] enemyHP;
        public int[] enemyMaxHP;
        public int[] enemyDebuffATKPercent;
        public int[] enemyDebuffATKTurn;
        public int[] enemyDebuffDEFPercent;
        public int[] enemyDebuffDEFTurn;

        public int currentTurn; // 0=Player,1=Enemy,2=End
        public bool isActionProcessing;
    }

    public BattleUIState GetUIState()
    {
        BattleUIState s = new BattleUIState();

        // player
        s.playerHP = battlePlayerHP;
        s.playerMaxHP = player != null ? player.MAXHP : 0;

        s.playerBuffDEFPercent = playerBuffDEF;
        s.playerBuffDEFTurn = playerBuffTurn;
        s.playerBuffATKPercent = playerBuffATKPercent;
        s.playerBuffATKTurn = playerBuffATKTurn;

        // turn
        s.currentTurn = (int)currentTurn;
        s.isActionProcessing = isActionProcessing;

        // enemies
        s.enemyCount = enemies.Count;
        s.enemyHP = new int[s.enemyCount];
        s.enemyMaxHP = new int[s.enemyCount];
        s.enemyDebuffATKPercent = new int[s.enemyCount];
        s.enemyDebuffATKTurn = new int[s.enemyCount];
        s.enemyDebuffDEFPercent = new int[s.enemyCount];
        s.enemyDebuffDEFTurn = new int[s.enemyCount];

        for (int i = 0; i < s.enemyCount; i++)
        {
            if (i >= enemies.Count) break;
            
            var e = enemies[i];
            s.enemyHP[i] = e != null ? e.HP : 0;
            s.enemyMaxHP[i] = e != null ? e.MAXHP : 0;

            if (e != null && enemyStatus.TryGetValue(e, out var st))
            {
                s.enemyDebuffATKPercent[i] = st.debuffATKPercent;
                s.enemyDebuffATKTurn[i] = st.debuffATKTurn;

                s.enemyDebuffDEFPercent[i] = st.debuffDEFPercent;
                s.enemyDebuffDEFTurn[i] = st.debuffDEFTurn;
            }
        }

        return s;
    }

    // =========================
    // 報酬計算＆付与
    // =========================
    /// <summary>
    /// バトル報酬を計算
    /// </summary>
    void CalculateBattleRewards()
    {
        // 現在のAct（Field ID）を取得
        int currentFieldId = stageData != null ? stageData.GetCurrentAct() : 1;
        
        // ボス判定
        bool isBossMiddle = stageData != null && stageData.GetCurrentNodeType() == NodeType.MidBoss;
        bool isBossLarge = stageData != null && stageData.GetCurrentNodeType() == NodeType.StageBoss;

        rewardGold = CalculateGoldReward(currentFieldId, battleStartEnemyCount);

        // =========================
        // スキルポイント
        // =========================
        if (isBossLarge)
        {
            rewardSkillPoint = 3;
        }
        else if (isBossMiddle)
        {
            rewardSkillPoint = 2;
        }
        else
        {
            // 通常戦
            rewardSkillPoint = battleStartEnemyCount; // 1体=1,2体=2,3体=3
        }

        // =========================
        // アイテム報酬
        // =========================
        CalculateItemRewards(currentFieldId);

        Debug.Log(
            $"[Reward] Gold:{rewardGold} " +
            $"Skill:{rewardSkillPoint} " +  
            $"Small:{rewardItemSmall} " +
            $"Middle:{rewardItemMiddle} " +
            $"Large:{rewardItemLarge}"
        );
        Debug.Log(
            $"[RewardCond] Field:{currentFieldId} StartEnemy:{battleStartEnemyCount} " +
            $"BossMiddle:{isBossMiddle} BossLarge:{isBossLarge}"
        );
    }

    /// <summary>
    /// ゴールド報酬を計算
    /// </summary>
    int CalculateGoldReward(int fieldId, int enemyCount)
    {
        int min = 0;
        int max = 0;

        if (fieldId == 1)
        {
            if (enemyCount == 1) { min = 100; max = 200; }
            else if (enemyCount == 2) { min = 300; max = 400; }
            else if (enemyCount == 3) { min = 500; max = 600; }
        }
        else if (fieldId == 2)
        {
            if (enemyCount == 1) { min = 200; max = 350; }
            else if (enemyCount == 2) { min = 450; max = 600; }
            else if (enemyCount == 3) { min = 700; max = 850; }
        }
        else if (fieldId == 3)
        {
            if (enemyCount == 1) { min = 300; max = 500; }
            else if (enemyCount == 2) { min = 600; max = 800; }
            else if (enemyCount == 3) { min = 900; max = 1100; }
        }

        if (min == 0 && max == 0) return 0;
        return Random.Range(min, max + 1);
    }

    /// <summary>
    /// アイテム報酬を計算
    /// </summary>
    void CalculateItemRewards(int currentFieldId)
    {
        rewardItemSmall = 0;
        rewardItemMiddle = 0;
        rewardItemLarge = 0;
        rewardItemSmallId = 0;
        rewardItemMiddleId = 0;
        rewardItemLargeId = 0;

        if (currentFieldId == 1)
        {
            // フィールド1
            rewardItemSmall = Random.Range(1, 4); // 1~3
            if (rewardItemSmall > 0)
            {
                rewardItemSmallId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
        }
        else if (currentFieldId == 2)
        {
            // フィールド2
            rewardItemSmall = Random.Range(3, 7); // 3~6
            rewardItemMiddle = Random.Range(0, 3); // 0~2
            if (rewardItemSmall > 0)
            {
                rewardItemSmallId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
            if (rewardItemMiddle > 0)
            {
                rewardItemMiddleId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
        }
        else if (currentFieldId == 3)
        {
            // フィールド3
            rewardItemSmall = Random.Range(3, 10); // 3~9
            rewardItemMiddle = Random.Range(1, 5); // 1~4
            rewardItemLarge = Random.Range(0, 2);  // 0~1
            if (rewardItemSmall > 0)
            {
                rewardItemSmallId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
            if (rewardItemMiddle > 0)
            {
                rewardItemMiddleId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
            if (rewardItemLarge > 0)
            {
                rewardItemLargeId = Random.Range(0, 9); // アイテムID 0-8からランダム
            }
        }
    }

    /// <summary>
    /// バトル報酬を付与
    /// </summary>
    void GiveBattleRewards()
    {
        // ゴールド
        player.MONEY += rewardGold;

        // スキルポイント
        player.SKILL += rewardSkillPoint;

        // アイテム付与（レアリティ0=Commonで付与）
        AddRewardItem(rewardItemSmallId, rewardItemSmall, 0); // 小=Common
        AddRewardItem(rewardItemMiddleId, rewardItemMiddle, 1); // 中=Rare
        AddRewardItem(rewardItemLargeId, rewardItemLarge, 2); // 大=Epic

        Debug.Log(
            $"[RewardApplied] MONEY:{player.MONEY} SKILL:{player.SKILL} " +
            $"ItemSmall(ID:{rewardItemSmallId})+{rewardItemSmall} " +
            $"ItemMiddle(ID:{rewardItemMiddleId})+{rewardItemMiddle} " +
            $"ItemLarge(ID:{rewardItemLargeId})+{rewardItemLarge}"
        );
    }

    /// <summary>
    /// 報酬アイテムを追加
    /// </summary>
    void AddRewardItem(int itemId, int addCount, int rarity)
    {
        if (addCount <= 0) return;
        if (itemMaster == null || !itemMaster.item_list.ContainsKey(itemId)) return;

        // レアリティごとの所持数を更新
        List<int> counts = itemMaster.item_list[itemId];
        if (rarity >= 0 && rarity < counts.Count)
        {
            counts[rarity] += addCount;
            Debug.Log($"battle_system: アイテムID {itemId} レアリティ {rarity} に {addCount} 個追加");
        }
    }

}