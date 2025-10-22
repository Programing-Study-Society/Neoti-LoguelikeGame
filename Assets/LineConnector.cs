using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public Transform pointA;  // 線を引く始点
    public Transform pointB;  // 線を引く終点
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;  // 始点と終点
        lineRenderer.startWidth = 0.1f;                   // 開始点の太さを0.1にする
        lineRenderer.endWidth = 0.1f;                     // 終了点の太さを0.1にする
    }

    void Update()
    {
        if (pointA != null && pointB != null)
        {
            lineRenderer.SetPosition(0, pointA.position);
            lineRenderer.SetPosition(1, pointB.position);
        }
    }
}
