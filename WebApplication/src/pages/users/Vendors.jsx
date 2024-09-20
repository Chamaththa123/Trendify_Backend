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

export const Vendors = () => {
  const [vendors, setVendors] = useState([]);
  const [filteredVendors, setFilteredVendors] = useState([]);
  const [vendorTableLoading, setVendorTableLoading] = useState(false);
  const handleLoading = () => setVendorTableLoading((pre) => !pre);

  const [statusFilter, setStatusFilter] = useState(null);
  const [nameFilter, setNameFilter] = useState(""); // Added state for name filter

  // Fetching vendors from the backend
  useEffect(() => {
    const fetchVendors = async () => {
      try {
        const response = await axiosClient.get("/Users/vendors");
        setVendors(response.data);
      } catch (error) {
        console.error("Failed to fetch Vendors", error);
      }
    };
    fetchVendors();
  }, [vendorTableLoading]);

  // Effect to filter vendors
  useEffect(() => {
    const filtered = vendors.filter((vendor) => {
      const matchesName = nameFilter
        ? `${vendor.first_Name} ${vendor.last_Name}`
            .toLowerCase()
            .includes(nameFilter.toLowerCase())
        : true;
  
      const matchesStatus = statusFilter && statusFilter.value !== ""
        ? vendor.isActive === statusFilter.value
        : true;
  
      return matchesName && matchesStatus;
    });
  
    setFilteredVendors(filtered);
  }, [statusFilter, vendors, nameFilter]);
  

  const handleStatusFilterChange = (selectedOption) => {
    setStatusFilter(selectedOption);
  };

  // Added handler for name filter
  const handleNameFilterChange = (event) => {
    setNameFilter(event.target.value);
  };

  const handleStatusChange = (vendor) => {
    Swal.fire({
      title: "Are you sure?",
      text: "Do you want to change the status of this vendor?",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Yes, change it!",
      cancelButtonText: "No, cancel!",
    }).then((result) => {
      if (result.isConfirmed) {
        axiosClient
          .put(`Users/status/${vendor.id}`)
          .then((response) => {
            handleLoading();
            Swal.fire({
              icon: "success",
              title: "Success!",
              text: `Vendor status updated successfully!`,
            });
          })
          .catch((error) => {
            console.error("Error updating vendor status:", error);
            Swal.fire({
              icon: "error",
              title: "Oops...",
              text: "Something went wrong while updating the vendor status.",
            });
          });
      }
    });
  };

  // Creating the table
  const TABLE_VENDOR = [
    {
      name: "Vendor Name",
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
        name: "Avg Rating",
        selector: (row) => (
          <Rating
            readonly
            initialRating={row.averageRating} // use the rating value from your data
            emptySymbol={<FaRegStar color="#ffd700" style={{ fontSize: "15px" }}/>}
            fullSymbol={<FaStar color="#ffd700" style={{ fontSize: "15px" }}/>}
            fractions={2} // Enables half-star ratings
          />
        ),
        wrap: false,
        minWidth: "150px",
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
  }, [vendors, vendorTableLoading]);

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
                &nbsp;Add New Vendor
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="container bg-white rounded-card p-4 mt-3">
        <DataTable
          columns={TABLE_VENDOR}
          responsive
          data={filteredVendors}
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

      <AddUsers id="exampleModalCenter" userType='vendor' handleLoading={handleLoading} />
    </section>
  );
};
