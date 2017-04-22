using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TurnModel
{

    public static TurnModel GetTurn(PlayEvent playEvent)
    {
        switch (playEvent)
        {
            case PlayEvent.PIECE:
                return new PieceTurn();
            //case PlayEvent.HOT:
            //    break;
            //case PlayEvent.COLD:
            //    break;
            //case PlayEvent.BEAST:
            //    break;
            //case PlayEvent.LANDSLIDE:
            //    break;
            //case PlayEvent.FLOOD:
            //    break;
            //case PlayEvent.FIRE:
            //    break;
            //case PlayEvent.EARTHQUAKE:
            //    break;
            //case PlayEvent.VOLCANO:
            //    break;
            //case PlayEvent.THUNDER:
            //    break;
            //case PlayEvent.ALIEN:
            //    break;
            //case PlayEvent.STARVE:
            //    break;
            //case PlayEvent.THEEND:
            //    break;
            default:
                return new PieceTurn();
        }
    }

    public TurnModel()
    {
        ReadyDuration = 5f;
        MoveDuration = 1f;
        CalcDuration = 3f;
    }

    /// <summary>
    /// 第几回合
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 回合事件
    /// </summary>
    public PlayEvent Event { get; set; }
    /// <summary>
    /// 准备时间
    /// </summary>
    public float ReadyDuration { get; set; }
    /// <summary>
    /// 移动时间
    /// </summary>
    public float MoveDuration { get; set; }
    /// <summary>
    /// 结算时间
    /// </summary>
    public float CalcDuration { get; set; }

}

public class PieceTurn : TurnModel
{
    public PieceTurn() 
    {
        Event = PlayEvent.PIECE;
    }
}