using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlanningsApp.UI.Models;
using PlanningsApp.UI.Services.Interfaces;

namespace PlanningsApp.UI.Services;

public class DepartmentService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : IDepartmentService
{
    private readonly Uri _baseUrl = new Uri(apiSettings.EmployeeBaseUrl);

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out AccessToken token))
        {
            HttpRequestMessage request = new(HttpMethod.Get, new Uri(_baseUrl, "api/departments"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            List<Department>? departments = await response.Content.ReadFromJsonAsync<
                List<Department>
            >();
            return departments ?? throw new ApplicationException("No departments found");
        }
        else
        {
            throw new ApplicationException("No authentication token found");
        }
    }
}
