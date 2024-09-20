import { useLocation } from "react-router-dom";
import { adminSidebarItems, userSidebarItems } from "../../utils/dataArrays";
import { useStateContext } from "../../contexts/NavigationContext";
import logo from "../../assets/images/Logo.png";

export const SideBar = ({
  handleSidebar,
  sidebar,
  handleLogout,
  toggleSidebarExpand,
  sidebarExpanded,
  showSalesman,
}) => {
  const location = useLocation(); // Get current location
  const { token, setUser, setToken, user } = useStateContext();
  const sidebarItems = user?.role === "1" ? adminSidebarItems : userSidebarItems;
  
  const isUsersSectionActive = location.pathname.startsWith('/users');

  return (
    <div>
      <div className="sidebar">
        <div>
          <img src={logo} alt="Logo" />
        </div>

        {sidebarItems.slice(0, 8).map((item, itemIndex) => {
               const isActive = location.pathname === item.link || (item.link === "/users/vendors" && isUsersSectionActive);
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
