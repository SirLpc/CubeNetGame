using UnityEngine;
using System.Collections;
using TNet;

public class GameCtr : MonoBehaviour
{

	private int channelID = 0;

	public static GameCtr Instance = null;

	/// <summary>
	/// Channel ID is not required for TNManager.Instantiate calls, however if you are working with
	/// multiple channels, you will want to pass which channel you want the object to be created in.
	/// </summary>
	public int ChannelID {get{ return channelID;}}

	public GameState CurrentState { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private IEnumerator Start () 
	{
		if (channelID < 1) channelID = TNManager.lastChannelID; 

		yield return null;

		SetStateTo (GameState.READY);

		GOCreator.Instance.CreateMapByHost ();

		yield return null;

		NetMgr.Instance.Init ();

		UICtr.Instance.ShowReadyButton ();

        //Before waiting...
        while (CurrentState == GameState.READY)
        {
            yield return null;
        }

	    if (TNManager.isHosting)
	    {
	        yield return null;
            StartGame();
	    }
	}
	
	public void SetStateTo(GameState state)
	{
		CurrentState = state;
	}

    private void StartGame()
    {
        Debug.Log("start");
        SetStateTo(GameState.MOVING);
    }
}
