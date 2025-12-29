using System.Collections.Generic;
using UnityEngine;

namespace RouteMap
{
    public class RouteMapGenerator : MonoBehaviour
    {
        [Header("基本設定")]
        [Tooltip("最終的に必要な数のステージ数に合わせる（スタート・中ボス・最終ボス含む）")]
        public int targetStageCount = 10;

        [Tooltip("スタートノードのY座標")]
        public float startY = 0f;

        [Tooltip("ノード間のY方向の距離（縦方向のステップ）")]
        public float yStep = 3f;

        [Tooltip("左右レーンのX方向オフセット（左レーン=+x, 右レーン=-x）")]
        public float laneXOffset = 1.5f;

        [Header("トレジャー設定")]
        [Tooltip("宝ノードの最大数（0以上）")]
        public int maxTreasureCount = 2;

        [Tooltip("ショップは前半1ステップ目に出てこない")]
        public bool forbidShopOnFirstStep = true;

        public GameObject nodePrefab;
        public GameObject linePrefab;

        [Header("ノード画像設定")]
        [Tooltip("スタートノード用のSprite")]
        public Sprite startSprite;

        [Tooltip("中ボス用のSprite")]
        public Sprite midBossSprite;

        [Tooltip("最終ボス用のSprite")]
        public Sprite finalBossSprite;

        [Tooltip("前半分岐用のSprite（5つの中からランダムに選択）")]
        public Sprite[] frontSprites = new Sprite[5];

        [Tooltip("後半分岐用のSprite（5つの中からランダムに選択）")]
        public Sprite[] backSprites = new Sprite[5];

        [SerializeField] private List<RouteNode> generatedNodes = new List<RouteNode>();
        public IReadOnlyList<RouteNode> GeneratedNodes => generatedNodes;
        [SerializeField] private int currentNodeId = 0; // プレイヤーが現在いるノードID
        public int CurrentNodeId => currentNodeId;

        private System.Random random;
        
        // ルートごとのSpriteインデックスを保存（キー: "F_L", "F_R", "B_L", "B_R"）
        private Dictionary<string, int> routeSpriteIndices = new Dictionary<string, int>();
        
        // 前半で使用されたSpriteオブジェクトを記録（後半で除外するため）
        private HashSet<Sprite> usedFrontSprites = new HashSet<Sprite>();

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
            float currentY = startY;

            // 0. Start
            var startNode = new RouteNode
            {
                id = currentId,
                position = new Vector2(0f, currentY),
                stageType = StageType.Start,
                stageId = "Start"
            };
            nodes.Add(startNode);

            // 目標設定: targetStageCount
            // Start(1)使用。中ボス・最終ボスを含めて合計10になるようにする。
            // 前半分岐→中ボス→後半分岐で合流させる。
            // 構成: Start(1) + 前半k + MidBoss(1) + 後半m + FinalBoss(1) = targetStageCount
            // つまり k + m = targetStageCount - 3
            int slotsForBranches = Mathf.Max(targetStageCount - 3, 2); // 最低でも2
            
            // 中ボスの位置を完全にランダムに決定（1〜(slotsForBranches-1)の範囲）
            // 最低1ステップ、最大slotsForBranches-1ステップ（後半に最低1ステップ残すため）
            int frontSteps = random.Next(1, slotsForBranches);
            int backSteps = slotsForBranches - frontSteps;

            // 前半は2レーンでfrontSteps回分岐 => 2*frontStepsノードが生成される
            // 後半も同様
            // Start(1) + 2*frontSteps + Mid(1) + 2*backSteps + Final(1) == targetStageCount になるように調整
            int totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
            int diff = targetStageCount - totalIf;
            
            // デバッグログ
            Debug.Log($"中ボス位置決定: slotsForBranches={slotsForBranches}, frontSteps={frontSteps}, backSteps={backSteps}, totalIf={totalIf}, diff={diff}");
            
