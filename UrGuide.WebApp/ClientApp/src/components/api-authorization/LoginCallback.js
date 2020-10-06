import React from 'react'
import authService from './AuthService';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';

const LoginCallBack = () => {
    
    const url = window.location.href;
    authService.completeSignIn(url);

    return (
        <div className='container'>
            <div className="row justify-content-center" style={{ marginTop: `28%` }} >
                <div className="col-12">
                    <h6 className="text-center" ><CircularProgress /></h6>

                </div>
            </div>
            <div className="row justify-content-center" >
                <div className="col-12">
                   <h6 className="text-center" >Please wait...</h6>
                </div>
            </div>
        </div>
        );
}

export default LoginCallBack;