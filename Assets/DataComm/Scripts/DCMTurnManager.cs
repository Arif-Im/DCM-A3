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

    [SyncVar] public int turnIndex;
    [SyncVar] public float turnNetworkBeginTime;

    public float curTurnTimeLeft;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

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

    private void LateUpdate()
    {
        if (curTurnTimeLeft <= 0)
        {
            List<MyNetworkPlayer> allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
            var networkPlayer = allPlayers.Find(x => x.index == turnIndex);
            if (networkPlayer == null)
                return;
            print("Pick");
            print(allPlayers.Count);

            if (isServer)
            {
                turnNetworkBeginTime = (float)NetworkTime.time;
                turnIndex++;
                if (turnIndex >= NetworkManager.singleton.numPlayers)
                {
                    turnIndex = 0;
                }
            }
            //
            if (turnMarker.gameObject != null)
            {
                networkPlayer.CmdPickUpMarker(turnMarker.gameObject);
            }
        }


        curTurnTimeLeft = turnDuration - Mathf.Abs((float)NetworkTime.time - turnNetworkBeginTime);
        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\n{(curTurnTimeLeft).ToString("F2")}";
    }
}