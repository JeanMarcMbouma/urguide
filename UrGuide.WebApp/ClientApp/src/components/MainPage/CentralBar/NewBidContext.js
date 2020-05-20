import React, { createContext } from "react";

const NewBidContext = createContext({
    postId:null,
    value: null,
    
});

export default NewBidContext;