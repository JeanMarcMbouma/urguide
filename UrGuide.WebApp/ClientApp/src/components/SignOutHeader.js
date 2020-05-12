import React, { Component, useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import SearchIcon from '@material-ui/icons/Search';
import IconButton from '@material-ui/core/IconButton';
import { FiLogIn } from 'react-icons/fi';
import { TiHomeOutline } from 'react-icons/ti';
import { Link } from 'react-router-dom';
import "./NavMenu.css";
import { NavbarBrand, Container } from 'reactstrap';
import { useAuthContext } from './api-authorization/AuthService';

const useStyles = makeStyles(() => ({
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
}));



function SignOutHeader() {

    const { manager } = useAuthContext();

    function signIn(e) {
        e.preventDefault();
        manager.signIn(window.location.href);
        return false;
    }

    const classes = useStyles();
    return (
        <>
            <nav className='navigation-bar' >
                <div className="container-fluid" >
                    <div className="row justify-content-between navbarRow">
                        <div className="col-6 col-sm-3 col-lg-3">
                            <NavbarBrand tag={Link} to="/" className={(classes.font)}>UrGuide</NavbarBrand>
                        </div>
                        <div className="col-6 col-sm-6 col-md-6 col-lg-4"  >
                            <div className='row justify-content-end signout-links-row'>
                                <div className='col-4 col-sm-2 text-center'>
                                    <Link to="/feed"  >
                                        <IconButton>
                                            <HomeOutlinedIcon fontSize="inherit" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-4 col-sm-2 text-center'>
                                    <Link to="/discover"  >
                                        <IconButton>
                                            <SearchIcon fontSize="inherit" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-4 col-sm-4 text-center'>
                                    <Link to='/sign-in' onClick={signIn}>
                                        <span className='signout-link' >Sign In</span><span className='nav-icon-link'><FiLogIn /></span>
                                    </Link>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </nav>

        </>
    )
}

export default SignOutHeader;