import React, {useReducer, useContext } from 'react';

import UserContext from './../UserContext'
import UserReducer from './../UserReducer'


const Counter = () => {
  const ctx = useContext(UserContext);
  const [state, dispatch] = useReducer(UserReducer, ctx);
 
  return (
    <>
      User: {state.username}
      <p>

        <button onClick={() => dispatch({type: 'login', data: 'jeanm'})}>Login</button>
        <button hidden={!state.isLoggedIn} onClick={() => dispatch({type: 'logout', data: 'jeanm'})}>Logout</button>
      </p>
    </>
  );
};

export default Counter
