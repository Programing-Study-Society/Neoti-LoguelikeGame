using System.Collections.Generic;
using UnityEngine;

namespace RouteMap
{
    public class RouteMapGenerator : MonoBehaviour
    {
        [Header("基本設定")]
        [Tooltip("最終的に必ずこの数に合わせる（スタート・中ボス・最終ボス含む）")]
        public int targetStageCount = 10;

        [Tooltip("スタートノードのX座標")]
        public float startX = 0f;

        [Tooltip("ノード間のX方向の距離")]
        public float xStep = 3f;

        [Tooltip("分岐レーンのYオフセット（上ルート=+y, 下ルート=-y）")]
        public float laneYOffset = 1.5f;

        [Header("コンテンツ設定")]
        [Tooltip("宝ノードの最大数（0以上）")]
        public int maxTreasureCount = 2;

        [Tooltip("ショップは前半1ステップ目に出さない")]
        public bool forbidShopOnFirstStep = true;

        public GameObject nodePrefab;
        public GameObject linePrefab;

        [SerializeField] private List<RouteNode> generatedNodes = new List<RouteNode>();
        public IReadOnlyList<RouteNode> GeneratedNodes => generatedNodes;
        [SerializeField] private int currentNodeId = 0; // プレイヤーがいるノードID
        public int CurrentNodeId => currentNodeId;

        private System.Random random;

        private void Awake() => random = new System.Random();
        private void Start() => GenerateAndBuild();

        public void GenerateAndBuild()
        {
            ClearOldMap();
            generatedNodes = GenerateRoute();
            RebuildNodeDict(generatedNodes);
            BuildVisuals(generatedNodes);
        }

        private void ClearOldMap()
        {
            var children = new List<Transform>();
            foreach (Transform child in transform) children.Add(child);
            foreach (var child in children) Destroy(child.gameObject);
        }

