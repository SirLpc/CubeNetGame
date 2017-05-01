using UnityEngine;
using System.Collections;
using TNet;

public class GameCtr : MonoBehaviour
{

	private int channelID = 0;

	public static GameCtr Instance = null;

    public CellGrid _cellGrid;
    public GuiController _guiController;

    public GameObject _globalMaskGo;

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

        //GOCreator.Instance.CreateMapByHost ();

		yield return null;

		NetMgr.Instance.Init ();

		//UICtr.Instance.ShowReadyButton ();
        _guiController.ShowReadyButton();

        //Before waiting...
        while (CurrentState == GameState.READY)
        {
            yield return null;
        }

	    if (TNManager.isHosting)
	    {
	        yield return null;
            NetMgr.Instance.StartGame();
	    }

        //游戏结束判定
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if(TNManager.isHosting)
            {
                NetMgr.Instance.EndTurn();
            }
        }
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void SetStateTo(GameState state)
	{
		CurrentState = state;
	}

    public void StartGame()
    {
        //TurnMgr.Instance.StartGame();
        _globalMaskGo.SetActive(false);
    }


    private void OnGUI()
    {
        GUILayout.Label(string.Format("\t\t===Game state:{0}==isHost:{1}===", CurrentState,TNManager.isHosting));
    }
}
