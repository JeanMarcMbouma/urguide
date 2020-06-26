import React, { Component, useState, useMemo } from "react";
import {
    CardHeader,
    Chip,
    Avatar,
    Typography,
    Button,
    TextField
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
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider } from "../../data/GlobalDataContext";
import "./message.css";
import "./../discover/DiscoverStyle.css"
import UserContext from "../UserContext";




function MessageLayout() {

    let { path } = useRouteMatch();
    const user = useAuthUser();
    const [show, setShow] = useState(false);
    const [suggestions, setSuggestions] = useState([]);
    const [chipped, setChip] = useState(false);
    const [selectedUser, setSelectedUser] = useState({});
    const [message, setMessage] = useState({selectedId:null, msg:'', userId:null});
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

    const handleChangedValue = prop => event => {
        setMessage({ ...message, [prop]: event.target.value });
    };


    function sendString(suggestion) {
        document.getElementById("search-contact").value = suggestion.firstName + " " + suggestion.lastName;
        setSelectedUser(suggestion);
        setChip(true);
    }

    const result = [{ id: "1020022-s0ssoo-jjama11-3030330", firstName: "Stephanie", lastName: "Kerr", ProfileImage: "data:image/jpeg;base64,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" },
        { id: "nnxbx022-s0ssoo-jjama11-3030330", firstName: "Stephane", lastName: "Doe", ProfileImage: "data:image/jpeg;base64,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" },
        { id: "ppamam200-s0ssoo-jjama11-3030330", firstName: "Steph", lastName: "Smith", ProfileImage: "data:image/jpeg;base64,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" },
        { id: "nbaaaq1-s0ssoo-jjama11-3030330", firstName: "Stephie", lastName: "Johanner", ProfileImage: "data:image/jpeg;base64,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" },
    ]

    function handleChange() {
        setShow(true);
        var search = document.getElementById("search-contact").value;
        var res = result.filter(item => item.firstName.match(`${search}`));
        res = res.slice(0, 10);
        setSuggestions(res);
    }

    return (
        <DataContextProvider>
            <div className="container">
                <div className="row justify-content-center">
                    <div className="col-12 col-md-8 col-lg-6 col-xl-6 conversation-layout">
                        <div className="sender-infos">
                            <div className="search-container">
                                <input type="text" placeholder="Who are you writing to ?" autoComplete="off" id="search-contact" onChange={handleChange} onBlur={() => setShow(false)} className="searchbar" />
                            </div>
                            {show ? <div className="search-suggestions">
                                <div className="container-fluid">
                                    <div className="row">
                                        {suggestions.length > 0 ?
                                            suggestions.map((suggestion, i) => (
                                                <div className="col-12 suggestion-line-contact" onMouseOver={() => sendString(suggestion)} key={i}>
                                                    <CardHeader
                                                        avatar={<Avatar src={suggestion.ProfileImage} />}
                                                        title={<span className="suggestion-text">{suggestion.firstName} {suggestion.lastName}</span>}
                                                    />
                                                </div>))
                                            : <div className="col-12 suggestion-line" >
                                                <AiOutlineStop className="suggestion-icon" />  <span className="suggestion-text-not-found">No user found.</span>
                                            </div>}
                                    </div>
                                </div>
                            </div> : null}
                            <br />
                            {
                                chipped ? <Chip
                                    color="primary"
                                    avatar={<Avatar alt={`${selectedUser.firstName} ${selectedUser.lastName}`} src={selectedUser.profileImage} />}
                                    label={`${selectedUser.firstName} ${selectedUser.lastName}`}
                                    onDelete={() => {
                                        setChip(false);
                                        setMessage({ ...message, ["selectedId"]: null });
                                    }}
                                /> : null
                            }
                        </div>
                        <div className="message-sent-layout">
                            <TextField fullWidth value={message.msg} onChange={handleChangedValue("msg")} multiline rows={5} rowsMax={7} id="outlined-basic" variant="outlined" placeholder="Write your message here !." />
                            <br />
                            <br />
                            <Button fullWidth variant="contained" color="primary">send message</Button>
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
