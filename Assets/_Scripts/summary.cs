using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class summary : MonoBehaviour
{
    public enum GameState{
        Title,      //タイトル画面中
        Map,        //マップ上で移動
        Battle,     //バトル中
        Result,     //バトル後の報酬
        Treasure,   //宝箱
        Shop        //ショップ
    }

    public GameState currentState;

    void Start(){
        ChangeState(GameState.Map);
    }


    public void ChangeState(GameState newState){
        currentState = newState;

        switch (newState)
        {
            case GameState.Title:
                Title();
                break;
            
            case GameState.Map:
                Map();
                break;

            case GameState.Battle:
                Battle();
                break;

            case GameState.Result:
                Result();
                break;

            case GameState.Treasure:
                Box();
                break;

            case GameState.Shop:
                Shop();
                break;
        }
    }
    

    public void Title(){
        
    }
    public void Map(){
        
    }
    public void Battle(){
        
    }
    public void Result(){
        
    }
    public List<int> Box(){
        return new List<int>() {0,0,0};
    }
    public void Shop(){
        
    }
}
