using Microsoft.EntityFrameworkCore;
using System;
using BbQ.Outcome;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core.Attributes;
using UrGuide.Core.Contracts;
using UrGuide.Data;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Abstraction
{
    public abstract class BaseService
    {
        protected BaseService(UrGuideContext context, IUserContext userContext)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public UrGuideContext Context { get; }
        public IUserContext UserContext { get; }

        public Task<Outcome<bool>> SetAttributeAsync<TInput>(string id, SetAttribute attribute, CancellationToken cancellationToken) where TInput : class, IAttributeEnabledEntity
        {
            return SetAttributesAsync<TInput>(id, new[] { attribute }, cancellationToken);
        }

        public Task<Outcome<bool>> SetAttributeRestrictedToUserAsync<TInput>(string id, SetAttribute attribute, CancellationToken cancellationToken) where TInput : class, IAttributeEnabledEntity, IUserOwnedEntity
        {
            return SetAttributesRestrictedToUserAsync<TInput>(id, new[] { attribute }, cancellationToken);
        }

        public async Task<Outcome<bool>> SetAttributesAsync<TInput>(string id, SetAttribute[] attributes, CancellationToken cancellationToken) where TInput: class, IAttributeEnabledEntity
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var item = await Context.Set<TInput>().Include(x => x.Attributes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item == null)
                return Result.Of(false).WithErrors($"Entity with the given id  '{id}' doesn't exists");

            // Optimize: Use dictionary for O(1) lookups instead of O(n) FirstOrDefault in loop
            // Group by Name to handle potential case-insensitive duplicates, taking the first occurrence
            var attributeDict = item.Attributes
                .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            
            foreach (var attribute in attributes)
            {
                if (attributeDict.TryGetValue(attribute.Name, out var existingAttr))
                {
                    if (string.IsNullOrEmpty(attribute.Value))
                    {
                        item.Attributes.Remove(existingAttr);
                        // Keep dictionary in sync with collection
                        attributeDict.Remove(attribute.Name);
                    }
                    else
                    {
                        existingAttr.Value = attribute.Value;
                    }
                }
                else
                {
                    var newAttr = new GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    };
                    item.Attributes.Add(newAttr);
                    // Keep dictionary in sync with collection
                    attributeDict[attribute.Name] = newAttr;
                }
            }
            if (item is ILastUpdatableEntity entity)
            {
                entity.LastUpdated = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Outcome<bool>> SetAttributesRestrictedToUserAsync<TInput>(string id, SetAttribute[] attributes, CancellationToken cancellationToken) where TInput : class, IUserOwnedEntity, IAttributeEnabledEntity
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var item = await Context.Set<TInput>().Include(x => x.Attributes)
                .Include(x => x.User)
                .Where(x => x.User.Id == UserContext.UserId)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item == null)
                return Result.Of(false).WithErrors($"Entity with the given id  '{id}' doesn't exists");

            // Optimize: Use dictionary for O(1) lookups instead of O(n) FirstOrDefault in loop
            // Group by Name to handle potential case-insensitive duplicates, taking the first occurrence
            var attributeDict = item.Attributes
                .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            
            foreach (var attribute in attributes)
            {
                if (attributeDict.TryGetValue(attribute.Name, out var existingAttr))
                {
                    if(string.IsNullOrEmpty(attribute.Value))
                    {
                        item.Attributes.Remove(existingAttr);
                        // Keep dictionary in sync with collection
                        attributeDict.Remove(attribute.Name);
                    }
                    else
                    {
                        existingAttr.Value = attribute.Value;
                    }
                }
                else
                {
                    var newAttr = new GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    };
                    item.Attributes.Add(newAttr);
                    // Keep dictionary in sync with collection
                    attributeDict[attribute.Name] = newAttr;
                }
            }

            if(item is ILastUpdatableEntity entity)
            {
                entity.LastUpdated = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }
    }
}
