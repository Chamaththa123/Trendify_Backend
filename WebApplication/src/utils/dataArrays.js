import { DashboardIcon, EstateIcon, SupplierIcon } from "./icons";

export const newNavigationItems = [
  {
    title: "Dashboard",
    link: "/",
    icon: DashboardIcon,
    children: 0,
  },
  {
    title: "Products",
    link: "/products",
    icon: EstateIcon,
    children: 0,
  },
  {
    title: "Suppliers",
    link: "/suppliers",
    icon: SupplierIcon,
    children: 0,
  },
];

export const adminSidebarItems = [
  { title: "Dashboard", link: "/", icon: DashboardIcon },
  { title: "Products", link: "/products", icon: EstateIcon },
  { title: "Suppliers", link: "/suppliers", icon: SupplierIcon },
  // Add more admin-specific items here
];

export const userSidebarItems = [
  { title: "Dashboard", link: "/", icon: DashboardIcon },
  { title: "Products", link: "/products", icon: EstateIcon },
  // User-specific items (fewer than admin)
];

export const subPathLinks = {
  "New Estate": "/estate/add",
  "New Supplier": "/supplier/add",
};

export const tableHeaderStyles = {
  headCells: {
    style: {
      font: "Poppins",
      fontWeight: "600",
      color: "#64728C",
      fontSize: "14px",
    },
  },
  cells: {
    style: {
      font: "Poppins",
      fontWeight: "normal",
      color: "#64728C",
      fontSize: "12px",
    },
  },
};

export const customSelectStyles = {
  control: (provided, state) => ({
    ...provided,
    fontSize: "14px",
    fontWeight: "600",
    color: state.isFocused ? "#64728C" : "#64728C82",
    borderColor: state.isFocused ? "#64728C" : provided.borderColor,
    boxShadow: state.isFocused ? "0 0 0 1px #64728C" : provided.boxShadow,
    "&:hover": {
      borderColor: state.isFocused ? "#64728C" : provided.borderColor,
    },
  }),
  option: (provided, state) => ({
    ...provided,
    color: state.isSelected ? "#64728C" : "#64728C",
    backgroundColor: state.isSelected ? "#e7e7e7" : "white",
    ":hover": {
      backgroundColor: state.isSelected ? "#ccc" : "#f3f3f3",
    },
    fontSize: "14px",
    fontWeight: "600",
  }),
  singleValue: (provided) => ({
    ...provided,
    color: "#64728C",
  }),
  placeholder: (provided, state) => ({
    ...provided,
    color: "#bdbdbd",
  }),
};
