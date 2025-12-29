using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAddSE : MonoBehaviour
{
    public List<Button> addSeButtonList;//特定のSEを入れたいボタンを入れるリスト
    public AudioSource buttonClickSE;//ボタンを押したときに鳴るSE

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < addSeButtonList.Count; i++){
            addSeButtonList[i].onClick.AddListener(OnClickSE);
        }
        void OnClickSE(){
            buttonClickSE.Play();
        }
    }
}
