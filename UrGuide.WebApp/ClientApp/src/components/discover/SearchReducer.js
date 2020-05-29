export default function SearchReducer(state, action) {
    let context = { ...state };
   
    context.data = action.data.data;
    
    switch (action.type) {
        case "search":
            console.log(context);
            return context;
            break;
    }

    return context;
}
