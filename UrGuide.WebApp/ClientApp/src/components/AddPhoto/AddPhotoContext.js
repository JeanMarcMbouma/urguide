import React, { createContext } from 'react';

var data = new Array();

const AddPhotoContext = createContext({
  files: data,
  currentFile: null,
});

export default AddPhotoContext;