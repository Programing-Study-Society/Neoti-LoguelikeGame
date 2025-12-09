using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill_L : MonoBehaviour
{
    //{アクティブ状態 , 取得状態}
    public Dictionary<string, List<List<int>>> skill_list = new Dictionary<string, List<List<int>>>(){
    {"attaku", new List<List<int>>(){
        new List<int>(){1,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0}
        }
    },
    {"defense", new List<List<int>>(){
        new List<int>(){1,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0}
        }
    },
    {"hp", new List<List<int>>(){
        new List<int>(){1,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0},
        new List<int>(){0,0}
        }
    }
    };
}
