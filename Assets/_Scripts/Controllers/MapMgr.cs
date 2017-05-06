using UnityEngine;
using System.Collections.Generic;
using TNet;

public class MapMgr :  TNBehaviour
{

	public static MapMgr Instance = null;

	public Color[] MapStyleColors;

	public int MapCellCount{ get; set;}
	[SerializeField]
	private string _playerPrefabName = "Player";

	private MapCell[] _mapCells;
	private Dictionary<int,int> _playersOnCellDic = null;
	private Dictionary<int,int> _cellStyleDic = null;
	private TNet.List<PlayerGo> _playerGos =null;

    public TNet.List<PlayerGo> PlayerGos { get { return _playerGos; } }
    public MapCell[] MapCells { get { return _mapCells; } }

	private void Awake()
	{
		Instance = this;

		_mapCells = GetComponentsInChildren<MapCell> ();
		MapCellCount = _mapCells.Length;
	}

	private void Start()
	{
		InitMapByHoset ();
	}

	public void InitPlayers()
	{
		var aps = this.GetAllPlayers();
		Dictionary<int, int> playerPosDic = new Dictionary<int, int>();

		var poses = new TNet.List<int>();
		foreach (var m in _cellStyleDic) {
			var style = (GroundType)m.Value;
			if (style == GroundType.CANYON || style == GroundType.HILL)
				continue;

			poses.Add (m.Key);
		}

		foreach (var p in aps)
		{
			var idx = Random.Range(0, poses.Count);

			playerPosDic.Add(p.id, poses[idx]);
			poses.Remove(idx);
		}

		tno.Send ("RFC_SyncPlayersPosInfoAndCreatePlayer", Target.All, playerPosDic.ToJsonString());
	}


	public void AddPlayerGo(PlayerGo p)
	{
		_playerGos.Add (p);
		//Debug.Log ("player go count = "+ _playerGos.Count);
	}

	public void MovePlayerToCellIfCould(MapCell cell)
	{
        //Debug.Log("move to " + cell.Index);

        tno.Send("RFC_MovePlayerToCellIfCound", Target.Host, cell.Index, TNManager.playerID);
    }

    public void DispEffectOnGround(TurnModel turn)
    {
        var evetCells = turn.EventCellArr;
        for (int i = 0; i < evetCells.Length; i++)
        {
            turn.DoEffectOnCell(_mapCells[evetCells[i]]);
        }
    }

	private void InitMapByHoset()
	{
		// Let's not try to create objects unless we are in this channel
		if (TNManager.isConnected && !TNManager.IsInChannel(GameCtr.Instance.ChannelID)) return;
		if (!TNManager.isHosting) return;

		var aps = this.GetAllPlayers ();
		var mapStyleDic = new Dictionary<int, int> ();

		var poses = new TNet.List<int>();
		for (int i = 0; i < MapMgr.Instance.MapCellCount; i++)
			poses.Add(i);

		for(int i = 0 ; i < aps.Count; i++)
		{
			var posIdx = Random.Range (0, poses.Count);
			mapStyleDic.Add (posIdx, (int)GroundType.PLAIN);
			poses.Remove (posIdx);
		}

		foreach (var p in poses)
		{
		    var ran = Random.Range(0, System.Enum.GetNames(typeof (GroundType)).Length);
            //山和峡谷不要太多
		    if (ran == (int)GroundType.CANYON || ran == (int)GroundType.HILL)
		        if (Random.Range(0, 10) > 4)
		            ran = (int) GroundType.PLAIN;
            mapStyleDic.Add (p, ran);
		}

		tno.Send ("RFC_InitMap", Target.All, mapStyleDic.ToJsonString());
	}
		
	[RFC]
	void RFC_SyncPlayersPosInfoAndCreatePlayer(string playersPosInfoDicStr)
	{
		Debug.Log (playersPosInfoDicStr);
        
        _playersOnCellDic = null;
        //var players = playersPosInfoDicStr.Split (',');
        //for (int i = 0; i < players.Length; i++) {
        //	var pinfo = players [i].Split (':');
        //	_playersPosInfoDic.Add(int.Parse(pinfo[0]), int.Parse(pinfo[1]));
        //}
        _playersOnCellDic = playersPosInfoDicStr.ToNoramDic();

		//foreach (var kvp in _playersOnCellDic)
		//{
		//	_mapCells [kvp.Value].SetPlayer (TNManager.GetPlayer(kvp.Key));
		//}
		_playerGos = new TNet.List<PlayerGo> ();

		var pos = _mapCells [_playersOnCellDic [TNManager.player.id]].GetPlayerStandPos();

		Color color = new Color(Random.value, Random.value, Random.value, 1f);
		TNManager.Instantiate(GameCtr.Instance.ChannelID, "RCC_SpawnPlayer", _playerPrefabName, true, pos, Quaternion.identity, color, TNManager.playerID);

		UICtr.Instance.HideReadyButton ();

		GameCtr.Instance.SetStateTo (GameState.WAITING);
	}

	[RCC]
	static GameObject RCC_SpawnPlayer (GameObject prefab, Vector3 pos, Quaternion rot, Color color, int playerId)
	{
		// Instantiate the prefab
		GameObject go = prefab.Instantiate();

		// Set the position and rotation based on the passed values
		Transform t = go.transform;
		t.position = pos;
		t.rotation = rot;

		go.GetComponentInChildren<Renderer>().material.color = color;

        var pgo = go.GetComponent<PlayerGo>();
        pgo.Init(playerId);
        MapMgr.Instance.AddPlayerGo (pgo);
        MapMgr.Instance._mapCells[MapMgr.Instance._playersOnCellDic[playerId]].SetPlayer(pgo);

        return go;
	}

	[RFC]
	void RFC_InitMap(string mapStr)
	{
		_cellStyleDic = null;
		_cellStyleDic = mapStr.ToNoramDic ();

		for (int i = 0; i < _mapCells.Length; i++) 
		{
			_mapCells [i].Init (i, _cellStyleDic[i]);
		}
		
		UICtr.Instance.InitMapStyleColorImg ();
	}

    [RFC]
    void RFC_MovePlayerToCellIfCound(int cellIdx, int playerId)
    {
        var playerOnCell = _mapCells[cellIdx].GetPlayerOnCell();
        if (playerOnCell != null)
            return;

        if (!TurnMgr.Instance.IsPlayerMoved(playerId))
            return;

        var lpci = MapMgr.Instance._playersOnCellDic[playerId];
        var letfOrRight = Mathf.Abs(lpci - cellIdx) == 1;
        var topOrBottom = Mathf.Abs(lpci / 10 - cellIdx / 10) == 1 && Mathf.Abs(lpci % 10 - cellIdx % 10) == 0;
        if (letfOrRight || topOrBottom)
        {
            tno.Send("RFC_MovePlayerTo", Target.All, playerId, lpci, cellIdx);
        }
    }

    [RFC]
    void RFC_MovePlayerTo(int playerId, int fromCellIdx, int toCellIdx)
    {
        foreach (var p in _playerGos)
        {
            if (p.PlayerId != playerId)
                continue;

            _mapCells[fromCellIdx].RemovePlayer();
            _mapCells[toCellIdx].SetPlayer(p);
            _playersOnCellDic[playerId] = toCellIdx;
            p.MoveTo(_mapCells[toCellIdx]);

            TurnMgr.Instance.AddMovedPlayer(playerId);
        }
    }
}
