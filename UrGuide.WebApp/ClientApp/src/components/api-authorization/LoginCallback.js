import React from 'react'
import authService from './AuthService';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    positionLoader: {
        top: '45%',
        left: '45%',
        position: 'absolute',
    },
}));


const LoginCallBack = () => {
    
    const url = window.location.href;
    authService.completeSignIn(url);
    return (
        <div className={styles.positionLoader}>
            <CircularProgress />
            <div>Please wait...</div>
        </div>
        );
}

export default LoginCallBack;