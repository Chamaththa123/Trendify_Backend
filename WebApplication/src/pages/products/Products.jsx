import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import DataTable from "react-data-table-component";
import axiosClient from "../../../axios-client";
import Select from "react-select";
import { tableHeaderStyles,customSelectStyles } from "../../utils/dataArrays";

export const Products = () => {
    const [products, setProducts] = useState([]);

    // Fetching users from the backend
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
      minWidth: "200px",
    },
    {
      name: "Price (Rs)",
      selector: (row) => row.price,
      wrap: false,
      minWidth: "200px",
    },
    {
        name: "Stock",
        selector: (row) => row.stock,
        wrap: false,
        minWidth: "200px",
    },
    {
      name: "Status",
      selector: (row) =>
        row.isActive === false ? (
          <p>Inactive</p>
        ) : row.isActive === true ? (
          <p>Active</p>
        ) : null,
      wrap: false,
      minWidth: "200px",
    },
    {
      name: "Action",
      cell: (row) => (
        <>
          {/* <Tooltip content="Edit User">
            <IconButton
              onClick={() => handleEditClick(row)}
              variant="text"
              className="mx-2 bg-white"
            >
              <EditNewIcon />
            </IconButton>
          </Tooltip> */}
        </>
      ),
    },
  ];
  return (
    <section>

    <div class="container bg-white rounded-card p-4 theme-text-color">
      <div className="row">
      <div className="col-6">
<div className="col-6">
<span style={{fontSize:'15px',fontWeight:'600'}}>Search Product</span>
      <Select
                classNamePrefix="select"
                isSearchable={true}
                name="color"
                styles={customSelectStyles}
              />
</div>
      
      </div>
      <div className="col-6 text-end">
      <button type="button" class="btn btn-outline theme-button-color">Add Product</button>
      </div>
      </div>
    </div>
    <div class="container bg-white rounded-card p-4 mt-3">
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
            noDataComponent={
              <div className="text-center">No data available</div>
            }
          />
    </div>
    </section>
  )
}
