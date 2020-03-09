using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace Albumify.Domain.Spotify
{
    /// <summary>
    /// Authorize the app to access the Spotify Platform by using Spotify's Client Credentials Flow. 
    /// 
    /// <para>
    /// If authenticated, the access token allows you to make requests to the Spotify Web API endpoints not requiring user authorization. 
    /// In other words, this does not grant permission to access nor modify a user's own data, such as playlists.
    /// </para>
    /// 
    /// <para>For more information, visit https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow </para>
    /// </summary>
    public class SpotifyClientCredentialsFlow : ISpotifyAuthorization
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        private SpotifyAuthorizationResult _authResult;

        public SpotifyClientCredentialsFlow(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }
        
        public async Task<SpotifyAuthorizationResult> RequestAsync()
        {
            // TODO: test; make sure to set the field too
            if (_authResult != null) return _authResult;

            var formContent = GenerateAuthenticationBody();
            var authenticationHeader = GenerateAuthenticationHeader();

            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
            var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", formContent);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                /*
                 * Spotify returns a BadRequest containing an Authentication Error Object in the body when authentication/authorization fails
                 * Look for Authentication Error Object in https://developer.spotify.com/documentation/web-api/#response-status-codes
                 * 
                 * Example body: "{"error":"invalid_client","error_description":"Invalid client secret"}"
                */
                var responseString = await response.Content.ReadAsStringAsync();
                var authenticationError = JsonSerializer.Deserialize<SpotifyAuthorizationError>(responseString);
                throw new SpotifyAuthorizationException(authenticationError);
            }

            var responseStream = response.Content.ReadAsStreamAsync();
            var spotifyAuthenticationResult = await JsonSerializer.DeserializeAsync<SpotifyAuthorizationResult>(await responseStream);

            return spotifyAuthenticationResult;
        }

        private static FormUrlEncodedContent GenerateAuthenticationBody()
        {
            // The body of this request must be encoded in application/x-www-form-urlencoded as defined in the OAuth 2.0 specification
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
                // There is also an option to refresh_token...might be worth looking into
            });
        }

        private AuthenticationHeaderValue GenerateAuthenticationHeader()
        {
            var spotifyClientId = _config.GetValue<string>("SpotifyClientId");
            var spotifyClientSecret = _config.GetValue<string>("SpotifyClientSecret");
            var authorizationBytes = System.Text.Encoding.ASCII.GetBytes($"{spotifyClientId}:{spotifyClientSecret}");
            var encodedAuthorization = Convert.ToBase64String(authorizationBytes);
            var authenticationHeader = new AuthenticationHeaderValue("Basic", encodedAuthorization);
            return authenticationHeader;
        }
    }
}
