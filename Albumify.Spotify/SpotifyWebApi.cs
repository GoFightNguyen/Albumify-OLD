using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using Albumify.Domain.Models;
using Albumify.Domain;
using Albumify.Spotify.Models;

namespace Albumify.Spotify
{
    public class SpotifyWebApi : I3rdPartyMusicService
    {
        private readonly HttpClient _httpClient;
        private readonly ISpotifyAuthorization _spotifyAuthorization;

        public SpotifyWebApi(HttpClient httpClient, ISpotifyAuthorization spotifyAuthorization)
        {
            _httpClient = httpClient;
            _spotifyAuthorization = spotifyAuthorization;
        }

        /// <summary>
        /// Get a specific album given its unique Spotify Id.
        /// </summary>
        /// <param name="thirdPartyId">The unique Spotify Id of the Album</param>
        /// <returns></returns>
        public async Task<Album> GetAlbumAsync(string thirdPartyId)
        {
            var accessToken = await _spotifyAuthorization.RequestAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);

            var encoded = WebUtility.HtmlEncode(thirdPartyId);
            var url = $"https://api.spotify.com/v1/albums/{encoded}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = response.Content.ReadAsStreamAsync();
                var album = await JsonSerializer.DeserializeAsync<AlbumObject>(await responseStream);
                return (Album)album;
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<UnsuccessfulResponse>(responseString);
                // Log error as warning
                return Album.CreateForUnknown(thirdPartyId);
            }
        }

        /// <summary>
        /// Search artists.
        /// There is no guarantee of order because v1 of the Spotify API does not support ordering
        /// </summary>
        /// <param name="name">For multiword artist names, match the words in order. For example, "Bob Dylan" will only match on anything containg "Bob Dylan".</param>
        /// <returns></returns>
        public async Task<List<Artist>> SearchArtistsByNameAsync(string name)
        {
            var accessToken = await _spotifyAuthorization.RequestAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);

            var queryParams = new Dictionary<string, string>
            {
                {"q", $"artist:\"{name}\"" }, // Keywords are matched in any order unless surrounded by double quotations
                {"type", "artist"},
                {"limit", "50" }    // 50 is the max. I don't expect there to be 50 artist matches, so I use the max in hopes of never having to page
            };

            var url = QueryHelpers.AddQueryString("https://api.spotify.com/v1/search", queryParams);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseStream = response.Content.ReadAsStreamAsync();
            var searchResult = await JsonSerializer.DeserializeAsync<SearchArtistsObject>(await responseStream);
            return searchResult.Artists.Items.ConvertAll(a => (Artist)a);
        }

        public async Task<List<Album>> GetAnArtistsAlbumsAsync(string thirdPartyId)
        {
            // TODO: no matches

            var accessToken = await _spotifyAuthorization.RequestAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);

            var queryParams = new Dictionary<string, string>
            {
                {"limit", "50" }    // 50 is the max. I don't expect there to be 50 artist matches, so I use the max in hopes of never having to page
                //since include_groups is not set, it includes albums, compilations, singles/eps, and appears on
            };

            var encoded = WebUtility.HtmlEncode(thirdPartyId);
            var url = QueryHelpers.AddQueryString($"https://api.spotify.com/v1/artists/{encoded}/albums", queryParams);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseStream = response.Content.ReadAsStreamAsync();
            var searchResult = await JsonSerializer.DeserializeAsync<PagingObject<SimplifiedAlbumObject>>(await responseStream);
            return searchResult.Items.ConvertAll(a => (Album)a);
        }
    }
}
