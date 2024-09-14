import { DashboardIcon, EstateIcon, SupplierIcon } from "./icons";

export const newNavigationItems = [
  {
    title: "Dashboard",
    link: "",
    icon: DashboardIcon,
    children: 0,
  },
  {
    title: "Estates",
    link: "estates",
    icon: EstateIcon,
    children: 0,
  },
  {
    title: "Suppliers",
    link: "suppliers",
    icon: SupplierIcon,
    children: 0,
  },
];

export const subPathLinks = {
  "New Estate": "/estate/add",
  "New Supplier": "/supplier/add",
};