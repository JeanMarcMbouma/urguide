import React, { useContext, useReducer } from 'react';
import HomeOutlinedIcon from '@material-ui/icons/HomeOutlined';
import SearchIcon from '@material-ui/icons/Search';
import NotificationsNoneOutlinedIcon from '@material-ui/icons/NotificationsNoneOutlined';
import { makeStyles } from "@material-ui/core/styles";
import ProfileContext from './Reducers/ProfileContext';
import ProfileReducer from './Reducers/ProfileReducer';
import { Avatar } from '@material-ui/core';

const useStyles = makeStyles(theme => ({
  header: {
    display: 'grid',
    gridTemplateColumns: '30% 15% 15% 25% 5% 7%',
    padding: '30px 30px 30px 40px',
    alignItems: 'center',
  },
  font: {
    fontSize: '17px'
  },
  avatar:{
    width:'40px',
    height:'40px'
  }
}));

const Header = () => {
  const ctx = useContext(ProfileContext);
  const [state, dispatch] = useReducer(ProfileReducer, ctx);

  const classes = useStyles();
  return (
    <div className='col-lg-12'>
      <div className={(classes.header )}>
        <div className={(classes.font)}><b>UrGuide</b></div>
        <HomeOutlinedIcon></HomeOutlinedIcon>
        <SearchIcon></SearchIcon>
        <NotificationsNoneOutlinedIcon></NotificationsNoneOutlinedIcon>
        {state.username}
        <div>MrIhor</div>
        <Avatar className={(classes.avatar)} src='https://img.favpng.com/20/5/24/social-media-computer-icons-avatar-user-internet-png-favpng-DwdFSAXdR58nGmLe4y67jEej0.jpg'/>
          {/* <button onClick={() => dispatch({ type: 'logged', data: 'MrIhor'})}>Log in</button>
          <button hidden={!state.isLoggedIn} onClick={() => dispatch({ type: 'unlogged', data: 'MrIhor' })}>Log out</button> */}
      </div>
    </div>
  )
}

export default Header;