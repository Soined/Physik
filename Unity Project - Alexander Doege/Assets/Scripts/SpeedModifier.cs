using System.Linq;

public class SpeedModifier : PhysicsModifier
{
    private void OnEnable()
    {
        var controllers = GetComponents<ControllerBase>().ToList();

        controllers.ForEach(c => c.RegisterModifier(this));
    }

    private void OnDisable()
    {
        var controllers = GetComponents<ControllerBase>().ToList();

        controllers.ForEach(c => c.UnregisterModifier(this));
    }
}
