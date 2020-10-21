import React, { createContext } from 'react';


const MessageContext = createContext({
    receiverId: null,
    content:'',
    items: [{}],
});

export default MessageContext;