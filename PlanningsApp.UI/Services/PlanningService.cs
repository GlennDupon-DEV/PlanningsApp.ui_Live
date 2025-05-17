using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlanningsApp.Ui.Mapper;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;
using PlanningsApp.Ui.Services.Interfaces;

namespace PlanningsApp.Ui.Services;

public class PlanningService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : IPlanningService
{
    private readonly Uri _baseUrl = new(apiSettings.PlanningBaseUrl);

    public async Task<DepartmentPlanning> CreatePlanningAsync(DepartmentPlanning planning)
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                string json = JsonSerializer.Serialize(planning);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new(
                    HttpMethod.Post,
                    new Uri(_baseUrl, "api/planning")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );
                request.Content = content;

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                DepartmentPlanning? planningResponse =
                    await response.Content.ReadFromJsonAsync<DepartmentPlanning>();
                return planningResponse
                    ?? throw new ApplicationException("Failed to create planning");
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating planning: {ex.Message}");
            throw;
        }
    }

    public async Task UpdatePlanningAsync(DepartmentPlanning planning)
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                string json = JsonSerializer.Serialize(planning);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new(
                    HttpMethod.Patch,
                    new Uri(_baseUrl, "api/planning")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );
                request.Content = content;

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                await response.Content.ReadFromJsonAsync<DepartmentPlanning>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while updating planning: {ex.Message}");
            throw;
        }
    }

    public async Task<DepartmentPlanning?> GetPlanningAsync(
        string departmentName,
        int year,
        int week
    )
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                UriBuilder builder = new($"{_baseUrl}api/planning");
                NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
                query["department"] = departmentName;
                query["year"] = year.ToString();
                query["week"] = week.ToString();
                builder.Query = query.ToString();

                HttpRequestMessage request = new(HttpMethod.Get, new Uri(builder.ToString()));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                DepartmentPlanning? planningResponse =
                    await response.Content.ReadFromJsonAsync<DepartmentPlanning>();
                return planningResponse;
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching planning: {ex.Message}");
            return null;
        }
    }
}
