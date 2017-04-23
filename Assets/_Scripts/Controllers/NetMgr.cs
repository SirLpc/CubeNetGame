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

    public void NewTurn(int turnType, string eventCellList)
    {
        tno.Send("RFC_NewTurn", Target.All, turnType, eventCellList);
    }

    public void SyncDamage(Dictionary<int,int> playerDmgDic)
    {
        if (playerDmgDic.Count < 1)
            return;

        var hd = playerDmgDic.ToJsonString();

        tno.Send("RFC_SyncDamage", Target.All, hd);
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
    private void RFC_NewTurn(int turnType, string eventCellList)
    {

        TurnMgr.Instance.StartNewTurn(turnType, eventCellList);
    }

    [RFC]
    private void RFC_SyncDamage(string damageDicStr)
    {
        var pgos = MapMgr.Instance.PlayerGos;
        var healthDic = damageDicStr.ToNoramDic();
        foreach (var p in pgos)
        {
            if (healthDic.ContainsKey(p.PlayerId))
                p.Damage( healthDic[p.PlayerId]);
        }
    }
}
