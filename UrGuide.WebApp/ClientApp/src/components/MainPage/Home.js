<<<<<<< HEAD
import React from 'react'
import LeftBar from "./LeftBar/LeftBar"
import CentralBar from "./CentralBar/CentralBar"
export default function Home() {
    return (
        <div className="col-lg-12 row">
            <LeftBar />
            <CentralBar />
            {/* RightBar */}
        </div>
    )
}
=======
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
>>>>>>> af79794e9d4cc2ef7a17b71320063e79a7b80b61
