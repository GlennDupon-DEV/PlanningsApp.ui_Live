using System;

namespace PlanningsApp.Ui.Models;

public class OidcSettings
{
    public string Authority { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;
    public string PostLogoutRedirectUri { get; set; } = default!;
    public string ResponseType { get; set; } = default!;
    public List<string> DefaultScopes { get; set; } = new();
}
