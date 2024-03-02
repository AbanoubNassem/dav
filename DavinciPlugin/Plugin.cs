using AbanoubNassem.Trinity.Plugins;

namespace DavinciPlugin;

public class DavinciPlugin : TrinityPlugin
{
    public override List<string> GetScriptSources()
    {
        return new List<string>()
        {
            "/main.js"
        };
    }

    public override List<string> GetStyleSources()
    {
        return new List<string>()
        {
            "/style.css"
        };
    }
}