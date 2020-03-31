import React from 'react';
import LeftBar from './LeftBar/LeftBar';
import CentralBar from './CentralBar/CentralBar';
import Header from './Header';
import Popular from './LeftSidebar/Popular';

export default function Home () {
  return (
    <div className="container-fluid">
      <div className='row col-12 mb-2'>
        <Header />
        <LeftBar />
        <CentralBar />
        <Popular />
      </div>
      {/* RightBar */}
    </div>
  );
}
