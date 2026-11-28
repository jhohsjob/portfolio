using System;


public static class TimeUtil
{
    public static bool IsSameDayUTC(long t1, long t2)
    {
        var d1 = DateTimeOffset.FromUnixTimeSeconds(t1).UtcDateTime.Date;
        var d2 = DateTimeOffset.FromUnixTimeSeconds(t2).UtcDateTime.Date;
        return d1 == d2;
    }
}