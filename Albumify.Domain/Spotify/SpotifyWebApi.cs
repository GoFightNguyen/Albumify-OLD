using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Albumify.Domain.Spotify
{
    public class SpotifyWebApi : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly ISpotifyAuthorization _spotifyAuthorization;

        public SpotifyWebApi(HttpClient httpClient, ISpotifyAuthorization spotifyAuthorization)
        {
            _httpClient = httpClient;
            _spotifyAuthorization = spotifyAuthorization;
        }

        /// <summary>
        /// Search albums created by a specific artist. 
        /// There is no guarantee of order because v1 of the Spotify API does not support ordering
        /// </summary>
        /// <param name="artistName">For multiword artist names, the words are matched in order. For example, "Bob Dylan" will only match on anything containg "Bob Dylan".</param>
        /// <returns></returns>
        public async Task<IEnumerable<SpotifySimplifiedAlbumObject>> FindAlbumsByArtistAsync(string artistName)
        {
            var accessToken = await _spotifyAuthorization.RequestAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);

            var queryParams = new Dictionary<string, string>
                    {
                        {"q", $"artist:\"{artistName}\"" }, // Keywords are matched in any order unless surrounded by double quotations
                        {"type", "album"}
                    };

            var url = QueryHelpers.AddQueryString("https://api.spotify.com/v1/search", queryParams);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseStream = response.Content.ReadAsStreamAsync();
            var searchResult = await JsonSerializer.DeserializeAsync<SpotifyFindAlbumsByArtistResult>(await responseStream);
            return searchResult.Albums.Items;
        }

        /// <summary>
        /// Get a specific album given its unique Spotify Id. 
        /// If no match is found, the null object pattern is followed.
        /// </summary>
        /// <param name="spotifyAlbumId"></param>
        /// <returns></returns>
        public async Task<SpotifyAlbumObject> GetAlbumAsync(string spotifyAlbumId)
        {
            var accessToken = await _spotifyAuthorization.RequestAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);

            var encoded = WebUtility.HtmlEncode(spotifyAlbumId);
            var url = $"https://api.spotify.com/v1/albums/{encoded}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = response.Content.ReadAsStreamAsync();
                var album = await JsonSerializer.DeserializeAsync<SpotifyAlbumObject>(await responseStream);
                return album;
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<SpotifyUnsuccessfulResponse>(responseString);
                // Log error
                return SpotifyAlbumObject.CreateForUnknownAlbum(spotifyAlbumId);
            }
        }
    }
}
