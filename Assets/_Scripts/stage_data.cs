using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノードタイプ（ステージの種類）
/// </summary>
public enum NodeType
{
    Battle,      // バトルステージ
    Treasure,    // 宝箱ステージ
    Shop,        // ショップステージ（未実装）
    MidBoss,     // 中ボスステージ
    StageBoss    // ステージボス（Act終了）
}

/// <summary>
/// ステージ進行度データ管理クラス
/// Act進行度、ステージ情報、ノード情報を格納
/// </summary>
public class stage_data : MonoBehaviour
{
    [Header("現在のAct情報")]
    [Tooltip("Act進行度 (1-3): ゲーム全体の進行度")]
    public int currentAct = 1;  // Act進行度（デフォルト: 1）
    
    [Tooltip("現在の惑星名（BattleScreen.PlanetTypeの文字列）")]
    public string currentPlanetName = "Forest_Planet";  // 現在の惑星名（デフォルト: Forest_Planet）
    
    [Tooltip("現在の惑星番号（1-3、1プレイで訪れる惑星の順番）")]
    public int currentPlanetNumber = 1;  // 現在の惑星番号（1-3）
    
    [Header("現在のステージ情報")]
    [Tooltip("現在のステージ番号 (1-11): Act内のステージ進行度")]
    public int currentStageNumber = 1;  // 現在のステージ番号（1-11、または0-11で12）
    
    [Header("現在のノード情報")]
    [Tooltip("現在のノードID (0-19): 全20ノード（ノーマル18 + 中ボス1 + ステージボス1）")]
    public int currentNodeId = 0;  // 現在のノードID（0-19）
    
    [Tooltip("現在のノードタイプ（Battle/Treasure/Shop/MidBoss/StageBoss）")]
    public NodeType currentNodeType = NodeType.Battle;  // 現在のノードタイプ
    
    [Tooltip("ノードが指すステージの文字列情報（識別用）")]
    public string currentNodeStage = "";  // ノードが指すステージの文字列情報
    
    [Header("最終ボスフラグ")]
    [Tooltip("最終ボス戦かどうか（Act3のステージボス撃破後にtrue）")]
    public bool isFinalBoss = false;  // 最終ボス戦フラグ
    
    /// <summary>
    /// Act進行度を取得（後方互換性のため残す）
    /// </summary>
    /// <returns>Act進行度 (1-3)</returns>
    [System.Obsolete("GetActProgress()は非推奨です。GetCurrentAct()を使用してください。")]
    public int GetActProgress()
    {
        return currentAct;
    }
    
    /// <summary>
    /// 現在のAct進行度を取得
    /// </summary>
    /// <returns>Act進行度 (1-3)</returns>
    public int GetCurrentAct()
    {
        return currentAct;
    }
    
    /// <summary>
    /// 現在のステージ番号を取得
    /// </summary>
    /// <returns>ステージ番号 (1-11)</returns>
    public int GetCurrentStageNumber()
    {
        return currentStageNumber;
    }
    
    /// <summary>
    /// 現在のノードIDを取得
    /// </summary>
    /// <returns>ノードID (0-19)</returns>
    public int GetCurrentNodeId()
    {
        return currentNodeId;
    }
    
    /// <summary>
    /// 現在のノードタイプを取得
    /// </summary>
    /// <returns>ノードタイプ</returns>
    public NodeType GetCurrentNodeType()
    {
        return currentNodeType;
    }
    
