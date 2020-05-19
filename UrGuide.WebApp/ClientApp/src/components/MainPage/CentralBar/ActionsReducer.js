export default function ActionsReducer(state, action) {
    let context = { ...state };

    
    switch (action.type) {

        case "like-action":
            context.post = action.data.post;
            context.posts = action.data.posts;

            context.posts.forEach((post, index) => {

                if (post.id === context.post.id && post.reactionType === 2) {
                    context.post.reactionType = 0;
                    context.post.likes = (context.post.likes - 1);
                    context.posts[index] = context.post;
                    context.like = false;
                    action.data.callback(context);
                    return context;
                }

                if (post.id === context.post.id && post.reactionType === 4 ) {
                    context.post.reactionType = 2;
                    context.post.dislikes = (context.post.dislikes - 1);
                    context.post.likes = (context.post.likes + 1);
                    context.posts[index] = context.post;
                    context.like = true;
                    action.data.callback(context);
                    return context;
                }
                if (post.id === context.post.id && post.reactionType === 0) {
                    context.post.reactionType = 2;
                    context.post.likes = (context.post.likes + 1);
                    context.posts[index] = context.post;
                    context.like = true;
                    action.data.callback(context);
                    return context;
                }
            });

            return context;

        case "dislike-action":
            context.post = action.data.post;
            context.posts = action.data.posts;
            context.posts.forEach((post, index) => {

                if (post.id === context.post.id && post.reactionType === 4) {
                    context.post.reactionType = 0;
                    context.post.dislikes = (context.post.dislikes - 1);
                    context.like = true;
                    context.posts[index] = context.post;
                    return context;
                }

                if (post.id === context.post.id && post.reactionType === 2) {
                    context.post.reactionType = 4;
                    context.post.likes = (context.post.likes - 1);
                    context.post.dislikes = (context.post.dislikes + 1);
                    context.like = false;
                    context.posts[index] = context.post;
                    return context;
                }
                if (post.id === context.post.id && post.reactionType === 0) {
                    context.post.reactionType = 4;
                    context.post.dislikes = (context.post.dislikes + 1);
                    context.like = false;
                    context.posts[index] = context.post;

                    return context;
                }
            });

            action.data.callback(context);
            return context;

    }
}
