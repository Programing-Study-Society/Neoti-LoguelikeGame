using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSE : MonoBehaviour
{
    public AudioSource situationSE;//シチュエーションに応じたSE

    // Start is called before the first frame update
    void OnEnable()
    {
        situationSE.Play();
    }
}
