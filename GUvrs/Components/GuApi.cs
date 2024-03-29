﻿using GUvrs.Models;
using GUvrs.Models.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GUvrs.Components;

public class GuApi
{
    private static string GU_API_LEGACY_CNAME = "game-legacy.prod.prod.godsunchained.com";
    private static string GU_API_CNAME = "api.godsunchained.com";

    private static HttpClient _http;

    public GuApi()
    {
        _http ??= new HttpClient();
    }

    public async Task<Dictionary<int, string>> GetModes()
    {
        var request = GetRequest(GU_API_CNAME, HttpMethod.Get, $"/v0/mode");
        var response = await _http.SendAsync(request);

        var responseOrError = await GetResponse<List<dynamic>>(response);
        if (responseOrError.Success)
        {
            var modes = new Dictionary<int, string>();
            foreach (var element in responseOrError.Response)
            {
                var mode = (JsonElement)element;
                if (mode.GetProperty("active").GetBoolean())
                    modes.Add(mode.GetProperty("id").GetInt32(), mode.GetProperty("name").GetString());
            }

            return modes;
        }

        return default;
    }

    public async Task<PlayerRankModel> GetRank(string guid, int gameMode = 13)
    {
        if (string.IsNullOrEmpty(guid) || guid == "-1")
            return null;

        var request = GetRequest(GU_API_LEGACY_CNAME, HttpMethod.Get, $"/user/{guid}/rank");
        var response = await _http.SendAsync(request);

        var responseOrError = await GetResponse<List<PlayerRankModel>>(response);
        if (responseOrError.Success)
            return responseOrError.Response.FirstOrDefault(t => t.GameMode == gameMode);

        return default;
    }

    private HttpRequestMessage GetRequest(string cname, HttpMethod method, string path, dynamic model = default)
    {
        var request = new HttpRequestMessage(method, $"https://{cname}{path}");
        if (model != null)
        {
            var body = JsonSerializer.Serialize(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private async Task<ResponseOrErrorModel<T>> GetResponse<T>(HttpResponseMessage message)
    {
        var content = await message.Content.ReadAsStringAsync();
        var response = new ResponseOrErrorModel<T>();
        if (message.StatusCode == HttpStatusCode.OK)
        {
            response.Success = true;
            response.Response = JsonSerializer.Deserialize<T>(content);
        }
        else
        {
            response.Success = false;
            response.Error = content;
        }

        return response;
    }
}
