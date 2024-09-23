import React, { useState, useEffect } from "react";
import admin from "../../assets/images/admin.png";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import { useStateContext } from "../../contexts/NavigationContext";
import axiosClient from "../../../axios-client";
import Swal from "sweetalert2";

export const UserProfile = () => {
  const { user } = useStateContext();
  const userId = user.id;

  const [users, setUser] = useState({
    first_Name: "",
    last_Name: "",
    email: "",
    nic: "",
    address: "",
  });

  const [passwordData, setPasswordData] = useState({
    currentPassword: "",
    newPassword: "",
  });

  const [loading, setLoading] = useState(true);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    const fetchUserById = async () => {
      try {
        const response = await axiosClient.get(`/Users/${userId}`);
        setUser(response.data);
      } catch (err) {
        console.error("Error fetching user", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUserById();
  }, [userId]);

  const handleChange = (e) => {
    setUser({ ...users, [e.target.name]: e.target.value });
  };

  const handlePasswordChange = (e) => {
    setPasswordData({ ...passwordData, [e.target.name]: e.target.value });
  };

  const validateUserDetails = () => {
    const newErrors = {};
    if (!users.first_Name) newErrors.first_Name = "First Name is required";
    if (!users.last_Name) newErrors.last_Name = "Last Name is required";
    if (!users.email) newErrors.email = "Email is required";
    if (!users.nic) newErrors.nic = "NIC is required";
    if (!users.address) newErrors.address = "Address is required";
    return newErrors;
  };

  const validatePasswordChange = () => {
    const newErrors = {};
    if (!passwordData.currentPassword)
      newErrors.currentPassword = "Current password is required";
    if (!passwordData.newPassword)
      newErrors.newPassword = "New password is required";
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const newErrors = validateUserDetails();
    setErrors(newErrors);

    if (Object.keys(newErrors).length === 0) {
      const result = await Swal.fire({
        title: "Are you sure?",
        text: "Do you want to update your profile details?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, update it!",
        cancelButtonText: "No, cancel",
        cancelButtonColor: "#d33",
        confirmButtonColor: "#3085d6",
      });

      if (result.isConfirmed) {
        try {
          await axiosClient.put(`/Users/update/${userId}`, users);
          Swal.fire("Updated!", "Your profile has been updated.", "success");
        } catch (err) {
          console.error("Error updating user", err);
          Swal.fire("Error!", "Failed to update profile.", "error");
        }
      }
    }
  };

  const handleChangePassword = async (e) => {
    e.preventDefault();
    const newErrors = validatePasswordChange();
    setErrors(newErrors);

    if (Object.keys(newErrors).length === 0) {
      const result = await Swal.fire({
        title: "Are you sure?",
        text: "Do you want to change your password?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, change it!",
        cancelButtonText: "No, cancel",
        cancelButtonColor: "#d33",
        confirmButtonColor: "#3085d6",
      });

      if (result.isConfirmed) {
        try {
          await axiosClient.put(`/Users/change-password`, {
            email: users.email,
            currentPassword: passwordData.currentPassword,
            newPassword: passwordData.newPassword,
          });
          Swal.fire("Changed!", "Your password has been changed.", "success");
          setPasswordData({ currentPassword: "", newPassword: "" });
        } catch (err) {
          console.error("Error changing password", err);
          Swal.fire("Error!", "Failed to change password.", "error");
        }
      }
    }
  };

  if (loading) {
    return <p>Loading...</p>;
  }

  return (
    <section className="mb-4">
      <div className="container bg-white rounded-card p-4 theme-text-color">
        <h3>Admin Profile</h3>
        <div className="row">
          <div className="col-8 mt-2">
            <h6 className="mt-4">Change User Details</h6>
            <form onSubmit={handleSubmit}>
              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  First Name
                </div>
                <div className="col-6">
                  <input
                    value={users.first_Name}
                    name="first_Name"
                    type="text"
                    onChange={handleChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.first_Name && (
                    <span className="error-text">{errors.first_Name}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  Last Name
                </div>
                <div className="col-6">
                  <input
                    value={users.last_Name}
                    name="last_Name"
                    type="text"
                    onChange={handleChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.last_Name && (
                    <span className="error-text">{errors.last_Name}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  Email
                </div>
                <div className="col-6">
                  <input
                    value={users.email}
                    name="email"
                    type="email"
                    onChange={handleChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.email && (
                    <span className="error-text">{errors.email}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  NIC
                </div>
                <div className="col-6">
                  <input
                    value={users.nic}
                    name="nic"
                    type="text"
                    onChange={handleChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.nic && (
                    <span className="error-text">{errors.nic}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  Address
                </div>
                <div className="col-6">
                  <input
                    value={users.address}
                    name="address"
                    type="text"
                    onChange={handleChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.address && (
                    <span className="error-text">{errors.address}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div style={{ fontWeight: "500" }} className="col-2 mt-3"></div>
                <div className="col-6 d-flex justify-content-end">
                  <button
                    type="submit"
                    className="btn btn-primary form-btn-text"
                  >
                    Update
                  </button>
                </div>
              </div>
            </form>

            <h6 className="mt-4">Change Password</h6>
            <form onSubmit={handleChangePassword}>
              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  Current Password
                </div>
                <div className="col-6">
                  <input
                    value={passwordData.currentPassword}
                    name="currentPassword"
                    type="password"
                    onChange={handlePasswordChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.currentPassword && (
                    <span className="error-text">{errors.currentPassword}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div
                  style={{ fontWeight: "500" }}
                  className="col-2 mt-3 modal-label"
                >
                  New Password
                </div>
                <div className="col-6">
                  <input
                    value={passwordData.newPassword}
                    name="newPassword"
                    type="password"
                    onChange={handlePasswordChange}
                    className="form-control my-2 modal-label"
                  />
                  {errors.newPassword && (
                    <span className="error-text">{errors.newPassword}</span>
                  )}
                </div>
              </div>

              <div className="d-flex gap-5">
                <div style={{ fontWeight: "500" }} className="col-2 mt-3"></div>
                <div className="col-6 d-flex justify-content-end">
                  <button
                    type="submit"
                    className="btn btn-primary form-btn-text"
                  >
                    Change Password
                  </button>
                </div>
              </div>
            </form>
          </div>
          <div className="col-4 d-flex justify-content-end">
            <img
              src={admin}
              style={{ width: "100%", height: "60%" }}
              alt="Admin"
            />
          </div>
        </div>
      </div>
    </section>
  );
};
