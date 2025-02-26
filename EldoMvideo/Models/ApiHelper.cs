﻿using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace EldoMvideo.Models;

public static class ApiHelper
{
    private static readonly string _url = "http://147.45.196.64:8080/api";

    public async static Task<T?> Get<T>(string model, long id = 0)
    {
        var client = new HttpClient();
        var request = id == 0 ? $"/{model}" : $"/{model}/{id}";
        var response = client.GetAsync($"{_url}/{model}/{(id != 0 ? id : string.Empty)}").Result;
        if (response.StatusCode != HttpStatusCode.OK) return default;
        return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
    }

    public async static Task<bool> Put<T>(string json, string model, long id)
    {
        var client = new HttpClient();
        HttpContent body = new StringContent(json, Encoding.UTF8, "application/json");
        var response = client.PutAsync($"{_url}/{model}/{id}", body).Result;
        if (response.StatusCode != HttpStatusCode.NoContent) return false;
        return true;
    }

    public async static Task<bool> Post<T>(string json, string model)
    {
        var client = new HttpClient();
        HttpContent body = new StringContent(json, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"{_url}/{model}", body).Result;
        if (response.StatusCode != HttpStatusCode.Created) return false;
        return true;
    }

    public async static Task<bool> Delete<T>(string model, long id)
    {
        var client = new HttpClient();
        var response = client.DeleteAsync($"{_url}/{model}/{id}").Result;
        if (response.StatusCode == HttpStatusCode.NoContent) return true;
        return false;
    }
}