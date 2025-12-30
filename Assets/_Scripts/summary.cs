using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class summary : MonoBehaviour
{
    public enum GameState{
        Title,      //タイトル画面中
        Map,        //マップ上で移動
        Battle,     //バトル中
        Result,     //バトル後の報酬
        Treasure,   //宝箱
        Shop        //ショップ
    }

    public GameState currentState;
    
    [Header("システム参照")]
    [SerializeField] private treasure treasureSystem; // 宝箱システム（Inspectorで設定推奨）
    [SerializeField] private TreasureScreen treasureScreen; // 宝箱画面UI（Inspectorで設定推奨）
    [SerializeField] private RewardScreen rewardScreen; // バトル報酬画面UI（Inspectorで設定推奨）
    [SerializeField] private stage_data stageData; // ステージ進行度データ（Inspectorで設定推奨）
    [SerializeField] private BattleScreen battleScreen; // バトル画面UI
    [SerializeField] private BackgroundScreen backgroundScreen; // 背景画像管理（Inspectorで設定推奨）

    void Start(){
        // 起動時に背景画像を一度だけ設定（アクティブクリアまたはゲームオーバーまで維持）
        SetupBackgroundImage();
        
        ChangeState(GameState.Title);
    }
    
    /// <summary>
    /// 背景画像を設定（起動時に一度だけ実行）
    /// BackgroundScreenに背景画像を設定
    /// </summary>
    private void SetupBackgroundImage()
    {
        // 惑星名を取得（stage_dataから取得、デフォルトはForest_Planet）
        string planetName = "Forest_Planet"; // デフォルト値
        stage_data targetStageData = stageData;
        if (targetStageData == null)
        {
            targetStageData = FindObjectOfType<stage_data>();
        }
        
        if (targetStageData != null)
        {
            planetName = targetStageData.GetCurrentPlanetName();
            Debug.Log($"summary: 背景画像を設定 - 惑星: {planetName}");
        }
        else
        {
            Debug.LogWarning("summary: stage_dataが見つかりません。デフォルトの惑星名を使用します");
        }

        // BackgroundScreenの背景画像を設定
        BackgroundScreen targetBackgroundScreen = backgroundScreen;
        if (targetBackgroundScreen == null)
        {
            targetBackgroundScreen = FindObjectOfType<BackgroundScreen>();
        }
        
        if (targetBackgroundScreen != null)
        {
            targetBackgroundScreen.SetPlanetBackground(planetName);
            targetBackgroundScreen.Show();
            Debug.Log($"summary: BackgroundScreenの背景画像を設定 - 惑星: {planetName}");
        }
        else
        {
            Debug.LogWarning("summary: BackgroundScreenが見つかりません（背景画像の設定をスキップ）");
        }

        Debug.Log($"summary: 背景画像設定完了 - 惑星: {planetName}（アクティブクリアまたはゲームオーバーまで維持）");
    }

    public void ChangeState(GameState newState){
        currentState = newState;

        switch (newState)
        {
            case GameState.Title:
                Title();
                break;
            
            case GameState.Map:
                Map();
                break;

            case GameState.Battle:
                Battle();
                break;

            case GameState.Result:
                Result();
                break;

            case GameState.Treasure:
                Box();
                break;

            case GameState.Shop:
                Shop();
                break;
        }
    }
    

    public void Title(){
        
    }
    public void Map(){
        
    }
    
    /// <summary>
    /// バトルを開始（内部用）
    /// </summary>
    public void Battle(){
        // テスト用：アイテムを適当に持たせる
        SetupTestItems();
        
        // battle_systemを取得してバトルを開始
        battle_system battleSystem = FindObjectOfType<battle_system>();
        if (battleSystem != null)
        {
            // battle_systemのStartBattle()を呼ぶ（フラグを立ててOnEnable()経由で開始）
            battleSystem.StartBattle();
            Debug.Log("summary: バトル開始 - battle_system.StartBattle()を呼び出しました");
        }
        else
        {
            Debug.LogError("summary: battle_systemが見つかりません");
        }
    }
    
    /// <summary>
    /// テスト用：アイテムを適当に持たせる
    /// </summary>
    private void SetupTestItems()
    {
        item itemData = FindObjectOfType<item>();
        if (itemData != null && itemData.item_list != null)
        {
            // 適当にアイテムを持たせる（ID, [Common, Rare, Epic]）
            itemData.item_list[0] = new List<int>() { 3, 2, 1 }; // リペアユニット: Common3, Rare2, Epic1
            itemData.item_list[1] = new List<int>() { 2, 1, 0 }; // ガトリングガン: Common2, Rare1, Epic0
            itemData.item_list[2] = new List<int>() { 1, 1, 1 }; // 重装ランチャー: Common1, Rare1, Epic1
            itemData.item_list[3] = new List<int>() { 2, 0, 0 }; // 火炎放射器: Common2, Rare0, Epic0
            itemData.item_list[4] = new List<int>() { 1, 2, 0 }; // サイバーハック: Common1, Rare2, Epic0
            itemData.item_list[5] = new List<int>() { 0, 1, 1 }; // オーバークロック: Common0, Rare1, Epic1
            itemData.item_list[6] = new List<int>() { 1, 0, 0 }; // スタンガン: Common1, Rare0, Epic0
            itemData.item_list[7] = new List<int>() { 0, 0, 1 }; // フィールドシールド: Common0, Rare0, Epic1
            itemData.item_list[8] = new List<int>() { 1, 1, 0 }; // EMPキャノン: Common1, Rare1, Epic0
            
            Debug.Log("summary: テスト用アイテムを設定しました");
        }
        else
        {
            Debug.LogWarning("summary: itemが見つかりません（アイテム設定をスキップ）");
        }
    }
    /// <summary>
    /// バトル結果画面
    /// Act3でステージボス撃破時にFinalBossフラグを立てる
    /// </summary>
    public void Result(){
        // Act3でステージボスを撃破した場合、FinalBossフラグを立てる
        CheckAndSetFinalBossFlag();
    }
    
    /// <summary>
    /// バトル報酬を表示（battle_systemから呼び出される）
    /// </summary>
    public void ShowBattleReward(int gold, int skillPoint,
        int itemSmallId, int itemSmallCount,
        int itemMiddleId, int itemMiddleCount,
        int itemLargeId, int itemLargeCount)
    {
        Debug.Log($"summary: バトル報酬を表示 - Gold:{gold}, Skill:{skillPoint}");
        
        // RewardScreenを取得
        RewardScreen targetRewardScreen = rewardScreen;
        if (targetRewardScreen == null)
        {
            targetRewardScreen = FindObjectOfType<RewardScreen>();
            if (targetRewardScreen == null)
            {
                Debug.LogError("summary: RewardScreenが見つかりません（Inspectorで設定するか、シーン内にRewardScreenオブジェクトを配置してください）");
                return;
            }
            Debug.LogWarning("summary: RewardScreenがInspectorで設定されていません。FindObjectOfTypeで取得しました");
        }
        
        // 確認ボタンのイベントに登録
        targetRewardScreen.onConfirmButtonClicked.RemoveAllListeners();
        targetRewardScreen.onConfirmButtonClicked.AddListener(OnRewardConfirmed);
        
        // 報酬画面を表示
        targetRewardScreen.Show();
        targetRewardScreen.ShowRewards(gold, skillPoint,
            itemSmallId, itemSmallCount,
            itemMiddleId, itemMiddleCount,
            itemLargeId, itemLargeCount);
        
        // ゲームステートをResultに変更
        ChangeState(GameState.Result);
        
        Debug.Log("summary: バトル報酬画面を表示しました");
    }
    
    /// <summary>
    /// 報酬画面の確認ボタンが押された時の処理
    /// </summary>
    private void OnRewardConfirmed()
    {
        Debug.Log("summary: 報酬画面の確認ボタンが押されました");
        
        // 報酬画面を非表示
        RewardScreen targetRewardScreen = rewardScreen;
        if (targetRewardScreen == null)
        {
            targetRewardScreen = FindObjectOfType<RewardScreen>();
        }
        
        if (targetRewardScreen != null)
        {
            targetRewardScreen.Hide();
        }
        
        // TODO: 次のステージに進むなどの処理を追加
        // 例: ChangeState(GameState.Map);
        
        Debug.Log("summary: バトル報酬画面完了");
    }
    
    /// <summary>
    /// Act3でステージボス撃破時にFinalBossフラグを立てる
    /// </summary>
    private void CheckAndSetFinalBossFlag()
    {
        if (stageData == null)
        {
            stageData = FindObjectOfType<stage_data>();
        }
        
        if (stageData == null)
        {
            Debug.LogWarning("summary: stage_dataが見つかりません（FinalBossフラグの設定をスキップ）");
            return;
        }
        
        // Act3でステージボス（ステージ11）を撃破した場合
        if (stageData.GetCurrentAct() == 3 && 
            stageData.GetCurrentStageNumber() == 11 && 
            stageData.GetCurrentNodeType() == NodeType.StageBoss)
        {
            stageData.SetFinalBoss(true);
            Debug.Log("summary: Act3のステージボスを撃破しました。FinalBossフラグを立てました。");
        }
    }
    
    /// <summary>
    /// Act進行度を設定（summary.csから管理）
    /// </summary>
    /// <param name="act">Act進行度 (1-3)</param>
    public void SetAct(int act)
    {
        if (stageData == null)
        {
            stageData = FindObjectOfType<stage_data>();
        }
        
        if (stageData != null)
        {
            stageData.SetCurrentAct(act);
            Debug.Log($"summary: Actを{act}に設定しました");
        }
        else
        {
            Debug.LogError("summary: stage_dataが見つかりません");
        }
    }
    
    /// <summary>
    /// 惑星名を設定（summary.csから管理）
    /// </summary>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列）</param>
    public void SetPlanetName(string planetName)
    {
        if (stageData == null)
        {
            stageData = FindObjectOfType<stage_data>();
        }
        
        if (stageData != null)
        {
            stageData.SetCurrentPlanetName(planetName);
            Debug.Log($"summary: 惑星名を{planetName}に設定しました");
        }
        else
        {
            Debug.LogError("summary: stage_dataが見つかりません");
        }
    }
    
    /// <summary>
    /// 宝箱ステージを開始（内部用）
    /// </summary>
    public void Box(){
        // treasureシステムを取得（Inspectorで設定されていない場合はFindObjectOfTypeで取得）
        treasure targetTreasure = treasureSystem;
        if (targetTreasure == null)
        {
            targetTreasure = FindObjectOfType<treasure>();
            if (targetTreasure == null)
            {
                Debug.LogError("summary: treasureが見つかりません（Inspectorで設定するか、シーン内にtreasureオブジェクトを配置してください）");
                return;
            }
            Debug.LogWarning("summary: treasureがInspectorで設定されていません。FindObjectOfTypeで取得しました");
        }

        // 報酬を抽選（treasure.Choose()を呼び出し）
        List<TreasureReward> rewards = targetTreasure.Choose();
        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("summary: 報酬が生成されませんでした");
            return;
        }

        Debug.Log($"summary: 報酬抽選完了 - {rewards.Count}個の報酬");

        // 惑星名を取得（stage_dataから取得、デフォルトはForest_Planet）
        string planetName = "Forest_Planet"; // デフォルト値
        stage_data targetStageData = stageData;
        if (targetStageData == null)
        {
            targetStageData = FindObjectOfType<stage_data>();
        }
        
        if (targetStageData != null)
        {
            planetName = targetStageData.GetCurrentPlanetName();
            Debug.Log($"summary: stage_dataから惑星名を取得 - {planetName}");
        }
        else
        {
            Debug.LogWarning("summary: stage_dataが見つかりません。デフォルトの惑星名を使用します");
        }

        // TreasureScreenを取得して表示（Inspectorで設定されていない場合はFindObjectOfTypeで取得）
        TreasureScreen targetTreasureScreen = treasureScreen;
        if (targetTreasureScreen == null)
        {
            targetTreasureScreen = FindObjectOfType<TreasureScreen>();
            if (targetTreasureScreen == null)
            {
                Debug.LogError("summary: TreasureScreenが見つかりません（Inspectorで設定するか、シーン内にTreasureScreenオブジェクトを配置してください）");
                return;
            }
            Debug.LogWarning("summary: TreasureScreenがInspectorで設定されていません。FindObjectOfTypeで取得しました");
        }

        // 確認ボタンのイベントに登録（前の登録をクリアしてから追加）
        targetTreasureScreen.onConfirmButtonClicked.RemoveAllListeners();
        targetTreasureScreen.onConfirmButtonClicked.AddListener(OnTreasureConfirmed);
        
        // TreasureScreenを表示して報酬を表示
        targetTreasureScreen.Show();
        targetTreasureScreen.ShowTreasure(planetName, rewards);
        Debug.Log($"summary: 宝箱画面を表示 - 惑星: {planetName}, 報酬数: {rewards.Count}");
    }
    
    /// <summary>
    /// 宝箱の確認ボタンが押された時の処理
    /// </summary>
    private void OnTreasureConfirmed()
    {
        Debug.Log("summary: 宝箱の確認ボタンが押されました");
        
        // 宝箱画面を非表示（Inspectorで設定されていない場合はFindObjectOfTypeで取得）
        TreasureScreen targetTreasureScreen = treasureScreen;
        if (targetTreasureScreen == null)
        {
            targetTreasureScreen = FindObjectOfType<TreasureScreen>();
        }
        
        if (targetTreasureScreen != null)
        {
            targetTreasureScreen.Hide();
        }
        
        // TODO: 次のステージに進むなどの処理を追加
        // 例: ChangeState(GameState.Map);
        
        Debug.Log("summary: 宝箱ステージ完了");
    }
    
    public void Shop(){
        
    }
    
    /// <summary>
    /// ステージ起動（テスト用：InspectorのContextMenuから実行可能）
    /// StageDataから情報を取得して、バトルか宝箱かを自動判定して起動
    /// </summary>
    [ContextMenu("ステージ起動")]
    public void StartStage()
    {
        // StageDataを取得
        stage_data targetStageData = stageData;
        if (targetStageData == null)
        {
            targetStageData = FindObjectOfType<stage_data>();
        }
        
        if (targetStageData == null)
        {
            Debug.LogError("summary: stage_dataが見つかりません（ステージ起動を中止）");
            return;
        }
        
        // 現在のノードタイプを取得
        NodeType nodeType = targetStageData.GetCurrentNodeType();
        
        Debug.Log($"summary: ステージ起動 - Act:{targetStageData.GetCurrentAct()}, " +
                  $"Stage:{targetStageData.GetCurrentStageNumber()}, " +
                  $"NodeType:{nodeType}");
        
        // ノードタイプに応じてバトルか宝箱かを判定
        switch (nodeType)
        {
            case NodeType.Battle:
            case NodeType.MidBoss:
            case NodeType.StageBoss:
                // バトル系のステージ
                Battle();
                break;
                
            case NodeType.Treasure:
                // 宝箱ステージ
                Box();
                break;
                
            case NodeType.Shop:
                // ショップステージ（未実装）
                Shop();
                Debug.LogWarning("summary: ショップステージは未実装です");
                break;
                
            default:
                Debug.LogWarning($"summary: 不明なノードタイプ: {nodeType}。バトルとして起動します。");
                Battle();
                break;
        }
    }
}
