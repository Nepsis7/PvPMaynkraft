using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BouncepadDefault", menuName = "BouncePad")]
public class BouncePadSettings : ScriptableObject
{
    [SerializeField,Range(0,5)] float speedBound = 1;
    [SerializeField, Range(-1, 1)] float xDir = 0;
    [SerializeField, Range(-1, 1)] float yDir = 0;
    [SerializeField, Range(-1, 1)] float zDir = 0;
    public Vector3 Direction => new Vector3(xDir, yDir, zDir).normalized * speedBound * 10;
}
