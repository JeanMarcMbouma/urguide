export default function NotificationsReducer(state, action) {
    let context = { ...state };

    context.itemsCount = action.data.itemsCount;
    context.pageNumber = action.data.pageNumber;
    

    switch (action.type) {
        case "all":
            context.items = action.data.items;
            return context;
            break;
        case "more":
            action.data.items.forEach((item, index) => {
                context.items.push(item);
            });
            return context;
            break;
        case "unread":
           
            context.items = action.data.items;
            context.items.unshift(action.data.notification);

            return context;
            break;
        case "clicked":
            context.items = action.data.items;
            action.data.markasread(action.data.notificationId, action.data.redirectUrl);
            return context;
            break;
    }

    return context;
}
