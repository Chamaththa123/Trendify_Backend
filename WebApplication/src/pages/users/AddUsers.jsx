import React, { useState } from "react";
import Swal from "sweetalert2";
import axiosClient from "../../../axios-client";
import "bootstrap/dist/js/bootstrap.bundle.min.js";

const AddUsers = ({ id, userType, handleLoading }) => {
  // Initial form data
  const initialFormData = {
    Username: "",
    First_Name: "",
    Last_Name: "",
    Email: "",
    PasswordHash: "",
    NIC: "",
    Address: "",
    Role: userType === "vendor" ? '3' : "2",
  };

  const [formData, setFormData] = useState(initialFormData);
  const [errors, setErrors] = useState({});

  // Handle form change
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
  };

  // Validating the data
  const validate = (data) => {
    const errors = {};
    if (!data.Username) errors.Username = "User Name is required.";
    if (!data.First_Name) errors.First_Name = "First Name is required.";
    if (!data.Last_Name) errors.Last_Name = "Last Name is required.";
    if (!data.Email) errors.Email = "Email is required.";
    if (!data.PasswordHash) errors.PasswordHash = "Password is required.";
    if (!data.NIC) errors.NIC = "NIC is required.";
    if (!data.Address) errors.Address = "Address is required.";
    return errors;
  };

  // Handle form submission
  const handleSubmit = async (event) => {
    event.preventDefault();
    const validateErrors = validate(formData);
    setErrors(validateErrors);

    if (Object.keys(validateErrors).length === 0) {
      try {
        await axiosClient.post("/Users/register", formData);
        Swal.fire({
          icon: "success",
          title: "Success!",
          text: `${userType === "vendor" ? "Vendor" : "CSR Member"} added successfully!`,
        });
        setFormData(initialFormData);
        handleLoading();
      } catch (error) {
        Swal.fire({
          icon: "error",
          title: `Fail to add ${userType === "vendor" ? "Vendor" : "CSR Member"} `,
          text: "Please try again.",
        });
      }
    }
  };

  // Handle cancel button click
  const handleCancel = () => {
    setFormData(initialFormData);
    setErrors({});
  };

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
            <h5 className="modal-title">
              + Add New {userType === "vendor" ? "Vendor" : "CSR Member"}
            </h5>
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <form onSubmit={handleSubmit}>
            <div className="modal-body theme-text-color">
              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">First Name</label>
                    <input
                      value={formData.First_Name}
                      name="First_Name"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.First_Name && (
                      <div className="error-text">{errors.First_Name}</div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Last Name</label>
                    <input
                      value={formData.Last_Name}
                      name="Last_Name"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.Last_Name && (
                      <div className="error-text">{errors.Last_Name}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">User Name</label>
                    <input
                      value={formData.Username}
                      name="Username"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.Username && (
                      <div className="error-text">{errors.Username}</div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">User Email</label>
                    <input
                      value={formData.Email}
                      name="Email"
                      type="email"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.Email && (
                      <div className="error-text">{errors.Email}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">NIC</label>
                    <input
                      value={formData.NIC}
                      name="NIC"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.NIC && (
                      <div className="error-text">{errors.NIC}</div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Address</label>
                    <input
                      value={formData.Address}
                      name="Address"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.Address && (
                      <div className="error-text">{errors.Address}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Password</label>
                    <input
                      value={formData.PasswordHash}
                      name="PasswordHash"
                      type="password"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.PasswordHash && (
                      <div className="error-text">{errors.PasswordHash}</div>
                    )}
                  </div>
                </div>
              </div>
            </div>

            <div className="modal-footer">
              <button
                type="button"
                className="btn btn-danger me-2 form-btn-text"
                onClick={handleCancel}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-primary form-btn-text"
              >
                Save
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default AddUsers;
