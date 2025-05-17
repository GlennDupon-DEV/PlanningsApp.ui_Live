using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using PlanningsApp.UI.Mapper;
using PlanningsApp.UI.Models;
using PlanningsApp.UI.Models.PlanningModels;
using PlanningsApp.UI.Services.Interfaces;

namespace PlanningsApp.UI.Services;

public class EmployeeService(
    HttpClient httpClient,
    IAccessTokenProvider tokenProvider,
    ApiSettings apiSettings
) : IEmployeeService
{
    private readonly Uri _baseUrl = new Uri(apiSettings.EmployeeBaseUrl);

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    new Uri(_baseUrl, "api/employees")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<Employee>? employees = await response.Content.ReadFromJsonAsync<
                    List<Employee>
                >();
                return employees ?? new List<Employee>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching employees: {ex.Message}"
            );
            return new List<Employee>();
        }
    }

    public async Task<List<PlanningEmployee>> GetEmployeesByDerpartmentNameAsync(
        string departmentName
    )
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    new Uri(_baseUrl, $"api/employees?departmentName={departmentName}")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                List<Employee>? employees = await response.Content.ReadFromJsonAsync<
                    List<Employee>
                >();
                return employees?.Map() ?? new List<PlanningEmployee>();
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while fetching employees & their absences: {ex.Message}"
            );
            return new List<PlanningEmployee>();
        }
    }

    public async Task<bool> CheckEmployeeAvailabilityByDate(
        MudItemDropInfo<PlanningEmployee> droppedEmployee
    )
    {
        try
        {
            AccessTokenResult tokenResult = await tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out AccessToken token))
            {
                int employeeId = droppedEmployee.Item.Id;
                string dropzoneDate = droppedEmployee.DropzoneIdentifier.Split('|')[2];

                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    new Uri(_baseUrl, $"api/employees/{employeeId}?date={dropzoneDate}")
                );
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Value
                );

                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                bool isAvailable = await response.Content.ReadFromJsonAsync<bool>();
                return isAvailable;
            }
            else
            {
                throw new ApplicationException("No authentication token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"An unexpected error occurred while checking employee availability: {ex.Message}"
            );
            return false;
        }
    }
}
