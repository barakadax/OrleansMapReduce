﻿using Extensions;
using Translators.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Net;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Translators;

[ExcludeFromCodeCoverage]
public class MicrosoftTranslator : IMicrosoftTranslator
{
    private const string CredentialsPath = "JSONs/credentials.json";
    private const string CredentialsValidationPath = "JSONs/credsValidation.json";
    private readonly string URL;
    private readonly string KEY;
    private readonly string REGION;

    public MicrosoftTranslator()
    {
        var credentialsJSchema = JSchema.Parse(File.ReadAllText(CredentialsValidationPath));
        var translateCredentialsJObj = JObject.Parse(File.ReadAllText(CredentialsPath));

        if (translateCredentialsJObj.IsValid(credentialsJSchema))
        {
            URL = translateCredentialsJObj["URL"].Value<string>();
            KEY = translateCredentialsJObj["KEY"].Value<string>();
            REGION = translateCredentialsJObj["REGION"].Value<string>();
        }
    }

    public bool CanTranslate()
    {
        return URL.NotNullNorEmpty() && KEY.NotNullNorEmpty() && REGION.NotNullNorEmpty();
    }

    public async Task<string> GetWordTranslation(string word)
    {
        if (!CanTranslate())
        {
            return null;
        }

        var body = new object[] { new { Text = word } };
        var requestBody = JsonConvert.SerializeObject(body);
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new(URL),
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
        };

        request.Headers.Add("Ocp-Apim-Subscription-Key", KEY);
        request.Headers.Add("Ocp-Apim-Subscription-Region", REGION);

        var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new WebException(response.StatusCode.ToString());
        }

        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<AllTranslation>>(jsonResult)[0].Translations[0].Text;
    }

    private class AllTranslation
    {
        public Translation[] Translations { get; set; }
    }

    private class Translation
    {
        public string Text { get; set; }
        public string To { get; set; }
    }
}
