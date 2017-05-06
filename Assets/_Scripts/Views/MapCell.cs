using UnityEngine;
using System.Collections;
using TNet;
using DG.Tweening;

public class MapCell : MonoBehaviour
{
    private Material _mat;
    private int _index;
    private GroundType _style;
    private Color _originColor;

    private PlayerGo _player = null;

    private Tweener _flashTweener = null;

    public int Index { get { return _index; } }

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
    }

    public void OnDestroy()
    {
        if (_flashTweener != null)
            _flashTweener.Kill();
    }

    public void Init(int index, int styleEnumIdx)
    {
        _index = index;

        //		var c =(_index / 10) % 2 == 0 ? 
        //			_index % 2 == 0 ? Color.red:Color.blue 
        //			: _index % 2 == 0 ? Color.blue:Color.red;

        _style = (GroundType)styleEnumIdx;

        _originColor = MapMgr.Instance.MapStyleColors[styleEnumIdx];
        _mat.color = _originColor;
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

    public void FlashColor(float duration)
    {
        int loopCount = (int)(duration / .5f);
        _mat.DOColor(Color.red, .5f).SetEase(Ease.Linear).SetLoops(loopCount, LoopType.Yoyo).OnComplete(()=>
        {
            _mat.color = _originColor;
        });
    }

    private void MovePlayerOnClick()
    {
        if (GameCtr.Instance.CurrentState != GameState.MOVING)
            return;

        if (_style == GroundType.CANYON || _style == GroundType.HILL)
            return;

        MapMgr.Instance.MovePlayerToCellIfCould(this);
    }

    private void OnClick()
    {
        MovePlayerOnClick();
    }
}
