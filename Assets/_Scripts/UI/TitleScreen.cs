using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// タイトル画面のUI管理クラス
/// </summary>
public class TitleScreen : MonoBehaviour
{
    [Header("タイトル画面UI要素")]
    [SerializeField] private Image backgroundImage; // 背景画像
    [SerializeField] private Button startButton; // ゲーム開始ボタン
    [SerializeField] private Button optionButton; // オプションボタン

    [Header("画像")]
    // "Assets/Sprites/UI/ui_title_background.png" - タイトル画面背景画像
    [SerializeField] private Sprite titleBackgroundSprite;
    // "Assets/Sprites/UI/ui_button_normal.png" - ボタン画像（共通）
    [SerializeField] private Sprite buttonSprite;

    [Header("イベント")]
    // ゲーム開始ボタンがクリックされた時のイベント（外部のGameManagerなどが購読）
    [SerializeField] private UnityEvent onStartButtonClicked;
    // オプションボタンがクリックされた時のイベント（外部のGameManagerなどが購読）
    [SerializeField] private UnityEvent onOptionButtonClicked;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// タイトル画面の初期化
    /// </summary>
    public void Initialize()
    {
        // 背景画像の設定
        if (backgroundImage != null && titleBackgroundSprite != null)
        {
            backgroundImage.sprite = titleBackgroundSprite;
        }

        // ゲーム開始ボタンの設定
        if (startButton != null)
        {
            SetupButton(startButton, buttonSprite, "スタート", OnStartButtonClicked);
        }

        // オプションボタンの設定
        if (optionButton != null)
        {
            SetupButton(optionButton, buttonSprite, "オプション", OnOptionButtonClicked);
        }
    }

    /// <summary>
    /// タイトル画面の表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// タイトル画面の非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ボタンの設定（画像、テキスト、クリック処理、ホバー時の縁取り）
    /// </summary>
    private void SetupButton(Button button, Sprite buttonSprite, string buttonText, UnityEngine.Events.UnityAction onClickAction)
    {
        if (button == null) return;

        // ボタンの画像を設定
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null && buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
        }

        // ボタンのテキストを設定
        Text buttonTextComponent = button.GetComponentInChildren<Text>();
        if (buttonTextComponent != null)
        {
            buttonTextComponent.text = buttonText;
        }

        // クリック処理を追加
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClickAction);

        // ホバー時の白い縁取りを設定
        SetupButtonHoverEffect(button);
    }

    /// <summary>
    /// ボタンのホバー時の白い縁取り効果を設定
    /// </summary>
    private void SetupButtonHoverEffect(Button button)
    {
        // Outlineコンポーネントを追加（まだない場合）
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
        {
            outline = button.gameObject.AddComponent<Outline>();
        }

        // 白い縁取りの設定
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3, 3);
        outline.useGraphicAlpha = true;

        // 初期状態では非表示
        outline.enabled = false;

        // ホバー時のイベントを設定
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // PointerEnter: マウスが入った時に縁取りを表示
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { outline.enabled = true; });
        trigger.triggers.Add(entryEnter);

        // PointerExit: マウスが出た時に縁取りを非表示
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { outline.enabled = false; });
        trigger.triggers.Add(entryExit);
    }

    /// <summary>
    /// ゲーム開始ボタンがクリックされた時の処理
    /// 外部のGameManagerなどが購読するイベントを発火するのみ
    /// </summary>
    private void OnStartButtonClicked()
    {
        Debug.Log("ゲーム開始ボタンがクリックされました");
        // 外部（GameManagerなど）にイベントを通知
        onStartButtonClicked?.Invoke();
    }

    /// <summary>
    /// オプションボタンがクリックされた時の処理
    /// 外部のGameManagerなどが購読するイベントを発火するのみ
    /// </summary>
    private void OnOptionButtonClicked()
    {
        Debug.Log("オプションボタンがクリックされました");
        // 外部（GameManagerなど）にイベントを通知
        onOptionButtonClicked?.Invoke();
    }
}

