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

    [Header("ボス表示設定")]
    [SerializeField] private float bossScale = 1.5f;  // ボスのスケール倍率
    [Tooltip("敵データファイル（敵タイプ判定用）。nullの場合は自動検索")]
    [SerializeField] private enemy_L enemyDataFile;  // 敵データファイル

    // 敵データ
    private EnemyData enemyData;
    private int currentHP;
    private int maxHP;

    // 位置情報
    private Vector3 initialPosition;  // 初期位置（攻撃前の位置）
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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

        // 初期位置を保存
        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition;
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

        // 名前の設定
        if (nameText != null)
        {
            nameText.text = data.enemyName;
            Debug.Log($"EnemySlot: 敵名を設定 - {data.enemyName}");
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

        // ボスの場合はスケールを大きくする
        ApplyBossScale(data);

        Debug.Log($"EnemySlot: {data.enemyName} を設定完了 (HP: {currentHP}/{maxHP})");
    }

    /// <summary>
    /// ボスの場合はスケールを大きくする
    /// </summary>
    private void ApplyBossScale(EnemyData data)
    {
        // 敵IDから敵タイプを取得
        if (int.TryParse(data.enemyId, out int enemyId))
        {
            // 敵データファイルを取得（参照がなければ自動検索）
            enemy_L dataFile = enemyDataFile;
            if (dataFile == null)
            {
                dataFile = FindObjectOfType<enemy_L>();
            }

            if (dataFile != null)
            {
                EnemyType enemyType = dataFile.GetEnemyType(enemyId);
                
                if (enemyType == EnemyType.Boss || enemyType == EnemyType.FinalBoss)
                {
                    // ボスの場合はスケールを大きくする
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.one * bossScale;
                        Debug.Log($"EnemySlot: ボス検出 - {data.enemyName} を {bossScale}倍に拡大");
                    }
                }
                else
                {
                    // 通常敵は通常サイズ
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.one;
                    }
                }
            }
            else
            {
                Debug.LogWarning("EnemySlot: 敵データファイルが見つかりません。ボス判定をスキップします");
            }
        }
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
    /// 攻撃アニメーションのコルーチン
    /// 突進 → 戻る の流れ
    /// </summary>
    private IEnumerator AttackAnimationCoroutine(Vector3 targetPosition)
    {
        if (rectTransform == null) yield break;

        Vector3 startPos = rectTransform.anchoredPosition;
        
        // プレイヤー方向への移動ベクトルを計算
        Vector3 direction = (targetPosition - startPos).normalized;
        Vector3 attackEndPos = startPos + direction * attackMoveDistance;

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
        }
    }
}

