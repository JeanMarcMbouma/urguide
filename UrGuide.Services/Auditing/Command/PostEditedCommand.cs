using System;
using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class PostEditedCommand : BaseAuditCommand
    {
        public PostEditedCommand(string userId, string postId): base(userId, referenceId: postId)
        {
            ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        }
        public override EventCodes EventCode => EventCodes.EditPost;

    }
    class PostEditedCommandHandler : BaseAuditEventCommandHandler<PostEditedCommand>
    {
        public PostEditedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}
