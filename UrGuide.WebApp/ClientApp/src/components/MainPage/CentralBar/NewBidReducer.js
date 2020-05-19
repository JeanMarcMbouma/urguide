export default function NewBidReducer(state, action) {
    let context = { ...state };

    switch (action.type) {

        case "new-bid":
            context.postId = action.data.postId;
            context.value = action.data.value;
            action.data.callback(context);
            return context;

    }
}

