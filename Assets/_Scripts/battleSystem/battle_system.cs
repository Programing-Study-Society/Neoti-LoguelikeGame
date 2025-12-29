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

    private int battlePlayerHP;
    private List<enemy_L> enemies = new List<enemy_L>();
    
    // ターン管理
    private enum BattleTurn { Player, Enemy, End }
    private BattleTurn currentTurn = BattleTurn.Player;
    
    // バトル開始フラグ（summary.Battle()が呼ばれた時だけtrueにする）
    private bool shouldStartBattle = false;

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
            currentTurn = BattleTurn.End;
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
        if (battleScreen != null && enemyIndex >= 0 && enemyIndex < enemies.Count)
        {
            enemy_L enemy = enemies[enemyIndex];
            if (enemy != null && enemy.gameObject != null)
            {
                battleScreen.UpdateEnemyHP(enemyIndex, enemy.HP, enemy.MAXHP);
            }
        }
    }

    /// <summary>
    /// 敵を攻撃する
    /// </summary>
    /// <param name="index">敵のインデックス</param>
    public void AttackEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count)
        {
            Debug.LogWarning($"battle_system: 無効な敵インデックス {index}");
            return;
        }

        enemy_L enemy = enemies[index];
        // Unityのカスタムnullチェック（GameObjectが破棄されている場合も検出）
        if (enemy == null || enemy.gameObject == null)
        {
            Debug.LogWarning($"battle_system: 敵が存在しません（インデックス: {index}）");
            enemies.RemoveAt(index); // 無効な参照をリストから削除
            return;
        }

        int damage = Mathf.Max(1, player.ATK - enemy.DEF);
        enemy.HP -= damage;

        Debug.Log($"battle_system: 敵に {damage} ダメージ - 残りHP: {enemy.HP}/{enemy.MAXHP}");

        // UIに敵HP更新を通知
        NotifyEnemyHPUpdate(index);

        if (enemy.HP <= 0)
        {
            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName} 撃破");
            
            // UIに敵撃破を通知
            if (battleScreen != null)
            {
                battleScreen.OnEnemyDefeated(index);
            }
            
            Destroy(enemy.gameObject);
            enemies.RemoveAt(index);

            //敵が全滅したらバトル終了
            if (enemies.Count == 0)
            {
                EndBattle();
                return;
            }
        }
        
        // 敵ターンに移行
        currentTurn = BattleTurn.Enemy;
        EnemyTurn();
    }
    
    /// <summary>
    /// 敵のターン
    /// </summary>
    public void EnemyTurn()
    {
        Debug.Log("battle_system: 敵のターン開始");
        
        // UIに敵ターン開始を通知
        NotifyEnemyTurnStart();

        foreach (enemy_L enemy in enemies)
        {
            // Unityのカスタムnullチェック（GameObjectが破棄されている場合も検出）
            if (enemy == null || enemy.gameObject == null) continue;

            int damage = Mathf.Max(1, enemy.ATK - player.DEF);
            battlePlayerHP -= damage;

            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName}の攻撃！ プレイヤーに {damage} ダメージ - 残りHP: {battlePlayerHP}/{player.MAXHP}");

            // UIにプレイヤーHP更新を通知
            NotifyPlayerHPUpdate();

            if (battlePlayerHP <= 0)
            {
                Debug.Log("battle_system: プレイヤー敗北");
                currentTurn = BattleTurn.End;
                return;
            }
        }

        Debug.Log("battle_system: 敵のターン終了");
        
        // プレイヤーターンに戻る
        currentTurn = BattleTurn.Player;
        NotifyPlayerTurnStart();
    }

    /// <summary>
    /// バトル終了処理
    /// </summary>
    void EndBattle()
    {
        Debug.Log("battle_system: バトル終了 - 敵全滅");
        // 既存の敵をクリア
        ClearEnemies();
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
        // TODO: アイテムシステム実装時に、battleItemCountsから取得するように変更
        // 現時点では、itemMasterから直接取得（仮実装）
        if (itemMaster != null && itemMaster.item_list != null)
        {
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

        // レアリティ→ItemLevel変換
        ItemLevel level = rarity switch
        {
            0 => ItemLevel.Small,   // Common
            1 => ItemLevel.Middle,  // Rare
            2 => ItemLevel.Large,   // Epic
            _ => ItemLevel.Middle
        };

        // TODO: アイテムシステム実装時に、PlayerUseItemを呼ぶ
        // PlayerUseItem(itemId, level, enemyIndex);
        
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
}
