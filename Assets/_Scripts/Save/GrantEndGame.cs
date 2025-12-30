using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrantEndGame : MonoBehaviour
{
    [HideInInspector]public TemplateSave templateSave;
    // Start is called before the first frame update
    void Start()
    {
        //ゲーム終了処理をスクリプトから持ってきてボタンに付与
        templateSave = FindObjectOfType<TemplateSave>();
        if (templateSave == null)
        {
            Debug.LogError("TemplateSaveコンポーネントが見つかりません。");
            return;
        }
        GetComponent<Button>().onClick.AddListener(templateSave.endGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
