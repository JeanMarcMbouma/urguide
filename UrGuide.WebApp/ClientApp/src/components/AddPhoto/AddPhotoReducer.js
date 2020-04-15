export default function AddPhotoReducer (state, action) {
  let context = {...state};
  context.currentFile = action.data.currentFile;
  context.files = action.data.files;

  switch (action.type) {
    case 'validate-files':
      if (context.currentFile != null) {
        context.files.push (context.currentFile);
      }

      console.log (context.files);

      return context;

      break;

    case 'remove-file':
      if (context.files.length === 1) {
        context.files.splice (0, 1);
      } else {
        context.files.splice (context.idToRemove, 1);
      }

      return context;

      break;
  }
  return context;
}
