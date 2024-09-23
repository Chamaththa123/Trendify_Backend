import React, { useState, useEffect } from "react";
import Swal from "sweetalert2";
import axiosClient from "../../../axios-client";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import { customSelectStyles } from "../../utils/dataArrays";
import Select from "react-select";

export const AddProduct = ({ id, userType, handleLoading }) => {
  const initialFormData = {
    name: "",
    product_idProductList: "",
    price: 0,
    description: "",
    stock: 0,
    lowStockLvl: 0,
    image: "",
  };

  const [formData, setFormData] = useState(initialFormData);
  const [errors, setErrors] = useState({});
  const [productList, setProductList] = useState([]);

  // Fetching current product list
  useEffect(() => {
    const fetchProductList = async () => {
      try {
        const response = await axiosClient.get("ProductLists/active");
        setProductList(
          response.data.map((list) => ({
            value: list.id,
            label: list.name,
          }))
        );
      } catch (error) {
        console.log(error);
      }
    };
    fetchProductList();
  }, []);

  const handleProductListChange = (selectedOption) => {
    setFormData((prevDetails) => ({
      ...prevDetails,
      product_idProductList: selectedOption.value,
    }));
  };

  // Handle form change
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevDetails) => ({
      ...prevDetails,
      [name]: value,
    }));
  };

  // Handle image file selection
  const handleImageChange = (e) => {
    setFormData((prevDetails) => ({
      ...prevDetails,
      image: e.target.files[0],
    }));
  };

  // Validating the data
  const validate = (data) => {
    const errors = {};
    if (!data.name) errors.name = "Product Name is required.";
    if (!data.product_idProductList)
      errors.product_idProductList = "Product List is required.";
    if (!data.description) errors.description = "Description is required.";
    if (!data.price) errors.price = "Price is required.";
    if (!data.stock) errors.stock = "Stock is required.";
    if (!data.lowStockLvl) errors.lowStockLvl = "Low Stock Level is required.";
    if (!data.image) errors.image = "Image is required.";
    return errors;
  };

  // Handle form submission
  const handleSubmit = async (event) => {
    event.preventDefault();
    const validateErrors = validate(formData);
    setErrors(validateErrors);

    if (Object.keys(validateErrors).length === 0) {
      const formDataImage = new FormData();
      formDataImage.append("file", formData.image);
      formDataImage.append("upload_preset", "my_unsigned_preset");

      try {
        const response = await fetch(
          "https://api.cloudinary.com/v1_1/dk7wsmllq/image/upload",
          {
            method: "POST",
            body: formDataImage,
          }
        );

        const data = await response.json();
        const imageUrl = data.secure_url;

        const productData = {
          ...formData,
          image: imageUrl,
        };

        await axiosClient.post("/Products", productData);
        Swal.fire({
          icon: "success",
          title: "Success!",
          text: `Product added successfully!`,
        });
        setFormData(initialFormData);
        // handleLoading();
      } catch (error) {
        Swal.fire({
          icon: "error",
          title: `Fail to add product`,
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
            <h5 className="modal-title">+ Add New Product</h5>
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
                    <label className="modal-label">Product List</label>
                    <Select
                      className="basic-single text-[14px] mt-2"
                      classNamePrefix="select"
                      isSearchable={true}
                      name="product_idProductList"
                      options={productList}
                      styles={customSelectStyles}
                      value={productList.find(
                        (option) =>
                          option.value === formData.product_idProductList
                      )}
                      onChange={handleProductListChange}
                    />
                    {errors.product_idProductList && (
                      <div className="error-text">
                        {errors.product_idProductList}
                      </div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Product Name</label>
                    <input
                      value={formData.name}
                      name="name"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.name && (
                      <div className="error-text">{errors.name}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Description</label>
                    <input
                      value={formData.description}
                      name="description"
                      type="text"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.description && (
                      <div className="error-text">{errors.description}</div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Price</label>
                    <input
                      value={formData.price}
                      name="price"
                      type="number"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.price && (
                      <div className="error-text">{errors.price}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Stock</label>
                    <input
                      value={formData.stock}
                      name="stock"
                      type="number"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.stock && (
                      <div className="error-text">{errors.stock}</div>
                    )}
                  </div>
                </div>
                <div className="col-12 col-md-6">
                  <div className="form-group">
                    <label className="modal-label">Low Stock Level</label>
                    <input
                      value={formData.lowStockLvl}
                      name="lowStockLvl"
                      type="number"
                      onChange={handleChange}
                      className="form-control my-2 modal-label"
                    />
                    {errors.lowStockLvl && (
                      <div className="error-text">{errors.lowStockLvl}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="row mt-2">
                <div className="col-12">
                  <div className="form-group">
                    <label className="modal-label">Image</label>
                    <input
                      type="file"
                      onChange={(e) =>
                        setFormData({ ...formData, image: e.target.files[0] })
                      }
                      className="form-control my-2 modal-label"
                    />
                    {errors.image && (
                      <div className="error-text">{errors.image}</div>
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
              <button type="submit" className="btn btn-primary form-btn-text">
                Save
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
