using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlanningsApp.Ui.Mapper;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;
using PlanningsApp.Ui.Services.Interfaces;

namespace PlanningsApp.Ui.Services;

public class ShiftService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : IShiftService
{
    private readonly Uri _baseUrl = new(apiSettings.EmployeeBaseUrl);

    public async Task<List<PlanningShift>> GetShiftsAsync()
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(HttpMethod.Get, new Uri(_baseUrl, "api/shifts"));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<Shift>? shifts = await response.Content.ReadFromJsonAsync<List<Shift>>();
                if (shifts == null)
                {
                    throw new ApplicationException("No shift data found");
                }

                return shifts.Map();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unexpected error occurred while fetching shifts: {e.Message}");
            return new List<PlanningShift>();
        }
    }
}
