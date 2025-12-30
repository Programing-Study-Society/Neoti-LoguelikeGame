using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class canClick : MonoBehaviour
{
    public List<Button> buttonList; // 全てのボタンをここに追加
    
    private int activeButtonIndex = -1; // 現在アクティブなボタンのインデックス

    public void OnButton(int buttonIndex)
    {
        // 現在のボタンが押されている場合
        if (activeButtonIndex == buttonIndex)
        {
            // Imageを非表示にして、全ボタンを再度有効化する
            SetButtonsInteractable(true);
            activeButtonIndex = -1; // アクティブなボタンなしに戻す
        }
        else
        {
            // すべてのボタンを無効化し、押されたボタンのImageのみ表示
            SetButtonsInteractable(false);
            buttonList[buttonIndex].interactable = true; // 現在のボタンだけ有効にしておく
            activeButtonIndex = buttonIndex; // アクティブなボタンを更新
        }
    }

    // 全ボタンを一括で有効/無効にする
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in buttonList)
        {
            button.interactable = interactable;
        }
    }
}

