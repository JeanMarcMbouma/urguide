import React, { createContext } from "react";

var data = new Array();

const FeedBackContext = createContext({
    postId: null,
    userFeedback:null,
    feedbacks: data, 
    textError: false,
});

export default FeedBackContext;