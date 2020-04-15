import React, { Component } from 'react';
//import { Container } from 'reactstrap';
import Header from './Header';
import './NavMenu.css';
import { makeStyles } from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';

//const useStyles = makeStyles(theme => ({
//    root: {
//        width: '100%',
//        '& > * + *': {
//            marginTop: theme.spacing(2),
         
//        },
//        zIndex: 3,
//        position:'fixed'
//    },
//}));

//function LinearIndeterminate() {
//    const classes = useStyles();

//    return (
//        <div className={classes.root} id='progress-bar'>
//            <LinearProgress />
//        </div>
//    );
//}

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
      return (
          <>
              <Header />
              <div className="container-fluid content">
              
                  <div className='row mb-4' >
                      <div className='col-12'>
                          {this.props.children}
                </div>
                  </div>
              </div>
         </>
    );
  }
}

