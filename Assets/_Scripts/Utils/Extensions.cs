using System;
using TNet;
using System.Text;
using System.Collections.Generic;

public static class Extensions
{

	public static TNet.List<Player> GetAllPlayers(this TNBehaviour tno)
	{
        TNet.List<Player> all = new TNet.List<Player> ();
		all.Add (TNManager.player);
		foreach (var item in TNManager.players) 
		{
			all.Add (item);
		}
		return all;
	}

    public static string ToJsonString<T, K>(this System.Collections.Generic.Dictionary<T, K> dic) where T : IConvertible where K : IConvertible
    {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("{");
		foreach (var item in dic) 
		{
			sb.AppendFormat ("{0}:{1},", item.Key.ToString (), item.Value.ToString ());
		}
		sb.Remove (sb.Length - 1, 1);
		sb.Append ("}");
		return sb.ToString();
	}

    public static System.Collections.Generic.Dictionary<int, int> ToNoramDic(this string str)
    {
        var resDic = new System.Collections.Generic.Dictionary<int, int>();

		str = str.Replace("{", string.Empty);
		str = str.Replace("}", string.Empty);

        var players = str.Split(',');
        for (int i = 0; i < players.Length; i++)
        {
            var pinfo = players[i].Split(':');
            
            resDic.Add(int.Parse(pinfo[0]), int.Parse(pinfo[1]));
        }

        return resDic;
    }

    public static void AddOrUpdate<T,K>(this System.Collections.Generic.Dictionary<T,K> oldDic, T key, K value)
    {
        if(!oldDic.ContainsKey(key))
        {
            oldDic.Add(key, value);
            return;
        }

        oldDic[key] = value;
    }


    public static  string ToJsonString<T>(this IList<T> list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        foreach (var item in list)
        {
            sb.AppendFormat("{0},", item.ToString());
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        return sb.ToString();
    }
    public static TNet.List<int> ToTNetList(this string jsonStr)
    {
        UnityEngine.Debug.Log(jsonStr);

        var resDic = new TNet.List<int>();

        jsonStr = jsonStr.Replace("{", string.Empty);
        jsonStr = jsonStr.Replace("}", string.Empty);

        var lst = jsonStr.Split(',');
        for (int i = 0; i < lst.Length; i++)
        {
            resDic.Add(int.Parse(lst[i]));
        }

        return resDic;
    }

}


