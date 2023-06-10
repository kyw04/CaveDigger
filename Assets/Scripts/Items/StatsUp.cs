using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsUp: Item
{
    public Stats upStats;

    public override void RunItem()
    {
        stats.Zero();
        stats.Add(upStats);
    }
}
