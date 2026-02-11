using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class PostDeletedCommand : BaseAuditCommand
    {
        public PostDeletedCommand(string userId, string postId) : base(userId, referenceId: postId)
        {
            ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        }
        public override EventCodes EventCode => EventCodes.DeletePost;

    }
    class PostDeletedCommandHandler : BaseAuditEventCommandHandler<PostDeletedCommand>
    {
        public PostDeletedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}
