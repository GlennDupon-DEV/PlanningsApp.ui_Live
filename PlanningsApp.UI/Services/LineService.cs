using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlanningsApp.Ui.Mapper;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;
using PlanningsApp.Ui.Services.Interfaces;

namespace PlanningsApp.Ui.Services;

public class LineService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : ILineService
{
    private readonly Uri _baseUrl = new(apiSettings.EmployeeBaseUrl);

    public async Task<List<PlanningLine>> GetLinesByDepartmentNameAsync(string department)
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                UriBuilder uriBuilder = new UriBuilder($"{_baseUrl}api/lines");
                NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["department"] = department;
                uriBuilder.Query = query.ToString();

                HttpRequestMessage request = new(HttpMethod.Get, new Uri(uriBuilder.ToString()));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<Line>? lines = await response.Content.ReadFromJsonAsync<List<Line>>();
                return lines?.Map() ?? new List<PlanningLine>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while fetching lines: {ex.Message}");
            return new List<PlanningLine>();
        }
    }
}
