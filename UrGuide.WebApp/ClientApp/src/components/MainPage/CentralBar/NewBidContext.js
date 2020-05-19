import React, { createContext } from "react";

const NewBidContext = createContext({
    postId:null,
    value: 0,
});

export default NewBidContext;