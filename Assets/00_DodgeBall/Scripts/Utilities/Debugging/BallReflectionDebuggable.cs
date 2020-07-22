public class BallReflectionDebuggable : GameDebuggable
{
    public override void SetActivity(bool toState)
    {
        GetComponent<DodgeballReflection>().MakesLogSpheres = toState;
        GetComponent<DodgeballReflection>().loggedSpheres.ForEach(s => s.SetActive(toState));
    }
}