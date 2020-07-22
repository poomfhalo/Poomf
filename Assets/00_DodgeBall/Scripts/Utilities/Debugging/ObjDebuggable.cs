public class ObjDebuggable : GameDebuggable
{
    public override void SetActivity(bool toState)
    {
        gameObject.SetActive(toState);
    }
}