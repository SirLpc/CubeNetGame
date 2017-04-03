using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UICtr : MonoBehaviour
{

	public static UICtr Instance = null;

	[SerializeField] private Button _readyButton;
	[SerializeField] private RawImage[] _mapStypeColorImg;

	private void Awake()
	{
		Instance = this;
		_readyButton.gameObject.SetActive (false);
	}

	private void Start()
	{
		_readyButton.onClick.AddListener (OnClickReady);
	}

	private void OnDestroy()
	{
		_readyButton.onClick.RemoveListener (OnClickReady);
	}

	public void ShowReadyButton()
	{
		_readyButton.gameObject.SetActive (true);
	}

	public void HideReadyButton()
	{
		_readyButton.gameObject.SetActive (false);
	}

	public void InitMapStyleColorImg()
	{
		for (int i = 0; i < _mapStypeColorImg.Length; i++) {
			_mapStypeColorImg [i].color = MapMgr.Instance.MapStyleColors [i];
		}
	}

	private void OnClickReady()
	{
		NetMgr.Instance.Ready ();
	}

}