    /// <summary>
    /// 現在のノードが指すステージの文字列情報を取得
    /// </summary>
    /// <returns>ステージの文字列情報</returns>
    public string GetCurrentNodeStage()
    {
        return currentNodeStage;
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
    /// 最終ボス戦フラグを取得
    /// </summary>
    /// <returns>最終ボス戦かどうか</returns>
    public bool IsFinalBoss()
    {
        return isFinalBoss;
    }
    
    /// <summary>
    /// 最終ボス戦フラグを設定
    /// </summary>
    /// <param name="value">最終ボス戦かどうか</param>
    public void SetFinalBoss(bool value)
    {
        isFinalBoss = value;
    }
    
    /// <summary>
    /// Act進行度を設定（後方互換性のため残す）
    /// </summary>
    /// <param name="progress">進行度 (1-3)</param>
    [System.Obsolete("SetActProgress()は非推奨です。SetCurrentAct()を使用してください。")]
    public void SetActProgress(int progress)
    {
        currentAct = Mathf.Clamp(progress, 1, 3);
    }
    
    /// <summary>
    /// 現在のAct進行度を設定
    /// </summary>
    /// <param name="act">Act進行度 (1-3)</param>
    public void SetCurrentAct(int act)
    {
        currentAct = Mathf.Clamp(act, 1, 3);
    }
    
    /// <summary>
    /// 現在のステージ番号を設定
    /// </summary>
    /// <param name="stageNumber">ステージ番号 (1-11)</param>
    public void SetCurrentStageNumber(int stageNumber)
    {
        currentStageNumber = Mathf.Clamp(stageNumber, 1, 11);
    }
    
    /// <summary>
    /// 現在のノード情報を設定
    /// </summary>
    /// <param name="nodeId">ノードID (0-19)</param>
    /// <param name="nodeType">ノードタイプ</param>
    /// <param name="nodeStage">ノードが指すステージの文字列情報</param>
    public void SetCurrentNode(int nodeId, NodeType nodeType, string nodeStage)
    {
        currentNodeId = Mathf.Clamp(nodeId, 0, 19);
        currentNodeType = nodeType;
        currentNodeStage = nodeStage ?? "";
    }
    
    /// <summary>
    /// ステージ番号を進める
    /// </summary>
    /// <param name="amount">進める量（デフォルト: 1）</param>
    public void AdvanceStageNumber(int amount = 1)
    {
        currentStageNumber = Mathf.Clamp(currentStageNumber + amount, 1, 11);
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
    /// Act進行度と惑星名を設定（後方互換性のため残す）
    /// </summary>
    /// <param name="progress">進行度 (1-3)</param>
    /// <param name="planetName">惑星名</param>
    [System.Obsolete("SetStageData()は非推奨です。SetCurrentAct()とSetCurrentPlanetName()を使用してください。")]
    public void SetStageData(int progress, string planetName)
    {
        SetCurrentAct(progress);
        SetCurrentPlanetName(planetName);
    }
    
    /// <summary>
    /// ステージ情報を一括設定
    /// </summary>
    /// <param name="act">Act進行度 (1-3)</param>
    /// <param name="planetName">惑星名</param>
    /// <param name="stageNumber">ステージ番号 (1-11)</param>
    /// <param name="nodeId">ノードID (0-19)</param>
    /// <param name="nodeType">ノードタイプ</param>
    /// <param name="nodeStage">ノードが指すステージの文字列情報</param>
    public void SetStageData(int act, string planetName, int stageNumber, int nodeId, NodeType nodeType, string nodeStage)
    {
        SetCurrentAct(act);
        SetCurrentPlanetName(planetName);
        SetCurrentStageNumber(stageNumber);
        SetCurrentNode(nodeId, nodeType, nodeStage);
    }
    
    /// <summary>
    /// 次のステージに進む（ステージ番号を+1）
    /// ステージ11の場合、次のActへ（Act3の場合は最終ボスへ）
    /// </summary>
    public void GoToNextStage()
    {
        if (currentStageNumber < 11)
        {
            AdvanceStageNumber(1);
        }
        else
        {
            // ステージ11（ステージボス）をクリアした場合
            if (currentAct < 3)
            {
                // Act1,2の場合は次のActへ
                SetCurrentAct(currentAct + 1);
                SetCurrentStageNumber(1); // 次のActのステージ1へ
                Debug.Log($"Act{currentAct}をクリアしました。Act{currentAct + 1}へ進みます。");
            }
            else
            {
                // Act3の場合は最終ボスへ
                Debug.Log("Act3をクリアしました。最終ボスへ進みます。");
                // TODO: 最終ボス戦への遷移処理
            }
        }
    }
}

