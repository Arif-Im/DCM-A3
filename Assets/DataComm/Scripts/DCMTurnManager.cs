using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DCMTurnManager : NetworkBehaviour
{
    public static DCMTurnManager Instance { get; private set; }

    public TurnMarker turnMarker;
    public TextMeshProUGUI turnText;


    public float turnDuration = 10f;

    [SyncVar] public int turnIndex;
    [SyncVar] public float turnNetworkBeginTime;

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

        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }

    private void Update()
    {
        if (curTurnTimeLeft >= 10 && isServer)
        {
            turnNetworkBeginTime = (float)NetworkTime.time;
            turnIndex++;
            if (turnIndex >= NetworkManager.singleton.numPlayers)
            {
                turnIndex = 0;
            }
        }

        if (isLocalPlayer)
        {
            MyNetworkPlayer localP = NetworkClient.localPlayer.gameObject.GetComponent<MyNetworkPlayer>();
            if (turnIndex==localP.index)
                localP.CmdPickUpMarker(turnMarker);
            else
                localP.CmdDropMarker(turnMarker);
        }

        curTurnTimeLeft = Mathf.Abs((float)NetworkTime.time - turnNetworkBeginTime);
        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }
}