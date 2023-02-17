using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BonusBase : MonoBehaviour
{
    public event Action OnPlayerEnter = null;
    [SerializeField] LayerMask playerMask = 1 << 6;
    private void Start()
    {
        
    }
    void Init()
    {
        
    }
    private void OnTriggerEnter(Collider _collier)
    {
        
    }
}
