using UnityEngine;
using System.Collections;

public class MapCell : MonoBehaviour 
{
	private Material _mat;
	private int _index;

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

	private void OnClick()
	{
		Debug.Log (_index + gameObject.name);
	}
}
