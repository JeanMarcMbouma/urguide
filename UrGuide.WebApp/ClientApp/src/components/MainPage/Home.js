import React from 'react'
import LeftBar from "./LeftBar/LeftBar"
import CentralBar from "./CentralBar/CentralBar"
import Header from './Header';
import Popular from './LeftSidebar/Popular';

export default function Home() {
    return (
        <div className="col-lg-12 row">
            <Header/>
            <LeftBar />
            <CentralBar />
            <Popular/>
            {/* RightBar */}
        </div>
    )
}