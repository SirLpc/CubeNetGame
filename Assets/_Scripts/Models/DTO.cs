using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class DTO_SyncDamageByEvent
{
    public int UnitId;
    public PlayEvent Evnt;
    public int DamageHealth;
    public DTO_SyncDamageByEvent(int unitId, PlayEvent evnt, int damageHealth)
    {
        this.UnitId = unitId;
        this.Evnt = evnt;
        this.DamageHealth = damageHealth;
    }
}

