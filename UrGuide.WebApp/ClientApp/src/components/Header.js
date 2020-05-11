import React, { Component, useState } from 'react';
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
import { NavbarBrand } from 'reactstrap';
import NotificationsBox from './NotificationsBox';
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

function ActivateLink(event) {

    var buttons = document.querySelectorAll("button");

    [].forEach.call(buttons, function (el) {
        el.classList.remove("active-icon");
    });

    var divs = document.querySelectorAll("div");

    [].forEach.call(divs, function (el) {
        el.classList.remove("active-div");
    });

    var target = event.target;
    var icon = target.closest("button");
    var div = target.closest("div"); 
    icon.className += ' active-icon';
    div.className += ' active-div';
 
}


function Header() {

    const [show, setShow] = useState(false);

    function ToggleNotifications() {
         setShow(!show);
    }

    const classes = useStyles();
    return (
        <>
            <nav className='navigation-bar' >
                <div className="container-fluid" >
                    <div className="row justify-content-between navbarRow">
                        <div className="col-6 col-sm-3 col-lg-3">
                            <NavbarBrand href="/" className={(classes.font)}>UrGuide</NavbarBrand>
                        </div>
                        <div className="col-8 col-sm-6 col-md-6 col-lg-4 centered-div"  >
                            <div className='row justify-content-end'>
                                <div className='col-3 col-md-3 col-lg-3 mid-2 text-center'>
                                    <Link to="/feed"  >
                                        <IconButton onClick={(e) => ActivateLink(e)}>
                                            <HomeOutlinedIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-2 text-center'>
                                    <Link to="/discover"  >
                                        <IconButton onClick={(e) => ActivateLink(e)}>
                                            <SearchIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-3 text-center'>
                                    <Link to="/user"  >
                                        <IconButton onClick={(e) => ActivateLink(e)} >
                                            <PersonIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-3 text-center'  >
                                    <IconButton onClick={(e) => ActivateLink(e)}>
                                        <MailOutlineIcon fontSize="large" />
                                    </IconButton>
                                </div>
                            </div>
                        </div>
                        <div className="col-6 col-sm-3 col-md-3 right-div">
                            <div className='row justify-content-start'>
                                <Link to="/user">
                                    <div className='col-2 col-sm-6 col-md-3'>
                                        <IconButton className={classes.avatarButton}>
                                            <Avatar className={(classes.avatar)} src='https://img.favpng.com/20/5/24/social-media-computer-icons-avatar-user-internet-png-favpng-DwdFSAXdR58nGmLe4y67jEej0.jpg' />
                                        </IconButton>
                                    </div>
                                </Link>

                                <div className='col-4 col-md-3 col-lg-4 username'>
                                    <span>MrIhor</span>
                                </div>
                                <div className='col-1 col-sm-1' >
                                    <IconButton onClick={ToggleNotifications}>
                                        <NotificationsNoneOutlinedIcon />
                                    </IconButton>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className='row justify-content-end'>
                        {show ? <div className='col-12'>
                            <NotificationsBox />
                        </div> : null }
                        
                    </div>
                </div>
        </nav>
       
        </>
    )
}

export default Header;