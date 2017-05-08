using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class CellGrid : MonoBehaviour
{
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;
    
    private CellGridState _cellGridState;//The grid delegates some of its behaviours to cellGridState object.
    public CellGridState CellGridState
    {
        private get
        {
            return _cellGridState;
        }
        set
        {
            if(_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public HexPlayer CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get { return NetMgr.Instance.HexPlayerId; } }

    public Transform PlayersParent;

    public List<HexPlayer> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }

    private Unit _myUnit;

    void Start()
    {
        Players = new List<HexPlayer>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<HexPlayer>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;
        //CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);

        Cells = new List<Cell>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
            {
                cell.Id = i;
                Cells.Add(cell);
            }
            else
                Debug.LogError("Invalid object in cells paretn game object");
        }
      
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }
             
        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(Cells);
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");

        //StartGame();
    }

    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as Cell);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        CellGridState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        _myUnit = Units.Find(u => u.PlayerNumber.Equals(CurrentPlayerNumber));

        if(GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        EnablePlayerAndUnit();
    }
    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        CellGridState = new CellGridStateTurnChanging(this);

        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });
        foreach (var u in Units)
        {
            u.OnTurnStart();
        }

        //CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        //while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        //{
        //    CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
        //}//Skipping players that are defeated.

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());

        EnablePlayerAndUnit();
    }

    public void EnablePlayerAndUnit()
    {
        //Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });

        foreach (var u in Units)
        {
            u.OnTurnStart();
        }

        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        FocusMyUnit();
    }


    public void MoveUnit(int unitId, int destCellId, string pathIdListStr)
    {
        var unit = Units.Find(u => u.Id == unitId);
        var destCell = Cells.Find(c => c.Id == destCellId);
        if(unit == null || destCell == null)
        {
            Debug.LogWarning("unit or cell == null");
            return;
        }
        TNet.List<int> pathIds = pathIdListStr.ToTNetList();
        List<Cell> pathCells = new List<Cell>();
        for (int i = 0; i < pathIds.Count; i++)
        {
            var ce = Cells.Find(c => c.Id == pathIds[i]);
            if (ce)
                pathCells.Add(ce);
        }

        unit.Move(destCell, pathCells);

        FocusMyUnit();
    }

    public void FocusMyUnit()
    {
        CellGridState = new CellGridStateUnitSelected(this, _myUnit);
        OnUnitClicked(_myUnit, new EventArgs());
    }

   
}
