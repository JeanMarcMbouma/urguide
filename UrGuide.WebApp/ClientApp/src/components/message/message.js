import React, { Component, useState, useMemo, useReducer, useContext, useEffect } from "react";
import {
    CardHeader,
    Chip,
    Avatar,
    Typography,
    Button,
    TextField,
    Paper
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
import { BsSearch } from 'react-icons/bs';
import { AiOutlineStop } from 'react-icons/ai';
import authService, { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider, useDataContext, ActionTypes } from "../../data/GlobalDataContext";
import "./message.css";
import UserContext from "../UserContext";
import { UsersClient, NotificationsClient, ChatMessage } from "../../api";
import MessageContext from "./MessageContext";
import MessageReducer from "./MessageReducer";




function MessageLayout() {

    const user = useAuthUser();
    const [show, setShow] = useState(false);
    const [chipped, setChip] = useState(false);
    const [disabled, setDisabled] = useState(true);
    const [selectedUser, setSelectedUser] = useState({});
    const [message, setMessage] = useState({ selectedId: null, msg: '' });
    const ctx = useContext(MessageContext);
    const [state, dispatch] = useReducer(MessageReducer, ctx);
    const { dcReducer } = useDataContext();

    useEffect(() => {
        let fetch = async () => {
            dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: true, url: "/message" } });
        }
        fetch();
        return () => { };
    }, [])

    function handleChangedValue(event) {

        var value = event.target.value;
        setMessage({ ...message, ["msg"]: value });
        if (message.msg.length > 4 && message.selectedId != null ) {
            setDisabled(false);
        }
        else {
            setDisabled(true);
        }

    };
    function navigateToReturnUrl(returnUrl) {

        window.location.replace(returnUrl);
    }


    async function sendMessage(state) {
        if (!user)
            return;
        var api = HttpClientFactory.get(NotificationsClient, user);
        var model = new ChatMessage({ to: state.receiverId, content: state.content, referenceLink: null });
        var url = window.location.href;
        navigateToReturnUrl(url);
        try {
            await api.chat(model);
        }
        catch (e) {
            console.log(e);
        }
    }

    function sendString(suggestion) {
        document.getElementById("search-contact").value = suggestion.firstName + " " + suggestion.lastName;
        setSelectedUser(suggestion);
        setMessage({ ...message, ["selectedId"]: suggestion.id });
        if (message.msg.length > 4 && message.selectedId != null) {
            setDisabled(false);
        }
        else {
            setDisabled(true);
        }
        setChip(true);

    }

    var api = HttpClientFactory.get(UsersClient);

    async function handleChange() {
        setShow(true);
        var search = document.getElementById("search-contact").value;

        api.search(search,false, 1).then((result) => {

            dispatch({
                type: "suggestions",
                data: {
                    items: result.items,
                }
            });
        }); 

       
    }

    return (
        <DataContextProvider>
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-11 col-sm-10 col-md-8 col-lg-5 col-xl-5 conversation-layout">
                        <div className="sender-infos">
                            <div className="search-container">
                                <input type="text" placeholder="Search contact" autoComplete="off" id="search-contact" onChange={ async () => handleChange()} onBlur={() => setShow(false)} className="searchbar" />
                            </div>
                            {show ? <Paper className="search-suggestions">
                                <div className="container-fluid">
                                    <div className="row">
                                        {state.items.length === 0 ? <div className="col-12 suggestion-line" >
                                            <AiOutlineStop className="suggestion-icon" />  <span className="suggestion-text-not-found">No user found.</span>
                                        </div> :
                                            <>
                                                {
                                                    state.items.map((suggestion, i) => (
                                                        <div className="col-12 suggestion-line-contact" onMouseOver={() => sendString(suggestion)} key={i}>
                                                            <CardHeader
                                                                avatar={<Avatar alt={suggestion.fullName} src={suggestion.profileImage} />}
                                                                title={<span className="suggestion-text">{suggestion.fullName}</span>}
                                                            />
                                                        </div>))
                                                }
                                               </>
                                           }
                                    </div>
                                </div>
                            </Paper> : null}
                            <br />
                            {
                                chipped ? <Chip
                                    color="primary"
                                    avatar={<Avatar alt={selectedUser.fullName} src={selectedUser.profileImage} />}
                                    label={selectedUser.fullName}
                                    onDelete={() => {
                                        setChip(false);
                                        setMessage({ ...message, ["selectedId"]: null });
                                    }}
                                /> : null
                            }
                       
                            <br />
                            <br />
                            <TextField fullWidth value={message.msg} onChange={(e) => handleChangedValue(e)} multiline rows={12} rowsMax={12} id="outlined-basic" variant="outlined" placeholder="Write your message here !" />
                            <br />
                            <br />
                            {disabled ? <Button fullWidth variant="contained" color="primary" disabled >send message</Button> : <Button fullWidth variant="contained" color="primary" onClick={() => dispatch({
                                type: "send",
                                data: {
                                    receiverId: message.selectedId,
                                    content: message.msg,
                                    callback: sendMessage,
                                }
                            })}>send message</Button>}
                        </div>
                    </div>
                </div>
              
            </div>
        </DataContextProvider>
    );
}

export default class Message extends Component {
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
            <MessageLayout />
        </UserContext.Provider>);
    }

}
