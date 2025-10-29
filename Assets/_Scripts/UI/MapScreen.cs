using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップ画面のUI管理クラス
/// 縦長のマップをスクロールで表示する
/// </summary>
public class MapScreen : MonoBehaviour
{
    [Header("マップ画面UI要素")]
    [SerializeField] private ScrollRect scrollRect; // スクロールビュー
    [SerializeField] private Image mapBackgroundImage; // マップ背景画像

    [Header("画像")]
    // "Assets/Sprites/UI/ui_map_background.png" - マップ背景画像（1920*6000などの縦長画像）
    [SerializeField] private Sprite mapBackgroundSprite;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// マップ画面の初期化
    /// </summary>
    public void Initialize()
    {
        // マップ背景画像の設定
        if (mapBackgroundImage != null && mapBackgroundSprite != null)
        {
            mapBackgroundImage.sprite = mapBackgroundSprite;
            
            // 画像のサイズに合わせてRectTransformを設定
            RectTransform mapRect = mapBackgroundImage.GetComponent<RectTransform>();
            if (mapRect != null && mapBackgroundSprite != null)
            {
                // 画像の元のサイズを取得して設定
                float imageWidth = mapBackgroundSprite.texture.width;
                float imageHeight = mapBackgroundSprite.texture.height;
                mapRect.sizeDelta = new Vector2(imageWidth, imageHeight);
            }
        }

        // スクロールビューの設定
        SetupScrollView();
    }

    /// <summary>
    /// スクロールビューの設定
    /// </summary>
    private void SetupScrollView()
    {
        if (scrollRect == null) return;

        // 縦方向のスクロールを有効にする
        scrollRect.vertical = true;
        scrollRect.horizontal = false;

        // スクロールバーは必要に応じて設定（今回はスクロールバーなしでも可）
        
        // 初期位置を最下部に設定（マップの上から見えるように）
        // 縦長のマップの場合、最初は最下部（ゴール部分）を表示
        scrollRect.verticalNormalizedPosition = 0f; // 0 = 最下部, 1 = 最上部
    }

    /// <summary>
    /// マップ画面の表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        
        // 表示時にスクロール位置をリセット
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f; // 最下部から表示
        }
    }

    /// <summary>
    /// マップ画面の非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

