using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class summary : MonoBehaviour
{
    public enum GameState{
        Map,        //マップ上で移動
        Battle,     //バトル中
        Result,     //バトル後の報酬
        Treasure,   //宝箱
        Shop        //ショップ
    }

    public GameState currentState;

    [Header("State Objects")]
    public GameObject mapRoot;
    public GameObject battleRoot;
    public GameObject resultRoot;
    public GameObject treasureRoot;
    public GameObject shopRoot;

    void Start(){
    }

    public void ChangeState(GameState newState){//引用例ChangeState(GameState.Battle);
        currentState = newState;

         // 全部いったん非表示
        mapRoot.SetActive(false);
        battleRoot.SetActive(false);
        resultRoot.SetActive(false);
        treasureRoot.SetActive(false);
        shopRoot.SetActive(false);


        switch (newState)
        {      
            case GameState.Map:
                mapRoot.SetActive(true);
                break;

            case GameState.Battle:
                battleRoot.SetActive(true);
                break;

            case GameState.Result:
                resultRoot.SetActive(true);
                break;

            case GameState.Treasure:
                treasureRoot.SetActive(true);
                break;

            case GameState.Shop:
                shopRoot.SetActive(true);
                break;
        }
    }
}
