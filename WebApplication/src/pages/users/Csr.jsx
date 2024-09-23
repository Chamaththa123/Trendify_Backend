import React, { useState, useEffect } from "react";
import DataTable from "react-data-table-component";
import axiosClient from "../../../axios-client";
import Select from "react-select";
import Swal from "sweetalert2";
import { tableHeaderStyles, customSelectStyles } from "../../utils/dataArrays";
import { AddUser, ProductCategory } from "../../utils/icons";
import { ChangeIcon, EditNewIcon } from "../../utils/icons";
import "bootstrap/dist/css/bootstrap.min.css";
import Rating from "react-rating";
import { FaStar, FaRegStar } from "react-icons/fa";
import AddUsers from "./AddUsers";

export const Csr = () => {
    const [csr, setCsr] = useState([]);
    const [filteredCsr, setFilteredCsr] = useState([]);
    const [csrTableLoading, setCsrTableLoading] = useState(false);
    const handleLoading = () => setCsrTableLoading((pre) => !pre);
  
    const [statusFilter, setStatusFilter] = useState(null);
    const [nameFilter, setNameFilter] = useState(""); // Added state for name filter
  
    // Fetching vendors from the backend
    useEffect(() => {
      const fetchCsr = async () => {
        try {
          const response = await axiosClient.get("/Users/csrs");
          setCsr(response.data);
        } catch (error) {
          console.error("Failed to fetch Vendors", error);
        }
      };
      fetchCsr();
    }, [csrTableLoading]);
  
    // Effect to filter vendors
    useEffect(() => {
      const filtered = csr.filter((csr) => {
          const matchesName = nameFilter
          ? `${csr.first_Name} ${csr.last_Name}`
              .toLowerCase()
              .includes(nameFilter.toLowerCase())
          : true;

            const matchesStatus = statusFilter && statusFilter.value !== ""
            ? csr.isActive === statusFilter.value
            : true;
            return matchesName && matchesStatus;
      });
  
      setFilteredCsr(filtered);
    }, [statusFilter, csr,nameFilter]);

    const handleStatusFilterChange = (selectedOption) => {
      setStatusFilter(selectedOption);
    };
  
    // Added handler for name filter
    const handleNameFilterChange = (event) => {
      setNameFilter(event.target.value);
    };
  
    const handleStatusChange = (csr) => {
      Swal.fire({
        title: "Are you sure?",
        text: "Do you want to change the status of this CSR Member?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, change it!",
        cancelButtonText: "No, cancel!",
      }).then((result) => {
        if (result.isConfirmed) {
          axiosClient
            .put(`Users/status/${csr.id}`)
            .then((response) => {
              handleLoading();
              Swal.fire({
                icon: "success",
                title: "Success!",
                text: `CSR Member status updated successfully!`,
              });
            })
            .catch((error) => {
              console.error("Error updating CSR Member status:", error);
              Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Something went wrong while updating the CSR Member status.",
              });
            });
        }
      });
    };
  
    // Creating the table
    const TABLE_CSR = [
      {
        name: "CSR Name",
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
        name: "NIC",
        selector: (row) => row.nic,
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
          row.isActive === 2 ? (
            <div className="status-inactive-btn">Inactive</div>
          ) : row.isActive === 1 ? (
            <div className="status-active-btn">Active</div>
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
    }, [csr, csrTableLoading]);
  
    // Define status options
    const statusOptions = [
      { value: "", label: "All" },
      { value: true, label: "Active" },
      { value: false, label: "Inactive" },
    ];
  return (
    <section>
      <div className="container bg-white rounded-card p-4 theme-text-color">
        <div className="row">
          <div className="col-6">
            <div className="d-flex align-items-center gap-3">
              <div className="col-5">
                <span style={{ fontSize: "15px", fontWeight: "600" }}>
                  Search by Name
                </span>
                <input
                  name="Name"
                  type="text"
                  className="form-control  col-5"
                  value={nameFilter}
                  onChange={handleNameFilterChange}
                />
              </div>

              <div className="col-6">
                <span style={{ fontSize: "15px", fontWeight: "600" }}>
                  Search by Status
                </span>
                <Select
                  classNamePrefix="select"
                  options={statusOptions}
                  onChange={handleStatusFilterChange}
                  isSearchable={true}
                  name="color"
                  styles={customSelectStyles}
                  className="col-9"
                />
              </div>
            </div>
          </div>

          <div className="col-6 d-flex justify-content-end gap-3">
            <div>
              <button
                className="modal-btn"
                type="button"
                data-bs-toggle="modal"
                data-bs-target="#exampleModalCenter"
              >
                <AddUser />
                &nbsp;Add New CSR
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="container bg-white rounded-card p-4 mt-3">
        <DataTable
          columns={TABLE_CSR}
          responsive
          data={filteredCsr}
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

      <AddUsers id="exampleModalCenter" userType='csr' handleLoading={handleLoading} />
    </section>
  )
}
