// src/components/Products.js
import React, { useState, useEffect } from "react";
import DataTable from "react-data-table-component";
import axiosClient from "../../../axios-client";
import Select from "react-select";
import { tableHeaderStyles, customSelectStyles } from "../../utils/dataArrays";
import { ProductCategory } from "../../utils/icons";
import { ProductListing } from "./ProductListing";
import "bootstrap/dist/css/bootstrap.min.css";

export const Products = () => {
  const [products, setProducts] = useState([]);

  // Fetching products from the backend
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await axiosClient.get("/Products");
        setProducts(response.data);
      } catch (error) {
        console.error("Failed to fetch products", error);
      }
    };
    fetchProducts();
  }, []);

  // Creating the table
  const TABLE_PRODUCTS = [
    {
      name: "Product Name",
      selector: (row) => row.name,
      wrap: false,
      maxWidth: "auto",
    },
    {
      name: "Product List",
      selector: (row) => row.productListName,
      wrap: false,
      minWidth: "auto",
    },
    {
      name: "Price (Rs)",
      selector: (row) => row.price,
      wrap: false,
      minWidth: "auto",
      left:true
    },
    {
      name: "Stock",
      selector: (row) => row.stock,
      wrap: false,
      minWidth: "auto",
    },
    {
      name: "Status",
      selector: (row) =>
        row.isActive === false ? (
          <div className="status-inactive-btn">Inactive</div>
        ) : row.isActive === true ? (
          <div className="status-active-btn">Active</div>
        ) : null,
      wrap: false,
      minWidth: "200px",
    },
    {
      name: "Action",
      cell: (row) => <>{/* Add actions here if needed */}</>,
    },
  ];

  return (
    <section>
      <div className="container bg-white rounded-card p-4 theme-text-color">
        <div className="row">
          <div className="col-6">
            <span style={{ fontSize: "15px", fontWeight: "600" }}>
              Search Product
            </span>
            <Select
              classNamePrefix="select"
              isSearchable={true}
              name="color"
              styles={customSelectStyles}
            />
          </div>
          <div className="col-6 d-flex justify-content-end">
            <div>
              <button
                className="modal-btn"
                type="button"
                data-bs-toggle="modal"
                data-bs-target="#exampleModalCenter"
              >
                <ProductCategory />
                &nbsp; Product Listing
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="container bg-white rounded-card p-4 mt-3">
        <DataTable
          columns={TABLE_PRODUCTS}
          responsive
          data={products}
          customStyles={tableHeaderStyles}
          className="mt-4"
          pagination
          paginationPerPage={5}
          paginationRowsPerPageOptions={[5, 10, 15]}
          paginationComponentOptions={{
            rowsPerPageText: "Entries per page:",
            rangeSeparatorText: "of",
          }}
          noDataComponent={<div className="text-center">No data available</div>}
        />
      </div>

      <ProductListing id="exampleModalCenter" title="Product Listing" />
    </section>
  );
};
