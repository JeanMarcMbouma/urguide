using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Shared.Contracts;

namespace UrGuide.Services.Abstraction
{
    internal abstract class BaseService
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

            var catalog = await Context.Set<TInput>().Include(x => x.Attributes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors($"Entity with the given id  '{id}' doesn't exists");

            var genericAttributes = catalog.Attributes;
            foreach (var attribute in attributes)
            {
                var attr = genericAttributes.FirstOrDefault(a => a.Name == attribute.Name);
                if (attr == null)
                {
                    catalog.Attributes.Add(new Data.Entities.Attributes.GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
                else
                {
                    attr.Value = attribute.Value;
                }
            }

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }

        public async Task<Result<bool>> SetAttributesRestrictedToUserAsync<TInput>(string id, SetAttribute[] attributes, CancellationToken cancellationToken) where TInput : class, IUserOwnedEntity, IAttributeEnabledEntity
        {
            if (!UserContext.IsAuthenticated)
                return Result.Of(false).WithErrors(ErrorMessages.NotAuthenticated);

            cancellationToken.ThrowIfCancellationRequested();

            var catalog = await Context.Set<TInput>().Include(x => x.Attributes)
                .Include(x => x.User)
                .Where(x => x.User.Id == UserContext.UserId)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (catalog == null)
                return Result.Of(false).WithErrors($"Entity with the given id  '{id}' doesn't exists");

            var genericAttributes = catalog.Attributes;
            foreach (var attribute in attributes)
            {
                var attr = genericAttributes.FirstOrDefault(a => a.Name == attribute.Name);
                if (attr == null)
                {
                    catalog.Attributes.Add(new Data.Entities.Attributes.GenericAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
                else
                {
                    if(string.IsNullOrEmpty(attribute.Value))
                    {
                        catalog.Attributes.Remove(attr);
                    } else
                    {
                        attr.Value = attribute.Value;
                    }
                }
            }

            await Context.SaveChangesAsync(cancellationToken);
            return Result.Of(true);
        }
    }
}