        private List<RouteNode> GenerateRoute()
        {
            var nodes = new List<RouteNode>();
            int currentId = 0;
            float currentX = startX;

            // 0. Start
            var startNode = new RouteNode
            {
                id = currentId,
                position = new Vector2(currentX, 0f),
                stageType = StageType.Start,
                stageId = "Start"
            };
            nodes.Add(startNode);

            // 目標総数: targetStageCount
            // 既に1（Start）使用。中ボス・最終ボスを入れて合計10に調整する。
            // 残りを前半分岐＋後半分岐で割り振る。
            // 配分: Start(1) + 前半k + MidBoss(1) + 後半m + FinalBoss(1) = targetStageCount
            // よって k + m = targetStageCount - 3
            int slotsForBranches = Mathf.Max(targetStageCount - 3, 2); // 少なくとも2
            int frontSteps = Mathf.Clamp(random.Next(2, 5), 1, slotsForBranches - 1);
            int backSteps = Mathf.Max(slotsForBranches - frontSteps, 1);

            // 前半は2レーンでfrontSteps回進む => 2*frontStepsノードが増える
            // 後半も同様
            // Start(1) + 2*frontSteps + Mid(1) + 2*backSteps + Final(1) == targetStageCount になるように再調整
            int totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
            int diff = targetStageCount - totalIf;
            // diffが正なら後半に足す、負なら後半を削る（最低1ステップは残す）
            backSteps = Mathf.Max(1, backSteps + (diff / 2)); // ざっくり補正
            // 再計算
            totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
            // まだズレる場合はfront/backの最後にNormalを足す/引くなどで補正
            while (totalIf < targetStageCount)
            {
                backSteps++;
                totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
            }
            while (totalIf > targetStageCount && backSteps > 1)
            {
                backSteps--;
                totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
            }

            float leftY = laneYOffset;
            float rightY = -laneYOffset;

            int leftCurrentId = currentId;
            int rightCurrentId = currentId;

            // 1. 前半分岐（通常バトル）
            for (int i = 0; i < frontSteps; i++)
            {
                currentX += xStep;

                int leftId = nodes.Count;
                var leftNode = CreateBranchNode(leftId, currentX, leftY, StageType.Battle, $"F_L_{i}");
                nodes.Add(leftNode);
                nodes[leftCurrentId].nextNodeIds.Add(leftId);
                leftCurrentId = leftId;

                int rightId = nodes.Count;
                var rightNode = CreateBranchNode(rightId, currentX, rightY, StageType.Battle, $"F_R_{i}");
                nodes.Add(rightNode);
                nodes[rightCurrentId].nextNodeIds.Add(rightId);
                rightCurrentId = rightId;
            }

            // 2. 中ボス（合流）
            currentX += xStep;
            int midBossId = nodes.Count;
            var midBoss = new RouteNode
            {
                id = midBossId,
                position = new Vector2(currentX, 0f),
                stageType = StageType.MidBoss,
                stageId = "MidBoss"
            };
            nodes.Add(midBoss);
            nodes[leftCurrentId].nextNodeIds.Add(midBossId);
            nodes[rightCurrentId].nextNodeIds.Add(midBossId);

            // 3. 後半分岐
            leftCurrentId = midBossId;
            rightCurrentId = midBossId;

            for (int i = 0; i < backSteps; i++)
            {
                currentX += xStep;

                int leftId = nodes.Count;
                var leftNode = CreateBranchNode(leftId, currentX, leftY, StageType.Battle, $"B_L_{i}");
                nodes.Add(leftNode);
                nodes[leftCurrentId].nextNodeIds.Add(leftId);
                leftCurrentId = leftId;

                int rightId = nodes.Count;
                var rightNode = CreateBranchNode(rightId, currentX, rightY, StageType.Battle, $"B_R_{i}");
                nodes.Add(rightNode);
                nodes[rightCurrentId].nextNodeIds.Add(rightId);
                rightCurrentId = rightId;
            }

            // 4. 最終ボス（合流）
            currentX += xStep;
            int bossId = nodes.Count;
            var bossNode = new RouteNode
            {
                id = bossId,
                position = new Vector2(currentX, 0f),
                stageType = StageType.FinalBoss,
                stageId = "FinalBoss"
            };
            nodes.Add(bossNode);
            nodes[leftCurrentId].nextNodeIds.Add(bossId);
            nodes[rightCurrentId].nextNodeIds.Add(bossId);

            // --- 5. 種別割り当て（宝・ショップ） ---
            AssignSpecialStages(nodes, frontSteps, backSteps);

            return nodes;
        }

        private RouteNode CreateBranchNode(int id, float x, float y, StageType type, string stageId)
        {
            return new RouteNode
            {
                id = id,
                position = new Vector2(x, y),
                stageType = type,
                stageId = stageId
            };
        }

