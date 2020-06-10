export default function NotificationsReducer(state, action) {
    let context = { ...state };

    context.itemsCount = action.data.itemsCount;
    context.pageNumber = action.data.pageNumber;
    context.items = action.data.items;

    switch (action.type) {
        case "all":
            return context;
            break;
        case "unread":
            return context;
            break;
    }

    return context;
}
