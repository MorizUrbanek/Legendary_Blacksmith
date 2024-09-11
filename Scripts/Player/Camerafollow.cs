using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] Transform followTargte;

    // Update is called once per frame
    void Update()
    {
        transform.position = followTargte.position;
    }
}
