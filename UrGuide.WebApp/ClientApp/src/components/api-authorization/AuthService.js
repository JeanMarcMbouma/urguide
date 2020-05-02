import React, { useCallback, useEffect, useState } from 'react';
import { UserManager, WebStorageStateStore } from 'oidc-client';
import { ApplicationPaths, ApplicationName} from './ApiAuthorizationConstants';


const onLoadUser = (dispatch) => (user) => dispatch({ type: 'login', user });
const onUserUnloaded = (dispatch) => () => dispatch({ type: 'logout' });
const onLoggingOut = (dispatch) => () => dispatch({ type: 'token_expired' });
const onLoading = (dispatch) => () => dispatch({ type: 'loading' });



export const UserReducer = (state, action) => {
    switch (action.type) {
        case 'login':
            return { ...state, isLoggedIn: true, user: action.user, loading: false, logginOut: false };
        case 'loading':
            return { ...state, loading: true, logginOut: false };
        case 'logout':
            return { ...state, loading: false, user: null, loggingOut: true };
        case 'token_expired':
            return { ...state, loading: false, loggingOut: true };
        default:
            return { ...state, isLoggedIn: false, user: null, loading: true, logginOut: false };
    }
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

    async signIn(returnUrl) {
        await this._initManager();
        this._onLoading();
        try {
            const user = await this._mgr.signinSilent();
            if (user) {
                return;
            }
        } catch (e) {

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

    isAuthRequired() {
        return this._user == null;
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
    authenticating: (<div>Authenticating...</div>)
}


export const AuthContext = React.createContext(defaultState);

export const useAuthContext = () => {
    return React.useContext(AuthContext);
}

export const useAuth = () => {
    const state = useAuthContext();
    state.manager._onLoading = useCallback(user => onLoading(dispatch)(user), []);
    state.manager._onLoadUser = useCallback(user => onLoadUser(dispatch)(user), []);
    state.manager._onUserUnloaded = useCallback(user => onUserUnloaded(dispatch)(user), []);
    state.manager._onLoggingOut = useCallback(() => onLoggingOut(dispatch)(), []);
    const [reducer, dispatch] = React.useReducer(UserReducer, state);
    return reducer;
}

export const useSecure = (component) => {
    const returnUrl = window.location.href;
   // const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`;
    const [allowed, setAllowed] = useState(false);
    const { loggingOut, manager, authenticating, user } = useAuth();

    useEffect(() => {
        if (!loggingOut) {
            manager.signIn(returnUrl);
            if (user && !allowed) {
                setAllowed(true);
            }
        }

        return () => {
            console.log('done')
        }
    }, [loggingOut, manager, returnUrl, allowed, user]);

    return !allowed ? authenticating : component;
}