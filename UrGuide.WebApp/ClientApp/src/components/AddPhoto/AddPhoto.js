import React, {useContext, useReducer} from 'react';
import AddPhotoContext from './AddPhotoContext';
import AddPhotoReducer from './AddPhotoReducer';

const AddPhoto = () => {
  const ctx = useContext (AddPhotoContext);
  const [state, dispatch] = useReducer (AddPhotoReducer, ctx);
  var currentFile = null;

  let data = state.files;

  function handleChange (event) {
    var file = URL.createObjectURL (event.target.files[0]);

    currentFile = {
      id: data.length,
      href: file,
      description: '',
      name: `gallery-${data.length}`,
    };

    document.getElementById ('data-sender').click ();
  }

  return (
    <Grid item xs={12}>
      <input
        type="file"
        className="input-file"
        id="file-init"
        accept=".png,.jpg"
        onChange={handleChange}
      />
      <button
        id="data-sender"
        className="input-file"
        onClick={() =>
          dispatch ({
            type: 'validate-files',
            data: {
              currentFile: currentFile,
              files: state.files,
            },
          })}
      />

      <Button
        fullWidth
        variant="contained"
        color="default"
        onClick={e => document.getElementById ('file-init').click ()}
      >
        <PhotoLibraryIcon />
        <span>UPLOAD PHOTOS OR VIDEOS</span>
      </Button>
    </Grid>
  );
};

export default AddPhoto;
