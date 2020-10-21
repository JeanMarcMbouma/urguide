import React, { Component, useState, useMemo, useEffect } from "react";
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
    Link,
    useParams
} from "react-router-dom";
import MailOutlineIcon from '@material-ui/icons/MailOutline';
import AuthRoute from "../api-authorization/AuthRoute";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider, useDataContext, ActionTypes } from "../../data/GlobalDataContext";
import "./message.css";
import UserContext from "../UserContext";
import { NotificationsClient, Client, LookupClient, UsersClient } from "../../api";


function MessageReceivedLayout(props) {

    let { path } = useRouteMatch();
    const user = useAuthUser();

    const [value, setValue] = useState({});
    const [author, setAuthor] = useState({});
    const { dcReducer } = useDataContext();


    useMemo(async () => {
        dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: false, url: "/message" } });
        if (!user)
            return;
            var api = HttpClientFactory.get(Client, user);
            var client = HttpClientFactory.get(UsersClient);
            api.notifications(props.msgId).then((notification) => {

                setValue(notification);

                console.log(notification);

                client.info(notification.authorId).then((result) => {
                    setAuthor(result);
                    console.log(result);
                });

                dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: true, url: "/message" } });

            });


    }, [user]);

    return (
        <DataContextProvider>
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-12 col-md-8 col-lg-6 col-xl-6 conversation-layout">
                        <div className="sender-infos">
                            <CardHeader
                                avatar={<Avatar alt={author.fistName} src={author.profileImage} />}
                                title={<><span className="sender-name">{`${author.firstName} ${author.lastName}`}</span>  <span className="btn-reply-span"><Link to="/message" variant="contained" color="primary">NEW MESSAGE <span className="email-icon"><MailOutlineIcon fontSize="small"></MailOutlineIcon></span> </Link></span></>}
                        
                            />
                            <br />
                            <br />
                            <br />
                            <br />
                            <p className="text-muted message-time">{value.created}</p>
                            <div className="message-sent" >
                                <p dangerouslySetInnerHTML={{ __html: value.content }} className="message-text"></p>
                            </div>
                    </div>
                    </div>
                </div>

            </div>
        </DataContextProvider>
    );
}

export default function Received() {
  
        let { msgId } = useParams();

        return (
            <MessageReceivedLayout msgId={msgId} />
       );

}

