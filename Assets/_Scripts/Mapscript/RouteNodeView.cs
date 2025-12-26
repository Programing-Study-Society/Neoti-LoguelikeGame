using UnityEngine;
using UnityEngine.EventSystems;

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

        public Color treasureColor = new Color(0.8f, 0.6f, 1f); // ラベンダー系
        public Color shopColor = new Color(0.6f, 1f, 1f);       // シアン系

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
            UpdateColor();
            UpdateLabel();
        }

        private void UpdateColor()
        {
            if (spriteRenderer == null) return;

            if (isSelected)
            {
                spriteRenderer.color = selectedColor;
                return;
            }

            switch (nodeData.stageType)
            {
                case StageType.Start:
                    spriteRenderer.color = startColor;
                    break;
                case StageType.MidBoss:
                    spriteRenderer.color = midBossColor;
                    break;
                case StageType.FinalBoss:
                    spriteRenderer.color = finalBossColor;
                    break;
                case StageType.Treasure:
                    spriteRenderer.color = treasureColor;
                    break;
                case StageType.Shop:
                    spriteRenderer.color = shopColor;
                    break;
                default:
                    spriteRenderer.color = normalColor;
                    break;
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
                idLabelInstance.text = $"ID:{nodeData.id}";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
            // ここでGameManagerに通知するなど
            Debug.Log($"Node clicked: id={nodeData.id}, stageType={nodeData.stageType}, stageId={nodeData.stageId}");
        }

        public void Select()
        {
            isSelected = true;
            UpdateColor();
            // TODO: 必要なら、他ノードの選択解除をRouteMapGeneratorに依頼する
        }

        public void Deselect()
        {
            isSelected = false;
            UpdateColor();
        }
    }
}