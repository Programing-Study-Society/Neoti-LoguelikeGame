using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoad : MonoBehaviour
{
    public TemplateSave save;
    // Start is called before the first frame update
    void Awake()
    {
        save.StartRoad();
    }
}
