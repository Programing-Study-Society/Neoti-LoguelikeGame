using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBGM : MonoBehaviour
{
    public GameObject situationBGMObject;//シチュエーションに応じたBGM
    
    void OnEnable()
    {
        situationBGMObject.SetActive(true);
    }
    void OnDisable()
    {
        situationBGMObject.SetActive(false);
    }
}
