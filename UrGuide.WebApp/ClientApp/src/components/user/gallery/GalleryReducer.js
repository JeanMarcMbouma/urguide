export default function GalleryReducer(state, action) {
  let context = { ...state };
  context.title = action.data.title;
  context.description = action.data.description;
  context.currentFile = action.data.currentFile;
  context.files = action.data.files;
  context.idToRemove = action.data.idToRemove;
  var description = String(context.description);

  let hasNoFiles = context.files.length === 0 ? true : false;
  let isDescriptionGotProperLength =
    description.length > 100 && description.length < 500 ? true : false;

  switch (action.type) {
    case "create-gallery":
      context.emptyGalleryMessage = hasNoFiles
        ? "Please upload some pictures or videos to this gallery."
        : "";
      context.titleError = context.title.length > 0 ? false : true;
      context.descriptionError = isDescriptionGotProperLength ? false : true;

      if (context.titleError || context.descriptionError || hasNoFiles) {
        return context;
      } else {
        action.data.callback(context);
        return context;
      }

      break;

    case "validate-files":
      if (context.currentFile != null && context.files.length < 9) {
        context.files.push(context.currentFile);
      }
      if (context.files.length === 9) {
        context.emptyGalleryMessage =
          "You can only upload up to 9 photos or videos !";
      } else {
        context.emptyGalleryMessage = "";
      }

      return context;

      break;

    case "remove-file":
      if (context.files.length === 1) {
        context.files.splice(0, 1);
      } else {
        context.files.splice(context.idToRemove, 1);
      }

      return context;

      break;
    case "update-gallery":
      context.titleError = context.name.length > 0 ? false : true;
      context.descriptionError = isDescriptionGotProperLength ? false : true;
      context.name = context.title;
      if (context.titleError || context.descriptionError) {
        return context;
      } else {
        action.data.callback(context);
        return context;
      }
    case "revert-changes":
      return action.data;
    case "set-data":
      context.galleries = action.data.galleries;
      context.loading = action.data.loading;
      //console.log(context.galleries);
      return context;
      break;
  }

  return context;
}
