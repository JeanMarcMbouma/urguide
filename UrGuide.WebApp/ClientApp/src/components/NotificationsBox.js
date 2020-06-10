import React, { useState, useEffect, useContext } from 'react';
import { Avatar, CardHeader, CircularProgress } from '@material-ui/core';
import { HttpClientFactory } from '../httpclient';
import { NotificationsClient } from '../api';
import "./NavMenu.css";
import NotificationsReducer from './NotificationsReducer';
import { useReducer } from 'react';
import NotificationsContext from './NotifcationsContext';


function Notification(props) {

    return (<li className="notification_li">
                <CardHeader
                    avatar={<Avatar alt={'A'} src='...' />}
                    title={<span >{'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'}</span>}
                    subheader={<span className='text-muted'>{'1 minute ago.'}</span>}
                />
          </li>);
}

function Loading() {
    return (<div className="loading-icon"><h6 className="text-center"><CircularProgress ></CircularProgress></h6></div>);
}

export default function NotificationsBox(props)
{

    const ctx = useContext(NotificationsContext);
    const [state, dispatch] = useReducer(NotificationsReducer, ctx);

    const[isLoading, setLoading] = useState(true);
    
    useEffect(() => {
        var fetch = async () => {

            if (props.show === false || props.user === null)
                return;
            const client = HttpClientFactory.get(NotificationsClient, props.user);
            try {

                var result = await client.all(1);
                console.log(result);
                dispatch({
                    type: "all",
                    data: {
                        itemsCount: result.itemsCount,
                        pageNumber: result.pageNumber,
                        items: result.items,
                    }
                });
                setLoading(false);
            }
            catch (e) {
                console.log(e);
            }
        };
        fetch();
        return () => { };
    }, [props.user]);

        return (<div class="notification_dd">
            <div className='notification_label'>
                <h5>Notifications</h5>
            </div>
            <ul class="notification_ul">
                {isLoading ? <Loading /> : state.items.count > 0 ? state.items.map((notif, i) => (<Notification notification={notif} />)) : <h4>No notifications yet.</h4>}
            </ul>
        </div>);

    }




