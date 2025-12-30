using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 敵1体分の表示を管理するコンポーネント
/// </summary>
public class EnemySlot : MonoBehaviour
{
    [Header("表示要素")]
    [SerializeField] private Image enemyImage;          // 敵の画像
    [SerializeField] private Slider hpBar;              // HPバー
    [SerializeField] private TextMeshProUGUI hpText;    // HP数値テキスト
    [SerializeField] private TextMeshProUGUI nameText;  // 敵の名前

    [Header("攻撃アニメーション設定")]
    [SerializeField] private float attackMoveDistance = 200f;  // 突進距離（ピクセル）
    [SerializeField] private float attackDuration = 0.3f;     // 突進時間（秒）
    [SerializeField] private float returnDuration = 0.2f;     // 戻る時間（秒）

    [Header("敵データファイル")]
    [Tooltip("敵データファイル（敵タイプ判定用）。nullの場合は自動検索")]
    [SerializeField] private enemy_L enemyDataFile;  // 敵データファイル

    // 敵データ
    private EnemyData enemyData;
    private int currentHP;
    private int maxHP;

    // 位置情報
    private Vector3 initialPosition;  // 初期位置（攻撃前の位置）
    private RectTransform rectTransform;
    private UnityEngine.UI.LayoutElement layoutElement; // LayoutGroupの影響を制御するため
    private Vector2 originalSize; // 元のサイズ（ボススケール用）
    
    // 最終ボス用の画像管理
    private Sprite originalSprite;     // 元の画像（攻撃後に戻すため）
    private Vector2 originalImageSize; // 元の画像サイズ
    private bool isFinalBoss = false;  // 最終ボスかどうか

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutElement = GetComponent<UnityEngine.UI.LayoutElement>();
        
