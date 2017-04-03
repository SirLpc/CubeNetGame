using UnityEngine;
using System.Collections;
using TNet;

public class MapCell : MonoBehaviour 
{
	private Material _mat;
	private int _index;

	private Player _player = null;

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

		var c = MapMgr.Instance.MapStyleColors [styleEnumIdx];

		_mat.color = c;
	}

	public void SetPlayer(Player player)
	{
		_player = player;
	}

	private void OnClick()
	{
		if (GameCtr.Instance.CurrentState != GameState.PLAYING)
			return;

		if (_player != null) {
			Debug.Log ("Already has player at " + _index);
			return;
		}

		var lpci = MapMgr.Instance.GetLocalPlayerCellIdx ();
		if (Mathf.Abs (lpci - _index) != 1 && Mathf.Abs (lpci / 10 - _index / 10) != 1) {
			Debug.Log ("The cell not close player");
			return;
		}

		MapMgr.Instance.MovePlayerTo (this);
	}
}
