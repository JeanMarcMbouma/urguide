import React from 'react';
import LeftBar from './LeftBar/LeftBar';
import CentralBar from './CentralBar/CentralBar';
import Popular from './Rightbar/Popular';
import './Home.css';

export default function Home() {
    return (
        <div className='home-content'>
            <div className='row justify-content-between'>
                <LeftBar />
                <CentralBar />
                <Popular />
            </div>
        </div>
  );
}
