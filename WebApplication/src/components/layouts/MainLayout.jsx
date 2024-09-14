import React, { useState, useEffect, useRef } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { UserIcon } from "@heroicons/react/24/solid";
import { SideBar } from "./SideBar";
import { useStateContext } from "../../contexts/NavigationContext";
import { subPathLinks } from "../../utils/dataArrays";
import { ArrowBack } from "../../utils/icons";

export const MainLayout = () => {
  const [signOutVisible, setSignOutVisible] = useState(false);
  const [sidebar, setSidebar] = useState(true); // Sidebar initially expanded
  const dropdownRef = useRef(null);
  const sideBardownRef = useRef(null);
  const sideBarButtondownRef = useRef(null);
  const { token, setUser, setToken } = useStateContext();

  const handleLogout = () => {
    setUser(null);
    setToken(null);
  };

  const [sidebarExpanded, setSidebarExpanded] = useState(true);
  const toggleSidebarExpand = () => {
    setSidebarExpanded((cur) => !cur);
  };

  const location = useLocation();

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setSignOutVisible(false);
      }
      if (
        sideBardownRef.current &&
        !sideBardownRef.current.contains(event.target)
      ) {
        if (
          sideBarButtondownRef.current &&
          sideBarButtondownRef.current.contains(event.target)
        ) {
          setSidebar(true);
        } else {
          setSidebar(false);
        }
      }
    };
    document.addEventListener("click", handleClickOutside);
    return () => {
      document.removeEventListener("click", handleClickOutside);
    };
  }, []);

  let mainPath = "";
  let subPaths = [];
  const pathParts = location.pathname.split("/");
  const id = pathParts[3];

  switch (location.pathname) {
    case "/":
      mainPath = "Dashboard";
      subPaths = ["Back", "New Job"];
      break;
    default:
      break;
  }

  return (
    <section className="d-flex">
      {/* Sidebar */}
      <div>
        <SideBar
          handleSidebar={() => setSidebar(!sidebar)}
          sidebar={sidebar}
          handleLogout={handleLogout}
          sidebarExpanded={sidebarExpanded}
          toggleSidebarExpand={toggleSidebarExpand}
        />
      </div>

      {/* Main Content Area */}
      <section
        style={{ marginLeft: "140px" }}
        className={`main-content transition-all duration-300 flex-grow-1`}
      >
        <div className="header">
          <div className="main-path">{mainPath}</div>
          {/* <div className="d-flex align-items-center">
              <div className="position-relative" ref={dropdownRef}>
                <div
                  className="d-flex align-items-center gap-2 cursor-pointer"
                  onClick={() => setSignOutVisible(!signOutVisible)}
                >
                  <div className="bg-secondary rounded-circle">
                    <UserIcon className="w-12 h-12 text-white" />
                  </div>
                </div>
                {signOutVisible && (
                  <div className="position-absolute top-100 end-0 bg-white w-150 p-3 shadow-lg">
                    <div className="w-100" onClick={handleLogout}>
                      <div className="py-2 cursor-pointer">Logout</div>
                    </div>
                  </div>
                )}
              </div>
            </div> */}
        </div>
        <div className="sub-header d-flex align-items-center">
          {subPaths.map((path, index) =>
            path === "Back" ? (
              <div key={index} className="me-3">
                <a href="#" onClick={() => window.history.back()}>
                  <button className="sub-header-back">
                    <ArrowBack className="me-2" />
                  </button>
                </a>
              </div>
            ) : (
              <Link key={index} to={subPathLinks[path]}>
                <button
                  className={`sub-header-path ${
                    location.pathname.includes(subPathLinks[path])
                      ? "sub-header-path-active"
                      : ""
                  }`}
                >
                  {path}
                </button>
              </Link>
            )
          )}
        </div>

        <div
          style={{
            marginLeft: "75px",
            marginRight: "20px",
            marginTop: "20px",
          }}
        >
          <Outlet />
        </div>
      </section>
    </section>
  );
};
