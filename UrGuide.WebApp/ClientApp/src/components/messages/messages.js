import React, { Component, useState, useMemo } from "react";
import Contacts from "./contacts";
//import Conversation from "./conversation";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useRouteMatch,
} from "react-router-dom";
import AuthRoute from "../api-authorization/AuthRoute";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider } from "../../data/GlobalDataContext";
import "./messages.css";
import UserContext from "../UserContext";


const mocksdata = [{
    id: 'c9954964-4889-42fd-81d7-5896174bb7e9',
    firtsName: 'Edward',
    lastName: 'Doe',
    lastMsg: 'Hi ! I sent you a message yesterday. Did you see it?',
    lastMsgDate: '17-May-2020 18:06:49',
    profilePic: 'https://raw.githubusercontent.com/Ashwinvalento/cartoon-avatar/master/lib/images/female/68.png',
},
    {
        id: '8cf4726b-2994-4d58-9c37-5dfe419d77d6',
        firtsName: 'Fabrcie',
        lastName: 'Bruno',
        lastMsg: 'Cool. I just wanted to know.',
        lastMsgDate: '17-May-2020 12:10:08',
        profilePic: 'https://pickaface.net/gallery/avatar/unr_randomavatar_170412_0236_9n4c2i.png',
    },
    {
        id: '9fec4925-9975-441f-9b00-98ae2deb705c',
        firtsName: 'Stephan',
        lastName: 'Anderson',
        lastMsg: 'Are pets allowed ?',
        lastMsgDate: '17-May-2020 09:22:10',
        profilePic: 'https://picsum.photos/id/237/200/300',
    },
    {
        id: '5fabf744-b0ad-4f49-80bf-50946bb19a15',
        firtsName: 'Jean Edgard',
        lastName: 'Hangban',
        lastMsg: 'Can i get a refund ? I just feel dizzy today. I do not think i will be there.',
        lastMsgDate: '17-May-2020 09:02:47',
        profilePic: 'https://picsum.photos/seed/picsum/200/300',
    },
    {
        id: 'a5d16a84-9e19-474d-8390-e860366e71e3',
        firtsName: 'Kevin',
        lastName: 'Miller',
        lastMsg: 'Ok. no problem.',
        lastMsgDate: '16-May-2020 19:30:59',
        profilePic: 'https://i.picsum.photos/id/1022/6000/3376.jpg',
    },
    {
        id: '3af1c6c2-2757-4b63-a9e7-eb31df2f8017',
        firtsName: 'Sydney',
        lastName: 'Mcdonnel',
        lastMsg: 'I am sorry. We wanted 2 seats not 3. We do not have a child.',
        lastMsgDate: '15-May-2020 07:35:51',
        profilePic: 'https://raw.githubusercontent.com/Ashwinvalento/cartoon-avatar/master/lib/images/female/68.png',
    },
    {
        id: '262a6598-923c-4eb9-9870-3fb3877f2c14',
        firtsName: 'Yacine',
        lastName: 'Mohammad',
        lastMsg: 'Just tell me more please.',
        lastMsgDate: '14-May-2020 16:12:08',
        profilePic: 'https://pickaface.net/gallery/avatar/unr_random_180527_1151_2bcb7h9.png',
    },
    {
        id: 'cc81090a-4e0a-4f8d-a184-9599ea89feb8',
        firtsName: 'Constantine',
        lastName: 'Toure',
        lastMsg: 'How much for this tour ? 49$?',
        lastMsgDate: '10-May-2020 22:10:49',
        profilePic: 'https://pickaface.net/gallery/avatar/unr_random_180410_1905_z1exb.png',
    },
    {
        id: 'a56f7219-2a7e-4da1-bf41-05e3d44ce0c0',
        firtsName: 'Roman',
        lastName: 'Ivanovic',
        lastMsg: 'Today??? If true, i Will be there at 02PM',
        lastMsgDate: '02-May-2020 13:56:50',
        profilePic: 'data:image/jpeg;base64,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',
    },
    {
        id: 'b87a9255-7590-4ac8-9150-67906e137361',
        firtsName: 'Celine',
        lastName: 'Augustine',
        lastMsg: 'Okay not a big deal. I got this.',
        lastMsgDate: '02-March-2020 10:22:40',
        profilePic: 'https://raw.githubusercontent.com/Ashwinvalento/cartoon-avatar/master/lib/images/female/10.png',
    },

]

function MessagesLayout() {

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
            <div className="container messages-layout-div">
                <div className="row">
                    <div className="col-md-4">
                        <Contacts />
                    </div>
                    <div className="col-12 col-md-8">
                       
                    </div>
                </div>
              
            </div>
        </DataContextProvider>
    );
}

export default class Messages extends Component {
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
            <MessagesLayout />
        </UserContext.Provider>);
    }

}



//<Switch>
//    <Route exact path={path} >
//        <NoConversation />
//    </Route>
//    <AuthRoute path={`${path}/:userId/`}>
//        <Conversation />
//    </AuthRoute>
//</Switch>