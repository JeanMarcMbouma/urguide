import React, { createContext } from "react";

const SearchContext = createContext({
    pageNumber: 1,
    itemsCount:0,
    items: [],
});

export default SearchContext;
