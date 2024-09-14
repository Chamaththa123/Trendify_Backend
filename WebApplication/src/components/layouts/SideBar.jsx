import { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import { newNavigationItems } from "../../utils/dataArrays";
import { AddCustomerIcon, CloseSidebarIcon } from "../../utils/icons";
import { useStateContext } from "../../contexts/NavigationContext";
import axiosClient from "../../../axios-client";
import logo from "../../assets/images/Logo.png";
import Swal from "sweetalert2";
import { ListGroup, Button } from "react-bootstrap";

export const SideBar = ({
  handleSidebar,
  sidebar,
  handleLogout,
  toggleSidebarExpand,
  sidebarExpanded,
  showSalesman,
}) => {
  const location = useLocation(); // Get current location

  return (
    <div>
      <div className="sidebar">
        <div>
          <img src={logo} />
        </div>

        {newNavigationItems.slice(0, 8).map((item, itemIndex) => {
          const isActive = location.pathname.startsWith(item.link);
          const NavIcon = item.icon;
          return (
            <a
              href={item.link}
              key={itemIndex}
              className={isActive ? "active" : ""}
              onClick={handleSidebar}
            >
              <NavIcon color={isActive ? "white" : "#64728C"} width={20} height={20}/>
             <span style={{marginLeft:"15px"}}> {item.title}</span>
            </a>
          );
        })}
      </div>
    </div>
  );
};
