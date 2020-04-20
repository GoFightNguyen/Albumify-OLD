﻿using Albumify.Domain;
using Albumify.Domain.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Albumify.AcceptanceTests")]

namespace Albumify.MongoDB
{
    public class MyCollectionInMongoDB : IMyCollectionRepository
    {
        internal readonly IMongoCollection<Album> _albums;

        public MyCollectionInMongoDB(IConfiguration configuration)
        {
            RegisterClassMapIfNotAlready();

            var hostScheme = configuration.GetValue<string>("MongoDBHostScheme");
            var host = configuration.GetValue<string>("MongoDBHost");
            var username = configuration.GetValue<string>("MongoDBUsername");
            var password = configuration.GetValue<string>("MongoDBPassword");

            MongoClient client;
            try
            {
                client = new MongoClient($"{hostScheme}://{username}:{password}@{host}/albumify?retryWrites=true&w=majority");
            }
            catch (MongoConfigurationException ex)
            {
                throw new AlbumRepositoryException("Failed to connect to MongoDB. Please verify the configuration for MongoDBHostScheme", ex);
            }

            var db = client.GetDatabase("albumify");
            _albums = db.GetCollection<Album>("albums");
        }

        public static void RegisterClassMapIfNotAlready()
        {
            /* Registering class maps is expected to be a one time thing.
             * If done multiple times, a System.ArgumentException is thrown: An item with the same key has already been added. Key: Albumify.Domain.Models.Album
             *
             * This issue did not present itself while running the app, but it did while running integration tests
             */

            if (BsonClassMap.IsClassMapRegistered(typeof(Album))) return;

            BsonClassMap.RegisterClassMap<Album>(a =>
            {
                a.AutoMap();
                a.MapMember(album => album.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });
        }

        public async Task<Album> AddAsync(Album album)
        {
            try
            {
                await _albums.InsertOneAsync(album);
                return album;
            }
            catch (MongoAuthenticationException ex)
            {
                throw new AlbumRepositoryException("Failed to authenticate with MongoDB. Please verify the configuration for MongoDBUsername and MongoDBPassword", ex);
            }
            catch (TimeoutException ex)
            {
                throw new AlbumRepositoryException("Failed to connect to MongoDB because of a timeout. Please verify the configuration for MongoDBHost", ex);
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

        public async Task<Album> FindBy3rdPartyIdAsync(string thirdPartyId)
        {
            var album = await _albums.Find(a => a.ThirdPartyId == thirdPartyId).SingleOrDefaultAsync();
            return album ?? Album.CreateForUnknown(thirdPartyId);
        }
    }

    // TODO: rename, does this belong in domain?
    public class AlbumRepositoryException : Exception
    {
        public AlbumRepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}