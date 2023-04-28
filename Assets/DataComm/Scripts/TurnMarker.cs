using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class TurnMarker : NetworkBehaviour
{
    public TextMeshPro turnIndexText;
    [SyncVar] public GameObject Parent;
    private void Update()
    {
        if (Parent != null)
        {
            transform.position = Parent.transform.position;
            transform.rotation = Parent.transform.rotation;
        }
        
        turnIndexText.text = $"P{1+DCMTurnManager.Instance.turnIndex}";;
    }

}
