using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

namespace RouteMap
{
    [RequireComponent(typeof(Collider2D))]
    public class RouteNodeView : MonoBehaviour, IPointerClickHandler
    {
        private RouteNode nodeData;
        private RouteMapGenerator owner;
        private SpriteRenderer spriteRenderer;
        private TextMesh idLabelInstance;

        [Header("色設定")]
        public Color normalColor = Color.white;
        public Color startColor = Color.green;
        public Color midBossColor = Color.yellow;
        public Color finalBossColor = Color.red;
        public Color selectedColor = Color.cyan;

        public Color treasureColor = new Color(0.8f, 0.6f, 1f); 
        public Color shopColor = new Color(0.6f, 1f, 1f);     

        [Header("デバッグ表示")]
        [Tooltip("ID表示用のTextMesh。未指定なら動的に生成します。")]
        public TextMesh idLabel;
        [Tooltip("IDラベルのローカルオフセット")]
        public Vector3 idLabelOffset = new Vector3(0f, 0.35f, 0f);

        private bool isSelected = false;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Initialize(RouteNode node, RouteMapGenerator mapGenerator)
        {
            nodeData = node;
            owner = mapGenerator;
            UpdateSprite();
            // UpdateLabel(); // ID表示はコメントアウト
        }

        private void UpdateSprite()
        {
            // SpriteはRouteMapGeneratorで既に設定されているので、ここでは色のみ更新
            if (spriteRenderer == null) return;

            if (isSelected)
            {
                spriteRenderer.color = selectedColor;
            }
            else
            {
                // 選択されていない場合は通常の色（白）
                spriteRenderer.color = Color.white;
            }
        }

        private void UpdateLabel()
        {
            if (nodeData == null) return;

            // 既存のラベルを使うか、無ければ生成
            if (idLabel == null)
            {
                var labelObj = new GameObject("IdLabel");
                labelObj.transform.SetParent(transform, false);
                labelObj.transform.localPosition = idLabelOffset;

                idLabelInstance = labelObj.AddComponent<TextMesh>();
                idLabelInstance.anchor = TextAnchor.MiddleCenter;
                idLabelInstance.characterSize = 0.1f;
                idLabelInstance.fontSize = 32;
                idLabelInstance.color = Color.black;
            }
            else
            {
                idLabelInstance = idLabel;
                idLabelInstance.transform.localPosition = idLabelOffset;
            }

            if (idLabelInstance != null)
            {
                idLabelInstance.text = $"ID:{nodeData.id}\n{nodeData.stageType}";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleClick();
        }

        // 2Dオブジェクト用のクリック検出（OnMouseDown）
        void OnMouseDown()
        {
            HandleClick();
            GetComponent<CircleCollider2D>().enabled = false;
            owner.Click += 1;
            
            for (int i = 0; i < owner.colliders.Count; i++)
            {
                if(owner.Click > (owner.midBossId + 1)/2)
                {
                    if(i > ((owner.Click-1) *2) -1 && i <= (owner.Click *2) -1)
                    {
                        owner.colliders[i].enabled = true;
                    }
                    else
                    {
                        owner.colliders[i].enabled = false;
                    }

                }
                else{
                    if(i > (owner.Click-1) *2 && i <= owner.Click *2)
                    {
                        owner.colliders[i].enabled = true;
                    }
                    else
                    {
                        owner.colliders[i].enabled = false;
                    }
                }
            }
        }

        private void HandleClick()
        {
            Select();
            // ここでGameManagerに通知するなど
            Debug.Log($"Node clicked: id={nodeData.id}, stageType={nodeData.stageType}, stageId={nodeData.stageId}");
            
            // NodeSummaryに情報を送信
            if (NodeSummary.Instance != null)
            {
                int layerNumber = NodeSummary.Instance.ExtractLayerNumber(nodeData.stageId);
                NodeSummary.Instance.AddNode(nodeData.id, nodeData.stageType, layerNumber);
                
                // 現在のノード情報を出力
                var currentNode = NodeSummary.Instance.GetCurrentNode();
                if (currentNode != null)
                {
                    Debug.Log($"現在のノード情報 - ID: {currentNode.id}, StageType: {currentNode.stageType}, LayerNumber: {currentNode.layerNumber}");
                }
                else
                {
                    Debug.Log("保存されているノード情報がありません");
                }
            }
        }

        public void Select()
        {
            isSelected = true;
            UpdateSprite();
            // TODO: 必要なら、他ノードの選択解除をRouteMapGeneratorに依頼する
        }

        public void Deselect()
        {
            isSelected = false;
            UpdateSprite();
        }
    }
}
