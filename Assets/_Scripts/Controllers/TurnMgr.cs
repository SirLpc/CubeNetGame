using UnityEngine;
using System.Collections.Generic;

public class TurnMgr : MonoBehaviour
{

    public static TurnMgr Instance = null;

    private TurnModel _currentTurn;
    private TNet.List<TurnModel> _turnHistory;


	private void Awake ()
    {
        Instance = this;
        _turnHistory = new TNet.List<TurnModel>();
	}
	
	public void StartGame()
    {
        NewTurn();
    }

    public void StartNewTurn(int turnType)
    {
        _currentTurn = TurnModel.GetTurn((PlayEvent)turnType);
        _currentTurn.Index = _turnHistory.Count;
        _turnHistory.Add(_currentTurn);

        GameCtr.Instance.SetStateTo(GameState.WAITING);
        Invoke("IvToMoveState", _currentTurn.ReadyDuration);
    }

    private void IvToMoveState()
    {
        GameCtr.Instance.SetStateTo(GameState.MOVING);

        Invoke("IvToMoveState", _currentTurn.MoveDuration);
    }

    private void IvToCalcState()
    {
        GameCtr.Instance.SetStateTo(GameState.CALCING);
        //TODO calc stuffs

        Invoke("NewTurn", _currentTurn.CalcDuration);
    }


    private void NewTurn()
    {
        NetMgr.Instance.NewTurn((int)(PlayEvent.PIECE));
    }

}
