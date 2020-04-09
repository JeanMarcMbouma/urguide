import React, { useContext, useReducer } from 'react';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import SearchIcon from '@material-ui/icons/Search';
import NotificationsNoneOutlinedIcon from '@material-ui/icons/NotificationsNoneOutlined';
import PersonIcon from '@material-ui/icons/Person';
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import { makeStyles } from "@material-ui/core/styles";
import IconButton from '@material-ui/core/IconButton';
import "./NavMenu.css";
//import ProfileContext from './Reducers/ProfileContext';
//import ProfileReducer from './Reducers/ProfileReducer';
import { Avatar } from '@material-ui/core';

const useStyles = makeStyles(theme => ({
    header: {
        display: 'grid',
        gridTemplateColumns: '30% 15% 15% 25% 5% 7%',
        padding: '6px 5px 6px 10px',
        alignItems: 'center',
        marginBottom: '40px'
    },
    font: {
        fontSize: '17px'
    },
    avatar: {
        width: '40px',
        height: '40px',
       
    },
    avatarButton: {
        marginTop: '-5px'

}
}));

const Header = () => {
  //const ctx = useContext(ProfileContext);
  //const [state, dispatch] = useReducer(ProfileReducer, ctx);

    const classes = useStyles();
  return (
          <nav className='navigation-bar'>
          <div class="container-fluid" >
              <div class="row justify-content-between navbarRow">
                      <div class="col-2 col-sm-3 col-md-3 left-div">
                          <a title="Urguide" >
                              <div className={(classes.font)}><b>UrGuide</b></div>
                          </a>
                      </div>

                  <div class="col-8 col-sm-6 col-md-6 col-lg-5 centered-div"  >
                      <div className='row justify-content-center'>
                          <div className='col-3 col-md-3 col-lg-2 mid-1'>
                              <IconButton>
                                  <HomeOutlinedIcon />
                              </IconButton>
                          </div>
                          <div className='col-3 col-md-3 col-lg-2 mid-2'>
                              <IconButton>
                                  <SearchIcon/>
                              </IconButton>
                          </div>
                          <div className='col-3 col-md-3 col-lg-2 mid-3'>
                              <IconButton>
                                  <PersonIcon />
                              </IconButton>
                          </div>
                          <div className='col-3 col-md-3 col-lg-2 mid-3'>
                              <IconButton>
                                  <MailOutlineIcon />
                              </IconButton>
                          </div>
                      </div>
                      </div>
                  <div class="col-5 col-sm-3 col-md-3 right-div">
                      <div className='row justify-content-center'>
                          <div className='col-6 col-sm-6 col-md-3'>
                              <IconButton className={classes.avatarButton}>
                                  <Avatar className={(classes.avatar)} src='https://img.favpng.com/20/5/24/social-media-computer-icons-avatar-user-internet-png-favpng-DwdFSAXdR58nGmLe4y67jEej0.jpg' />
                              </IconButton>
                          </div>
                          <div className='col-md-5 username'>
                              <span>MrIhor</span>
                          </div>
                          <div className=' col-6 col-sm-6 col-md-3' >
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