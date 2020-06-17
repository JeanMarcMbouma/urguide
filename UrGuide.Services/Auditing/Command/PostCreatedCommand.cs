using UrGuide.Data;
using UrGuide.Data.Entities.Event;
using UrGuide.Services.Auditing.Abstraction;

namespace UrGuide.Services.Auditing.Command
{
    class PostCreatedCommand : BaseAuditCommand
    {
        public PostCreatedCommand(string userId, string postId)
        {
            UserId = userId ?? throw new System.ArgumentNullException(nameof(userId));
            ReferenceId = postId ?? throw new System.ArgumentNullException(nameof(postId));
        }
        public override EventCodes EventCode => EventCodes.CreatePost;

    }
    class PostCreatedCommandHandler : BaseAuditEventCommandHandler<PostCreatedCommand>
    {
        public PostCreatedCommandHandler(UrGuideContext context) : base(context)
        {
        }
    }
}
