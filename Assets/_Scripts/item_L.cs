using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public Dictionary<int, List<int>> item_list = new Dictionary<int, List<int>>(){
    {0, new List<int>() {0,0,0}},
    {1, new List<int>() {0,0,0}},
    {2, new List<int>() {0,0,0}},
    {3, new List<int>() {0,0,0}},
    {4, new List<int>() {0,0,0}},
    {5, new List<int>() {0,0,0}},
    {6, new List<int>() {0,0,0}},
    {7, new List<int>() {0,0,0}},
    {8, new List<int>() {0,0,0}}
    };
    public Dictionary<int, string> item_name_list = new Dictionary<int, string>(){
    {0,"リペアユニット"},
    {1,"ガトリングガン"},
    {2,"重装ランチャー"},
    {3,"火炎放射器"},
    {4,"サイバーハックモジュール"},
    {5,"オーバークロックモジュール"},
    {6,"スタンガン"},
    {7,"フィールドシールド"},
    {8,"EMPパルスキャノン"}
    };
}
