import React, { createContext } from "react";

var sample = new Array();

const SearchContext = createContext({
    data: sample,
});

export default SearchContext;
