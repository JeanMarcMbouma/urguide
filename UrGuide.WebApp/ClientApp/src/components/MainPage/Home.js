import React from 'react';
import LeftBar from './LeftBar/LeftBar';
import CentralBar from './CentralBar/CentralBar';
import Popular from './Rightbar/Popular';
import Skeleton from '@material-ui/lab/Skeleton';
import './Home.css';
import { createStore } from 'redux'
import Data from './Data'
import { Provider } from 'react-redux'

// import { createStore , combineReducers  } from 'redux'

const store = createStore(Data)

export default function Home() {
    return (
        <Provider store={store}>
            <div className='home-content'>
                <div className='row justify-content-between'>
                    <LeftBar />
                    <CentralBar />
                    <Popular />
                </div>
            </div>
        </Provider>
    );}
