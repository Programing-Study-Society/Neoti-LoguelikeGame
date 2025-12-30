using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵出現管理クラス
/// ノードタイプとステージ番号から出現する敵を選出する
/// </summary>
public class EnemySpawnManager : MonoBehaviour
{
    [Header("データ参照")]
    [SerializeField] private stage_data stageData;      // ステージ進行度データ
    [SerializeField] private enemy_L enemyDataFile;      // 敵データファイル

    /// <summary>
    /// 現在の進行状況から出現する敵IDリストを生成
    /// stage_dataからノードタイプ、ステージ番号、惑星名を取得して選出
    /// </summary>
    /// <param name="count">出現数（-1の場合は1~3体をランダムに決定）</param>
    /// <returns>出現する敵IDのリスト</returns>
    public List<int> GenerateEnemyIds(int count = -1)
    {
        if (stageData == null)
        {
            Debug.LogError("EnemySpawnManager: stage_dataが設定されていません");
            return new List<int>();
        }

        if (enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: enemy_Lが設定されていません");
            return new List<int>();
        }

        // stage_dataから進行状況を取得
        NodeType nodeType = stageData.GetCurrentNodeType();
        int stageNumber = stageData.GetCurrentStageNumber();
        string planetName = stageData.GetCurrentPlanetName();
        bool isFinalBoss = stageData.IsFinalBoss();

        return GenerateEnemyIds(nodeType, stageNumber, planetName, isFinalBoss, count);
    }

    /// <summary>
    /// ノードタイプ、ステージ番号、惑星名から出現する敵IDリストを生成
    /// </summary>
    /// <param name="nodeType">ノードタイプ</param>
    /// <param name="stageNumber">ステージ番号 (1-11)</param>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列）</param>
    /// <param name="isFinalBoss">最終ボス戦かどうか</param>
    /// <param name="count">出現数（-1の場合は1~3体をランダムに決定）</param>
    /// <returns>出現する敵IDのリスト</returns>
    public List<int> GenerateEnemyIds(NodeType nodeType, int stageNumber, string planetName, bool isFinalBoss, int count = -1)
    {
        if (enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: enemy_Lが設定されていません");
            return new List<int>();
        }

        // 最終ボス戦の場合
        if (isFinalBoss)
        {
            List<int> finalBossIds = enemyDataFile.GetEnemyIdsByType(EnemyType.FinalBoss);
            if (finalBossIds.Count > 0)
            {
                Debug.Log($"EnemySpawnManager: 最終ボス戦 - 敵ID: {finalBossIds[0]}");
                return new List<int>() { finalBossIds[0] }; // 最終ボスは1体のみ
            }
            else
            {
                Debug.LogWarning("EnemySpawnManager: 最終ボスが見つかりません");
                return new List<int>();
            }
        }

        // ノードタイプに応じて敵タイプを決定
        List<int> candidateIds = new List<int>();

        if (nodeType == NodeType.MidBoss)
        {
            // 中ボスステージ：中ボス1体のみ
            int midBossId = enemyDataFile.GetMidBossId(planetName);
            if (midBossId >= 0)
            {
                // 中ボスの名前・画像を初期化（まだ初期化されていない場合）
                enemyDataFile.InitializeMidBoss(planetName);
                candidateIds.Add(midBossId);
                Debug.Log($"EnemySpawnManager: 中ボスステージ - 敵ID: {midBossId}");
            }
            else
            {
                Debug.LogWarning($"EnemySpawnManager: 惑星 '{planetName}' の中ボスが見つかりません");
            }
        }
        else if (nodeType == NodeType.StageBoss)
        {
            // ステージボス：ステージボス1体のみ
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Boss);
            Debug.Log($"EnemySpawnManager: ステージボス - 候補数: {candidateIds.Count}");
        }
        else if (nodeType == NodeType.Battle)
        {
            // 通常バトル：ステージ番号に応じて敵タイプを決定
            if (stageNumber >= 1 && stageNumber <= 3)
            {
                // ステージ1-3: 雑魚敵のみ
                candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
                Debug.Log($"EnemySpawnManager: ステージ{stageNumber} - 雑魚敵のみ");
            }
            else if (stageNumber >= 4 && stageNumber <= 6)
            {
                // ステージ4-6: 雑魚敵と固有敵の混合
                var common = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
                var unique = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Unique);
                candidateIds.AddRange(common);
                candidateIds.AddRange(unique);
                Debug.Log($"EnemySpawnManager: ステージ{stageNumber} - 雑魚敵と固有敵の混合");
            }
            else if (stageNumber >= 7 && stageNumber <= 10)
            {
                // ステージ7-10: 固有敵のみ
                candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Unique);
                Debug.Log($"EnemySpawnManager: ステージ{stageNumber} - 固有敵のみ");
            }
            else
            {
                Debug.LogWarning($"EnemySpawnManager: 無効なステージ番号 {stageNumber}（1-11の範囲外）");
                // フォールバック：雑魚敵を返す
                candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
            }
        }
        else
        {
            // Treasure/Shopなど、バトル以外のノードタイプ
            Debug.LogWarning($"EnemySpawnManager: バトル以外のノードタイプ {nodeType} が指定されました");
            return new List<int>();
        }

        // 候補が空の場合はエラー
        if (candidateIds.Count == 0)
        {
            Debug.LogWarning($"EnemySpawnManager: 候補敵が見つかりません - 惑星: {planetName}, ステージ: {stageNumber}, ノードタイプ: {nodeType}");
            return new List<int>();
        }

        // 中ボス・ステージボス・最終ボスの場合は1体のみ
        if (nodeType == NodeType.MidBoss || nodeType == NodeType.StageBoss || isFinalBoss)
        {
            if (candidateIds.Count > 0)
            {
                Debug.Log($"EnemySpawnManager: {nodeType} - 敵ID: {candidateIds[0]}");
                return new List<int>() { candidateIds[0] };
            }
        }

        // 通常バトルの場合：出現数を決定（-1の場合は1~3体をランダム）
        if (count < 0)
        {
            count = Random.Range(1, 4); // 1, 2, or 3
        }

        // 候補からランダムに選出（重複なし）
        List<int> result = new List<int>();
        List<int> availableCandidates = new List<int>(candidateIds); // コピーを作成

        for (int i = 0; i < count && availableCandidates.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableCandidates.Count);
            int selectedId = availableCandidates[randomIndex];
            result.Add(selectedId);
            availableCandidates.RemoveAt(randomIndex); // 重複を防ぐ
        }

        Debug.Log($"EnemySpawnManager: {result.Count}体の敵を選出 - 敵ID: {string.Join(", ", result)}");
        return result;
    }

    /// <summary>
    /// テスト用：現在のステージ情報から敵を選出
    /// </summary>
    [ContextMenu("Test: Spawn Enemies")]
    public void TestEnemySpawn()
    {
        if (stageData == null || enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: stage_dataまたはenemy_Lが設定されていません");
            return;
        }

        List<int> enemyIds = GenerateEnemyIds();
        Debug.Log($"テスト結果: {enemyIds.Count}体の敵が選出されました");
        foreach (int id in enemyIds)
        {
            string name = enemyDataFile.GetEnemyName(id);
            EnemyType type = enemyDataFile.GetEnemyType(id);
            Debug.Log($"  - 敵ID: {id}, 名前: {name}, タイプ: {type}");
        }
    }

}

