using UnityEngine;
using System.Collections;
using TNet;

public class PlayerGo : TNBehaviour
{

    public int PlayerId { get; private set; }
    public int Health { get; private set; }

    public GameObject _meLabel;

    public void Init(int playerId)
    {
        PlayerId = playerId;

        Health = 100;

        _meLabel.SetActive(tno.isMine);
    }

    public void MoveTo(MapCell mapCell)
    {
        transform.position = mapCell.GetPlayerStandPos();
    }

    public void Damage(int dmg)
    {
        Health -= dmg;
    }


    public void SetHealth(int health)
    {
        Health = health;
    }

    private void OnGUI()
    {
        if (!tno.isMine)
            return;

        GUI.color = Color.red;

        var HpInfoStr = "\n\t\t";
        foreach (var item in MapMgr.Instance.PlayerGos)
        {
            HpInfoStr += string.Format("[ID:{0}{2},HP:{1}] ", item.PlayerId, 
                item.Health > 0 ? item.Health.ToString() : "Dead",
                item.PlayerId == TNManager.playerID ? "Me":"");
        }
        GUILayout.Label(HpInfoStr);
    }
}
