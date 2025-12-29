using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// テスト用GameManager
/// 宝箱画面の動作確認用（本番では削除してください）
/// </summary>
public class TestGameManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TreasureScreen treasureScreen; // 直接参照

    [Header("テスト設定")]
    [SerializeField] private string testPlanetName = "Forest_Planet";
    
    [Header("テストアイテムID（0-8）")]
    [Tooltip("0:リペアユニット, 1:ガトリングガン, 2:重装ランチャー, 3:火炎放射器, 4:サイバーハックモジュール, 5:オーバークロックモジュール, 6:スタンガン, 7:フィールドシールド, 8:EMPパルスキャノン")]
    [SerializeField] private List<int> testItemIds = new List<int> { 0, 1, 2 };

    [Header("自動テスト")]
    [SerializeField] private bool autoShowOnStart = true; // 起動時に自動表示

    private void Awake()
    {
        // TreasureScreenの初期化
        if (treasureScreen != null)
        {
            treasureScreen.Initialize();
            treasureScreen.Hide(); // 初期状態では非表示
        }
    }

    private void Start()
    {
        if (autoShowOnStart)
        {
            // 少し待ってから表示（UIの初期化を待つ）
            Invoke(nameof(ShowTreasureScreen), 0.5f);
        }
    }

    /// <summary>
    /// 宝箱画面を表示（テスト用）
    /// Inspectorのボタンや他のスクリプトから呼び出し可能
    /// </summary>
    [ContextMenu("宝箱画面を表示")]
    public void ShowTreasureScreen()
    {
        if (treasureScreen == null)
        {
            Debug.LogError("TestGameManager: TreasureScreenが設定されていません");
            return;
        }

        Debug.Log($"TestGameManager: 宝箱画面を表示 - 惑星: {testPlanetName}, アイテムID: {string.Join(", ", testItemIds)}");

        // 宝箱画面を表示してアイテムを表示
        treasureScreen.Show();
        treasureScreen.ShowTreasure(testPlanetName, testItemIds);
    }

    /// <summary>
    /// 確認ボタンが押された時の処理
    /// TreasureScreenのonConfirmButtonClickedイベントに設定してください
    /// </summary>
    public void OnTreasureConfirmed()
    {
        Debug.Log("TestGameManager: 確認ボタンが押されました");
        
        // 宝箱画面を非表示
        if (treasureScreen != null)
        {
            treasureScreen.Hide();
        }

        Debug.Log("TestGameManager: テスト完了！");
    }

    /// <summary>
    /// 惑星を変更してテスト
    /// </summary>
    [ContextMenu("惑星を変更してテスト (Ice_Planet)")]
    public void TestWithIcePlanet()
    {
        testPlanetName = "Ice_Planet";
        ShowTreasureScreen();
    }

    /// <summary>
    /// 惑星を変更してテスト
    /// </summary>
    [ContextMenu("惑星を変更してテスト (Old_Empire_Planet)")]
    public void TestWithOldEmpirePlanet()
    {
        testPlanetName = "Old_Empire_Planet";
        ShowTreasureScreen();
    }

    /// <summary>
    /// 全アイテムをテスト表示（ID: 0-8）
    /// </summary>
    [ContextMenu("全アイテムを表示")]
    public void ShowAllItems()
    {
        testItemIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        ShowTreasureScreen();
    }

    /// <summary>
    /// ランダムに3つのアイテムを表示
    /// </summary>
    [ContextMenu("ランダム3アイテムを表示")]
    public void ShowRandomItems()
    {
        List<int> allIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        
        // シャッフル
        for (int i = allIds.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = allIds[i];
            allIds[i] = allIds[j];
            allIds[j] = temp;
        }
        
        // 最初の3つを取得
        testItemIds = new List<int> { allIds[0], allIds[1], allIds[2] };
        ShowTreasureScreen();
    }
}
