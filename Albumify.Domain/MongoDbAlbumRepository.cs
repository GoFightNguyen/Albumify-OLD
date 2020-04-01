using Albumify.Domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Albumify.Domain
{
    // TODO: write majority
    public class MongoDbAlbumRepository
    {
        private readonly IMongoCollection<Album> _albums;

        public MongoDbAlbumRepository(IConfiguration configuration)
        {
            var username = configuration.GetValue<string>("MongoDBUsername");
            var password = configuration.GetValue<string>("MongoDBPassword");
            
            var client = new MongoClient($"mongodb://{username}:{password}@{configuration.GetValue<string>("MongoDBHost")}/albumify");
            var db = client.GetDatabase("albumify");
            _albums = db.GetCollection<Album>("albums");
        }

        public async Task AddAsync(Album album)
        {
            try
            {
                await _albums.InsertOneAsync(album);
            }
            catch (MongoAuthenticationException ex)
            {
                throw new AlbumRepositoryException("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBHost, MongoDBUsername, and MongoDBPassword", ex);
            }
            catch (TimeoutException ex)
            {
                throw new AlbumRepositoryException("Failed to connect to MongoDB because of a timeout. Please verify the configuration for MongoDBHost, MongoDBUsername, and MongoDBPassword", ex);
            }
        }

        public async Task<Album> GetAsync(string id)
        {
            return await _albums.Find(a => a.Id == id).SingleAsync();
        }

        public async Task RemoveAsync(string id)
        {
            await _albums.DeleteOneAsync(a => a.Id == id);
        }
    }

    public class AlbumRepositoryException : Exception
    {
        public AlbumRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
