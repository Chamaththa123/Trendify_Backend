import React, { useState, useEffect } from "react";
import DataTable from "react-data-table-component";
import axiosClient from "../../../axios-client";
import Select from "react-select";
import Swal from "sweetalert2";
import { tableHeaderStyles, customSelectStyles } from "../../utils/dataArrays";
import { AddUser, ProductCategory } from "../../utils/icons";
import { ChangeIcon, EditNewIcon } from "../../utils/icons";
import "bootstrap/dist/css/bootstrap.min.css";

export const PendingCustomer = () => {
    const [customers, setCustomers] = useState([]);
    const [customerTableLoading, setCustomerTableLoading] = useState(false);
    const handleLoading = () => setCustomerTableLoading((pre) => !pre);

  
    // Fetching vendors from the backend
    useEffect(() => {
      const fetchCustomers = async () => {
        try {
          const response = await axiosClient.get("/Users/customers/pending");
          setCustomers(response.data);
        } catch (error) {
          console.error("Failed to fetch customers", error);
        }
      };
      fetchCustomers();
    }, [customerTableLoading]);
  
    const handleStatusChange = (customer) => {
      Swal.fire({
        title: "Are you sure?",
        text: "Do you want to activate this customer account?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, activate it!",
        cancelButtonText: "No, cancel!",
        cancelButtonColor: "#d33",
        confirmButtonColor: "#3085d6",
      }).then((result) => {
        if (result.isConfirmed) {
          axiosClient
            .put(`Users/status/${customer.id}`)
            .then((response) => {
              handleLoading();
              Swal.fire({
                icon: "success",
                title: "Success!",
                text: `Customer account activated successfully!`,
              });
            })
            .catch((error) => {
              console.error("Error activating Customer account:", error);
              Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Something went wrong while activating the Customer account.",
              });
            });
        }
      });
    };
  
    // Creating the table
    const TABLE_VENDOR = [
      {
        name: "Customer Name",
        selector: (row) => (
          <span>
            {row.first_Name} {row.last_Name}
          </span>
        ),
        wrap: true,
        compact: true,
        maxWidth: "auto",
        cellStyle: {
          whiteSpace: "normal",
          wordBreak: "break-word",
        },
      },
      {
        name: "Email",
        selector: (row) => row.email,
        wrap: true,
        compact: true,
        maxWidth: "auto",
        cellStyle: {
          whiteSpace: "normal",
          wordBreak: "break-word",
        },
      },
      {
        name: "Address",
        selector: (row) => row.address,
        wrap: true,
        compact: true,
        maxWidth: "auto",
        cellStyle: {
          whiteSpace: "normal",
          wordBreak: "break-word",
        },
      },
      {
        name: "Status",
        selector: (row) =>
          row.isActive === 0 ? (
            <div className="status-pending-btn">Pending</div>
          ) : null,
        wrap: false,
        minWidth: "200px",
      },
      {
        name: "Action",
        cell: (row) => (
          <div>
            <button
              className="edit-btn"
              onClick={() => handleStatusChange(row)}
              data-bs-toggle="tooltip"
              data-bs-placement="top"
              title="Change Status"
            >
              <ChangeIcon />
            </button>
          </div>
        ),
      },
    ];
  
    useEffect(() => {
      // Destroy any previous tooltips to avoid duplication or issues
      const existingTooltips = document.querySelectorAll(".tooltip");
      existingTooltips.forEach((tooltip) => tooltip.remove());
  
      // Initialize Bootstrap tooltips
      const tooltipTriggerList = [].slice.call(
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
      );
      const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
      });
  
      return () => {
        // Clean up on unmount or when content changes
        tooltipList.forEach((tooltip) => tooltip.dispose());
      };
    }, [customers, customerTableLoading]);
  
    // Define status options
    const statusOptions = [
      { value: "", label: "All" },
      { value: 1, label: "Active" },
      { value: 2, label: "Inactive" },
    ];
    return (
      <section>
        <div className="container bg-white rounded-card p-4 mt-3">
          <DataTable
            columns={TABLE_VENDOR}
            responsive
            data={customers}
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
  
      </section>
    );
  };