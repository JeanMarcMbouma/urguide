import React, { Component } from 'react';
//import { Container } from 'reactstrap';
import Header from './Header';
import SignOutHeader from './SignOutHeader';
import './NavMenu.css';
import { makeStyles } from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';
import { useAuthContext } from './api-authorization/AuthService';
import { useDataContext, ActionTypes } from '../data/GlobalDataContext';

const useStyles = makeStyles(theme => ({
    root: {
        width: '100%',
        '& > * + *': {
            marginTop: theme.spacing(2),
         
        },
        zIndex: 3,
        position:'fixed'
    },
}));

function LinearIndeterminate() {
    const classes = useStyles();

    return (
        <div className={classes.root} id='progress-bar'>
            <LinearProgress />
        </div>
    );
}

function Navbar() {

    const { user } = useAuthContext();

    return (user ? <Header /> : <SignOutHeader />);

}

export function Loader() {

    const { dataContext } = useDataContext();
    return (<>
        {dataContext.loadingcompleted ? null : <LinearIndeterminate />}
    </>);
}

export default class Layout extends Component {

    render(){

       
        return (
            <>
               <Loader/>
                <Navbar />
                <div className="container-fluid content">

                    <div className='row' >
                        <div className='col-12'>
                            {this.props.children}
                        </div>
                    </div>
                </div>
            </>
        );

    }
   
}

