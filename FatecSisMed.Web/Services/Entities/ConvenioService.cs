﻿using FatecSisMed.Web.Models;
using FatecSisMed.Web.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace FatecSisMed.Web.Services.Entities;

public class ConvenioService : IConvenioService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _options;

    public ConvenioService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    private const string apiEndpoint = "/api/convenio/";

    public async Task<ConvenioViewModel> CreateConvenio(ConvenioViewModel convenio, string token)
    {
        var client = _clientFactory.CreateClient("MedicoAPI");
        PutTokenInHeaderAuthorization(token, client);

        StringContent content = new StringContent(JsonSerializer.Serialize(convenio), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<ConvenioViewModel>(apiResponse, _options);
            }
            return null;
        }
    }

    public async Task<bool> DeleteConvenioById(int id, string token)
    {
        var client = _clientFactory.CreateClient("MedicoAPI");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync(apiEndpoint + id))
        {
            return response.IsSuccessStatusCode;
        }
    }

    public async Task<ConvenioViewModel> FindConvenioById(int id, string token)
    {
        var client = _clientFactory.CreateClient("MedicoAPI");
        PutTokenInHeaderAuthorization(token, client);
        
        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode && response.Content is not null)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<ConvenioViewModel>(apiResponse, _options);
            }
            return null;
        }
    }

    public async Task<IEnumerable<ConvenioViewModel>> GetAllConvenios(string token)
    {
        var client = _clientFactory.CreateClient("MedicoAPI");
        PutTokenInHeaderAuthorization(token, client);

        var response = await client.GetAsync(apiEndpoint);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<ConvenioViewModel>>(apiResponse, _options);
        }
        return null;
    }

    public async Task<ConvenioViewModel> UpdateConvenio(ConvenioViewModel convenioViewModel, string token)
    {
        var client = _clientFactory.CreateClient("MedicoAPI");
        PutTokenInHeaderAuthorization(token, client);

        ConvenioViewModel convenio = new ConvenioViewModel();

        using (var response = await client.PutAsJsonAsync(apiEndpoint, convenioViewModel))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<ConvenioViewModel>(apiResponse, _options);
            }
            return null;
        }

    }
    private static void PutTokenInHeaderAuthorization(string token, HttpsClient client)
    {
        cliente.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }
}
