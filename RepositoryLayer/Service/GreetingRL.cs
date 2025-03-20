using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Interface;
using RepositoryLayer.Contexts;
using ModelLayer.Model;
using ModelLayer.Entity;
using NLog;
using StackExchange.Redis;

namespace RepositoryLayer.Service
{
    /// <summary>
    /// GreetingRL Class implementing IGreetingRL
    /// </summary>
    public class GreetingRL : IGreetingRL
    {
        private readonly GreetingAppContext _dbContext;
        private readonly IDatabase _cache;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor for GreetingRL
        /// </summary>
        public GreetingRL(GreetingAppContext dbContext, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _cache = redis.GetDatabase();
            Logger.Info("GreetingRL instance created.");
        }

        // UC4 - Save Greeting
        /// <summary>
        /// Adds a greeting to the database.
        /// </summary>
        /// <param name="greetingModel">The model containing greeting details.</param>
        /// <returns>Returns the saved GreetEntity object.</returns>
        public GreetEntity SaveGreetingRL(GreetingModel greetingModel)
        {
            try
            {
                bool userExists = _dbContext.Users.Any(u => u.UserId == greetingModel.Uid);
                if (!userExists)
                {
                    Logger.Warn("User with ID: {0} not found. Cannot save greeting.", greetingModel.Uid);
                    return null;
                }

                var newMessage = new GreetEntity
                {
                    Message = greetingModel.Message,
                    UserId = greetingModel.Uid
                };

                _dbContext.Greet.Add(newMessage);
                _dbContext.SaveChanges();

                _cache.KeyDelete("AllGreetings"); // Invalidate cache
                Logger.Info("Greeting saved successfully with ID: {0}", newMessage.Id);
                return newMessage;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while saving greeting.");
                throw;
            }
        }

        // UC5 - Get Greeting by ID with caching
        /// <summary>
        /// Retrieves a greeting by its ID.
        /// </summary>
        /// <param name="id">The ID of the greeting to retrieve.</param>
        /// <returns>Returns the GreetingModel associated with the specified ID.</returns>
        public GreetingModel GetGreetingByIdRL(int id)
        {
            try
            {
                Logger.Info($"Fetching greeting with ID: {id}");
                string cacheKey = $"Greeting:{id}";
                string cachedGreeting = _cache.StringGet(cacheKey);

                if (!string.IsNullOrEmpty(cachedGreeting))
                {
                    Logger.Info("Greeting found in cache.");
                    return JsonSerializer.Deserialize<GreetingModel>(cachedGreeting);
                }

                var entity = _dbContext.Greet.FirstOrDefault(g => g.Id == id);
                if (entity != null)
                {
                    var greetingModel = new GreetingModel
                    {
                        Id = entity.Id,
                        Message = entity.Message,
                        Uid = entity.UserId
                    };

                    _cache.StringSet(cacheKey, JsonSerializer.Serialize(greetingModel), TimeSpan.FromMinutes(10));
                    Logger.Info("Greeting retrieved from database and cached.");
                    return greetingModel;
                }

                Logger.Warn("Greeting not found for ID: {0}", id);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while fetching greeting by ID: {0}", id);
                throw;
            }
        }

        // UC6 - Get All Greetings with caching
        /// <summary>
        /// Retrieves all greetings.
        /// </summary>
        /// <returns>Returns a list of all GreetEntity objects.</returns>
        public List<GreetEntity> GetAllGreetingsRL()
        {
            try
            {
                Logger.Info("Fetching all greetings.");
                string cacheKey = "AllGreetings";
                string cachedGreetings = _cache.StringGet(cacheKey);

                if (!string.IsNullOrEmpty(cachedGreetings))
                {
                    Logger.Info("All greetings found in cache.");
                    return JsonSerializer.Deserialize<List<GreetEntity>>(cachedGreetings);
                }

                var greetings = _dbContext.Greet.ToList();
                _cache.StringSet(cacheKey, JsonSerializer.Serialize(greetings), TimeSpan.FromMinutes(10));
                Logger.Info("All greetings retrieved from database and cached.");
                return greetings;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while fetching all greetings.");
                throw;
            }
        }

        // UC7 - Edit Greeting with cache invalidation
        /// <summary>
        /// Edits an existing greeting.
        /// </summary>
        /// <param name="id">The ID of the greeting to edit.</param>
        /// <param name="greetingModel">The updated greeting model.</param>
        /// <returns>Returns the updated GreetEntity object.</returns>
        public GreetEntity EditGreetingRL(int id, GreetingModel greetingModel)
        {
            try
            {
                bool userExists = _dbContext.Users.Any(u => u.UserId == greetingModel.Uid);
                if (!userExists)
                {
                    Logger.Warn("User with ID: {0} not found. Cannot update greeting.", greetingModel.Uid);
                    return null;
                }

                var entity = _dbContext.Greet.FirstOrDefault(g => g.Id == id);
                if (entity != null)
                {
                    entity.Message = greetingModel.Message;
                    entity.UserId = greetingModel.Uid;

                    _dbContext.Greet.Update(entity);
                    _dbContext.SaveChanges();

                    _cache.KeyDelete("AllGreetings"); // Invalidate cache
                    _cache.KeyDelete($"Greeting:{id}"); // Invalidate individual greeting cache
                    Logger.Info("Greeting updated successfully for ID: {0}", id);
                    return entity;
                }

                Logger.Warn("Greeting not found for ID: {0} to update", id);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while updating greeting with ID: {0}", id);
                throw;
            }
        }

        // UC8 - Delete Greeting with cache invalidation
        /// <summary>
        /// Deletes a greeting by its ID.
        /// </summary>
        /// <param name="id">The ID of the greeting to delete.</param>
        /// <returns>Returns true if the deletion was successful; otherwise, false.</returns>
        public bool DeleteGreetingRL(int id)
        {
            try
            {
                var entity = _dbContext.Greet.FirstOrDefault(g => g.Id == id);
                if (entity != null)
                {
                    _dbContext.Greet.Remove(entity);
                    _dbContext.SaveChanges();

                    _cache.KeyDelete("AllGreetings"); // Invalidate cache
                    _cache.KeyDelete($"Greeting:{id}");
                    Logger.Info("Greeting deleted successfully for ID: {0}", id);
                    return true;
                }

                Logger.Warn("Greeting not found for ID: {0} to delete", id);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while deleting greeting with ID: {0}", id);
                throw;
            }
        }
    }
}
