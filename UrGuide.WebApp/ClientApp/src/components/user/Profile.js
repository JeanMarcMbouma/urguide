import React, { Component, useState, useMemo } from "react";
import Posts from "./Posts";
import Galleries from "./Galleries";
import EditProfile from "./EditProfile";
import ChangePassword from "./ChangePassword";
import UpperSection from "./UpperSection";
import Reviews from "./Reviews";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useRouteMatch,
} from "react-router-dom";
import { CreateNewGallery } from "./CreateNewGallery";
import UserContext from "./../UserContext";
import AuthRoute from "../api-authorization/AuthRoute";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { DataContextProvider } from "../../data/GlobalDataContext";



function ProfileLayout() {

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
            <div className="container-fluid user-page-container">
                <div className="row">
                    <div className="col-12">
                        <UpperSection values={values} visitor={false} />
                    </div>
                </div>
                <Switch>
                    <Route exact path={path} >
                        <Reviews />
                    </Route>
                    <Route path={`${path}/posts`}>
                        <Posts />
                    </Route>
                    <Route path={`${path}/galleries`}>
                        <Galleries />
                    </Route>
                    <AuthRoute path={`${path}/details`}>
                        <EditProfile isGuide={true} />
                    </AuthRoute>
                    <AuthRoute path={`${path}/password`}>
                        <ChangePassword isGuide={true} />
                    </AuthRoute>
                    <AuthRoute path={`${path}/creategallery`}>
                        <CreateNewGallery />
                    </AuthRoute>
                </Switch>
            </div>
        </DataContextProvider>
    );
}

export default class Profile extends Component {
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
            <ProfileLayout />
        </UserContext.Provider>);
    }

}