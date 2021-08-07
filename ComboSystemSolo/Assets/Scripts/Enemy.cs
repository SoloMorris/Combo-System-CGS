using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Combatant
{
    private Rigidbody2D myBody;
    

    // Destroy all this and refactor it
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
    }
    
}
