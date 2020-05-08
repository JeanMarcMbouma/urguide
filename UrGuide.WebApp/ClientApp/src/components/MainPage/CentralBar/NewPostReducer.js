export default function NewPostReducer(state, action) {
    let context = { ...state };

    switch (action.type) {

        case "update-details":
            //context.email = action.data.email;
            context.description = action.data.description;
            //context.location = action.data.location;
            //context.date = action.data.date;
            //context.startTime = action.data.startTime;
            //context.endTime = action.data.endTime;
            //context.seats = action.data.seats;
            //context.budget = action.data.budget;
            //context.categories = action.data.categories;
            context.files = action.data.files;
            context.showPost = action.data.showPost;
            if (context.description.length >= 10 ) {
                context.isButtonEnabled = true;
            }

            console.log(context.isButtonEnabled);

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
    }
}
