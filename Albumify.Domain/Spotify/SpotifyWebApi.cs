using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

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

        public async Task<IEnumerable<SpotifySearchAlbumResult>> FindAlbumsByArtistAsync(string artistName)
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
    }
}
