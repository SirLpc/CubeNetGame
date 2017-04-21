using UnityEngine;
using System.Collections;

public class TurnMgr : MonoBehaviour
{
    public static TurnMgr Instance = null;

    private int CurrentTurn;

    

	private void Awake ()
    {
        Instance = this;
	}
	
	public void StartGame()
    {

    }
}
