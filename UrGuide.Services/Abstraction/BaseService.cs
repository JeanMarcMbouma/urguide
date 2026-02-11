using Microsoft.EntityFrameworkCore;
using System;
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

        public Task<Result<bool>> SetAttributeAsync<TInput>(string id, SetAttribute attribute, CancellationToken cancellationToken) where TInput : class, IAttributeEnabledEntity
        {
            return SetAttributesAsync<TInput>(id, new[] { attribute }, cancellationToken);
        }

        public Task<Result<bool>> SetAttributeRestrictedToUserAsync<TInput>(string id, SetAttribute attribute, CancellationToken cancellationToken) where TInput : class, IAttributeEnabledEntity, IUserOwnedEntity
        {
            return SetAttributesRestrictedToUserAsync<TInput>(id, new[] { attribute }, cancellationToken);
        }

        public async Task<Result<bool>> SetAttributesAsync<TInput>(string id, SetAttribute[] attributes, CancellationToken cancellationToken) where TInput: class, IAttributeEnabledEntity
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var item = await Context.Set<TInput>().Include(x => x.Attributes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item == null)
                return Result.Of(false).WithErrors($"Entity with the given id  '{id}' doesn't exists");

            // Optimize: Use dictionary for O(1) lookups instead of O(n) FirstOrDefault in loop
            var attributeDict = item.Attributes.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
            
            foreach (var attribute in attributes)
            {
                if (attributeDict.TryGetValue(attribute.Name, out var existingAttr))
                {
                    existingAttr.Value = attribute.Value;
                }
                else
                {
                    item.Attributes.Add(new GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
            }
            if (item is ILastUpdatableEntity entity)
            {
                entity.LastUpdated = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> SetAttributesRestrictedToUserAsync<TInput>(string id, SetAttribute[] attributes, CancellationToken cancellationToken) where TInput : class, IUserOwnedEntity, IAttributeEnabledEntity
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
            var attributeDict = item.Attributes.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
            
            foreach (var attribute in attributes)
            {
                if (attributeDict.TryGetValue(attribute.Name, out var existingAttr))
                {
                    if(string.IsNullOrEmpty(attribute.Value))
                    {
                        item.Attributes.Remove(existingAttr);
                    } else
                    {
                        existingAttr.Value = attribute.Value;
                    }
                }
                else
                {
                    item.Attributes.Add(new GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
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
