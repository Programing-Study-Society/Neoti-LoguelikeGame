using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ進行度データ管理クラス
/// Act進行度と惑星名を格納
/// MAP担当者が本実装するまでの仮実装
/// </summary>
public class stage_data : MonoBehaviour
{
    [Header("現在のステージ情報")]
    [Tooltip("Act進行度 (1-10): 1惑星=1Act、10ステージまで")]
    public int actProgress = 1;  // Act進行度（デフォルト: 1）
    
    [Tooltip("現在の惑星名（BattleScreen.PlanetTypeの文字列）")]
    public string currentPlanetName = "Forest_Planet";  // 現在の惑星名（デフォルト: Forest_Planet）
    
    [Header("追加情報（必要に応じて使用）")]
    [Tooltip("現在の惑星番号（1-3、1プレイで訪れる惑星の順番）")]
    public int currentPlanetNumber = 1;  // 現在の惑星番号（1-3）
    
    /// <summary>
    /// Act進行度を取得
    /// </summary>
    /// <returns>Act進行度 (1-10)</returns>
    public int GetActProgress()
    {
        return actProgress;
    }
    
    /// <summary>
    /// 現在の惑星名を取得
    /// </summary>
    /// <returns>惑星名（BattleScreen.PlanetTypeの文字列）</returns>
    public string GetCurrentPlanetName()
    {
        return currentPlanetName;
    }
    
    /// <summary>
    /// Act進行度を設定
    /// </summary>
    /// <param name="progress">進行度 (1-10)</param>
    public void SetActProgress(int progress)
    {
        actProgress = Mathf.Clamp(progress, 1, 10);
    }
    
    /// <summary>
    /// Act進行度を進める
    /// </summary>
    /// <param name="amount">進める量（デフォルト: 1）</param>
    public void AdvanceActProgress(int amount = 1)
    {
        actProgress = Mathf.Clamp(actProgress + amount, 1, 10);
    }
    
    /// <summary>
    /// 現在の惑星名を設定
    /// </summary>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列）</param>
    public void SetCurrentPlanetName(string planetName)
    {
        currentPlanetName = planetName;
    }
    
    /// <summary>
    /// Act進行度と惑星名を設定
    /// </summary>
    /// <param name="progress">進行度 (1-10)</param>
    /// <param name="planetName">惑星名</param>
    public void SetStageData(int progress, string planetName)
    {
        SetActProgress(progress);
        SetCurrentPlanetName(planetName);
    }
    
    /// <summary>
    /// 次のステージに進む（Act進行度を+1）
    /// Act進行度が10の場合、次の惑星へ（未実装）
    /// </summary>
    public void GoToNextStage()
    {
        if (actProgress < 10)
        {
            AdvanceActProgress(1);
        }
        else
        {
            // Act進行度が10の場合、次の惑星へ
            // TODO: 次の惑星選択ロジック（MAP担当者が実装）
            Debug.Log("Act進行度が10に達しました。次の惑星へ進む処理は未実装です。");
        }
    }
}

