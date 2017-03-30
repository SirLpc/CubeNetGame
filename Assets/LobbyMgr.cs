using System;
using UnityEngine;
using UnityEngine.UI;
using TNet;

public class LobbyMgr : TNBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private string connectIp = "115.28.87.227";
    [SerializeField] private int connectPort = 5127;
    [SerializeField] private int roomPlayerCount = 2;

    private Button button_play;
    private Button button_connect;
    private Button button_disconnect;



    private void Awake()
    {
        if (canvas == null)
        {
            Debug.LogError("set canvas");
            Debug.Break();
        }

        button_play = canvas.transform.FindChild("play").GetComponent<Button>();
        button_play.onClick.AddListener(button_play_onClick);

        button_connect = canvas.transform.FindChild("connect").GetComponent<Button>();
        button_connect.onClick.AddListener(button_connect_onClick);

        button_disconnect = canvas.transform.FindChild("disconnect").GetComponent<Button>();
        button_disconnect.onClick.AddListener(button_disconnect_onClick);

        //when joining a channel the scene is reloaded (if using one scene)
        if (TNManager.isConnected)
        {
            button_connect.interactable = false;
            button_disconnect.interactable = true;

            //play button is only available if in the lobby channel
            if (TNManager.IsInChannel(1000)) button_play.interactable = true;
            else button_play.interactable = false;
        }
        else
        {
            button_play.interactable = false;
            button_connect.interactable = true;
            button_disconnect.interactable = false;
        }
    }

    new void OnEnable()
    {
        TNManager.onConnect += network_onConnect;
        TNManager.onDisconnect += network_onDisconnect;

        TNManager.onJoinChannel += network_onJoinChannel;
        TNManager.onPlayerJoin += network_onPlayerJoin;
    }

    void OnDisable()
    {
        TNManager.onConnect -= network_onConnect;
        TNManager.onDisconnect -= network_onDisconnect;

        TNManager.onJoinChannel -= network_onJoinChannel;
        TNManager.onPlayerJoin -= network_onPlayerJoin;
    }

    private void button_connect_onClick()
    {
        button_connect.interactable = false;

        TNManager.Connect(connectIp, connectPort);
    }

    private void button_disconnect_onClick()
    {
        button_disconnect.interactable = false;

        TNManager.Disconnect();
    }

    private void button_play_onClick()
    {
        //button_play.interactable = false;

        Debug.Log(TNManager.player.Get<bool>("isQue"));

        TNManager.SetPlayerData("isQue", true);

        tno.Send("RFC_QueGame", Target.Host, TNManager.playerID, TNManager.playerName);
    }

    #region RFC's

    [RFC]
    void RFC_QueGame(int playerid, string playername)
    {
        //this RFC is only sent to the channel host
        //the 'isHosting' player would get Que priority

        List<Player> players = new List<Player>();

        //client player
        Debug.Log(TNManager.player.Get<bool>("isQue"));
        msg += "==in RFC==";

        if (TNManager.player.Get<bool>("isQue"))
            players.Add(TNManager.player);
     
        //network players
        foreach (Player p in TNManager.players)
        {
            Debug.Log(p.Get<bool>("isQue"));

            if (p.Get<bool>("isQue"))
            {
                players.Add(p);
            }
            if (players.Count >= roomPlayerCount)
            {
                break;
            }

            //if the 'isHosting' player is not in the Que - take the first
            //2 players from the list
            //or respond to the player that there is no other players available
        }

        if (players.Count < roomPlayerCount)
        {
            msg += "==not enough player==";
            return;
        }

        int newchannelid = UnityEngine.Random.Range(1,999);
        foreach (var player in players)
        {
            tno.Send("RFC_EnterGame", player.id, newchannelid);
        }
    }

    [RFC]
    void RFC_EnterGame(int channelid)
    {
        //set Que to false here
        //or set when entering any new channel via the callback

        TNManager.SetPlayerData("isQue", false);

        TNManager.JoinChannel(channelid, "4. Chat", false, roomPlayerCount, null, true);
    }

    #endregion

    #region TNET callbacks

    private void network_onConnect(bool success, string message)
    {
        if (success)
        {
            button_disconnect.interactable = true;

            //everyone joins main catch-all lobby channel
            TNManager.JoinChannel(1000, "lobby", true, 500, null, true);
        }
        else Debug.Log(string.Format("network_onConnect: success={0} message={1}", success, message));
    }

    private void network_onDisconnect()
    {
        button_connect.interactable = true;
    }

    private void network_onJoinChannel(int channelID, bool success, string message)
    {
        Debug.Log(string.Format("network_onJoinChannel: channelID={0} success={1} message={2}", channelID, success, message));

        TNManager.SetPlayerData("isQue", false);
    }

    private void network_onPlayerJoin(int channelID, Player p)
    {
        Debug.Log(string.Format("network_onPlayerJoin: channelID={0} p.name={1}", channelID, p.name));
    }

    #endregion

    private string msg = "";
    private void OnGUI()
    {
        GUILayout.Label(msg);
    }
}