        // LayoutElementがない場合は追加（LayoutGroupの影響を制御するため）
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        }
    }

    /// <summary>
    /// 敵スロットの初期設定
    /// </summary>
    /// <param name="data">敵データ</param>
    /// <param name="hpColor">HPバーの色</param>
    /// <param name="bgColor">HPバー背景の色</param>
    /// <param name="placeholderColor">プレースホルダーの色</param>
    public void Setup(EnemyData data, Color hpColor, Color bgColor, Color placeholderColor)
    {
        enemyData = data;
        currentHP = data.currentHP;
        maxHP = data.maxHP;

        Debug.Log($"EnemySlot.Setup() 開始 - 敵名: {data.enemyName}, HP: {currentHP}/{maxHP}");

        // 初期位置とサイズを保存
        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition;
            originalSize = rectTransform.sizeDelta; // 元のサイズを保存
        }
        else
        {
            Debug.LogWarning("EnemySlot: rectTransformがnullです");
        }

        // 敵画像の設定
        if (enemyImage != null)
        {
            if (data.enemySprite != null)
            {
                enemyImage.sprite = data.enemySprite;
                enemyImage.color = Color.white;
                
                // 元の画像とサイズを保存
                originalSprite = data.enemySprite;
                RectTransform imageRect = enemyImage.GetComponent<RectTransform>();
                if (imageRect != null)
                {
                    originalImageSize = imageRect.sizeDelta;
                }
                
                Debug.Log($"EnemySlot: 敵画像を設定 - {data.enemySprite.name}");
            }
            else
            {
                // プレースホルダー（色付き四角）
                enemyImage.sprite = null;
                enemyImage.color = placeholderColor;
                Debug.LogWarning($"EnemySlot: 敵画像がnullのため、プレースホルダーを使用");
            }
        }
        else
        {
            Debug.LogError("EnemySlot: enemyImageがnullです！Inspectorで設定してください");
        }
        
        // 最終ボスかどうかを判定
        CheckIfFinalBoss(data);

        // 名前の設定
        if (nameText != null)
        {
            nameText.text = data.enemyName;
            
            // アウトラインを設定（視認性向上のため）
            // TextMeshProUGUIのアウトラインは、フォントアセットのマテリアルに依存するため、
            // マテリアルが存在する場合のみ設定を試みる
            try
            {
                // フォントアセットが設定されているかチェック
                if (nameText.font != null)
                {
                    // アウトラインを設定
                    nameText.outlineWidth = 0.2f;  // アウトラインの太さ
                    nameText.outlineColor = Color.black;  // アウトラインの色（黒）
                    Debug.Log($"EnemySlot: 敵名を設定 - {data.enemyName} (アウトライン付き)");
                }
                else
                {
                    Debug.LogWarning($"EnemySlot: フォントアセットが設定されていません。アウトラインをスキップします - {data.enemyName}");
                }
            }
            catch (System.Exception e)
            {
                // アウトラインの設定に失敗した場合は警告を出して続行
                Debug.LogWarning($"EnemySlot: アウトラインの設定に失敗しました（マテリアルが設定されていない可能性があります） - {e.Message}");
            }
        }
        else
        {
            Debug.LogError("EnemySlot: nameTextがnullです！Inspectorで設定してください");
        }

        // HPバーの設定
        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
            SetSliderColors(hpBar, hpColor, bgColor);
            Debug.Log($"EnemySlot: HPバーを設定 - Max: {maxHP}, Current: {currentHP}");
        }
        else
        {
            Debug.LogError("EnemySlot: hpBarがnullです！Inspectorで設定してください");
        }

        // HPテキストの設定
        UpdateHPText();

        Debug.Log($"EnemySlot: {data.enemyName} を設定完了 (HP: {currentHP}/{maxHP})");
    }

    /// <summary>
    /// HPを更新
    /// </summary>
    public void UpdateHP(int newCurrentHP, int newMaxHP)
    {
        currentHP = newCurrentHP;
        maxHP = newMaxHP;

        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }

        UpdateHPText();
    }

    /// <summary>
    /// 撃破された時の処理
    /// </summary>
    public void OnDefeated()
    {
        // HPを0に
        UpdateHP(0, maxHP);

        // 敵を暗くする（簡易的な撃破表現）
        if (enemyImage != null)
        {
            enemyImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }

        Debug.Log($"EnemySlot: {enemyData.enemyName} が撃破されました");
    }

    /// <summary>
    /// HPテキストを更新
    /// </summary>
    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHP}/{maxHP}";
            Debug.Log($"EnemySlot: HPテキストを更新 - {currentHP}/{maxHP}");
        }
        else
        {
            Debug.LogError("EnemySlot: hpTextがnullです！Inspectorで設定してください");
        }
    }

    /// <summary>
    /// スライダーの色を設定
    /// </summary>
    private void SetSliderColors(Slider slider, Color fillColor, Color bgColor)
    {
        // Fill部分の色を設定
        Transform fill = slider.fillRect;
        if (fill != null)
        {
            Image fillImage = fill.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = fillColor;
            }
        }

        // Background部分の色を設定
        Transform background = slider.transform.Find("Background");
        if (background != null)
        {
            Image bgImage = background.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = bgColor;
            }
        }
    }

    /// <summary>
    /// 敵データを取得
    /// </summary>
    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    /// <summary>
    /// 現在HPを取得
    /// </summary>
    public int GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// 攻撃アニメーション（突進）を実行
    /// ロジック側から呼び出される
    /// </summary>
    /// <param name="targetPosition">攻撃目標位置（プレイヤーの位置など）</param>
    public void PlayAttackAnimation(Vector3 targetPosition)
    {
        if (rectTransform == null)
        {
            Debug.LogWarning("EnemySlot: RectTransformが見つかりません");
            return;
        }

        StartCoroutine(AttackAnimationCoroutine(targetPosition));
    }

    /// <summary>
    /// 最終ボスかどうかを判定
    /// </summary>
    private void CheckIfFinalBoss(EnemyData data)
    {
        if (int.TryParse(data.enemyId, out int enemyId))
        {
            enemy_L dataFile = enemyDataFile;
            if (dataFile == null)
            {
                dataFile = FindObjectOfType<enemy_L>();
            }

            if (dataFile != null)
            {
                EnemyType enemyType = dataFile.GetEnemyType(enemyId);
                isFinalBoss = (enemyType == EnemyType.FinalBoss);
                Debug.Log($"EnemySlot: 最終ボス判定 - {data.enemyName}: {isFinalBoss}");
            }
        }
    }

    /// <summary>
    /// 最終ボスの攻撃画像（_Punch）をロードして差し替える
    /// </summary>
    private void LoadPunchImage()
    {
        if (!isFinalBoss || enemyImage == null || originalSprite == null) return;

        // 元の画像名から_Punch画像名を生成
        string originalImageName = originalSprite.name;
        string punchImageName = originalImageName + "_Punch";
        
        // Resourcesから_Punch画像をロード
        Sprite punchSprite = Resources.Load<Sprite>($"enemies/{punchImageName}");
        
        if (punchSprite != null)
        {
            // 画像を差し替え
            enemyImage.sprite = punchSprite;
            
            // 画像サイズを621×600に調整
            RectTransform imageRect = enemyImage.GetComponent<RectTransform>();
            if (imageRect != null)
            {
                imageRect.sizeDelta = new Vector2(621f, 600f);
                Debug.Log($"EnemySlot: 最終ボスの攻撃画像に差し替え - {punchImageName} (サイズ: 621×600)");
            }
        }
        else
        {
            Debug.LogWarning($"EnemySlot: 最終ボスの攻撃画像が見つかりません - Resources/enemies/{punchImageName}");
        }
    }

    /// <summary>
    /// 元の画像に戻す
    /// </summary>
    private void RestoreOriginalImage()
    {
        if (!isFinalBoss || enemyImage == null || originalSprite == null) return;

        // 元の画像に戻す
        enemyImage.sprite = originalSprite;
        
        // 元のサイズに戻す
        RectTransform imageRect = enemyImage.GetComponent<RectTransform>();
        if (imageRect != null)
        {
            imageRect.sizeDelta = originalImageSize;
            Debug.Log($"EnemySlot: 最終ボスの画像を元に戻しました (サイズ: {originalImageSize})");
        }
    }

    /// <summary>
    /// 攻撃アニメーションのコルーチン
    /// 突進 → 戻る の流れ
    /// 最終ボスの場合は攻撃時に_Punch画像に差し替える
    /// </summary>
    private IEnumerator AttackAnimationCoroutine(Vector3 targetPosition)
    {
        if (rectTransform == null) yield break;

        // 最終ボスの場合は攻撃画像に差し替え
        if (isFinalBoss)
        {
            LoadPunchImage();
        }

        // LayoutGroupの影響を無効化（アニメーション中は位置を手動制御するため）
        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = true;
        }

        // アニメーション開始時の位置を保存（元の位置として使用）
        Vector3 startPos = rectTransform.anchoredPosition;
        initialPosition = startPos; // 元の位置を更新
        
        Debug.Log($"EnemySlot: アニメーション開始 - 開始位置: {startPos}, 元の位置: {initialPosition}");
        
        // プレイヤー方向への移動ベクトルを計算
        // 座標系が異なる可能性があるため、水平方向（X方向）のみに移動する
        Vector3 direction = (targetPosition - startPos);
        direction.y = 0f; // Y方向は固定（水平移動のみ）
        direction = direction.normalized;
        Vector3 attackEndPos = startPos + direction * attackMoveDistance;
        
        Debug.Log($"EnemySlot: 攻撃目標位置: {targetPosition}, 移動方向: {direction}, 攻撃終了位置: {attackEndPos}");

        // 突進（プレイヤー方向へ）
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / attackDuration;
            // EaseOut（最初速く、最後遅く）
            t = 1f - Mathf.Pow(1f - t, 3f);
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, attackEndPos, t);
            yield return null;
        }
        rectTransform.anchoredPosition = attackEndPos;

        // 少し待機（攻撃のインパクト）
        yield return new WaitForSeconds(0.1f);

        // 元の位置に戻る
        elapsed = 0f;
        Debug.Log($"EnemySlot: 元の位置に戻る開始 - 現在位置: {rectTransform.anchoredPosition}, 目標位置: {initialPosition}");
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            // EaseIn（最初遅く、最後速く）
            t = t * t;
            rectTransform.anchoredPosition = Vector3.Lerp(attackEndPos, initialPosition, t);
            yield return null;
        }
        rectTransform.anchoredPosition = initialPosition;
        Debug.Log($"EnemySlot: 元の位置に戻りました - 最終位置: {rectTransform.anchoredPosition}");

        // LayoutGroupの影響を再有効化
        if (layoutElement != null)
        {
            layoutElement.ignoreLayout = false;
            // LayoutGroupに位置の再計算を強制
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform.parent as RectTransform);
        }

        // 最終ボスの場合は元の画像に戻す
        if (isFinalBoss)
        {
            RestoreOriginalImage();
        }

        Debug.Log($"EnemySlot: {enemyData.enemyName} の攻撃アニメーション完了");
    }

    /// <summary>
    /// アニメーションを停止して初期位置に戻す
    /// </summary>
    public void ResetPosition()
    {
        if (rectTransform != null)
        {
            StopAllCoroutines();
            rectTransform.anchoredPosition = initialPosition;
            
            // LayoutGroupの影響を再有効化
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = false;
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform.parent as RectTransform);
            }
        }
    }
}

