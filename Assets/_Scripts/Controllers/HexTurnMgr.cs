using System.Collections.Generic;
using UnityEngine;

public class HexTurnMgr :MonoBehaviour
{
    public static HexTurnMgr Instance = null;

    private TurnModel _currentTurn;
    private TNet.List<TurnModel> _turnHistory;

    private void Awake()
    {
        Instance = this;
        _turnHistory = new TNet.List<TurnModel>();
    }

    public void NewTurnByHost()
    {
        if (_turnHistory.Count > 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                NetMgr.Instance.NewTurn((int)(PlayEvent.HOT), CalcEventCell());

            }
            else
                NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
        else
        {
            NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
    }

    public void IvDoCalcByHost()
    {
        if (_currentTurn.Event == PlayEvent.PIECE)
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
        NetMgr.Instance.SyncDamage(dmgDic);
    }

    public void StartNewTurn(int turnType, string eventCellArrStr)
    {
        InitTurnModel(turnType, eventCellArrStr);
        _turnHistory.Add(_currentTurn);

        //MapMgr.Instance.DispEffectOnGround(_currentTurn);

        NetMgr.Instance.EndTurn();
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

   


    private string CalcEventCell()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var num = new int[43];
        for (int i = 0; i < num.Length; i++)
        {
            num[i] = i;
        }
        for (int i = num.Length - 1; i >= 1; i--)
        {
            var ran = UnityEngine.Random.Range(0, i + 1);
            int temp = num[i];
            num[i] = num[ran];
            num[ran] = temp;

            sb.Append(num[i].ToString("00"));
            sb.Append(",");
        }

        return sb.Remove(sb.Length - 1, 1).ToString();
    }

}
