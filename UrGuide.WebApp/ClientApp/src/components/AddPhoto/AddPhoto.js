import React, {useContext, useReducer} from 'react';
import AddPhotoContext from './AddPhotoContext';
import AddPhotoReducer from './AddPhotoReducer';
import PhotoLibraryIcon from '@material-ui/icons/PhotoLibrary';
import {Grid, Button} from '@material-ui/core';

const AddPhoto = ({fileInput, update}) => {
  const ctx = useContext (AddPhotoContext);
  const [state, dispatch] = useReducer (AddPhotoReducer, ctx);

  React.useEffect(() => {
    update(state.files);
  }, [state])
  console.log(ctx)
  var currentFile = null;
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
        disabled={state.files.length>=3}
        ref={fileInput}
        id="file-init"
        accept=".png,.jpg"
        onChange={handleChange}
      />
      <button
        
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
    
    </Grid>
  );
};

export default AddPhoto;

export const PhotoX = () => {
  return (
    <AddPhotoContext.Consumer>
      {
       ({files}) => ( files.length ? ( <div className="p-3 mb-3 bg-white rounded">
        {files.map((f, index) => <img key={f.name} src={f.href} height='100' width='120'/>)}
        </div>) : <></>
        )
      }
    </AddPhotoContext.Consumer>
  )
}