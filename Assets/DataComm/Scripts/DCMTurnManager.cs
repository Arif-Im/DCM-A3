using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class DCMTurnManager : NetworkBehaviour
{
    public static DCMTurnManager Instance { get; private set; }

    public TurnMarker turnMarker;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI matchEndText;


    public float turnDuration = 10f;

    [SyncVar(hook = nameof(HandleTurnIndexUpdate))]
    public int turnIndex;

    [SyncVar] public float turnNetworkBeginTime;

    float curTurnTimeLeft;
    
    [SyncVar]
    float matchTime;

    float TimeUntilMatchEnds = 120; // 2 minutes until match ends
    private float offsetTime;
    
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

        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\nTurnTime: {(curTurnTimeLeft).ToString("F2")}";
    }

    private void OnEnable()
    {
        offsetTime = (float)NetworkTime.time;
    }

    private void OnDisable()
    {

        matchEndText.gameObject.SetActive(false);
    }

    public List<MyNetworkPlayer> allPlayers;
    private void LateUpdate()
    {
        // List<MyNetworkPlayer> allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
        allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
        if (curTurnTimeLeft <= 0)
        {
            if (allPlayers.Count <= 0)
                return;
            // print("Pick");
            // print(allPlayers.Count);

            if (isServer)
            {
                turnNetworkBeginTime = (float)NetworkTime.time;

                allPlayers.OrderBy(x => x.index);
                allPlayers.Reverse();
                // var highestTurnIndexPlayer = allPlayers.First();
  
                var NextPlayerFound = allPlayers.Find(x => x.index >= turnIndex+1);


                if (NextPlayerFound == null)
                {   
                    turnIndex = 0;
                }
                else
                {
                    turnIndex = NextPlayerFound.index;
                }
                
                // if (turnIndex >= NetworkManager.singleton.numPlayers)
                // if (turnIndex >= highestTurnIndexPlayer.index)
                // {
                //     turnIndex = 0;
                // }
            }
        }

        curTurnTimeLeft = turnDuration - Mathf.Abs((float)NetworkTime.time - turnNetworkBeginTime);
        if (matchTime >= TimeUntilMatchEnds)
        {
            matchEndText.gameObject.SetActive(true);
            allPlayers.OrderByDescending(x => x.score).First();
            foreach (var p in allPlayers)
            {
                p._controller.enabled = false;
            }

            var winPlayer = allPlayers.First();
            matchEndText.text = $"{winPlayer.displayNameText.text} Wins\n With Score of {winPlayer.score}";
        }
        else
        {
            if(isServer)
                matchTime = (float)NetworkTime.time - offsetTime;
        }

        string timeMinutes = DisplayTimeMinutes(matchTime);
        turnText.text = $"<size=130%>Player {1 + turnIndex}'s </size>\nTurn\nTurnTime  : {(curTurnTimeLeft).ToString("F2")}\nElapsedTime: {timeMinutes}";
    }

    private void HandleTurnIndexUpdate(int oldIndex, int newIndex)
    {
        // print(isOwned);
        if (turnMarker != null)
        {
            List<MyNetworkPlayer> allPlayers = FindObjectsOfType<MyNetworkPlayer>().ToList();
            var oldNetworkPlayer = allPlayers.Find(x => x.index == oldIndex);
            if (oldNetworkPlayer && oldNetworkPlayer.isOwned)
                oldNetworkPlayer.CmdDropMarker(turnMarker.gameObject);

            var newNetworkPlayer = allPlayers.Find(x => x.index == newIndex);
            if (newNetworkPlayer)
                if (isServer)
                {
                    newNetworkPlayer.PickupMarker(turnMarker.gameObject);
                    newNetworkPlayer.ClientRPCPickUpMarker(turnMarker.gameObject);
                }
        }
    }

    string DisplayTimeMinutes(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}