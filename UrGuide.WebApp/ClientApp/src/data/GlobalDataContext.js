import React, { createContext, useReducer, useContext, useCallback } from "react";
import { defaultState } from "../components/api-authorization/AuthService";


const initialData = {
    posts: [],
    categories: [],
    top: [],
    discover: [],
    cat: null,
    profile: null,
    loading: false,
    hasuser: false,
    loadingcompleted: false,
    url: null,
    profileUrl:null,
}

export const ActionTypes = {
    POSTS: 'posts',
    CATEGORIES: 'categories',
    TOP: 'top',
    DISCOVER:'discover',
    PROFILE: 'profile',
    LOADING: 'loading',
    RESET: 'reset',
    LOADINGCOMPLETED: 'loadingiscompleted',
 
};

export const DataContext = createContext(initialData);

const dataContextReducer = (state, action) => {
    switch (action.type) {
        case ActionTypes.POSTS:
            return { ...state, posts: action.data, loading: false };
        case ActionTypes.CATEGORIES:
            return { ...state, categories: action.data, loading: false };
        case ActionTypes.DISCOVER:
            return { ...state, discover: action.data.items, url: action.data.url, cat: action.data.cat, loading: false };
        case ActionTypes.PROFILE:
            return { ...state, profile: action.data, loading: false };
        case ActionTypes.TOP:
            return { ...state, top: action.data, loading: false };
        case ActionTypes.LOADINGCOMPLETED:
            if (state.posts.length > 0 && action.data.url === "/feed" || state.discover.length > 0 && action.data.url === "/discover" && state.cat === action.data.cat) {
                action.data.completed = true;
            }
            if (action.data.profileUrl) {
                return { ...state, loadingcompleted: action.data.completed, url: action.data.url, profileUrl: action.data.profileUrl, loading: false, };
            }
            return { ...state, loadingcompleted: action.data.completed, url: action.data.url, loading: false, };
        case ActionTypes.RESET:
        case ActionTypes.RESET:
            var r = { ...state };
            if (r.hasuser && !action.data)
                return { ...defaultState };
            if (!r.hasuser && action.data) {
                return { ...defaultState };
            }
            return r;
        default:
            return { ...state, loading: true }
    }
}

export const DataContextProvider = (props) => {
    console.log('Creating DataContextProvider');
    const [reducer, dispatch] = useReducer(dataContextReducer, initialData);
    const resetCallback = useCallback((user) => dispatch({ type: ActionTypes.RESET, data: user }), []);
    return <DataContext.Provider value={{ dataContext: reducer, dcReducer: dispatch, resetCallback }}>
        {props.children}
    </DataContext.Provider>
}

export const useDataContext = () => React.useContext(DataContext);