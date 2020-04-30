import React, { Component } from 'react';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import SearchIcon from '@material-ui/icons/Search';
import NotificationsNoneOutlinedIcon
  from '@material-ui/icons/NotificationsNoneOutlined';
import PersonIcon from '@material-ui/icons/Person';
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import {makeStyles} from '@material-ui/core/styles';
import IconButton from '@material-ui/core/IconButton';
import { Link } from 'react-router-dom';
import "./NavMenu.css";
import { NavbarBrand, Container } from 'reactstrap';
//import ProfileContext from './Reducers/ProfileContext';
//import ProfileReducer from './Reducers/ProfileReducer';
import { Avatar } from '@material-ui/core';
//import {AuthenticationContext, useReactOidc} from '@axa-fr/react-oidc-context';

const useStyles = makeStyles (() => ({
  header: {
    display: 'grid',
    gridTemplateColumns: '30% 15% 15% 25% 5% 7%',
    padding: '6px 5px 6px 10px',
    alignItems: 'center',
    marginBottom: '40px',
  },
  font: {
    fontSize: '28px',
    fontWeight: 'bold',
    margin: '0px',
    padding: '0px',
  },
  avatar: {
    width: '40px',
    height: '40px',
  },
  avatarButton: {
    marginTop: '-5px',
  },
}));

const Header = () => {


    const classes = useStyles();
    return (
        <nav className='navigation-bar'>
            <div className="container-fluid" >
                <div className="row justify-content-between navbarRow">
                    <div className="col-6 col-sm-3 col-lg-3">
                        <NavbarBrand tag={Link} to="/" className={(classes.font)}>UrGuide</NavbarBrand>
                    </div>
                    <div className="col-8 col-sm-6 col-md-6 col-lg-5 centered-div"  >
                        <div className='row justify-content-center'>
                            <div className='col-3 col-md-3 col-lg-2 mid-1'>
                                <Link to="/">
                                    <IconButton>
                                        <HomeOutlinedIcon fontSize="large" />
                                    </IconButton>
                                </Link>

                            </div>
                            <div className='col-3 col-md-3 col-lg-2 mid-2'>
                                <Link to="/explorer">
                                    <IconButton>
                                        <SearchIcon fontSize="large" />
                                    </IconButton>
                                </Link>
                            </div>
                            <div className='col-3 col-md-3 col-lg-2 mid-3'>
                                <Link to="/user">
                                    <IconButton>
                                        <PersonIcon fontSize="large" />
                                    </IconButton>
                                </Link>
                            </div>
                            <div className='col-3 col-md-3 col-lg-2 mid-3'>
                                <IconButton>
                                    <MailOutlineIcon fontSize="large" />
                                </IconButton>
                            </div>
                        </div>
                    </div>
                    <div className="col-6 col-sm-3 col-md-3 right-div">
                        <div className='row justify-content-start'>
                            <Link to="/user">
                                <div className='col-3 col-sm-6 col-md-3'>
                                    <IconButton className={classes.avatarButton}>
                                        <Avatar className={(classes.avatar)} src='https://img.favpng.com/20/5/24/social-media-computer-icons-avatar-user-internet-png-favpng-DwdFSAXdR58nGmLe4y67jEej0.jpg' />
                                    </IconButton>
                                </div>
                            </Link>

                            <div className='col-4 col-md-3 col-lg-4 username'>
                                <span>MrIhor</span>
                            </div>
                            <div className='col-3 col-sm-3 col-md-2' >
                                <IconButton>
                                    <NotificationsNoneOutlinedIcon />
                                </IconButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </nav>

    )
}

export default Header;