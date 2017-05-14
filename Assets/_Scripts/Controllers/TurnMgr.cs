using UnityEngine;
using System.Collections.Generic;
using TNet;

public class TurnMgr : MonoBehaviour
{

    public static TurnMgr Instance = null;

    private TurnModel _currentTurn;
    private TNet.List<TurnModel> _turnHistory;
    /// <summary>
    /// 保证一个玩家一回合只能走一次
    /// </summary>
    private TNet.List<int> _movedPlayers;


	private void Awake ()
    {
        Instance = this;
        _turnHistory = new TNet.List<TurnModel>();
        _movedPlayers = new TNet.List<int>();
	}
	
	public void StartGame()
    {
        NewTurnByHost();
    }

    public void StartNewTurn(int turnType, string eventCellArrStr)
    {
        InitTurnModel(turnType, eventCellArrStr);
        _turnHistory.Add(_currentTurn);

        MapMgr.Instance.DispEffectOnGround(_currentTurn);

        _movedPlayers.Clear();
        GameCtr.Instance.SetStateTo(GameState.WAITING);
        Invoke("IvToMoveState", _currentTurn.ReadyDuration);
    }


    public void AddMovedPlayer(int playerId)
    {
        _movedPlayers.Add(playerId);
    }

    public bool IsPlayerMoved(int playerId)
    {
        return !_movedPlayers.Contains(playerId);
    }

    private void IvToMoveState()
    {
        GameCtr.Instance.SetStateTo(GameState.MOVING);

        Invoke("IvToCalcState", _currentTurn.MoveDuration);
    }

    private void IvToCalcState()
    {
        GameCtr.Instance.SetStateTo(GameState.CALCING);

        if(TNManager.isHosting)
        {
            //等0.5S，以防止有人没移动完成数据同步
            Invoke("IvDoCalcByHost", .5f);
        }
    }

    private void InitTurnModel(int turnType, string eventCellArrStr)
    {
        _currentTurn = TurnModel.GetTurn((PlayEvent)turnType);
        _currentTurn.Index = _turnHistory.Count;
        int[] eca = new int[_currentTurn.Range];
        var cs = eventCellArrStr.Split(',');
        for (int i = 0; i < eca.Length; i++)
            eca[i] = int.Parse(cs[i]);
        _currentTurn.EventCellArr = eca;
    }


    private void NewTurnByHost()
    {
        if(_turnHistory.Count > 0)
        {
            if (Random.Range(0,2) == 0)
            {
                NetMgr.Instance.NewTurn((int)(PlayEvent.STARVE), CalcEventCell());
            }
            else
                NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
        else
        {
            NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
    }

    private void IvDoCalcByHost()
    {
        if(_currentTurn.Event == PlayEvent.PIECE)
        {
            Invoke("NewTurnByHost", _currentTurn.CalcDuration);
            return;
        }

        var cells = MapMgr.Instance.MapCells;
        var willDmgPlayers = new TNet.List<int>();
        for (int i = 0; i < _currentTurn.EventCellArr.Length; i++)
        {
            var p = cells[_currentTurn.EventCellArr[i]].GetPlayerOnCell();
            if (p != null)
            {
                willDmgPlayers.Add(p.PlayerId);
            }
        }
        Dictionary<int, int> dmgDic = new Dictionary<int, int>();
        foreach (var pid in willDmgPlayers)
        {
            dmgDic.Add(pid, _currentTurn.GetDamageNum());
        }
        //NetMgr.Instance.SyncDamage(dmgDic);

        Invoke("NewTurnByHost", _currentTurn.CalcDuration);
    }





    private string CalcEventCell()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var num = new int[MapMgr.Instance.MapCellCount];
        for (int i = 0; i < num.Length; i++)
        {
            num[i] = i;
        }
        for (int i = num.Length - 1; i >= 1; i--)
        {
            var ran = UnityEngine. Random.Range(0, i + 1);
            int temp = num[i];
            num[i] = num[ran];
            num[ran] = temp;

            sb.Append(num[i].ToString("00"));
            sb.Append(",");
        }

        return sb.Remove(sb.Length - 1, 1).ToString();
    }

}
