using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpform : MonoBehaviour
{

    public Movement Movement;
    public float JumpBoost;
    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        Movement.jumpPadHeight = JumpBoost;

    }

    
}
