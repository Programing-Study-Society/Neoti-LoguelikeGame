using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalTreasure : MonoBehaviour
{
    int[] itemlist;
    
    int recovery = 0;
    int combo = 1;
    int storong = 2;
    int persistent = 3;
    int AttackDebuff = 4;
    int AttackBuff = 5;
    int protectDebuff = 6;
    int protectBuff = 7;
    int range = 8;

    public void kakuritu()
    {
        itemlist[0] = recovery;
        itemlist[1] = combo;
        itemlist[2] = storong;
        itemlist[3] = persistent;
        itemlist[4] = AttackDebuff;
        itemlist[5] = AttackBuff;
        itemlist[6] = protectDebuff;
        itemlist[7] = protectBuff;
        itemlist[8] = range;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
