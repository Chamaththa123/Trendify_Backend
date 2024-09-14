import { RouterProvider } from "react-router-dom";
import React from "react";
import ReactDOM from "react-dom/client";
import 'bootstrap/dist/css/bootstrap.min.css';
import router from "./routes/router.jsx";
import './index.css'
import { ContextProvider } from "./contexts/NavigationContext.jsx";

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <ContextProvider>
        {/* <Provider store={store}> */}
        <RouterProvider router={router} />
        {/* </Provider> */}
    </ContextProvider>
  </React.StrictMode>
);