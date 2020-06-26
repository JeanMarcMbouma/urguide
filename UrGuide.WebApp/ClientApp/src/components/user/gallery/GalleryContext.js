import React, { createContext } from "react";

var data = new Array();

const GalleryContext = createContext({
    title: null,
    description: null,
    titleError: false,
    locationError: false,
    descriptionError: false,
    files: data,
    currentFile: null,
    idToRemove: 0,
    emptyGalleryMessage:'',
    galleries: [],
    loading: true,
});

export default GalleryContext;