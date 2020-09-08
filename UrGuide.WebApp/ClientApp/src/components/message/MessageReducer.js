export default function MessageReducer(state, action) {
    let context = { ...state };

    switch (action.type) {
        case "suggestions":
            context.items = action.data.items;
            return context;
            break;
        case "send":
            context.receiverId = action.data.receiverId;
            context.content = action.data.content;
            action.data.callback(context);
            return context;
            break;
    }

    return context;
}
