using System.Collections.Generic;

public class MPTeam
{
    public TeamTag t = TeamTag.A;
    public List<int> actors = new List<int>();
    public MPTeam() { }
    public MPTeam(TeamTag t, List<int> actors)
    {
        this.t = t;
        this.actors = actors;
    }
    public void FillTeamData(ref Dictionary<int, int[]> data)
    {
        if (data.ContainsKey((int)t))
            return;
        int[] actors = new int[this.actors.Count];
        for (int i = 0; i < actors.Length; i++)
        {
            actors[i] = this.actors[i];
        }
        data[(int)t] = actors;
    }
    public void CleanUp()
    {
        actors.RemoveAll(a => N_TeamsManager.GetPlayer(a) == null);
    }
}