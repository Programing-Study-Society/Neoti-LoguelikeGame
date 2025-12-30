using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背景画像管理クラス
/// Canvas直下に配置し、全ての画面で共通の背景画像を表示
/// </summary>
public class BackgroundScreen : MonoBehaviour
{
    /// <summary>
    /// 惑星の種類（BattleScreen、TreasureScreenと共通）
    /// </summary>
    public enum PlanetType
    {
        Forest_Planet,
        Ice_Planet,
        Old_Empire_Planet,
        Desert_Planet,
        Volcano_Planet
    }

    [Header("背景画像")]
    [SerializeField] private Image backgroundImage; // 背景画像を表示するImageコンポーネント

    [Header("惑星背景画像（Inspectorで設定）")]
    [SerializeField] private Sprite forestPlanetSprite;
    [SerializeField] private Sprite icePlanetSprite;
    [SerializeField] private Sprite oldEmpirePlanetSprite;
    [SerializeField] private Sprite desertPlanetSprite;
    [SerializeField] private Sprite volcanoPlanetSprite;
    [SerializeField] private Sprite defaultBackgroundSprite; // デフォルト背景画像（惑星が不明な場合）

    /// <summary>
    /// 惑星名（文字列）から背景を設定する
    /// </summary>
    /// <param name="planetName">惑星名（例: "Forest_Planet"）</param>
    public void SetPlanetBackground(string planetName)
    {
        Debug.Log($"BackgroundScreen: 惑星背景を設定 - {planetName}");

        if (System.Enum.TryParse<PlanetType>(planetName, out PlanetType planetType))
        {
            SetPlanetBackground(planetType);
        }
        else
        {
            Debug.LogWarning($"BackgroundScreen: 不明な惑星名 '{planetName}'、デフォルト背景を使用");
            SetBackgroundSprite(defaultBackgroundSprite);
        }
    }

    /// <summary>
    /// 惑星タイプから背景を設定する
    /// </summary>
    /// <param name="planetType">惑星タイプ</param>
    public void SetPlanetBackground(PlanetType planetType)
    {
        Sprite selectedSprite = GetPlanetSprite(planetType);

        if (selectedSprite != null)
        {
            SetBackgroundSprite(selectedSprite);
            Debug.Log($"BackgroundScreen: {planetType} の背景を設定しました");
        }
        else
        {
            Debug.LogWarning($"BackgroundScreen: {planetType} の背景画像が見つかりません、デフォルト背景を使用");
            SetBackgroundSprite(defaultBackgroundSprite);
        }
    }

    /// <summary>
    /// 惑星タイプから対応するスプライトを取得
    /// </summary>
    /// <param name="planetType">惑星タイプ</param>
    /// <returns>対応するスプライト（見つからない場合はnull）</returns>
    private Sprite GetPlanetSprite(PlanetType planetType)
    {
        return planetType switch
        {
            PlanetType.Forest_Planet => forestPlanetSprite,
            PlanetType.Ice_Planet => icePlanetSprite,
            PlanetType.Old_Empire_Planet => oldEmpirePlanetSprite,
            PlanetType.Desert_Planet => desertPlanetSprite,
            PlanetType.Volcano_Planet => volcanoPlanetSprite,
            _ => defaultBackgroundSprite
        };
    }

    /// <summary>
    /// 背景スプライトを設定
    /// </summary>
    /// <param name="sprite">設定するスプライト</param>
    private void SetBackgroundSprite(Sprite sprite)
    {
        if (backgroundImage == null || backgroundImage.gameObject == null)
        {
            Debug.LogError("BackgroundScreen: backgroundImageが設定されていないか、破棄されています");
            return;
        }

        if (sprite != null)
        {
            backgroundImage.sprite = sprite;
            Debug.Log($"BackgroundScreen: 背景画像を設定 - {sprite.name}");
        }
        else
        {
            Debug.LogWarning("BackgroundScreen: スプライトがnullです");
        }
    }

    /// <summary>
    /// 背景画像を表示
    /// </summary>
    public void Show()
    {
        if (backgroundImage != null && backgroundImage.gameObject != null)
        {
            backgroundImage.gameObject.SetActive(true);
            Debug.Log("BackgroundScreen: 背景画像を表示");
        }
        else
        {
            Debug.LogWarning("BackgroundScreen: backgroundImageまたはそのgameObjectが破棄されています");
        }
    }

    /// <summary>
    /// 背景画像を非表示
    /// </summary>
    public void Hide()
    {
        if (backgroundImage != null && backgroundImage.gameObject != null)
        {
            backgroundImage.gameObject.SetActive(false);
            Debug.Log("BackgroundScreen: 背景画像を非表示");
        }
        else
        {
            Debug.LogWarning("BackgroundScreen: backgroundImageまたはそのgameObjectが破棄されています");
        }
    }
}

