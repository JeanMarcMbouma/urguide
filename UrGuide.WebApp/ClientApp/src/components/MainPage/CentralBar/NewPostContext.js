import React, { createContext } from "react";

const NewPostContext = createContext({
    showPost:false,
    email: null,
    description: null,
    location: null,
    date: false,
    startTime: false,
    endTime: null,
    seats: 0,
    budget: 0,
    categories: null,
    files: [],
    idToRemove: null,
    isButtonEnabled:false,
});

export default NewPostContext;