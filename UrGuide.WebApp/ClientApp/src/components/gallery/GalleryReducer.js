import { string } from "prop-types";

export default function GuideReducer(state, action) {
    let context = { ...state };
    let nameRegex = /^[^-\s][\w\s-]+$/;
    context.title = action.data.title;
    context.location = action.data.location;
    context.description = action.data.description;
    context.currentFile = action.data.currentFile;
    context.files = action.data.files;
    context.idToRemove = action.data.idToRemove;
    var description = String(context.description);

    let hasNoFiles = context.files.length === 0 ? true : false;
    let isDescriptionGotProperLength =
        description.length > 100 && description.length < 500 ? true : false;
    let validtitle = nameRegex.test(context.title);
    let validlocation = nameRegex.test(context.location);
    let validdesrcription = nameRegex.test(context.description);

    switch (action.type) {
        case "validate-gallery":
            context.emptyGalleryMessage = hasNoFiles ? "Please upload some pictures or videos to this gallery." : "";
            context.titleError = validtitle ? false : true;
            context.locationError = validlocation ? false : true;
            context.descriptionError =
                isDescriptionGotProperLength && validdesrcription ? false : true;

            if (!context.titleError && !context.locationError && !context.descriptionError && hasNoFiles ) {
                
               return context;
            }

        case "validate-files":

            if (context.currentFile != null) {

                context.files.push(context.currentFile);
            }

            console.log(context.files);

            return context;

            break;

        case "remove-file":

            
            if (context.files.length === 1) {
                context.files.splice(0, 1);
            }

            else
            {
                context.files.splice(context.idToRemove, 1);
            } 

            return context;

            break;

    }

    return context;
}
