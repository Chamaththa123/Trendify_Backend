import React, { useState } from "react";
import Swal from "sweetalert2";
import axiosClient from "../../../axios-client";

export const UpdateStock = ({ id, title, productId, handleLoading }) => {
  const [type, setType] = useState(null); // To store 0 (reduce) or 1 (add)
  const [stockChange, setStockChange] = useState("");
  const [error, setError] = useState({ type: false, stockChange: false });

  const resetForm = () => {
    setType(null);
    setStockChange("");
    setError({ type: false, stockChange: false });
  };

  const handleSave = async () => {
    // Form validation
    const newError = {
      type: type === null,
      stockChange: !stockChange,
    };
    setError(newError);

    if (newError.type || newError.stockChange) {
      return;
    }

    // Confirmation box
    Swal.fire({
      title: "Are you sure?",
      text: `You are about to ${
        type === 1 ? "add to" : "reduce"
      } the stock by ${stockChange}`,
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: `Yes, ${type === 1 ? "add" : "reduce"} it!`,
      cancelButtonColor: '#d33',
      confirmButtonColor: '#3085d6'
    }).then((result) => {
      if (result.isConfirmed) {
        // If confirmed, send the data to the server
        axiosClient
          .put(`Products/stocks/update/${productId}`, {
            type,
            stockChange: parseInt(stockChange),
          })
          .then((response) => {
            Swal.fire("Saved!", "Your stock has been updated.", "success");
            resetForm(); // Reset form fields after successful save
            handleLoading(); // Call handleLoading to refresh data
          })
          .catch((error) => {
            Swal.fire("Error", "Something went wrong!", "error");
          });
      }
    });
  };

  return (
    <div
      className="modal fade p-2"
      id={id}
      tabIndex="-1"
      aria-labelledby={`${id}Title`}
      aria-hidden="true"
    >
      <div className="modal-dialog modal-dialog-centered modal-md">
        <div className="modal-content">
          <div className="modal-header theme-text-color">
            <h5 className="modal-title">{title}</h5>
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
              onClick={resetForm} // Reset form when modal is closed
            ></button>
          </div>
          <div className="modal-body theme-text-color">
            <div className="row mt-2">
              <div className="col-12">
                {/* Radio Buttons for Type */}
                <div className="form-group mb-3">
                  <label className="modal-label">Stock Change Type</label>
                  <div className="d-flex ">
                    <button
                      className={`btn form-btn-text ${
                        type === 1 ? "btn-primary" : "btn-outline-secondary"
                      }`}
                      onClick={() => setType(1)}
                      type="button"
                    >
                      +
                    </button>
                    <button
                      className={`btn ms-2 form-btn-text ${
                        type === 0 ? "btn-primary" : "btn-outline-secondary"
                      }`}
                      onClick={() => setType(0)}
                      type="button"
                    >
                      -
                    </button>
                  </div>
                  {error.type && (
                    <small className="error-text">
                      Please select a stock change type.
                    </small>
                  )}
                </div>

                {/* Input for Stock Change */}
                <div className="form-group">
                  <label className="modal-label">Stock Change Amount</label>
                  <input
                    type="text"
                    className={`form-control ${
                      error.stockChange ? "is-invalid" : ""
                    }`}
                    value={stockChange}
                    onChange={(e) => setStockChange(e.target.value)}
                  />
                  {error.stockChange && (
                    <small className="error-text">
                      Please enter the stock change amount.
                    </small>
                  )}
                </div>
              </div>
            </div>
          </div>

          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-danger me-2 form-btn-text"
              onClick={resetForm}
            >
              Cancel
            </button>
            <button
              type="button"
              className="btn btn-primary form-btn-text"
              onClick={handleSave}
            >
              Save
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
