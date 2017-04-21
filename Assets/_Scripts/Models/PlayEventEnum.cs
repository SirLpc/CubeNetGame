

public enum PlayEventEnum
{
    /// <summary>
    /// 宁静
    /// </summary>
    PIECE   =   0,
    /// <summary>
    /// 炎热（不在河与树林中的人都掉血）
    /// </summary>
    HOT =   1,
    /// <summary>
    /// 严寒（山洞寒冷天气不掉血)
    /// </summary>
    COLD =   2,
    /// <summary>
    /// 野兽出没（地图上生成一些会移动的野兽，一定不会到达山洞）
    /// </summary>
    BEAST =   3,
    /// <summary>
    /// 山体滑坡（山地或峡谷附近人掉血）
    /// </summary>
    LANDSLIDE =   4,
    /// <summary>
    /// 洪水（河流附近敌人掉血）
    /// </summary>
    FLOOD =   5,
    /// <summary>
    /// 火灾（树林掉血）
    /// </summary>
    FIRE =   6,

    /// <summary>
    /// 地震（随机地图上小区域掉血）
    /// </summary>
    EARTHQUAKE =   7,
    /// <summary>
    /// 火山喷发（随机地图上小区域掉血）
    /// </summary>
    VOLCANO =   8,
    /// <summary>
    /// 雷电（随机地图上小区域掉血）
    /// </summary>
    THUNDER =   9,
    /// <summary>
    /// 外星人入侵（随机中区域掉血）
    /// </summary>
    ALIEN =   10,
    /// <summary>
    /// 饥荒（随机全地图掉血）
    /// </summary>
    STARVE =   11,
    /// <summary>
    /// 世界末日（全地图超大范围掉血）
    /// </summary>
    THEEND =   12,

}

