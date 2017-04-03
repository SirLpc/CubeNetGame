//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2016 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using TNet;

/// <summary>
/// This script shows how to create objects dynamically over the network.
/// The same Instantiate call will work perfectly fine even if you're not currently connected.
/// This script is attached to the floor in Example 2.
/// </summary>

public class GOCreator : MonoBehaviour
{
	public string prefabName = "Created Cube";

//	public int mapSizeX = 10;
//	public int mapSizeY = 10;

	public static GOCreator Instance = null;

	private void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Create a new object above the clicked position
	/// </summary>

	public void CreateMapByHost ()
	{
		// Let's not try to create objects unless we are in this channel
		if (TNManager.isConnected && !TNManager.IsInChannel(GameCtr.Instance.ChannelID)) return;
		if (!TNManager.isHosting) return;

		TNManager.Instantiate(GameCtr.Instance.ChannelID, "RCC_CreateMap", prefabName, true, Vector3.zero, Quaternion.identity);

//		var halfX = mapSizeX / 2;
//		var halfY = mapSizeY / 2;
//		for (int i = -1*halfX; i < halfX; i++) 
//		{
//			for (int j = -1*halfY; j < halfY; j++) 
//			{
//				// Object's position will be up in the air so that it can fall down
//				Vector3 pos = new Vector3(i + .5f, 0, j + 0.5f);
//
//				// Object's rotation is completely random
//				Quaternion rot = Quaternion.identity;
//
//				// Object's color is completely random
//				Color color = new Color(Random.value, Random.value, Random.value, 1f);
//
//				// Create the object using a custom creation function defined below.
//				// Note that passing "channelID" is optional. If you don't pass anything, TNet will pick one for you.
//				TNManager.Instantiate(GameCtr.Instance.ChannelID, "RCC_CreateMap", prefabName, true, pos, rot, color);
//			}
//		}
	}

	/// <summary>
	/// RCCs (Remote Creation Calls) allow you to pass arbitrary amount of parameters to the object you are creating.
	/// TNManager will call this function, passing a prefab to it that you should then instantiate.
	/// </summary>

	[RCC]
	static GameObject RCC_CreateMap (GameObject prefab, Vector3 pos, Quaternion rot)
	{
		// Instantiate the prefab
		GameObject go = prefab.Instantiate();

		// Set the position and rotation based on the passed values
		Transform t = go.transform;
		t.position = pos;
		t.rotation = rot;

		return go;
	}
}
