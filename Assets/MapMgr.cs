using UnityEngine;
using System.Collections.Generic;
using TNet;

public class MapMgr :  TNBehaviour
{

	public static MapMgr Instance = null;

	public Color[] MapStyleColors;

	public int MapCellCount{ get; set;}
	[SerializeField]
	private string _playerPrefabName = "PlayerPrefabName";

	private MapCell[] _mapCells;
	private Dictionary<int,int> _playersPosDic = null;
	private Dictionary<int,int> _mapStyleDic = null;

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
		foreach (var m in _mapStyleDic) {
			var style = (MapStyle)m.Value;
			if (style == MapStyle.CANYON || style == MapStyle.HILL)
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
			mapStyleDic.Add (posIdx, (int)MapStyle.PLAIN);
			poses.Remove (posIdx);
		}

		foreach (var p in poses)
		{
			mapStyleDic.Add (p, Random.Range(0, System.Enum.GetNames(typeof(MapStyle)).Length));
		}

		tno.Send ("RFC_InitMap", Target.All, mapStyleDic.ToJsonString());
	}
		
	[RFC]
	void RFC_SyncPlayersPosInfoAndCreatePlayer(string playersPosInfoDicStr)
	{
		Debug.Log (playersPosInfoDicStr);

		UICtr.Instance.HideReadyButton ();

        _playersPosDic = null;
        //var players = playersPosInfoDicStr.Split (',');
        //for (int i = 0; i < players.Length; i++) {
        //	var pinfo = players [i].Split (':');
        //	_playersPosInfoDic.Add(int.Parse(pinfo[0]), int.Parse(pinfo[1]));
        //}
        _playersPosDic = playersPosInfoDicStr.ToNoramDic();

		Debug.Log (_playersPosDic.Count);

		var pos = _mapCells [_playersPosDic [TNManager.player.id]].transform.position + Vector3.up;

		Color color = new Color(Random.value, Random.value, Random.value, 1f);

		TNManager.Instantiate(GameCtr.Instance.ChannelID, "RCC_SpawnPlayer", _playerPrefabName, true, pos, Quaternion.identity, color);
	}

	[RCC]
	static GameObject RCC_SpawnPlayer (GameObject prefab, Vector3 pos, Quaternion rot, Color color)
	{
		// Instantiate the prefab
		GameObject go = prefab.Instantiate();

		// Set the position and rotation based on the passed values
		Transform t = go.transform;
		t.position = pos;
		t.rotation = rot;

		go.GetComponentInChildren<Renderer>().material.color = color;

		return go;
	}

	[RFC]
	void RFC_InitMap(string mapStr)
	{
		_mapStyleDic = null;
		_mapStyleDic = mapStr.ToNoramDic ();

		for (int i = 0; i < _mapCells.Length; i++) 
		{
			_mapCells [i].Init (i, _mapStyleDic[i]);
		}


		UICtr.Instance.InitMapStyleColorImg ();
	}
}
