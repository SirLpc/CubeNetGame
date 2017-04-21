using UnityEngine;
using System.Collections;
using TNet;

public class MapCell : MonoBehaviour 
{
	private Material _mat;
	private int _index;
    private MapStyle _style;

	private PlayerGo _player = null;

    public int Index { get { return _index; } }

	private void Awake()
	{
		_mat = GetComponent<Renderer> ().material;
	}


	public void Init(int index, int styleEnumIdx)
	{
		_index = index;

        //		var c =(_index / 10) % 2 == 0 ? 
        //			_index % 2 == 0 ? Color.red:Color.blue 
        //			: _index % 2 == 0 ? Color.blue:Color.red;

        _style = (MapStyle)styleEnumIdx;

		var c = MapMgr.Instance.MapStyleColors [styleEnumIdx];
		_mat.color = c;
	}

	public void SetPlayer(PlayerGo player)
	{
		_player = player;
	}

    public void RemovePlayer()
    {
        _player = null;
    }

    public PlayerGo GetPlayerOnCell()
    {
        return _player;
    }

    public Vector3 GetPlayerStandPos()
    {
        return transform.position + Vector3.up;
    }

    private void MovePlayerOnClick()
    {
        if (GameCtr.Instance.CurrentState != GameState.MOVING)
            return;

        if (_style == MapStyle.CANYON || _style == MapStyle.HILL)
            return;

        MapMgr.Instance.MovePlayerToCellIfCould(this);
    }

	private void OnClick()
	{
        MovePlayerOnClick();
    }
}
