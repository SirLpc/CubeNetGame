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
            NetMgr.Instance.NewTurn((int)(PlayEvent.HOT), CalcEventCell());
            //if (Random.Range(0, 1) == 0)
            //    NetMgr.Instance.NewTurn((int)(PlayEvent.HOT), CalcEventCell());
            //else
            //    NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
        else
        {
            NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE), string.Empty);
        }
    }

    public void DoCalcByHost()
    {
        if (_currentTurn == null)
            return;
        if (_currentTurn.Event == PlayEvent.PIECE)
            return;

        var units = GameCtr.Instance.CellGrid.Units;
        var willDmgPlayers = new System.Collections.Generic.List<DTO_SyncDamageByEvent>();
        for (int i = 0; i < units.Count; i++)
        {
            var t = (units[i].Cell as MyHexagon).GroundType;
            if (_currentTurn.WillDamageGroundType.Contains(t))
            {
                DTO_SyncDamageByEvent dto = new DTO_SyncDamageByEvent(
                    units[i].Id, _currentTurn.Event, _currentTurn.GetDamageNum());
                willDmgPlayers.Add(dto);
            }
        }
        if (willDmgPlayers.Count <= 0)
            return;
   
        NetMgr.Instance.SyncDamage(willDmgPlayers);
    }

    public void StartNewTurn(int turnType, string eventCellArrStr)
    {
        _currentTurn = InitTurnModel(turnType, eventCellArrStr);
        _turnHistory.Add(_currentTurn);
        //MapMgr.Instance.DispEffectOnGround(_currentTurn);

        GameCtr.Instance.CellGrid.EndTurn();
        //NetMgr.Instance.EndTurn();
    }

   

    private TurnModel InitTurnModel(int turnType, string eventCellArrStr)
    {
        var t = TurnModel.GetTurn((PlayEvent)turnType);
        t.Index = _turnHistory.Count;
        int[] eca = new int[t.Range];
        var cs = eventCellArrStr.Split(',');
        for (int i = 0; i < eca.Length; i++)
            eca[i] = int.Parse(cs[i]);
        t.EventCellArr = eca;
        return t;
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
