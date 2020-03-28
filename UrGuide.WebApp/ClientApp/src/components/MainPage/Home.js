import React from 'react';
import Header from './Header';
import Popular from './LeftSidebar/Popular';
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles(theme => ({
  body:{
    // backgroundColor:"pink"
  }
}))

const Home = () => {
  const classes = useStyles();
  return (
    <div className={(classes.body)}>
      <Header />
      <Popular />
    </div>
  )
}

export default Home;