using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵出現管理クラス
/// Act進行度と惑星名から出現する敵を選出する
/// </summary>
public class EnemySpawnManager : MonoBehaviour
{
    [Header("データ参照")]
    [SerializeField] private stage_data stageData;      // ステージ進行度データ
    [SerializeField] private enemy_L enemyDataFile;      // 敵データファイル

    /// <summary>
    /// 現在の進行状況から出現する敵IDリストを生成
    /// stage_dataから進行度と惑星名を取得して選出
    /// </summary>
    /// <param name="count">出現数（-1の場合は1~2体をランダムに決定）</param>
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
        int actProgress = stageData.GetActProgress();
        string planetName = stageData.GetCurrentPlanetName();

        return GenerateEnemyIds(actProgress, planetName, count);
    }

    /// <summary>
    /// Act進行度と惑星名から出現する敵IDリストを生成
    /// </summary>
    /// <param name="actProgress">Act進行度 (1-10)</param>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列）</param>
    /// <param name="count">出現数（-1の場合は1~2体をランダムに決定）</param>
    /// <returns>出現する敵IDのリスト</returns>
    public List<int> GenerateEnemyIds(int actProgress, string planetName, int count = -1)
    {
        if (enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: enemy_Lが設定されていません");
            return new List<int>();
        }

        // 出現数を決定（-1の場合は1~2体をランダム）
        if (count < 0)
        {
            count = Random.Range(1, 3); // 1 or 2
        }

        List<int> candidateIds = new List<int>();

        // Act進行度に応じて敵タイプを決定
        if (actProgress >= 1 && actProgress <= 3)
        {
            // 序盤：雑魚敵のみ
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
            Debug.Log($"EnemySpawnManager: Act進行度 {actProgress} - 雑魚敵のみ");
        }
        else if (actProgress >= 4 && actProgress <= 6)
        {
            // 中盤：雑魚敵と中間敵の混合
            var common = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
            var midBoss = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.MidBoss);
            candidateIds.AddRange(common);
            candidateIds.AddRange(midBoss);
            Debug.Log($"EnemySpawnManager: Act進行度 {actProgress} - 雑魚敵と中間敵の混合");
        }
        else if (actProgress >= 7 && actProgress <= 9)
        {
            // 終盤：中間敵のみ
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.MidBoss);
            Debug.Log($"EnemySpawnManager: Act進行度 {actProgress} - 中間敵のみ");
        }
        else if (actProgress == 10)
        {
            // ボス戦：惑星ボスのみ
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Boss);
            Debug.Log($"EnemySpawnManager: Act進行度 {actProgress} - 惑星ボスのみ");
        }
        else
        {
            Debug.LogWarning($"EnemySpawnManager: 無効なAct進行度 {actProgress}（1-10の範囲外）");
            // フォールバック：雑魚敵を返す
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
        }

        // 候補が空の場合はエラー
        if (candidateIds.Count == 0)
        {
            Debug.LogWarning($"EnemySpawnManager: 候補敵が見つかりません - 惑星: {planetName}, 進行度: {actProgress}");
            return new List<int>();
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
    /// テスト用：指定した進行度と惑星で敵を選出
    /// </summary>
    /// <param name="actProgress">Act進行度</param>
    /// <param name="planetName">惑星名</param>
    /// <param name="count">出現数</param>
    /// <returns>出現する敵IDのリスト</returns>
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

    /// <summary>
    /// 詳細テスト：指定した条件で候補敵と選出結果を表示
    /// </summary>
    /// <param name="actProgress">Act進行度</param>
    /// <param name="planetName">惑星名</param>
    /// <param name="testCount">テスト実行回数（複数回実行してランダム性を確認）</param>
    private void TestDetailed(int actProgress, string planetName, int testCount = 3)
    {
        if (enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: enemy_Lが設定されていません");
            return;
        }

        Debug.Log($"========================================");
        Debug.Log($"【テストケース】");
        Debug.Log($"  惑星: {planetName}");
        Debug.Log($"  Act進行度: {actProgress}");
        Debug.Log($"========================================");

        // 候補となる敵を取得
        List<int> candidateIds = new List<int>();
        string stageType = "";

        if (actProgress >= 1 && actProgress <= 3)
        {
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
            stageType = "雑魚敵のみ";
        }
        else if (actProgress >= 4 && actProgress <= 6)
        {
            var common = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Common);
            var midBoss = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.MidBoss);
            candidateIds.AddRange(common);
            candidateIds.AddRange(midBoss);
            stageType = "雑魚敵と中間敵の混合";
        }
        else if (actProgress >= 7 && actProgress <= 9)
        {
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.MidBoss);
            stageType = "中間敵のみ";
        }
        else if (actProgress == 10)
        {
            candidateIds = enemyDataFile.GetEnemyIdsByPlanetAndType(planetName, EnemyType.Boss);
            stageType = "ボス戦";
        }

        // 候補敵を表示
        Debug.Log($"\n【{planetName} - Act{actProgress}】出現可能な敵一覧");
        Debug.Log($"ステージタイプ: {stageType}");
        Debug.Log($"候補敵数: {candidateIds.Count}体");
        if (candidateIds.Count == 0)
        {
            Debug.LogWarning($"  ⚠️ 候補敵が見つかりません！");
        }
        else
        {
            foreach (int id in candidateIds)
            {
                string name = enemyDataFile.GetEnemyName(id);
                EnemyType type = enemyDataFile.GetEnemyType(id);
                string planet = enemyDataFile.GetEnemyPlanet(id);
                Debug.Log($"  - 敵ID: {id}, 名前: {name}, タイプ: {type}, 出現惑星: {planet}");
            }
        }

        // 複数回テスト実行（ランダム性確認）
        Debug.Log($"\n【{planetName} - Act{actProgress}】{testCount}回の選出テスト実行");
        for (int i = 0; i < testCount; i++)
        {
            List<int> result = GenerateEnemyIds(actProgress, planetName, -1); // ランダム数（1~2体）
            Debug.Log($"  テスト {i + 1}: {result.Count}体選出");
            if (result.Count == 0)
            {
                Debug.LogWarning($"    ⚠️ 敵が選出されませんでした！");
            }
            else
            {
                foreach (int id in result)
                {
                    string name = enemyDataFile.GetEnemyName(id);
                    EnemyType type = enemyDataFile.GetEnemyType(id);
                    Debug.Log($"    → 敵ID: {id}, 名前: {name}, タイプ: {type}");
                }
            }
        }
        Debug.Log($"========================================\n");
    }

    /// <summary>
    /// 全テストケースを実行：全惑星の全Act進行度（1-10）をテスト
    /// </summary>
    [ContextMenu("テスト: 全ケース実行")]
    public void RunAllTestCases()
    {
        if (enemyDataFile == null)
        {
            Debug.LogError("EnemySpawnManager: enemy_Lが設定されていません");
            return;
        }

        string[] planets = { "Forest_Planet", "Old_Empire_Planet", "Volcano_Planet", "Ice_Planet" };
        int[] actProgresses = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // 全Act進行度

        Debug.Log("========================================");
        Debug.Log("【EnemySpawnManager 全テストケース実行】");
        Debug.Log($"テスト対象惑星: {string.Join(", ", planets)}");
        Debug.Log($"テスト対象Act進行度: {string.Join(", ", actProgresses)}");
        Debug.Log($"合計テストケース数: {planets.Length}惑星 × {actProgresses.Length}Act = {planets.Length * actProgresses.Length}ケース");
        Debug.Log("========================================\n");

        int testCaseCount = 0;
        int totalTests = planets.Length * actProgresses.Length;
        
        foreach (string planet in planets)
        {
            Debug.Log($"\n■■■ 惑星: {planet} のテスト開始 ■■■");
            foreach (int act in actProgresses)
            {
                testCaseCount++;
                Debug.Log($"\n>>> [{testCaseCount}/{totalTests}] {planet} - Act{act} <<<");
                TestDetailed(act, planet, 3); // 各ケースを3回実行
            }
            Debug.Log($"\n■■■ 惑星: {planet} のテスト完了 ■■■\n");
        }

        Debug.Log("========================================");
        Debug.Log($"【全テスト完了】");
        Debug.Log($"  実行ケース数: {testCaseCount}ケース");
        Debug.Log($"  テスト惑星数: {planets.Length}惑星");
        Debug.Log($"  テストAct数: {actProgresses.Length}Act");
        Debug.Log("========================================");
    }
}

