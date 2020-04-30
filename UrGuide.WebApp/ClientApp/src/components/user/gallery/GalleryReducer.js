import { string } from "prop-types";

async function sendGallery(state) {

    const response = await fetch('/galleries', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({
            title: state.title,
            location: state.location,
            description: state.description,
            files: state.files,
            userId: null,
        })
    });
    if (response.status == 200 || response.status == 304 || response.status == 204) {


        window.location.replace(`${window.location.origin}/user`);

        console.log(response);
    }
    else
    {
        // we got an error
        if (response.status == 400) // BadRequest
        {
            //console.log(response);

            return response;

        }


    }
}

export default function GuideReducer(state, action) {
    let context = { ...state };
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
   

    switch (action.type) {
        case "validate-gallery":
            context.emptyGalleryMessage = hasNoFiles ? "Please upload some pictures or videos to this gallery." : "";
            context.titleError = context.title.length > 0 ? false : true;
            context.locationError = context.location.length > 0 ? false : true;
            context.descriptionError =
                isDescriptionGotProperLength ? false : true;

            if (context.titleError || context.locationError || context.descriptionError || hasNoFiles)
            {
                
                return context;
            }
            else
            {

                const response = sendGallery(context);

                if (response.status === 400)
                {
                    context.emptyGalleryMessage = "Sorry , something went wrong !";

                    console.log(response);

                }

                return context;
            }

            break;

        case "validate-files":

            if (context.currentFile != null && context.files.length < 9 ) {

                context.files.push(context.currentFile);
            }
            if (context.files.length === 9) {
                context.emptyGalleryMessage = "You can only upload up to 9 photos or videos !";
            }
            else
            {
                context.emptyGalleryMessage = '';
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
