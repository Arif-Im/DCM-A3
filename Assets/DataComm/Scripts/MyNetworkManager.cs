using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MyNetworkManager : NetworkManager
{
    public override void Awake()
    {
        base.Awake();
        if (MyNetworkManager.singleton != null && MyNetworkManager.singleton !=this )
        {   
            var foundNetworkManager = (MyNetworkManager)(MyNetworkManager.singleton );
            foundNetworkManager.InGameCinemachine = this.InGameCinemachine;
            foundNetworkManager.NoPlayerCinemachine = this.NoPlayerCinemachine;
            Destroy(this.gameObject);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        // Debug.Log("OnClientConnect");
        Debug.Log("You have connected to the server");
        if (NoPlayerCinemachine)
            NoPlayerCinemachine.gameObject.SetActive(false);
        if (InGameCinemachine)
            InGameCinemachine.gameObject.SetActive(true);

    }

    public CinemachineVirtualCamera NoPlayerCinemachine;
    public CinemachineVirtualCamera InGameCinemachine;


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        if (NoPlayerCinemachine)
            NoPlayerCinemachine.gameObject.SetActive(false);
        if (InGameCinemachine)
            InGameCinemachine.gameObject.SetActive(true);

        // Debug.Log("OnServerAddPlayer ");
        Debug.Log($"Current Number of Players {numPlayers} ");

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.index = numPlayers;

        player.setDisplayName($"Player {numPlayers}");

        Color displayColor = new Color(Random.Range(0, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));

        player.setDisplayColor(displayColor);
        player.setScoreText(numPlayers - 1);

    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        if (NoPlayerCinemachine)
            NoPlayerCinemachine.gameObject.SetActive(true);
        if (InGameCinemachine)
            InGameCinemachine.gameObject.SetActive(false);
        
        
        // Restart Scene to Reset All RigidBody Transform Positions
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (this.isNetworkActive == false)
        {
            // Restart Scene to Reset All RigidBody Transform Positions
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}