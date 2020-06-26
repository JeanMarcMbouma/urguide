import React, { Component, useState, useMemo, useEffect } from "react";
import Posts from "./Posts";
import Galleries from "./Galleries";
import UpperSection from "./UpperSection";
import Reviews from "./Reviews";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useParams,
    useRouteMatch
} from "react-router-dom";
import UserContext from "./../UserContext";
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from "../../httpclient";
import { UsersClient } from "../../api";


function ProfileLayout() {

    let { path } = useRouteMatch();
    let { userId } = useParams();
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


    useEffect(() => {

        let doWork = async () => {
            var api = HttpClientFactory.get(UsersClient);
            var data = await api.info(userId);

            setValues({
                userId: data.id,
                profileImage: data.profileImage,
                username: `${data.firstName} ${data.lastName}`,
                location: `${data.city}, ${data.country}`,
                description: data.description,
                loading: false,
                rating: data.rating
            });

        }

        doWork();
        return () => { };

    }, [user]);

    return (

        <div className="container-fluid user-page-container">
            <div className="row">
                <div className="col-12">
                    <UpperSection values={values} visitor={true} /> 
                </div>
            </div>
            <Switch>
                <Route exact path={path} >
                    <Reviews userId={userId} />
                </Route>
                <Route path={`${path}/posts`}>
                    <Posts />
                </Route>
                <Route path={`${path}/galleries`}>
                    <Galleries />
                </Route>
            </Switch>

        </div>
    );
}

export default class Guide extends Component {
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