        /// 宝とショップの配置制約を反映
        /// - 宝: 最大 maxTreasureCount
        /// - ショップ: forbidShopOnFirstStep が true の場合、前半1ステップ目には置かない
        private void AssignSpecialStages(List<RouteNode> nodes, int frontSteps, int backSteps)
        {
            // Start(0) / MidBoss / FinalBoss は対象外
            var candidateIds = new List<int>();
            foreach (var n in nodes) // 通常戦闘（Battle）ノードのみ抽出
            {
                if (n.stageType == StageType.Battle)
                    candidateIds.Add(n.id);
            }

            // シャッフル
            Shuffle(candidateIds);

            int treasurePlaced = 0;
            bool shopPlaced = false;

            foreach (int id in candidateIds)
            {
                var node = nodes[id];

                // ショップは前半1ステップ目に置かない制約
                // 前半1ステップ目のノードIDは Start を0として、1/2が前半1ステップ目（左・右）
                bool isFirstFrontStep =
                    (node.stageId.StartsWith("F_L_0") || node.stageId.StartsWith("F_R_0"));

                // まず宝を優先配置
                if (treasurePlaced < maxTreasureCount)
                {
                    node.stageType = StageType.Treasure;
                    node.stageId = node.stageId + "_Treasure";
                    treasurePlaced++;
                    continue;
                }

                // 次にショップを1つ配置（まだ置いていない場合）
                if (!shopPlaced && !(forbidShopOnFirstStep && isFirstFrontStep))
                {
                    node.stageType = StageType.Shop;
                    node.stageId = node.stageId + "_Shop";
                    shopPlaced = true;
                    continue;
                }

                // それ以外は Battle のまま
            }

            // もしショップがまだ置けていなければ、最後のBattleに置く
            if (!shopPlaced)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    if (nodes[i].stageType == StageType.Battle)
                    {
                        nodes[i].stageType = StageType.Shop;
                        nodes[i].stageId = nodes[i].stageId + "_Shop";
                        break;
                    }
                }
            }
        }

        private void Shuffle(List<int> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void BuildVisuals(List<RouteNode> nodes)
        {
            if (nodePrefab == null)
            {
                Debug.LogError("RouteMapGenerator: nodePrefab が設定されていません。");
                return;
            }

            var nodeObjects = new Dictionary<int, GameObject>();

            foreach (var node in nodes)
            {
                var nodeObj = Instantiate(nodePrefab, node.position, Quaternion.identity, transform);
                nodeObj.name = $"Node_{node.id}_{node.stageType}";

                var view = nodeObj.GetComponent<RouteNodeView>();
                if (view != null) view.Initialize(node, this);

                nodeObjects[node.id] = nodeObj;
            }

            if (linePrefab == null)
            {
                Debug.LogWarning("RouteMapGenerator: linePrefab が設定されていないため、接続線は描画されません。");
                return;
            }

            foreach (var node in nodes)
            {
                if (!nodeObjects.ContainsKey(node.id)) continue;
                Vector3 fromPos = nodeObjects[node.id].transform.position;

                foreach (int nextId in node.nextNodeIds)
                {
                    if (!nodeObjects.ContainsKey(nextId)) continue;
                    Vector3 toPos = nodeObjects[nextId].transform.position;

                    var lineObj = Instantiate(linePrefab, transform);
                    lineObj.name = $"Line_{node.id}_to_{nextId}";

                    var lr = lineObj.GetComponent<LineRenderer>();
                    if (lr != null)
                    {
                        lr.positionCount = 2;
                        lr.SetPosition(0, fromPos);
                        lr.SetPosition(1, toPos);
                    }
                }
            }
        }

        public RouteNode GetNodeById(int id)
        {
            if (generatedNodes == null) return null;
            if (id < 0 || id >= generatedNodes.Count) return null;
            return generatedNodes[id];
        }
    
        // ノードID -> RouteNode の辞書
        private Dictionary<int, RouteNode> nodeDict = new Dictionary<int, RouteNode>();
        public IReadOnlyDictionary<int, RouteNode> NodeDict => nodeDict;

        // 生成直後に辞書を再構築
        private void RebuildNodeDict(List<RouteNode> nodes)
        {
            nodeDict.Clear();
            foreach (var n in nodes)
            {
                nodeDict[n.id] = n;
            }
        }

        /// プレイヤーの現在地を更新（外部から呼び出し想定）
        public void SetCurrentNode(int nodeId)
        {
            if (nodeDict.ContainsKey(nodeId))
            {
                currentNodeId = nodeId;
            }
            else
            {
                Debug.LogWarning($"RouteMapGenerator: 無効なノードID {nodeId} が指定されました。");
            }
        }

        /// マップ状態を辞書形式で取得し、他コードに渡せるようにする
        public Dictionary<string, object> GetMapStatusDictionary()
        {
            // RouteNode は参照型なので、簡易的に浅いコピーを返す
            var nodeCopy = new Dictionary<int, RouteNode>(nodeDict);
            return new Dictionary<string, object>
            {
                { "currentNodeId", currentNodeId },
                { "nodes", nodeCopy }
            };
        }
    }
}
