import React, { createContext } from "react";

const ActionsContext = createContext({
    post: null,
    posts: [],
    like: false,
});

export default ActionsContext;