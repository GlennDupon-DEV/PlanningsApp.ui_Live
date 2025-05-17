using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlanningsApp.UI.Models;
using PlanningsApp.UI.Services.Interfaces;

namespace PlanningsApp.UI.Services;

public class AbsenceService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : IAbsenceService
{
    private readonly Uri _baseUrl = new Uri(apiSettings.EmployeeBaseUrl);

    public async Task<List<EmployeeAbsence>> GetEmployeeAbsences()
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    new Uri(_baseUrl, "api/employeeAbsences")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<EmployeeAbsence>? absences = await response.Content.ReadFromJsonAsync<
                    List<EmployeeAbsence>
                >();
                return absences ?? new List<EmployeeAbsence>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching absences: {ex.Message}"
            );
            return new List<EmployeeAbsence>();
        }
    }

    public async Task<EmployeeAbsence> PatchEmployeeAbsence(EmployeeAbsence employeeAbsenceToUpdate)
    {
        try
        {
            Uri patchUri = new Uri(_baseUrl, $"api/employeeAbsences/{employeeAbsenceToUpdate.Id}");
            var payload = new
            {
                employeeId = employeeAbsenceToUpdate.EmployeeId,
                absenceId = employeeAbsenceToUpdate.AbsenceId,
                startDate = employeeAbsenceToUpdate.StartDate?.ToString("yyyy-MM-dd"),
                endDate = employeeAbsenceToUpdate.EndDate?.ToString("yyyy-MM-dd"),
                description = employeeAbsenceToUpdate.Description,
            };
            string jsonPayload = JsonSerializer.Serialize(payload);
            StringContent httpContent = new(jsonPayload, Encoding.UTF8, "application/json");

            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                HttpRequestMessage request = new(new HttpMethod("PATCH"), patchUri)
                {
                    Content = httpContent,
                };
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );
                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                EmployeeAbsence updatedEmployeeAbsence =
                    await response.Content.ReadFromJsonAsync<EmployeeAbsence>();
                return updatedEmployeeAbsence!;
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public async Task<EmployeeAbsence> PostEmployeeAbsence(EmployeeAbsence newEmployeeAbsence)
    {
        Uri postUri = new(_baseUrl, "api/employeeAbsences");
        var payload = new
        {
            employeeId = newEmployeeAbsence.EmployeeId,
            absenceId = newEmployeeAbsence.AbsenceId,
            startDate = newEmployeeAbsence.StartDate?.ToString("yyyy-MM-dd"),
            endDate = newEmployeeAbsence.EndDate?.ToString("yyyy-MM-dd"),
            description = newEmployeeAbsence.Description,
        };
        string jsonPayload = JsonSerializer.Serialize(payload);
        StringContent httpContent = new(jsonPayload, Encoding.UTF8, "application/json");
        AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            HttpRequestMessage request = new(HttpMethod.Post, postUri) { Content = httpContent };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            EmployeeAbsence updatedEmployeeAbsence =
                await response.Content.ReadFromJsonAsync<EmployeeAbsence>();
            return updatedEmployeeAbsence!;
        }
        else
        {
            throw new ApplicationException("No authentication token found");
        }
    }

    public async Task<List<Absence>> GetAbsencesAsync()
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(HttpMethod.Get, new Uri(_baseUrl, "api/absences"));
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<Absence>? absences = await response.Content.ReadFromJsonAsync<List<Absence>>();
                return absences ?? new List<Absence>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching absences: {ex.Message}"
            );
            return new List<Absence>();
        }
    }
}
