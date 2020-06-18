import React, { Component, useState, useMemo } from "react";
import {
    Card,
    CardHeader,
    CardContent,
    CardActions,
    Avatar,
    IconButton,
    InputLabel,
    Input,
    FormControl,
    InputAdornment,
    FormHelperText,
    Typography,
    Button,
    CardMedia,
    TextField,
    Chip,
    Paper,
    Grid,
    Box,
    FormControlLabel,

} from "@material-ui/core";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useRouteMatch,
    Link,
} from "react-router-dom";
import AuthRoute from "../api-authorization/AuthRoute";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider } from "../../data/GlobalDataContext";
import "./messages.css";

export default function Contacts() {

    let { path } = useRouteMatch();
    const user = useAuthUser();

    //const [values, setValues] = useState({
    //    userId: null,
    //    profileImage: null,
    //    username: null,
    //    location: null,
    //    description: null,
    //    loading: true,
    //    rating: 0,
    //});

    //useMemo(async () => {

    //    if (!user)
    //        return;
    //    var client = HttpClientFactory.getClient(user);
    //    var data = await client.getdetails();
    //    setValues({
    //        userId: data.id,
    //        profileImage: data.profileImage,
    //        username: `${data.firstName} ${data.lastName}`,
    //        location: `${data.city}, ${data.country}`,
    //        description: data.description,
    //        loading: false,
    //        rating: data.rating
    //    });

    //}, [user]);

    return (
        <div className="contacts-panel">
            <div className="container-fluid">
                <div className="row">
                    <div className='col-12'>
                        <CardHeader
                            avatar={<Link to="/"><Avatar alt={'H'} src={'...'} /></Link>}
                            title={<Typography variant="body1" component="p"><Link to='/user'>{'Jean Edgard'}</Link></ Typography>}
                        />
                        <hr/>
                    </div>
                    <div className='col-12' >
                        <Link to="/">
                            <div className='row'>
                                <div className='col-4'>
                                    <Avatar alt={'H'} src={'...'} />
                                </div>
                                <div className='col-8'>
                                    <div className='row'>
                                        <div className='col-12'>
                                            <Typography variant="body1" component="p">{'jean edgard'}</ Typography>
                                        </div>
                                        <div className='col-12 text-justify text-truncate'>
           <p>Cool. but i need my refund cause i don't want to go there anymore.</p>
                                        </div>
                                    </div>
                                </div>
                            </div>



                     </Link>
                    </div>
                </div>
            </div>
        </div>
    );
}


//<CardHeader
//    avatar={<Avatar alt={'H'} src={'...'} />}
//    title={<Typography variant="body1" component="p">{'jean edgard'}</ Typography>}
//    subheader={<div className='container' ><Typography
//        className="text-justify text-truncate"
//        variant="subtitle1"
//        color="textSecondary"
//        component="p">
//        Cool. but i need my refund cause i don't want to go there anymore
//                                </Typography></div>}
///>