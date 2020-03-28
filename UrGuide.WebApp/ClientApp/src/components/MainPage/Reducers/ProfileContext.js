import React, { createContext } from 'react';

const ProfileContext = createContext({
  username: null,
  isLoggedIn: false,
  token:null
});

export default ProfileContext;