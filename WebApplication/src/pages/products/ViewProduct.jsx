import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import axiosClient from "../../../axios-client";

export const ViewProduct = () => {
  const { id } = useParams(); // Get product ID from the URL
  const [product, setProduct] = useState(null); // State to store product details
  const [loading, setLoading] = useState(true); // State to manage loading state

  useEffect(() => {
    // Function to fetch product by ID
    const fetchProductById = async () => {
      try {
        const response = await axiosClient.get(`/Products/${id}`);
        setProduct(response.data);
      } catch (err) {
        console.error("Error fetching user", error);
      } finally {
        setLoading(false);
      }
    };

    fetchProductById();
  }, [id]);

  if (loading) {
    return <p>Loading...</p>;
  }

  return (
    <section>
      <div className="container bg-white rounded-card p-4 theme-text-color">
        <h3 className="mb-2">{product.name}</h3>
        <div className="row">
          <div className="col-6 d-flex justify-content-center">
            <img
              src={product.image}
              alt={product.name}
              style={{ width: "80%", height: "auto", marginTop: "10px" }} // Adjust size as needed
            />
          </div>

          <div className="col-5">
            <div className="d-flex justify-content-between gap-3">
              <div className="col-4 view-text1">Product Listing Type</div>
              <div className="col-9 view-details1">
                {product.productListName}
              </div>
            </div>
            <div className="d-flex mt-2 justify-content-between gap-3">
              <div className="col-4 view-text1">Unit Price</div>
              <div className="col-9 view-details1">
                Rs. {product.price.toFixed(2)}
              </div>
            </div>
            <div className="d-flex mt-2 justify-content-between gap-3">
              <div className="col-4 view-text1">Available Stock</div>
              <div className="col-9 view-details1">{product.stock}</div>
            </div>
            <div className="d-flex mt-2 justify-content-between gap-3">
              <div className="col-4 view-text1">Low Stock Level</div>
              <div className="col-9 view-details1">{product.lowStockLvl}</div>
            </div>
            <div className="d-flex mt-2 justify-content-between gap-3">
              <div className="col-4 view-text1">Stock Status</div>
              <div className="col-9 view-details1">
                {product.stockStatus === "Out of Stock" ? (
                  <div
                    className="status-inactive-btn"
                    style={{ width: "70px" }}
                  >
                    No Stock
                  </div>
                ) : product.stockStatus === "In Stock" ? (
                  <div className="status-active-btn" style={{ width: "70px" }}>
                    In Stock
                  </div>
                ) : product.stockStatus === "Low Stock" ? (
                  <div className="status-active-btn" style={{ width: "70px" }}>
                    Low Stock
                  </div>
                ) : null}
              </div>
            </div>

            <div className="d-flex mt-2 justify-content-between gap-3">
              <div className="col-4 view-text1">Product Status</div>
              <div className="col-9 view-details1">
                {product.isActive === false ? (
                  <div className="status-inactive-btn">Inactive</div>
                ) : product.isActive === true ? (
                  <div className="status-active-btn">Active</div>
                ) : null}
              </div>
            </div>


            <div className="d-flex mt-4 justify-content-between gap-3">
              <div className="col-4 view-text1">Description</div>
              <div className="col-9 view-details1">{product.description}</div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};
