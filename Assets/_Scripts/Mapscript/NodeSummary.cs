using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RouteMap
{
    /// <summary>
    /// クリックしたノードの簡易情報を保存するクラス
    /// </summary>
    [Serializable]
    public class ClickedNodeInfo
    {
        public int id;
        public StageType stageType;
        public int layerNumber; // 実際のステージ数
        //public int ACT;
        //public 

        public ClickedNodeInfo(int nodeId, StageType type, int layer)
        {
            id = nodeId;
            stageType = type;
            layerNumber = layer;
            
        }
    }



    /// クリックしたノードの情報をまとめて保存するクラス
    public class NodeSummary : MonoBehaviour
    {
        public static NodeSummary Instance { get; private set; }

        [Header("保存されたノード情報")]
        [SerializeField] private List<ClickedNodeInfo> clickedNodes = new List<ClickedNodeInfo>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// stageIdから階層番号（数字）を抽出
        public int ExtractLayerNumber(string stageId)
        {
            if (string.IsNullOrEmpty(stageId))
                return -1;

            // 最後のアンダースコア以降の数字を抽出
            // 例: "F_L_0" → "0", "B_R_1_Treasure" → "Treasure"（数字ではない）
            // より確実に、数字が含まれる部分を探す
            Match match = Regex.Match(stageId, @"_(\d+)");
            if (match.Success && match.Groups.Count > 1)
            {
                if (int.TryParse(match.Groups[1].Value, out int layer))
                {
                    return layer;
                }
            }

            // 数字が見つからない場合（Start, MidBoss, FinalBossなど）
            return -1;
        }

        

        

        /// ノード情報を追加
        public void AddNode(int nodeId, StageType stageType, int layerNumber = -1)
        {
            clickedNodes.Add(new ClickedNodeInfo(nodeId, stageType, layerNumber));
        }

        /// リストをクリア
        public void ClearNodes()
        {
            clickedNodes.Clear();
        }
        
        /// 全ノードを取得
        public List<ClickedNodeInfo> GetAllNodes()
        {
            return clickedNodes;
        }

        /// 最後に追加されたノード（現在のノード）を取得
        public ClickedNodeInfo GetCurrentNode()
        {
            if (clickedNodes.Count > 0)
            {
                return clickedNodes[clickedNodes.Count - 1];
            }
            return null;
        }
    }
}