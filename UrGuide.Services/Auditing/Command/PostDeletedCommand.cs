using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class PostDeletedCommand : BaseAuditCommand
    {
        public PostDeletedCommand(string userId, string postId)
        {
            UserId = userId ?? throw new System.ArgumentNullException(nameof(userId));
            ReferenceId = postId ?? throw new System.ArgumentNullException(nameof(postId));
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
