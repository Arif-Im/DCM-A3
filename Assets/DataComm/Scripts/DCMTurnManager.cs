using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class DCMTurnManager : NetworkBehaviour
{
    public static DCMTurnManager Instance { get; private set; }

    public TurnMarker turnMarker;
    public TextMeshProUGUI turnText;
    

    public float TurnTime = 10f;

    [SyncVar]
    public int TurnIndex;
    [SyncVar]
    public float TurnNetworkBeginTime;

    public float curTurnTimeLeft;


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

        turnText.text = $"<size=130%>Player {1+TurnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }

    private void Update()
    {
        if (curTurnTimeLeft >= 10 && isServer)
        {
            TurnNetworkBeginTime = (float)NetworkTime.time;
            TurnIndex++;
            if (TurnIndex > NetworkManager.singleton.numPlayers)
            {
                TurnIndex = 0;
            }
        }

        curTurnTimeLeft = Mathf.Abs((float)NetworkTime.time - TurnNetworkBeginTime);
        turnText.text = $"<size=130%>Player {1+TurnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }
}