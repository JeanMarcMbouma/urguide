export default function NewPostReducer(state, action) {
    let context = { ...state };

    switch (action.type) {

        case "update-details":
            context.files = action.data.files;
            context.showPost = action.data.showPost;
            context.itineraries = action.data.itineraries;
            return context;
            break;

        case "remove-file":
           if (context.files.length === 1) {
                context.files.splice(0, 1);
            }

            else {
                context.files.splice(context.idToRemove, 1);
            }

            return context;
            break;

        case "create-post":
            context.text = "Some Dummy Text !";
            context.description = action.data.description;
            context.geoLocation = action.data.geoLocation;
            context.date = action.data.date;
            context.startTime = action.data.startTime;
            context.endTime = action.data.endTime;
            context.seats = action.data.seats;
            context.unitPrice = action.data.unitPrice;
            context.categories = action.data.categories;
            context.files = action.data.files;
            context.showPost = action.data.showPost;
            context.itineraries = action.data.itineraries;
            context.bidOptIn = action.data.bidOptIn;
            context.priceRange = `$${context.unitPrice[0]} - $${context.unitPrice[1]}`;

            console.log(context);
            if (context.description.length >= 10 && context.geoLocation.length >= 4)
            {
                action.data.callback(context);
            }
           
            return context;
        
    }
}
