using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack Movelist")]

public class AttackMovelist : ScriptableObject
{
    public List<Attack> attacks;
}
