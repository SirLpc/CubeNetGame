using UnityEngine;
using System.Collections;
using TNet;

public class PlayerGo : TNBehaviour
{

    public int PlayerId { get; private set; }

    public void Init(int playerId)
    {
        PlayerId = playerId;
    }

    public void MoveTo(MapCell mapCell)
    {
        transform.position = mapCell.GetPlayerStandPos();
    }

}
