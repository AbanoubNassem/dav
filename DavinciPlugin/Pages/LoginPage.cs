using AbanoubNassem.Trinity.Pages;
using Dapper;

namespace DavinciPlugin.Pages;

public class LoginPage : TrinityPage
{
    public override string Slug => "Login";

    public override string PageView => "Login";

    public override bool CanView => !User.Identity?.IsAuthenticated ?? true;
    
}