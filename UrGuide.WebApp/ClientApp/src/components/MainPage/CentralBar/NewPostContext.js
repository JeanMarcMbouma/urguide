import React, { createContext } from "react";

const NewPostContext = createContext({
    showPost:false,
    text: null,
    description: null,
    geoLocation: null,
    date: new Date(),
    startTime: false,
    endTime: null,
    seats: 0,
    unitPrice: 0,
    categories:[],
    files: [],
    idToRemove: null,
    itineraries: [],
    priceRange: '',
    bidOptIn: true,
 
});

export default NewPostContext;