using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectKeeper : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera; // 対象となるカメラ

    [SerializeField]
    private Vector2 aspectVec;   // 目的のアスペクト比（例：16, 9）

    void Update()
    {
        var screenAspect = Screen.width / (float)Screen.height; // 画面のアスペクト比
        var targetAspect = aspectVec.x / aspectVec.y;           // 目的のアスペクト比

        var magRate = targetAspect / screenAspect;              // 目的のアスペクト比に合わせる倍率

        var viewportRect = new Rect(0, 0, 1, 1);                // Viewport 用の Rect を作成

        if (magRate < 1)
        {
            viewportRect.width = magRate;                       // 使用する横幅を変更
            viewportRect.x = 0.5f - viewportRect.width * 0.5f;  // 中央寄せ
        }
        else
        {
            viewportRect.height = 1 / magRate;                  // 使用する縦幅を変更
            viewportRect.y = 0.5f - viewportRect.height * 0.5f; // 中央寄せ
        }

        targetCamera.rect = viewportRect;                       // カメラに Viewport を適用
    }
}