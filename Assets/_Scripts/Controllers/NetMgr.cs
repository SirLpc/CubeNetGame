using UnityEngine;
using System.Collections.Generic;
using TNet;
using System.Collections;

public class NetMgr : TNBehaviour
{

    public static NetMgr Instance = null;

    public CellGrid _cellGrid;

    private int _hexPlayerId = -1;
    public int HexPlayerId {
        get {
            if(_hexPlayerId == -1)
            {
                var ap = this.GetAllPlayers();
                ap.Sort((p1, p2) => p2.id - p1.id);
                _hexPlayerId = ap.IndexOf(TNManager.player);
            }
            return _hexPlayerId;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
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

    //called by ui onclick
    public void Ready()
    {
        TNManager.SetPlayerData("isReady", true);
        Debug.Log(TNManager.players.Count);
        tno.Send("RFC_ReadyGame", Target.Host);
    }

    public void NewTurn(int turnType, string eventCellList)
    {
        tno.Send("RFC_NewTurn", Target.All, turnType, eventCellList);
    }

    public void SyncDamage(Dictionary<int, int> playerDmgDic)
    {
        if (playerDmgDic.Count < 1)
            return;

        var hd = playerDmgDic.ToJsonString();

        tno.Send("RFC_SyncDamage", Target.All, hd);
    }

    public void MoveUnitToCell(int unitId, int destCellId, string pathIdListStr)
    {
        float wait = TNManager.isHosting ? Random.Range(0.03f, 0.08f) : 0f;
        StartCoroutine(CoMoveUnitToCell(unitId, destCellId, pathIdListStr, wait));
    }

    private IEnumerator CoMoveUnitToCell(int unitId, int destCellId, string pathIdListStr, float wait)
    {
        yield return new WaitForSeconds(wait);
        tno.Send("RFC_MoveUnitToCell_HostCheck", Target.Host, unitId, destCellId, pathIdListStr);
    }

    public void EndTurn()
    {
        tno.Send("RFC_EndTurn", Target.All);
    }

    public void StartGame()
    {
        tno.Send("RFC_StartGame", Target.All);
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

        //MapMgr.Instance.InitPlayers ();

        GameCtr.Instance.SetStateTo(GameState.WAITING);
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
                p.Damage(healthDic[p.PlayerId]);
        }
    }

    [RFC]
    void RFC_MoveUnitToCell_HostCheck(int unitId, int destCellId, string pathIdListStr)
    {
        var destCell = _cellGrid.Cells.Find(c => c.Id == destCellId);
        if (!destCell.IsTaken)
        {
            destCell.IsTaken = true;
            tno.Send("RFC_MoveUnitToCell", Target.All, unitId, destCellId, pathIdListStr);
        }
    }

    [RFC]
    void RFC_MoveUnitToCell(int unitId, int destCellId, string pathIdListStr)
    {
        _cellGrid.MoveUnit(unitId, destCellId, pathIdListStr);
    }

    [RFC]
    void RFC_EndTurn()
    {
        _cellGrid.EndTurn();
    }

    [RFC]
    void RFC_StartGame()
    {
        _cellGrid.StartGame();
        GameCtr.Instance.StartGame();
    }



}
