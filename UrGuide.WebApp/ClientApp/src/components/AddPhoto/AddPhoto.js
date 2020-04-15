import React, {useContext, useReducer} from 'react';
import AddPhotoContext from './AddPhotoContext';
import AddPhotoReducer from './AddPhotoReducer';

const AddPhoto = () => {
  const ctx = useContext (AddPhotoContext);
  const [state, dispatch] = useReducer (AddPhotoReducer, ctx);
  var currentFile = null;
  const fileInput = React.createRef();
  const dataSender = React.createRef();
  let data = state.files;

  function handleChange (event) {
    var file = URL.createObjectURL (event.target.files[0]);

    currentFile = {
      id: data.length,
      href: file,
      description: '',
      name: `gallery-${data.length}`,
    };

    dataSender.current.click ();
  }

  return (
    <Grid item xs={12}>
      <input
        type="file"
        className="input-file"
        ref={fileInput}
        id="file-init"
        accept=".png,.jpg"
        onChange={handleChange}
      />
      <button
        disabled={state.files.length>=3}
        ref={dataSender}
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
        onClick={e => fileInput.current.click()}
      >
        <PhotoLibraryIcon />
        <span>UPLOAD PHOTOS OR VIDEOS</span>
      </Button>
    </Grid>
  );
};

export default AddPhoto;
