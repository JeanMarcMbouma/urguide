import React, { Component } from 'react';
import { Avatar, CardHeader } from '@material-ui/core';
import "./NavMenu.css";

function Notification() {

    return (<li className="notification_li">
                <CardHeader
                    avatar={<Avatar alt={'A'} src='...' />}
                    title={<span >{'Lorem Ipsum is simply dummy text of the printing and typesetting industry.'}</span>}
                    subheader={<span className='text-muted'>{'1 minute ago.'}</span>}
                />
          </li>);
}

export default class NotificationsBox extends Component
{

    render() {

      
        return (<div class="notification_dd">
            <div className='notification_label'>
                <h5>Notifications</h5>
            </div>
            <ul class="notification_ul">
                    <Notification />
                    <Notification />
                    <Notification />
                <Notification />
                <Notification />
                <Notification />
                <Notification />
            </ul>
        </div>);

    }
}




