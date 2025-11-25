using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    
    public int MAXHP = player.MAXHP;
    public int ATK = player.ATK;
    public int DEF = player.DEF;
    public int SKILL = player.SKILL;
    Dictionary<string, int> item_dict = new Dictionary<string, int>(){
        {"ガトリング", 0},
        {"大砲", 0},
        {"スタンガン", 0}      
    };

    public test TEST;
    public battle_system Battel_System;
   

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attack()
    {
        
    }
}
