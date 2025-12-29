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

    [Header("プレイヤー")]
    public player player;

    [Header("敵配置設定")]
    [SerializeField] private float enemySpacing = 2f; // 敵同士の間隔

    private int battlePlayerHP;
    private List<enemy_L> enemies = new List<enemy_L>();

    void OnEnable()
    {
        player = GetComponent<player>();
        
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
        
        StartBattle();
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

    void StartBattle()
    {
        // バトル開始時に全回復
        battlePlayerHP = player.MAXHP;

        Debug.Log($"battle_system: バトル開始 - プレイヤーHP: {battlePlayerHP}/{player.MAXHP}, 敵の数: {enemies.Count}");
    }

    public void PlayerTakeDamage(int damage)
    {
        int realDamage = Mathf.Max(1, damage - player.DEF);
        battlePlayerHP -= realDamage;

        Debug.Log("プレイヤー被ダメージ: " + realDamage);

        if (battlePlayerHP <= 0)
        {
            Debug.Log("プレイヤー敗北");
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
        if (enemy == null)
        {
            Debug.LogWarning($"battle_system: 敵が存在しません（インデックス: {index}）");
            return;
        }

        int damage = Mathf.Max(1, player.ATK - enemy.DEF);
        enemy.HP -= damage;

        Debug.Log($"battle_system: 敵に {damage} ダメージ - 残りHP: {enemy.HP}/{enemy.MAXHP}");

        if (enemy.HP <= 0)
        {
            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName} 撃破");
            Destroy(enemy.gameObject);
            enemies.RemoveAt(index);

            //敵が全滅したらバトル終了
            if (enemies.Count == 0)
            {
                EndBattle();
                return;
            }
        }
        EnemyTurn();
    }
    /// <summary>
    /// 敵のターン
    /// </summary>
    public void EnemyTurn()
    {
        Debug.Log("battle_system: 敵のターン開始");

        foreach (enemy_L enemy in enemies)
        {
            if (enemy == null) continue;

            int damage = Mathf.Max(1, enemy.ATK - player.DEF);
            battlePlayerHP -= damage;

            string enemyName = enemyDataFile != null ? enemyDataFile.GetEnemyName(enemy.enemyId) : "敵";
            Debug.Log($"battle_system: {enemyName}の攻撃！ プレイヤーに {damage} ダメージ - 残りHP: {battlePlayerHP}/{player.MAXHP}");

            if (battlePlayerHP <= 0)
            {
                Debug.Log("battle_system: プレイヤー敗北");
                return;
            }
        }

        Debug.Log("battle_system: 敵のターン終了");
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
}