            // diffが正なら後半に追加、負なら後半を減らす（最低1ステップは残す）
            // ただし、frontStepsのランダム性を保つため、backStepsのみ調整
            if (diff != 0)
            {
                // diffを2で割って、backStepsに加算（2レーンなので、1ステップ増やすと2ノード増える）
                int backStepsAdjustment = diff / 2;
                backSteps = Mathf.Max(1, backSteps + backStepsAdjustment);
                
                // 再計算
                totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
                diff = targetStageCount - totalIf;
                
                // まだ足りない場合はbackStepsのみ増やす（frontStepsは変更しない）
                while (totalIf < targetStageCount && backSteps < slotsForBranches * 2)
                {
                    backSteps++;
                    totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
                }
                // 多すぎる場合はbackStepsのみ減らす（frontStepsは変更しない、最低1は残す）
                while (totalIf > targetStageCount && backSteps > 1)
                {
                    backSteps--;
                    totalIf = 1 + 2 * frontSteps + 1 + 2 * backSteps + 1;
                }
            }
            
            Debug.Log($"調整後: frontSteps={frontSteps}, backSteps={backSteps}, totalIf={totalIf}");

            float leftX = laneXOffset;
            float rightX = -laneXOffset;

            // ルートごとのSpriteインデックスを事前に決定
            routeSpriteIndices.Clear();
            usedFrontSprites.Clear();
            
            if (frontSprites != null && frontSprites.Length > 0)
            {
                // 前半左レーン：偶数インデックス（0, 2, 4）から選ぶ
                int frontLeftIndex = random.Next(0, (frontSprites.Length + 1) / 2) * 2;
                frontLeftIndex = Mathf.Clamp(frontLeftIndex, 0, frontSprites.Length - 1);
                routeSpriteIndices["F_L"] = frontLeftIndex;
                if (frontSprites[frontLeftIndex] != null)
                {
                    usedFrontSprites.Add(frontSprites[frontLeftIndex]);
                }
                
                // 前半右レーン：奇数インデックス（1, 3）から選ぶ
                int frontRightIndex = random.Next(0, frontSprites.Length / 2) * 2 + 1;
                frontRightIndex = Mathf.Clamp(frontRightIndex, 0, frontSprites.Length - 1);
                routeSpriteIndices["F_R"] = frontRightIndex;
                if (frontSprites[frontRightIndex] != null)
                {
                    usedFrontSprites.Add(frontSprites[frontRightIndex]);
                }
            }
            
            if (backSprites != null && backSprites.Length > 0)
            {
                // 後半左レーン：偶数インデックス（0, 2, 4）から選ぶ
                // 前半で使用されたSpriteと同じでないようにする
                List<int> availableLeftIndices = new List<int>();
                for (int i = 0; i < backSprites.Length; i += 2) // 偶数インデックスのみ
                {
                    // 前半で使用されていないSpriteの場合のみ追加
                    if (backSprites[i] != null && !usedFrontSprites.Contains(backSprites[i]))
                    {
                        availableLeftIndices.Add(i);
                    }
                }
                
                int backLeftIndex;
                if (availableLeftIndices.Count > 0)
                {
                    backLeftIndex = availableLeftIndices[random.Next(0, availableLeftIndices.Count)];
                }
                else
                {
                    // フォールバック：使用可能な偶数インデックスから選ぶ（前半で使用されていても選ぶ）
                    backLeftIndex = random.Next(0, (backSprites.Length + 1) / 2) * 2;
                }
                backLeftIndex = Mathf.Clamp(backLeftIndex, 0, backSprites.Length - 1);
                routeSpriteIndices["B_L"] = backLeftIndex;
                
                // 後半右レーン：奇数インデックス（1, 3）から選ぶ
                // 前半で使用されたSpriteと同じでないようにする
                List<int> availableRightIndices = new List<int>();
                for (int i = 1; i < backSprites.Length; i += 2) // 奇数インデックスのみ
                {
                    // 前半で使用されていないSpriteの場合のみ追加
                    if (backSprites[i] != null && !usedFrontSprites.Contains(backSprites[i]))
                    {
                        availableRightIndices.Add(i);
                    }
                }
                
                int backRightIndex;
                if (availableRightIndices.Count > 0)
                {
                    backRightIndex = availableRightIndices[random.Next(0, availableRightIndices.Count)];
                }
                else
                {
                    // フォールバック：使用可能な奇数インデックスから選ぶ（前半で使用されていても選ぶ）
                    backRightIndex = random.Next(0, backSprites.Length / 2) * 2 + 1;
                }
                backRightIndex = Mathf.Clamp(backRightIndex, 0, backSprites.Length - 1);
                routeSpriteIndices["B_R"] = backRightIndex;
            }

