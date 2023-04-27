using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DCMTurnManager : NetworkBehaviour
{
    public static DCMTurnManager Instance { get; private set; }

    public GameObject TurnMarker;
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    
}
