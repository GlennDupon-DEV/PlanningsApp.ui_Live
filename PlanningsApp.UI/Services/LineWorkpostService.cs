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

public class LineWorkpostService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : ILineWorkpostService
{
    private readonly Uri _baseUrl = new(apiSettings.EmployeeBaseUrl);

    public async Task<List<PlanningWorkPost>> GetLineWorkpostsByDepartmentName(
        string departmentName
    )
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                UriBuilder uriBuilder = new UriBuilder($"{_baseUrl}api/lineworkposts");
                NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["department"] = departmentName;
                uriBuilder.Query = query.ToString();

                HttpRequestMessage request = new(HttpMethod.Get, new Uri(uriBuilder.ToString()));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<LineWorkpost>? linesWorkposts = await response.Content.ReadFromJsonAsync<
                    List<LineWorkpost>
                >();
                return linesWorkposts?.Map() ?? new List<PlanningWorkPost>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching line workposts: {ex.Message}"
            );
            return new List<PlanningWorkPost>();
        }
    }

    public async Task<List<PlanningWorkPost>> GetWorkpostsByLineId(int lineId)
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    new Uri(_baseUrl, $"api/lineworkposts?lineId={lineId}")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<LineWorkpost>? linesWorkposts = await response.Content.ReadFromJsonAsync<
                    List<LineWorkpost>
                >();
                return linesWorkposts?.Map() ?? new List<PlanningWorkPost>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching workposts by line ID: {ex.Message}"
            );
            return new List<PlanningWorkPost>();
        }
    }
}
