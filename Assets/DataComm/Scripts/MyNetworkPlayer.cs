using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private Renderer displayColorRenderer;
    [SerializeField] private GameObject displayScoreUI;
    
    [SyncVar(hook = nameof(OnSetIndex))]
    public int index = 0;
    

    [SyncVar(hook = nameof(OnUpdateScore))]
    [SerializeField]
    private int score = 0;

    [SyncVar(hook = nameof(HandleDisplayNameUpdate))] 
    [SerializeField]
    private string displayName = "Missing Name";
    
    [SyncVar(hook = nameof(HandleDisplayColourUpdate))] 
    [SerializeField]
    private Color displayColor = Color.black;
    private void OnEnable()
    {
        GameObject.Find("Canvas").transform.Find("Panel").Find("Title").GetChild(0).gameObject.SetActive(true); 
    }

    private void OnDisable()
    {
        if (displayScoreUI != null)
        {
            displayScoreUI.transform.GetChild(0).gameObject.SetActive(false);
        }    
        // GameObject.Find("Canvas").transform.Find("Panel").Find("Title").GetChild(0).gameObject.SetActive(false);
        
        if(isOwned)
            if(CurrentMarker!=null)
                this.CmdDropMarker(CurrentMarker);
    }

    private void Update()
    {
        HandleDisplayScore();
    }

    #region Server
    [Server]
    public void setDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
    [Server]
    public void setDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }

    [Server]
    public void setScoreText(int _index)
    {
        index = _index;

        // displayScoreText = GameObject.Find("Canvas").transform.Find("Image").GetChild(_index).GetComponent<TMP_Text>();
        // Debug.Log(displayScoreText.name);

    }

    [Server]
    public void setScore(int _score)
    {
        score += _score;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    { 
        // server authority to limit displayName into 2-20 letter length
        if(newDisplayName.Length<2||newDisplayName.Length>20)
        {
            return;
        }
        RpcDisplayNewName(newDisplayName);
        setDisplayName(newDisplayName);
    }


    // [Command]
    // public void CmdSetScore()
    // {
    //     setScore(100);
    //     // RpcDisplayScore();
    // }

    #endregion

    #region Client

    private void HandleDisplayColourUpdate(Color oldColor, Color newColor)
    {
        displayColorRenderer.material.SetColor("_BaseColor",newColor);
        // displayColorRenderer.material.color = newColor;
    }
    private void HandleDisplayNameUpdate(string oldName, string newName)
    {
        displayNameText.text = newName;
    }
    public void OnSetIndex(int oldIndex, int newIndex)
    {
        index = newIndex;
    }

    private void OnUpdateScore(int oldScore, int newScore)
    {
        score = newScore;
        if(isOwned)
            CommandRPCDisplayScore();

    }

    [ContextMenu("Set this Name")]
    private void SetThisName()
    {
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcDisplayNewName(string newDisplayName)
    {
        Debug.Log(newDisplayName);
    }
    
    
    public void HandleDisplayScore()
    {

        displayScoreUI = GameObject.Find("Canvas").transform.Find("Panel").Find($"PlayerScore {index}").gameObject;
        displayScoreUI.transform.GetChild(0).gameObject.SetActive(true);
        displayScoreUI.GetComponentInChildren<TextMeshProUGUI>().text = $"{displayNameText.text}: {score}";
        
        // displayScoreText = GameObject.Find("Canvas").transform.Find("Image").GetChild(index).GetComponent<TMP_Text>();
        // displayScoreText.text = $"Player {index + 1}: {score}";
    }
    [ClientRpc]
    public void ClientRpcDisplayScore()
    {

        HandleDisplayScore();
        // if (displayScoreUI == null)
        //     RpcSetDisplayScore();
        // // displayScoreText.text = $"Player {index + 1}: {score}";
        //
        // displayScoreUI.GetComponentInChildren<TextMeshProUGUI>().text = $"{displayNameText.text}: {score}";
    }

    [Command]
    public void CommandRPCDisplayScore()
    {
        ClientRpcDisplayScore();
    }
    

    #endregion

    #region Pass Authority

    public GameObject CurrentMarker;
    public void PickupMarker(GameObject marker)
    {
        CurrentMarker = marker;
        var turnMarker = marker.GetComponent<TurnMarker>();
        turnMarker.netIdentity.RemoveClientAuthority();
        turnMarker.netIdentity.AssignClientAuthority(connectionToClient);
        turnMarker.Parent = this.gameObject;
    }
    
    
    // Ref: https://youtu.be/nkU-dgExUlI?t=548
    [Command]
    public void CmdPickUpMarker(GameObject marker)
    {
        PickupMarker(marker);
    }

    
    [ClientRpc]
    public void ClientRPCPickUpMarker(GameObject marker)
    {
        PickupMarker(marker);
    }
    
    [Command]
    public void CmdDropMarker(GameObject marker)
    {
        CurrentMarker = null;
        var turnMarker = marker.GetComponent<TurnMarker>();
        turnMarker.Parent = null;
    }


    #endregion
}