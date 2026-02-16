$(function () {
    // Fetch Calls on Page Load
    fetchCalls();

    // Fetch dropdown options on page load
    fetchDropdownOptions();

    // Add New Call Button Event
    $("#btnAddCall").on("click", function () {
        openAddModal(); // Open the modal for adding a new call
    });

    // Close Modal Event
    $("#closeModal").on("click", function () {
        closeCallModal();
    });


    // Add Real-Time Validation on Input Change
    $("#problemSelect, #employeeSelect, #technicianSelect, #notes").on("input change", function () {
        validateField($(this)); // Validate the field on input or change
    });

    // Submit Event for Call Modal Form
    $("#callForm").on("submit", function (e) {
        e.preventDefault();

        // Run validation on all fields
        const isValid = validateForm();

        if (isValid) {
            const isEditMode = $("#callForm").data("editMode"); // Check if in edit mode
            if (isEditMode) {
                updateCall(); // Call the update function
            } else {
                addCall(); // Call the add function
            }
        } else {
            showModalStatusMessage("Please fix the errors before submitting.", "danger");
        }
    });

    // Delete Button Event with Confirmation
    $("#delete").on("click", function () {
        const callId = $("#callForm").data("callId"); // Get the call ID from the form
        if (!callId) {
            showModalStatusMessage("No call selected for deletion.", "info");
            return;
        }

        // Show confirmation message
        showDeleteConfirmation(callId);
    });

    // Function to show delete confirmation
    function showDeleteConfirmation(callId) {
        // Append a bottom-centered confirmation dialog if it doesn't already exist
        if (!$("#deleteConfirmation").length) {
            const confirmationDialog = `
            <div id="deleteConfirmation" class="alert alert-danger text-center position-fixed" style="bottom: 20px; left: 50%; transform: translateX(-50%); z-index: 1050; max-width: 400px; display: none;">
                <strong>Are you sure you want to delete call ${callId}?</strong>
                <div class="mt-3">
                    <button id="confirmDelete" class="btn btn-danger mx-2">Yes</button>
                    <button id="cancelDelete" class="btn btn-secondary mx-2">No</button>
                </div>
            </div>
        `;
            $("body").append(confirmationDialog);
        }

        // Show the confirmation dialog
        $("#deleteConfirmation").fadeIn();

        // Handle confirmation
        $("#confirmDelete").on("click", function () {
            deleteCall(callId); // Proceed with the deletion
            $("#deleteConfirmation").fadeOut(); // Hide the confirmation dialog
        });

        // Handle cancellation
        $("#cancelDelete").on("click", function () {
            $("#deleteConfirmation").fadeOut(); // Simply hide the confirmation dialog
        });
    }


    // Display message at the top
    function showTopStatusMessage(message, type) {
        const topStatus = $("#messageDiv");
        topStatus.text(message).attr("class", `alert alert-${type} text-center`).show();
    }

    // Display modal-specific messages
    function showModalStatusMessage(message, type) {
        $("#modalstatus").text(message).attr("class", `text-center alert alert-${type}`).show();
    }


    const formatDate = (date) => {
        let d;
        (date === undefined) ? d = new Date() : d = new Date(Date.parse(date));
        let _day = d.getDate();
        if (_day < 10) {
            _day = "0" + _day;
        }
        let _month = d.getMonth() + 1;
        if (_month < 10) {
            _month = "0" + _month;
        }
        let _year = d.getFullYear();
        let _hour = d.getHours();
        if (_hour < 10) {
            _hour = "0" + _hour;
        }
        let _min = d.getMinutes();
        if (_min < 10) {
            _min = "0" + _min;
        }
        return _year + "-" + _month + "-" + _day + " " + _hour + ":" + _min;
    };



    // Function to Fetch Calls and Populate Table
    function fetchCalls() {
        $.ajax({
            url: "/api/call",
            method: "GET",
            success: function (data) {
                console.log(data);
                const tbody = $("#callsTable tbody");
                tbody.empty();

                // Populate the table with fetched data
                data.forEach((call) => {
                    const row = `
                        <tr data-id="${call.id}">
                            <td>${formatDate(call.dateOpened) }</td>     
                            <td>${call.employeeName}</td>
                            <td>${call.problemDescription}</td>
                        </tr>
                    `;
                    tbody.append(row);
                });

                // Attach click event to table rows
                $("#callsTable tbody tr").on("click", function () {
                    const callId = $(this).data("id");
                    openCallModal(callId); // Open the modal with the call details
                });
            },
            error: function () {
                showTopStatusMessage("Failed to fetch calls. Please try again.", "danger");
            },
        });
    }

    // Fetch dropdown options for the modal
    function fetchDropdownOptions() {
        // Fetch employees for Employee and Technician dropdowns
        $.ajax({
            url: "/api/employee",
            method: "GET",
            success: function (data) {
                const employeeDropdown = $("#employeeSelect");
                const technicianDropdown = $("#technicianSelect");

                employeeDropdown.empty();
                technicianDropdown.empty();

                data.forEach((employee) => {
                    const option = `<option value="${employee.id}">${employee.lastname}</option>`;
                    employeeDropdown.append(option);
                    technicianDropdown.append(option);
                });
            },
            error: function () {
                showTopStatusMessage("Failed to load employees.", "danger");
            },
        });

        // Fetch problems for Problem dropdown
        $.ajax({
            url: "/api/problem",
            method: "GET",
            success: function (data) {
                const problemDropdown = $("#problemSelect");
                problemDropdown.empty();

                data.forEach((problem) => {
                    const option = `<option value="${problem.id}">${problem.description}</option>`;
                    problemDropdown.append(option);
                });
            },
            error: function () {
                showTopStatusMessage("Failed to load problems.", "danger");
            },
        });
    }

    // Open the modal for editing a call
    function openAddModal() {
        // Reset the modal fields
        $("#problemSelect").val("");
        $("#employeeSelect").val("");
        $("#technicianSelect").val("");
        $("#dateOpened").val(formatDate());
        $("#notes").val("");

        // Set up initial validation messages
        validateField($("#problemSelect"));
        validateField($("#employeeSelect"));
        validateField($("#technicianSelect"));
        validateField($("#notes"));

        // Hide "Date Closed" and "Close Call" fields
        $(".form-group:has(#dateClosed)").hide();
        $(".form-group:has(#closeCall)").hide();

        // Set the form mode to Add
        $("#callForm").data("editMode", false).data("callId", null);

        // Show Add button and hide Update/Delete buttons
        $(".btn-add").show();
        $(".Update").hide();
        $("#delete").hide();

        // Show the modal
        $("#callModal").fadeIn();
    }

    async function openCallModal(callId) {
        if (!callId) {
            showModalStatusMessage("Invalid call ID.", "danger");
            return;
        }

        try {
            const response = await fetch(`/api/call/${callId}`);
            if (response.ok) {
                const call = await response.json();

                // Populate modal fields
                $("#problemSelect").val(call.problemId);
                $("#employeeSelect").val(call.employeeId);
                $("#technicianSelect").val(call.techId);
                $("#dateOpened").val(formatDate(call.dateOpened));
                $("#notes").val(call.notes || "");

                // Check if the call is closed
                if (call.dateClosed) {
                    $("#closeCall").prop("checked", true);
                    $("#dateClosed").val(formatDate(call.dateClosed));

                    // If the call is closed, disable fields and hide "Update" button
                    $("#problemSelect").prop("disabled", true);
                    $("#employeeSelect").prop("disabled", true);
                    $("#technicianSelect").prop("disabled", true);
                    $("#dateOpened").prop("disabled", true);
                    $("#notes").prop("readonly", true);
                    $("#closeCall").prop("disabled", true);

                    $(".Update").hide(); // Hide the Update button
                    $("#delete").show(); // Show the Delete button
                } else {
                    $("#closeCall").prop("checked", false);
                    $("#dateClosed").val("");

                    // If the call is not closed, make fields editable and show "Update" button
                    $("#problemSelect").prop("disabled", false);
                    $("#employeeSelect").prop("disabled", false);
                    $("#technicianSelect").prop("disabled", false);
                    $("#dateOpened").prop("disabled", false);
                    $("#notes").prop("readonly", false);
                    $("#closeCall").prop("disabled", false);

                    $(".Update").show(); // Show the Update button
                    $("#delete").hide(); // Hide the Delete button
                }

                // Show "Date Closed" and "Close Call" fields for edit mode
                $(".form-group:has(#dateClosed)").show();
                $(".form-group:has(#closeCall)").show();

                // Set form mode to Edit
                $("#callForm").data("editMode", true).data("callId", callId);

                // Show the modal
                $("#callModal").fadeIn();
            } else {
                showModalStatusMessage("Failed to load call details.", "danger");
            }
        } catch (error) {
            console.error("Error loading call details:", error);
            showModalStatusMessage("Error loading call details.", "danger");
        }

        $("#delete").show(); // Hide the Delete button
        $(".btn-add").hide();
    }




    // Add a new call
    async function addCall() {
        const newCall = {
            employeeId: parseInt($("#employeeSelect").val()),
            problemId: parseInt($("#problemSelect").val()),
            techId: parseInt($("#technicianSelect").val()) || null,
            dateOpened: new Date($("#dateOpened").val()).toISOString(),
            notes: $("#notes").val() || "",
        };

        try {
            const response = await fetch("/api/call", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(newCall),
            });

            if (response.ok) {
                const result = await response.json(); // Parse the response to get the call details
                const callId = result.id; // Assuming the API returns the new call's ID in the response

                showTopStatusMessage(`Call ${callId} added! - Calls Loaded`, "success");

                closeCallModal();
                fetchCalls(); // Refresh the list of calls
            } else {
                const errorData = await response.json();
                showModalStatusMessage(errorData.msg || "Failed to add call.", "danger");
            }
        } catch (error) {
            showModalStatusMessage("Error adding call.", "danger");
        }
    }


    // Update an existing call
    async function updateCall() {
        const callId = $("#callForm").data("callId");
        if (!callId) {
            showModalStatusMessage("Invalid Call ID.", "danger");
            return;
        }

        const dateClosedValue = $("#dateClosed").val();
        const updatedCall = {
            id: callId,
            employeeId: parseInt($("#employeeSelect").val()),
            problemId: parseInt($("#problemSelect").val()),
            techId: parseInt($("#technicianSelect").val()) || null,
            dateOpened: new Date($("#dateOpened").val()).toISOString(),
            notes: $("#notes").val() || "",
            dateClosed: dateClosedValue ? new Date(dateClosedValue).toISOString() : null,
        };

        try {
            const response = await fetch(`/api/call/${callId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(updatedCall),
            });

            if (response.ok) {
                showTopStatusMessage("Call updated successfully!", "success");

                // If the call is marked as closed, disable the form and show only "Delete"
                if ($("#closeCall").is(":checked")) {
                    isCallClosed = true; // Set the flag

                    // Disable all fields
                    $("#problemSelect").prop("disabled", true);
                    $("#employeeSelect").prop("disabled", true);
                    $("#technicianSelect").prop("disabled", true);
                    $("#dateOpened").prop("disabled", true);
                    $("#notes").prop("readonly", true);
                    $("#closeCall").prop("disabled", true);

                    // Hide the "Update" button and show only "Delete"
                    $(".Update").hide();
                    $("#delete").show();

                    showModalStatusMessage("Call has been finalized and closed.", "success");
                }

                closeCallModal();
                fetchCalls(); // Refresh the table to show the updated data
            } else {
                const errorData = await response.json();
                showModalStatusMessage(errorData.msg || "Failed to update call.", "danger");
            }
        } catch (error) {
            showModalStatusMessage("Error updating call.", "danger");
        }
    }



    // Delete a call
    async function deleteCall(callId) {
        try {
            const response = await fetch(`/api/call/${callId}`, {
                method: "DELETE",
            });

            if (response.ok) {
                showTopStatusMessage(`Call ${callId} successfully deleted!`, "success"); 
                closeCallModal();
                fetchCalls();
            } else {
                const errorData = await response.json();
                showModalStatusMessage(errorData.msg || "Failed to delete call.", "danger");
            }
        } catch (error) {
            showModalStatusMessage("Error deleting call.", "danger");
        }
    }

    function closeCallModal() {
        $("#callModal").fadeOut();
    }


    $(document).on('input', '#searchInput', function () {
        const searchValue = $(this).val().toLowerCase();

        $("#callsTable tbody tr").filter(function () {
            const employeeName = $(this).find('td:nth-child(2)').text().toLowerCase(); // Assuming "FOR" is in the second column
            $(this).toggle(employeeName.includes(searchValue));
        });
    });


    $(function () {
        let isCallClosed = false; // Flag to track if the call is closed

        // Function to handle the "Close Call" checkbox (does not disable yet)
        $("#closeCall").on("change", function () {
            if ($(this).is(":checked")) {
                const closedDate = formatDate();
                $("#dateClosed").val(closedDate); // Set the "Date Closed"
                showModalStatusMessage("Close Call checked. Press 'Update' to finalize.", "info");
            } else {
                $("#dateClosed").val(""); // Clear the "Date Closed" if unchecked
                showModalStatusMessage("Close Call unchecked. You can still edit.", "info");
            }
        });

        function showModalStatusMessage(message, type) {
            $("#modalstatus")
                .text(message)
                .attr("class", `text-center alert alert-${type}`)
                .show();
        }

        // Utility function to show top-level messages
        function showTopStatusMessage(message, type) {
            const topStatus = $("#messageDiv");
            topStatus.text(message).attr("class", `alert alert-${type} text-center`).show();
        }
    });


    // Validation Function for a Single Field
    function validateField(field) {
        const id = field.attr("id");
        let isValid = true;

        if (id === "problemSelect" || id === "employeeSelect" || id === "technicianSelect") {
            if (!field.val()) {
                field.addClass("is-invalid");
                field.next(".error-message").text("This field is required.");
                isValid = false;
            } else {
                field.removeClass("is-invalid");
                field.next(".error-message").text("");
            }
        }

        if (id === "notes") {
            const notesValue = field.val().trim();
            if (notesValue.length < 1 || notesValue.length > 250) {
                field.addClass("is-invalid");
                field.next(".error-message").text("Required: 1-250 chars.");
                isValid = false;
            } else {
                field.removeClass("is-invalid");
                field.next(".error-message").text("");
            }
        }

        return isValid;
    }

    // Validation Function for the Entire Form
    function validateForm() {
        let isValid = true;

        // Validate all required fields
        $("#problemSelect, #employeeSelect, #technicianSelect, #notes").each(function () {
            const fieldValid = validateField($(this));
            if (!fieldValid) isValid = false;
        });

        return isValid;
    }

    // Display message at the top
    function showModalStatusMessage(message, type) {
        $("#modalstatus").text(message).attr("class", `text-center alert alert-${type}`).show();
    }

    // Add Error Message Elements Next to Fields
    function addErrorMessages() {
        $("#problemSelect, #employeeSelect, #technicianSelect, #notes").each(function () {
            if (!$(this).next(".error-message").length) {
                $(this).after('<div class="error-message text-danger small"></div>');
            }
        });
    }

    // Call the function to add error message elements
    addErrorMessages();

});
