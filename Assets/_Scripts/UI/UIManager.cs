using UnityEngine;

/// <summary>
/// UI全体の管理を行うクラス
/// 各画面の表示・非表示を制御する
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("画面管理")]
    [SerializeField] private TreasureScreen treasureScreen; // 宝箱画面

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// UI管理の初期化
    /// </summary>
    private void Initialize()
    {
        // 宝箱画面の初期化
        if (treasureScreen != null)
        {
            treasureScreen.Initialize();
            treasureScreen.Hide(); // 初期状態では非表示
        }
    }

    /// <summary>
    /// 宝箱画面を表示する
    /// </summary>
    public void ShowTreasureScreen()
    {
        if (treasureScreen != null)
        {
            treasureScreen.Show();
        }
    }

    /// <summary>
    /// 宝箱画面を非表示にする
    /// </summary>
    public void HideTreasureScreen()
    {
        if (treasureScreen != null)
        {
            treasureScreen.Hide();
        }
    }

    /// <summary>
    /// 宝箱画面のTreasureScreenを取得（ロジック側がアイテム表示を呼び出すため）
    /// </summary>
    public TreasureScreen GetTreasureScreen()
    {
        return treasureScreen;
    }
}
