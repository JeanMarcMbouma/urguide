import React, { Component } from 'react';
//import { Container } from 'reactstrap';
import Header from './Header';
import SignOutHeader from './SignOutHeader';
import './NavMenu.css';
import { makeStyles } from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';
import { useAuthContext } from './api-authorization/AuthService';


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

export class Layout extends Component {

    constructor(props) {
        super(props);
        this.handleLoad = this.handleLoad.bind(this);
        this.state = { Loading:true}
    }

    componentDidMount() {
        window.addEventListener('load', this.handleLoad);
    }

    componentWillUnmount() {
        window.removeEventListener('load', this.handleLoad)
    }

    handleLoad() {
        this.setState(state => ({
            Loading: !state.Loading
        }));
    }
 

    render() {

        const loader = this.state.Loading ? <LinearIndeterminate /> : null;
  
       return (
           <>
               {loader}
              <Navbar />
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

