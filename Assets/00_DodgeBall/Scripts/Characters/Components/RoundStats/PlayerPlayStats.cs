using System;

[Serializable]
public class PlayerPlayStats
{
    public int passes = 0;
    public int throws = 0;
    public int revivies = 0;

    public int knocks = 0;
    public int deaths = 0;
    public string GetKD() => knocks + " : " + deaths;

    public static PlayerPlayStats operator +(PlayerPlayStats s1, PlayerPlayStats s2)
    {
        PlayerPlayStats newStats = new PlayerPlayStats();
        newStats.passes = s1.passes + s2.passes;
        newStats.throws = s1.throws + s2.throws;
        newStats.revivies = s1.revivies + s2.revivies;

        newStats.knocks = s1.knocks + s2.knocks;
        newStats.deaths = s1.deaths + s2.deaths;
        return newStats;
    }
}
