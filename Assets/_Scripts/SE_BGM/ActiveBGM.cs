using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBGM : MonoBehaviour
{
    public GameObject situationBGMObject;//シチュエーションに応じたBGM
    
    void OnEnable()
    {
        // nullチェック：オブジェクトが破棄されている場合は処理をスキップ
        // try-catchで囲むことで、破棄されたオブジェクトへのアクセスを安全に処理
        try
        {
            if (situationBGMObject != null)
            {
                situationBGMObject.SetActive(true);
            }
        }
        catch (MissingReferenceException)
        {
            // オブジェクトが破棄されている場合は何もしない
            Debug.LogWarning("ActiveBGM: situationBGMObjectが破棄されています（OnEnable）");
        }
    }
    void OnDisable()
    {
        // nullチェック：オブジェクトが破棄されている場合は処理をスキップ
        // try-catchで囲むことで、破棄されたオブジェクトへのアクセスを安全に処理
        try
        {
            if (situationBGMObject != null)
            {
                situationBGMObject.SetActive(false);
            }
        }
        catch (MissingReferenceException)
        {
            // オブジェクトが破棄されている場合は何もしない
            Debug.LogWarning("ActiveBGM: situationBGMObjectが破棄されています（OnDisable）");
        }
    }
}
