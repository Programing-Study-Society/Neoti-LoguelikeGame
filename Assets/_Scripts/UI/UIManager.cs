using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI全体の管理を行うクラス
/// 各画面の表示・非表示を制御する
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("画面管理")]
    [SerializeField] private TitleScreen titleScreen; // タイトル画面
    [SerializeField] private MapScreen mapScreen; // マップ画面

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// UI管理の初期化
    /// </summary>
    private void Initialize()
    {
        // タイトル画面の初期化
        if (titleScreen != null)
        {
            titleScreen.Initialize();
            ShowTitleScreen();
        }

        // マップ画面の初期化
        if (mapScreen != null)
        {
            mapScreen.Initialize();
            mapScreen.Hide(); // 初期状態では非表示
        }
    }

    /// <summary>
    /// タイトル画面を表示する
    /// </summary>
    public void ShowTitleScreen()
    {
        if (titleScreen != null)
        {
            titleScreen.Show();
        }
    }

    /// <summary>
    /// タイトル画面を非表示にする
    /// </summary>
    public void HideTitleScreen()
    {
        if (titleScreen != null)
        {
            titleScreen.Hide();
        }
    }

    /// <summary>
    /// マップ画面を表示する
    /// </summary>
    public void ShowMapScreen()
    {
        if (mapScreen != null)
        {
            mapScreen.Show();
        }
    }

    /// <summary>
    /// マップ画面を非表示にする
    /// </summary>
    public void HideMapScreen()
    {
        if (mapScreen != null)
        {
            mapScreen.Hide();
        }
    }
}
