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
            case PlayEvent.HOT:
                return new HotTurn();
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
            case PlayEvent.STARVE:
                return new StarveTurn();
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
    /// <summary>
    /// 范围，度量为格子
    /// </summary>
    public int Range { get; set; }
    /// <summary>
    /// 事件发生格子的数组
    /// </summary>
    public int[] EventCellArr { get; set; }

    public virtual void DoEffectOnCell(MapCell cell) { }
    public virtual int GetDamageNum() { return 0; }
    public List<GroundType> WillDamageGroundType { get; protected set; }
}

public class PieceTurn : TurnModel
{
    public PieceTurn() 
    {
        Event = PlayEvent.PIECE;

        Range = 0;
    }
}

public class HotTurn : TurnModel
{
    public HotTurn()
    {
        Event = PlayEvent.HOT;

        WillDamageGroundType = new List<GroundType>();
        WillDamageGroundType.Add(GroundType.PLAIN);
    }

    public override int GetDamageNum()
    {
        Random ran = new Random();
        return ran.Next(20, 27);
    }


}

public class StarveTurn : TurnModel
{
    public StarveTurn()
    {
        Event = PlayEvent.STARVE;

        Range = (int)(MapMgr.Instance.MapCellCount * .7f);
    }

    public override void DoEffectOnCell(MapCell cell)
    {
        cell.FlashColor(ReadyDuration);
    }

    public override int GetDamageNum()
    {
        Random ran = new Random();
        return ran.Next(10, 30);
    }
}