            int leftCurrentId = currentId;
            int rightCurrentId = currentId;

            // 1. 前半分岐（通常分岐）
            for (int i = 0; i < frontSteps; i++)
            {
                currentY += yStep;

                int leftId = nodes.Count;
                var leftNode = CreateBranchNode(leftId, leftX, currentY, StageType.Battle, $"F_L_{i}");
                nodes.Add(leftNode);
                nodes[leftCurrentId].nextNodeIds.Add(leftId);
                leftCurrentId = leftId;

                int rightId = nodes.Count;
                var rightNode = CreateBranchNode(rightId, rightX, currentY, StageType.Battle, $"F_R_{i}");
                nodes.Add(rightNode);
                nodes[rightCurrentId].nextNodeIds.Add(rightId);
                rightCurrentId = rightId;
            }

            // 2. 中ボス（合流）
            currentY += yStep;
            int midBossId = nodes.Count;
            var midBoss = new RouteNode
            {
                id = midBossId,
                position = new Vector2(0f, currentY),
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
                currentY += yStep;

                int leftId = nodes.Count;
                var leftNode = CreateBranchNode(leftId, leftX, currentY, StageType.Battle, $"B_L_{i}");
                nodes.Add(leftNode);
                nodes[leftCurrentId].nextNodeIds.Add(leftId);
                leftCurrentId = leftId;

                int rightId = nodes.Count;
                var rightNode = CreateBranchNode(rightId, rightX, currentY, StageType.Battle, $"B_R_{i}");
                nodes.Add(rightNode);
                nodes[rightCurrentId].nextNodeIds.Add(rightId);
                rightCurrentId = rightId;
            }

            // 4. 最終ボス（合流）
            currentY += yStep;
            int bossId = nodes.Count;
            var bossNode = new RouteNode
            {
                id = bossId,
                position = new Vector2(0f, currentY),
                stageType = StageType.FinalBoss,
                stageId = "FinalBoss"
            };
            nodes.Add(bossNode);
            nodes[leftCurrentId].nextNodeIds.Add(bossId);
            nodes[rightCurrentId].nextNodeIds.Add(bossId);

            // --- 5. 宝とショップの配置（宝とショップ） ---
            AssignSpecialStages(nodes, frontSteps, backSteps);

            return nodes;
        }

        private RouteNode CreateBranchNode(int id, float x, float y, StageType type, string stageId)
        {
            return new RouteNode
            {
                id = id,
                position = new Vector2(x, y), // x: 左右オフセット, y: 縦方向の位置
                stageType = type,
                stageId = stageId
            };
        }

        /// 宝とショップの配置を決定
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

                // ショップは前半1ステップ目に置かない条件
                // 前半1ステップ目のノードIDは Start が0として、1/2が前半1ステップ目（左右）
                bool isFirstFrontStep =
                    (node.stageId.StartsWith("F_L_0") || node.stageId.StartsWith("F_R_0"));

                // まず宝を配置
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

            // まだショップがまだ置いていない場合は、最後のBattleに置く
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

