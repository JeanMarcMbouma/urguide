import React, { useState, useEffect, useContext } from 'react';
import { Avatar, CardHeader, CircularProgress } from '@material-ui/core';
import { HttpClientFactory } from '../httpclient';
import { NotificationsClient } from '../api';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import SearchIcon from '@material-ui/icons/Search';
import NotificationsNoneOutlinedIcon
  from '@material-ui/icons/NotificationsNoneOutlined';
import PersonIcon from '@material-ui/icons/Person';
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import {makeStyles} from '@material-ui/core/styles';
import IconButton from '@material-ui/core/IconButton';
import Badge from '@material-ui/core/Badge';
import { Link } from 'react-router-dom';
import "./NavMenu.css";
import { NavbarBrand } from 'reactstrap';
import NotificationsReducer from './NotificationsReducer';
import { useReducer } from 'react';
import NotificationsContext from './NotificationsContext';
import { useAuthContext } from './api-authorization/AuthService';
import { FiLogOut } from 'react-icons/fi';
import Logo from '../Logo.png'
import { SignalRClient } from '../hub';


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
  logo: {
      width: '155px',
      height: '55px',
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




export default function Header() {

   
    const [show, setShow] = useState(false);
    const [unread, setUnread] = useState(0);
    const [pageNumber, setpageNumber] = useState(2);
    const ctx = useContext(NotificationsContext);
    const [state, dispatch] = useReducer(NotificationsReducer, ctx);

    const { manager, user } = useAuthContext();

    const { profile } = user || {
        profile: {}
    };



    async function loadMoreNotifications(e) {
        var obj = e.target;
        if (obj.scrollTop === (obj.scrollHeight - obj.offsetHeight)) {

            if (user === null)
                return;
            const client = HttpClientFactory.get(NotificationsClient, user);
            try {

                var result = await client.all(pageNumber);
                if (result.items.length > 0)
                {
                    dispatch({
                        type: "more",
                        data: {

                            itemsCount: result.itemsCount,
                            pageNumber: result.pageNumber,
                            items: result.items,
                        }
                    });

                    setpageNumber(result.pageNumber + 1);
                    console.log(pageNumber);

                }
               
            }
            catch (e) {
                console.log(e);
            }

            
        }
    }

    async function clickedNotification(notificationId, redirectUrl) {
        if (user === null)
            return;

        console.log(user);
        const client = HttpClientFactory.get(NotificationsClient, user);
        await client.mark_as_read(notificationId).then((status) => {
            alert(status);
            if (unread > 0 && status) {

                setUnread(unread - 1);

            }

            window.location.replace(redirectUrl);

          });
       
    }

    SignalRClient.get((userId, notification) => {

        if (!profile)
            return;

        if (userId === profile.sub) {
            setUnread(unread + 1);
            dispatch({
                type: "unread",
                data: {
                    notification: notification,
                    itemsCount: state.itemsCount,
                    pageNumber: state.pageNumber,
                    items: state.items,
                }
            });
        }
        
    }, user);

    useEffect(() => {
        var fetch = async () => {

            if (user === null)
                return;

            console.log(user);
            const client = HttpClientFactory.get(NotificationsClient, user);
            try {

                var result = await client.all(1);
                dispatch({
                    type: "all",
                    data: {

                        itemsCount: result.itemsCount,
                        pageNumber: result.pageNumber,
                        items: result.items,
                    }
                });

                //setUnread(result.itemsCount);

            }
            catch (e) {
                console.log(e);
            }
        };
        fetch();
        return () => { };
    }, [user]);

    function ToggleNotifications() {

        setUnread(0);
        setShow(!show);
    }

    async function signOut(e) {
        e.preventDefault();
        await manager.signOut();
        return false;
    }

    const classes = useStyles();

    return (
        <>
            <nav className='navigation-bar' >
                <div className="container-fluid" >
                    <div className="row justify-content-between navbarRow">
                        <div className="col-4 col-md-2 col-sm-3 col-lg-3 logo">
                            <NavbarBrand className={classes.font} href="/"><img className={classes.logo} src={ Logo } alt='Logo' /></NavbarBrand>
                        </div>
                        <div className="col-8 col-sm-6 col-md-4 col-lg-4 centered-div"  >
                            <div className='row justify-content-end'>
                                <div className='col-3 col-md-3 col-lg-3 mid-2 text-center'>
                                    <Link to="/feed"  >
                                        <IconButton onClick={(e) => ActivateLink(e)}>
                                            <HomeOutlinedIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-2 text-center'>
                                    <Link to="/discover/nearme"  >
                                        <IconButton onClick={(e) => ActivateLink(e)}>
                                            <SearchIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-3 text-center'>
                                    {profile.role === "guide" ? <Link to="/profile"  >
                                        <IconButton onClick={(e) => ActivateLink(e)} >
                                            <PersonIcon fontSize="large" />
                                        </IconButton>
                                    </Link> : <Link to="/account/details"  >
                                            <IconButton onClick={(e) => ActivateLink(e)} >
                                                <PersonIcon fontSize="large" />
                                            </IconButton>
                                        </Link>}
                                </div>
                                <div className='col-3 col-md-3 col-lg-3 mid-3 text-center'  >
                                    <Link to="/messages">
                                        <IconButton onClick={(e) => ActivateLink(e)}>
                                            <MailOutlineIcon fontSize="large" />
                                        </IconButton>
                                    </Link>
                                </div>
                            </div>
                        </div>
                        <div className="col-6 col-sm-6 col-md-4 d-flex justify-content-end mr-5 right-div">
                            <div className='row'>
                                {
                                    profile.role === "guide" ? <Link to="/profile">
                                        <div className='col-2 col-sm-6 col-md-3 userImage'>
                                            <IconButton className={classes.avatarButton}>
                                                <Avatar className={(classes.avatar)} src={profile.picture} />
                                            </IconButton>
                                        </div>
                                    </Link>

                                        :

                                        <Link to="/account/details">
                                            <div className='col-2 col-sm-6 col-md-3 userImage'>
                                                <IconButton className={classes.avatarButton}>
                                                    <Avatar className={(classes.avatar)} src={profile.picture} />
                                                </IconButton>
                                            </div>
                                        </Link>
                                }

                                <div className='col-4 col-md-3 col-lg-4 username'>
                                    <span>{ user.profile.given_name }</span>
                                </div>
                                <div className='col-1 col-sm-1 d-flex justify-content-between' >
                                    <div>
                                        <IconButton onClick={ToggleNotifications}>
                                            <Badge badgeContent={unread} max={9} color="error">
                                                <NotificationsNoneOutlinedIcon />
                                            </Badge>
                                        </IconButton>
                                    </div>
                                    <div>
                                        <IconButton onClick={signOut}>
                                            <FiLogOut />
                                        </IconButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className='row justify-content-end'>
                        {show ? <div className='col-12'>
                            <div className="notification_dd">
                                <div className='notification_label'>
                                    <h5>Notifications</h5>
                                </div>
                                <ul className="notification_ul" onScroll={(e) => loadMoreNotifications(e)}>
                                    {state.itemsCount > 0 ? state.items.map((notification, i) => (<li key={i} className="notification_li" onClick={() => {

                                        if (!state.items[i].read) {

                                            state.items[i].read = true;
                                            dispatch({
                                                type: "clicked",
                                                data: {
                                                    itemsCount: state.itemsCount,
                                                    pageNumber: state.pageNumber,
                                                    items: state.items,
                                                    notificationId: notification.id,
                                                    markasread: clickedNotification,
                                                    redirectUrl: notification.referenceLink,
                                                }
                                            });

                                        }
                                 }}>
                              
                                            <div className="container">
                                                <div className="row notification_row">
                                                    <div className="col-2">
                                                        <Avatar alt={'P'} src={notification.authorImage} />
                                                    </div>
                                                    <div className="col-10">
                                                        <div className="row">
                                                            <div className="col-12">
                                                                {notification.read ? <><p dangerouslySetInnerHTML={{ __html: notification.content }} className="block-with-text" /><div className="notification_time" >{notification.created}</div></> : <><p className="block-with-text_unread" dangerouslySetInnerHTML={{ __html: notification.content }} /><div className="notification_time_unread" >{notification.created}</div></>}
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                    </li>)) : <div style={{ marginLeft: `-20px` }}><br /><h5 className='text-center text-muted'>No notifications yet.</h5></div>}
                                </ul>
                            </div>
                        </div> : null }
                        
                    </div>
                </div>
        </nav>
       
        </>
    )
}


//function Loading() {
//    return (<div className="loading-icon"><h6 className="text-center"><CircularProgress ></CircularProgress></h6></div>);
//}