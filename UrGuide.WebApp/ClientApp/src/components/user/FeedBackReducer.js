export default function FeedBackReducer(state, action) {
    let context = { ...state };
    context.postId = action.data.postId;
    context.userFeedback = action.data.userFeedback;
    context.feedbacks = action.data.feedbacks;
    var text = String(context.userFeedback.review);
    let isTextValid  =
        text.length > 4 && text.length < 500 ? true : false;
    context.textError = isTextValid ? false : true;

    switch (action.type) {

        case "user-feedback":
            console.log(context.userFeedback);
            if (isTextValid) {
                context.feedbacks.push(context.userFeedback);
                action.data.callback(context.userFeedback);
            }
            
            return context;

            break;

    }

    return context;
}
