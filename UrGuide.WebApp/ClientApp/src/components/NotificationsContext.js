import React, { createContext } from 'react';


const NotificationsContext = createContext({
    pageNumber: 1,
    itemsCount: 0,
    items: [{}]
});

export default NotificationsContext;