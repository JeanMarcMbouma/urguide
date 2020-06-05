import React, { createContext } from "react";

const FeedBackContext = createContext({
    postId: null,
    userFeedback:null,
    feedbacks: [], 
    textError: false,
});

export default FeedBackContext;