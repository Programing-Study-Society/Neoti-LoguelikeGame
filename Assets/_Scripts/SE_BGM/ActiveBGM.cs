using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBGM : MonoBehaviour
{
    public GameObject situationBGMObject;//シチュエーションに応じたBGM
    
    void OnEnable()
    {
        // nullチェック：オブジェクトが破棄されている場合は処理をスキップ
        // Unityでは、破棄されたオブジェクトに対して == null を使うと自動的にnullとして扱われる
        if (situationBGMObject != null)
        {
            situationBGMObject.SetActive(true);
        }
    }
    void OnDisable()
    {
        // nullチェック：オブジェクトが破棄されている場合は処理をスキップ
        // Unityでは、破棄されたオブジェクトに対して == null を使うと自動的にnullとして扱われる
        // .gameObject にアクセスする必要はない（GameObject自体が破棄されている可能性があるため）
        if (situationBGMObject != null)
        {
            situationBGMObject.SetActive(false);
        }
    }
}
