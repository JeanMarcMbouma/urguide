import React, { useCallback, useEffect, useState, useMemo } from 'react';
import { UserManager, WebStorageStateStore } from 'oidc-client';
import { ApplicationPaths, ApplicationName, QueryParameterNames } from './ApiAuthorizationConstants';
import Loader from './loader';


const onLoadUser = (dispatch) => (user) => dispatch({ type: 'login', user });
const onUserUnloaded = (dispatch) => () => dispatch({ type: 'logout' });
const onLoggingOut = (dispatch) => () => dispatch({ type: 'token_expired' });
const onLoading = (dispatch) => () => dispatch({ type: 'loading' });



export const UserReducer = (state, action) => {
    let res = { ...state };
    switch (action.type) {
        case 'login':
            res = { ...state, isLoggedIn: true, user: action.user, logginOut: false };
            break;
        case 'loading':
            res = { ...state, loading: true, logginOut: false };
            break;
        case 'logout':
            res = { ...state, user: null, loggingOut: true };
            break;
        case 'token_expired':
            res = { ...state, loggingOut: true };
            break;
        default:
            return { ...state, isLoggedIn: false, user: null, loading: true, logginOut: false };
    }
    console.log(res);
    return res;
}
class AuthService {
    
    constructor() {
        this._mgr = null;
        this._user = null;
        this._onLoadUser = (u) => { };
        this._onLoading = () => { };
        this._onUserUnloaded = (u) => { };
        this._onLoggingOut = () => {}
    }

    async isAuthenticated() {
        const user = await this.getUser();
        return !!user;
    }

    async getUser() {
        if (this._user && this._user.profile) {
            return this._user.profile;
        }

        await this._initManager();
        const user = await this._mgr.getUser();
        var result = user && user.profile;
        if (result && !this._user) {
            this._user = user;
            this._onLoadUser(user);
        }
        return result;
    }


    getReturnUrl(state){
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get(QueryParameterNames.ReturnUrl);
       
        return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
    }

    navigateToReturnUrl(returnUrl){
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        window.location.replace(returnUrl);
    }


    async completeSignIn(returnUrl) {
        try {
            await this._initManager();
            const user = await this._mgr.signinCallback(returnUrl);
            this._onLoadUser(user);
            this.navigateToReturnUrl(this.getReturnUrl(user.state));
        } catch (e) {
            console.log(e);
        }
    }


    async signIn(returnUrl) {
        await this._initManager();
        this._onLoading();
        try {
            const user = await this._mgr.signinSilent();
            this._onLoadUser(user);
            return;
        } catch (e) {
            console.log(e);
            try {
                const user = await this._mgr.signinPopup(this.createArguments());
                this._onLoadUser(user);
                return;
            } catch (e) {
                console.log(e);
            }
        }
        
        await this._mgr.signinRedirect(this.createArguments({ returnUrl }));
    }

    async signOut() {
        await this._initManager();
        await this._mgr.signoutRedirect();
    }

    createArguments(state) {
        return { useReplaceToNavigate: true, data: state };
    }

    async _initManager() {
        if (this._mgr) {
            return;
        }

        let response = await fetch(ApplicationPaths.ApiAuthorizationClientConfigurationUrl);
        if (!response.ok) {
            throw new Error(`Could not load settings for '${ApplicationName}'`);
        }

        let settings = await response.json();
        settings.automaticSilentRenew = true;
        settings.includeIdTokenInSilentRenew = true;
        settings.userStore = new WebStorageStateStore({
            prefix: ApplicationName
        });

        this._mgr = new UserManager(settings);

        this._mgr.events.addUserLoaded((user) => {
            this._onLoadUser(user);
            this._user = user;
        });

        this._mgr.events.addUserUnloaded(() => {
            this._onUserUnloaded(undefined);
            this._user = null;
        });

        this._mgr.events.addAccessTokenExpiring(() => {
            this._onLoggingOut();
        });

        this._mgr.events.addUserSignedOut(async () => {
            await this._mgr.removeUser();
            this._onUserUnloaded(undefined);
            this._user = null;
        });
    }
    static get instance() { return authService; }
} 


const authService = new AuthService();


export default authService;


export const defaultState = {
    user: null,
    isLoggedIn: false,
    loading: false,
    loggingOut: false,
    manager: authService,
    authenticating: (<Loader/>)
}


export const AuthContext = React.createContext(defaultState);

export const useAuthContext = () => {
    return React.useContext(AuthContext);
}

export const AuthContextProvider = (props) => {
    const [reducer, dispatch] = React.useReducer(UserReducer, defaultState);
    reducer.manager._onLoading = useCallback(user => onLoading(dispatch)(user), []);
    reducer.manager._onLoadUser = useCallback(user => onLoadUser(dispatch)(user), []);
    reducer.manager._onUserUnloaded = useCallback(user => onUserUnloaded(dispatch)(user), []);
    reducer.manager._onLoggingOut = useCallback(() => onLoggingOut(dispatch)(), []);
    reducer.manager.isAuthenticated();
    return <AuthContext.Provider value={{...reducer}}>
        {props.children}
    </AuthContext.Provider>
}


export const useAuthUser = () => {
    const { user, manager } = useAuthContext();
    const [authUser, setAuthUser] = useState(user);

    useEffect(() => {
        async function checkUser() {
            if (await manager.isAuthenticated()) {
                if (manager._user && !authUser) {
                    setAuthUser(manager._user);
                }
            }
        }

        checkUser();
        return () => { };
    }, [manager, authUser, user]);

    return authUser;
}

export const useSecure = (component) => {
    const returnUrl = window.location.href;
   // const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`;
    const [allowed, setAllowed] = useState(false);
    const { loading, manager, authenticating, user } = useAuthContext();

    useEffect(() => {
        if (!loading) {
            manager.signIn(returnUrl);
            
        }
        if (user && !allowed) {
            setAllowed(true);
        }
        return () => {
            console.log('done')
        }
    }, [loading, manager, returnUrl, allowed, user]);
    console.log(loading, manager, authenticating, user);
    return !allowed ? authenticating : component;
}