                // Rigidbody2Dが自動追加されている場合は、Body TypeをKinematicに変更
                var rb2d = nodeObj.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.bodyType = RigidbodyType2D.Kinematic;
                }

                // ノードのSpriteを設定
                Sprite spriteToUse = GetSpriteForNode(node);
                if (spriteToUse != null)
                {
                    // SpriteRendererを取得（子オブジェクトも含めて検索）
                    var sr = nodeObj.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sprite = spriteToUse;
                        Debug.Log($"Node {node.id}: Sprite設定成功 - {spriteToUse.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"Node {node.id}: SpriteRendererが見つかりません");
                    }
                }
                else
                {
                    Debug.LogWarning($"Node {node.id}: Spriteがnullです (stageId: {node.stageId}, stageType: {node.stageType})");
                }

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

        // 生成されたノードから辞書を再構築
        private void RebuildNodeDict(List<RouteNode> nodes)
        {
            nodeDict.Clear();
            foreach (var n in nodes)
            {
                nodeDict[n.id] = n;
            }
        }

        /// プレイヤーの現在地を更新（前のノードから進む経路）
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

        /// マップ状態を辞書形式で取得し、セーブデータに渡すようにする
        public Dictionary<string, object> GetMapStatusDictionary()
        {
            // RouteNode は参照型なので、簡易的にコピーを返す
            var nodeCopy = new Dictionary<int, RouteNode>(nodeDict);
            return new Dictionary<string, object>
            {
                { "currentNodeId", currentNodeId },
                { "nodes", nodeCopy }
            };
        }

        /// ノードに応じたSpriteを取得
        private Sprite GetSpriteForNode(RouteNode node)
        {
            // スタートノード
            if (node.stageType == StageType.Start)
            {
                return startSprite;
            }

            // 中ボス
            if (node.stageType == StageType.MidBoss)
            {
                return midBossSprite;
            }

            // 最終ボス
            if (node.stageType == StageType.FinalBoss)
            {
                return finalBossSprite;
            }

            // 前半分岐（F_L_* または F_R_*）
            if (node.stageId != null && node.stageId.StartsWith("F_"))
            {
                if (frontSprites == null || frontSprites.Length == 0) return null;
                
                // 事前に決定したルートのSpriteインデックスを使用
                bool isLeft = node.stageId.StartsWith("F_L_");
                string routeKey = isLeft ? "F_L" : "F_R";
                
                if (routeSpriteIndices.ContainsKey(routeKey))
                {
                    int spriteIndex = routeSpriteIndices[routeKey];
                    return frontSprites[spriteIndex];
                }
                
                // フォールバック（辞書にない場合）
                int fallbackIndex = isLeft ? 0 : 1;
                return frontSprites[Mathf.Clamp(fallbackIndex, 0, frontSprites.Length - 1)];
            }

            // 後半分岐（B_L_* または B_R_*）または宝・ショップ
            if (node.stageId != null && (node.stageId.StartsWith("B_") || 
                node.stageType == StageType.Treasure || node.stageType == StageType.Shop))
            {
                if (backSprites == null || backSprites.Length == 0) return null;
                
                // 宝・ショップの場合は元のstageIdから判定
                string baseStageId = node.stageId;
                if (baseStageId.Contains("_Treasure"))
                {
                    baseStageId = baseStageId.Replace("_Treasure", "");
                }
                if (baseStageId.Contains("_Shop"))
                {
                    baseStageId = baseStageId.Replace("_Shop", "");
                }
                
                bool isLeft = baseStageId.StartsWith("B_L_");
                string routeKey = isLeft ? "B_L" : "B_R";
                
                // 事前に決定したルートのSpriteインデックスを使用
                if (routeSpriteIndices.ContainsKey(routeKey))
                {
                    int spriteIndex = routeSpriteIndices[routeKey];
                    return backSprites[spriteIndex];
                }
                
                // フォールバック（辞書にない場合）
                int fallbackIndex = isLeft ? 0 : 1;
                return backSprites[Mathf.Clamp(fallbackIndex, 0, backSprites.Length - 1)];
            }
            
            // 前半分岐の宝・ショップも対応
            if (node.stageId != null && (node.stageType == StageType.Treasure || node.stageType == StageType.Shop))
            {
                string baseStageId = node.stageId;
                if (baseStageId.Contains("_Treasure"))
                {
                    baseStageId = baseStageId.Replace("_Treasure", "");
                }
                if (baseStageId.Contains("_Shop"))
                {
                    baseStageId = baseStageId.Replace("_Shop", "");
                }
                
                // 前半分岐の宝・ショップ
                if (baseStageId.StartsWith("F_"))
                {
                    if (frontSprites == null || frontSprites.Length == 0) return null;
                    
                    bool isLeft = baseStageId.StartsWith("F_L_");
                    string routeKey = isLeft ? "F_L" : "F_R";
                    
                    // 事前に決定したルートのSpriteインデックスを使用
                    if (routeSpriteIndices.ContainsKey(routeKey))
                    {
                        int spriteIndex = routeSpriteIndices[routeKey];
                        return frontSprites[spriteIndex];
                    }
                    
                    // フォールバック（辞書にない場合）
                    int fallbackIndex = isLeft ? 0 : 1;
                    return frontSprites[Mathf.Clamp(fallbackIndex, 0, frontSprites.Length - 1)];
                }
            }

            return null;
        }
    }
}
