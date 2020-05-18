using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain
{
    public interface IMyCollectionRepository
    {
        /// <summary>
        /// Add an Album.
        /// Return the added Album with its collection Id set.
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        Task<Album> AddAsync(Album album);
        /// <summary>
        /// Get a specific album given its unique Third-Party Id.
        /// If the album is not found, then return <see cref="Album.CreateForUnknown(thirdpartyId)"/>.
        /// </summary>
        /// <param name="thirdPartyId"></param>
        /// <returns></returns>
        Task<Album> FindBy3rdPartyIdAsync(string thirdPartyId);

        /// <summary>
        /// Using the list of artist Third-Party Ids provided, return the ones which are in my collection
        /// </summary>
        /// <param name="thirdPartyIds"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> ContainsWhichOfTheseArtists(params string[] thirdPartyIds);
    }
}
