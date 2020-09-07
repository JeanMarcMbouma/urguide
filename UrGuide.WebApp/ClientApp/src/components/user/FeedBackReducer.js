
function getTime() {
    var date = new Date();
    var time = `${date.getDate()}-${date.getMonth()}-${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
    return time;
}

export default function FeedBackReducer(state, action) {
    let context = { ...state, ...action.data };
    

    switch (action.type) {

        case "more-feedbacks":

            return context;

            break;
        case "new-item":
            let feedbacks = context.feedbacks.slice();
            feedbacks.unshift({ ...action.data.review, authorImage: action.data.user.profile.picture, authorFullName: action.data.user.profile.name[1], publicationDate: "Just now." });
            return { ...context, feedbacks: feedbacks };

        case "user-feedback":

            var text = String(context.userFeedback.review);
            let isTextValidUser =
                text.length > 4 && text.length < 500 ? true : false;
            context.textError = isTextValidUser ? false : true;
            //console.log(context.userFeedback);
            if (isTextValidUser) {
                action.data.callback(context.userFeedback);
            }
            
            return context;

            break;

        case "post-feedback":

            var text = String(context.userFeedback.review);
            let isTextValidPost =
                text.length > 4 && text.length < 500 ? true : false;
            context.textError = isTextValidPost ? false : true;
            if (isTextValidPost) {

                action.data.callback(context.userFeedback);
            }

            return context;

            break;
        case "load-more":

            context.pageNumber = action.data.pageNumber;
            context.itemsCount = action.data.itemsCount;
            context.items = action.data.items;
            return context;
            break;
    }

    return context;
}
