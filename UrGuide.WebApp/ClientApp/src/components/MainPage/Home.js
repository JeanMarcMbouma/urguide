import React from 'react'
import LeftBar from "./LeftBar/LeftBar"
import CentralBar from "./CentralBar/CentralBar"
export default function Home() {
    return (
        <div className="col-lg-12 row">
            <LeftBar />
            <CentralBar />
            {/* RightBar */}
        </div>
    )
}