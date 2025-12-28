using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のための名前空間

public class Title : MonoBehaviour
{
    // タイトルシーンに戻るメソッド
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("StartScene"); // タイトルシーンをロード
    }
}

