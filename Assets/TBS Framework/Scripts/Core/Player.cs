using UnityEngine;

public abstract class HexPlayer : MonoBehaviour
{
    public int PlayerNumber;  
    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>         
    public abstract void Play(CellGrid cellGrid);
}