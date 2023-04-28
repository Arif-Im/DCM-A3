using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class DCMTurnManager : NetworkBehaviour
{
    public static DCMTurnManager Instance { get; private set; }

    public GameObject TurnMarker;
    public TextMeshProUGUI turnText;
    public int TurnIndex;

    public float TurnTime = 10f;

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

        turnText.text = $"<size=130%>Player {TurnIndex}'s </size>\nTurn\n{curTurnTimeLeft.ToString("F2")}";
    }

    private void Update()
    {
        if (curTurnTimeLeft >= 10)
        {
            TurnNetworkBeginTime = (float)NetworkTime.time;
            TurnIndex++;
        }

        curTurnTimeLeft = (float)NetworkTime.time - TurnNetworkBeginTime;
        turnText.text = $"<size=130%>Player {TurnIndex}'s </size>\nTurn\n{curTurnTimeLeft.ToString("F2")}";
    }
}