export default function SearchReducer(state, action) {
    let context = { ...state };

    context.itemsCount = action.data.itemsCount;
    context.pageNumber = action.data.pageNumber;
    context.items = action.data.items;
    
    switch (action.type) {
        case "search":
            return context;
            break;
        case "near-me":
          return context;
            break;
    }

    return context;
}
