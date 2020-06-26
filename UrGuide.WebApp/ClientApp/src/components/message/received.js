import React, { Component, useState, useMemo } from "react";
import {
    CardHeader,
    CardContent,
    CardActions,
    Avatar,
    Typography,
    Button
}
    from "@material-ui/core";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useRouteMatch,
    Link
} from "react-router-dom";
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import AuthRoute from "../api-authorization/AuthRoute";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider } from "../../data/GlobalDataContext";
import "./message.css";
import UserContext from "../UserContext";


function MessageReceivedLayout() {

    let { path } = useRouteMatch();
    const user = useAuthUser();

    const [values, setValues] = useState({
        userId: null,
        profileImage: null,
        username: null,
        location: null,
        description: null,
        loading: true,
        rating: 0,
    });

    useMemo(async () => {

        if (!user)
            return;
        var client = HttpClientFactory.getClient(user);
        var data = await client.getdetails();
        setValues({
            userId: data.id,
            profileImage: data.profileImage,
            username: `${data.firstName} ${data.lastName}`,
            location: `${data.city}, ${data.country}`,
            description: data.description,
            loading: false,
            rating: data.rating
        });

    }, [user]);

    return (
        <DataContextProvider>
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-12 col-md-8 col-lg-6 col-xl-6 conversation-layout">
                        <div className="sender-infos">
                            <CardHeader
                                avatar={<Avatar alt={'H'} src={'...'} />}
                                title={<><Link to='/user'><span className="sender-name">{'Jean Edgard'}</span></Link>  <span className="btn-reply-span"><Button variant="contained" color="secondary">Reply <span className="email-icon"><MailOutlineIcon fontSize="small"></MailOutlineIcon></span> </Button></span></>}
                            />
                        </div>
                        <div className="message-sent-layout">
                            <p className="text-muted message-time">18-may-2020 14:50:43</p>
                            <div className="message-sent" >
                                <p className="message-text">
                                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </DataContextProvider>
    );
}

export default class Received extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: [],
            error: null,
            isLoaded: false,
        };

    }


    render() {
        return (<UserContext.Provider value={{ userData: this.state.data }} >
            <MessageReceivedLayout />
        </UserContext.Provider>);
    }

}

