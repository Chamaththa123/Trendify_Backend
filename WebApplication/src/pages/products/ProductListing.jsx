import React, { useState, useEffect } from "react";
import DataTable from "react-data-table-component";
import Swal from "sweetalert2";
import axiosClient from "../../../axios-client";
import { tableHeaderStyles } from "../../utils/dataArrays";
import { ChangeIcon, EditNewIcon } from "../../utils/icons";
import "bootstrap/dist/js/bootstrap.bundle.min.js";

export const ProductListing = ({ id, title }) => {
  const [productList, setProductList] = useState([]);
  const [productListTableLoading, setProductListTableLoading] = useState(false);
  const [errors, setErrors] = useState({});

  const [editProductList, setEditProductList] = useState({
    id: "0",
    Name: "",
    Description: "",
  });

  const [addProductList, setAddProductList] = useState({
    Name: "",
    Description: "",
  });

  const handleLoading = () => setProductListTableLoading((pre) => !pre);

  const resetEditForm = () => {
    setEditProductList({
      id: "0",
      Name: "",
      Description: "",
    });
    setAddProductList({
      Name: "",
      Description: "",
    });
    setErrors({});
  };

  //Fetching current zones
  useEffect(() => {
    const fetchProductList = () => {
      axiosClient
        .get(`ProductLists`)
        .then((res) => {
          setProductList(res.data);
        })
        .catch((error) => {
          console.log(error);
        });
    };
    fetchProductList();
  }, [productListTableLoading]);

  const handleEditProductList = (productList) => {
    setEditProductList({
      id: productList.id,
      Name: productList.name,
      Description: productList.description,
    });
  };

  const handleEdit = () => {
    const validationErrors = validate(editProductList);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    axiosClient
      .put(`ProductLists/${editProductList.id}`, editProductList)
      .then((response) => {
        handleLoading();
        Swal.fire({
          icon: "success",
          title: "Success!",
          text: "Product list updated successfully!",
        });
        resetEditForm();
      })
      .catch((error) => {
        console.error("Error updating product list:", error);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: "Something went wrong while updating the product list.",
        });
      });
  };

  const handleCancelEditProductList = () => {
    setEditProductList({
      id: "0",
      Name: "",
      Description: "",
    });
  };

  // Validation function for form data
  const validate = (valData) => {
    const errors = {};
    if (!valData.Name) {
      errors.Name = "* Name is required.";
    }

    if (!valData.Description) {
      errors.Description = "* Description is required.";
    }
    return errors;
  };

  const handleSave = () => {
    const validationErrors = validate(addProductList);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    axiosClient
      .post("ProductLists", addProductList)
      .then((response) => {
        handleLoading();
        resetEditForm();
        Swal.fire({
          icon: "success",
          title: "Success!",
          text: "Product list added successfully!",
        });
      })
      .catch((error) => {
        console.error("Error saving Zone:", error);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: "Something went wrong while adding the product list.",
        });
      });
  };

  const handleStatusChange = (productList) => {
    axiosClient
      .patch(`ProductLists/${productList.id}/state`)
      .then((response) => {
        handleLoading();
        Swal.fire({
          icon: "success",
          title: "Success!",
          text: `Product status updated Successfully !!`,
        });
      })
      .catch((error) => {
        console.error("Error updating product status:", error);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: "Something went wrong while updating the product status.",
        });
      });
  };

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
  }, [productList, productListTableLoading]);

  const TABLE_PRODUCT_LIST = [
    {
      name: "Name",
      selector: (row) => row.name,
      wrap: false,
      maxWidth: "auto",
      cellStyle: {
        whiteSpace: "normal",
        wordBreak: "break-word",
      },
    },
    {
      name: "Description",
      selector: (row) => row.description,
      wrap: false,
      cellStyle: {
        whiteSpace: "normal",
        wordBreak: "break-word",
      },
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
            className="edit-btn me-4"
            onClick={() => handleEditProductList(row)}
            title="Edit Product"
            data-bs-toggle="tooltip"
            data-bs-placement="top"
          >
            <EditNewIcon />
          </button>
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
      minWidth: "50px",
    },
  ];

  return (
    <div
      className="modal fade p-2"
      id={id}
      tabIndex="-1"
      aria-labelledby={`${id}Title`}
      aria-hidden="true"
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content">
          <div className="modal-header theme-text-color">
            <h5 className="modal-title" id={`${id}Title`}>
              {title}
            </h5>
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body theme-text-color">
            <div className=""></div>
            <div className="row">
              <div className="col-12 col-md-6">
                <div class="form-group">
                  <div className="modal-label">Name</div>
                  {editProductList.id !== "0" ? (
                    <input
                      name="Name"
                      type="text"
                      class="form-control my-2 modal-label"
                      value={editProductList.Name}
                      onChange={(e) =>
                        setEditProductList({
                          ...editProductList,
                          Name: e.target.value,
                        })
                      }
                    />
                  ) : (
                    <input
                      value={addProductList.Name}
                      name="Name"
                      type="text"
                      class="form-control my-2 modal-label"
                      onChange={(e) =>
                        setAddProductList({
                          ...addProductList,
                          Name: e.target.value,
                        })
                      }
                    />
                  )}
                  {errors.Name && (
                    <div className=" error-text">{errors.Name}</div>
                  )}
                </div>
              </div>
              <div className="col-12 col-md-6">
                <div class="form-group">
                  <div className="modal-label">Description</div>
                  {editProductList.id !== "0" ? (
                    <input
                      name="Description"
                      type="text"
                      class="form-control my-2 modal-label"
                      value={editProductList.Description}
                      onChange={(e) =>
                        setEditProductList({
                          ...editProductList,
                          Description: e.target.value,
                        })
                      }
                    />
                  ) : (
                    <input
                      value={addProductList.Description}
                      name="Description"
                      type="text"
                      class="form-control my-2 modal-label"
                      onChange={(e) =>
                        setAddProductList({
                          ...addProductList,
                          Description: e.target.value,
                        })
                      }
                    />
                  )}
                  {errors.Description && (
                    <div className=" error-text">{errors.Description}</div>
                  )}
                </div>
              </div>
            </div>

            <div className="mt-4">
              {editProductList.id !== "0" ? (
                <div className="d-flex justify-content-end">
                  <button
                    onClick={() => handleCancelEditProductList()}
                    className="btn btn-danger me-2 form-btn-text"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={() => handleEdit()}
                    className="btn btn-primary form-btn-text"
                  >
                    Update
                  </button>
                </div>
              ) : (
                <div className="d-flex justify-content-end">
                  <button
                    type="submit"
                    onClick={() => handleSave()}
                    className="btn btn-primary form-btn-text"
                  >
                    Add List
                  </button>
                </div>
              )}
            </div>

            <div>
              <DataTable
                columns={TABLE_PRODUCT_LIST}
                responsive
                data={productList}
                customStyles={tableHeaderStyles}
                className="mt-4 "
                pagination
                paginationPerPage={4}
                paginationRowsPerPageOptions={[4]}
                paginationComponentOptions={{
                  rowsPerPageText: "Entries per page:",
                  rangeSeparatorText: "of",
                }}
                noDataComponent={
                  <div className="text-center">No data available</div>
                }
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
