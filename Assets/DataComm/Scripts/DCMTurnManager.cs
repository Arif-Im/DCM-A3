using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SyncVar(hook = nameof(HandleTurnIndexUpdate))]
    public int turnIndex;
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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        turnIndex = 1;
    }

    private void LateUpdate()
    {
        if (curTurnTimeLeft <= 0)
        {
            List<MyNetworkPlayer> allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
            if (allPlayers.Count<=0)
                return;
            // print("Pick");
            // print(allPlayers.Count);

            if (isServer)
            {
                turnNetworkBeginTime = (float)NetworkTime.time;
                turnIndex++;
                if (turnIndex >= NetworkManager.singleton.numPlayers)
                {
                    turnIndex = 0;
                }
            }

        }


        curTurnTimeLeft = turnDuration - Mathf.Abs((float)NetworkTime.time - turnNetworkBeginTime);
        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }

    private void HandleTurnIndexUpdate(int oldIndex, int newIndex)
    {
        // print(isOwned);
        if (turnMarker.gameObject != null)
        {
            List<MyNetworkPlayer> allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
            var oldNetworkPlayer = allPlayers.Find(x => x.index == oldIndex);
            if(oldNetworkPlayer && oldNetworkPlayer.isOwned)
                oldNetworkPlayer.CmdDropMarker(turnMarker.gameObject);
            
            var newNetworkPlayer = allPlayers.Find(x => x.index == newIndex);
            if(newNetworkPlayer)
                if (isServer)
                {
                    newNetworkPlayer.PickupMarker(turnMarker.gameObject);
                    newNetworkPlayer.ClientRPCPickUpMarker(turnMarker.gameObject);
                }
        }
    }
}