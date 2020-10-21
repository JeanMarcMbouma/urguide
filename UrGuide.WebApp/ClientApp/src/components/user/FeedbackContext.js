import React, { createContext } from "react";

const FeedBackContext = createContext({
    postId: null,
    userFeedback:null,
    feedbacks: [], 
    textError: false,
    items: [],
    pageNumber: 1,
    itemsCount:0,
});

export default FeedBackContext;