using UnityEngine;
using System.Collections.Generic;
using TNet;


public class NetMgr : TNBehaviour 
{

	public static NetMgr Instance = null;

	private void Awake()
	{
		Instance = this;
	}

	public bool Init()
	{
		if (TNManager.isConnected)
		{
			TNManager.SetPlayerData("isReady", false);
			return true;
		}

		return false;
	}

	public void Ready()
	{
		TNManager.SetPlayerData("isReady", true);
		Debug.Log (TNManager.players.Count);
		tno.Send("RFC_ReadyGame", Target.Host);
	}

    public void NewTurn(int turnType)
    {

        tno.Send("RFC_NewTurn", Target.All, turnType);
    }

	[RFC]
	void RFC_ReadyGame()
	{
        var aps = this.GetAllPlayers();

		foreach (Player p in aps)
		{
			if (!p.Get<bool>("isReady"))
			{
				return;
			}
		}

		MapMgr.Instance.InitPlayers ();
	}

    [RFC]
    private void RFC_NewTurn(int turnType)
    {
        TurnMgr.Instance.StartNewTurn(turnType);

    }
}
