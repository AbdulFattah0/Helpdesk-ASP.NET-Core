$(() => {
    // Function to fetch and display the list of employees
    const loadEmployees = async () => {
        try {
            $("#employeeTableBody").html("<tr><td colspan='3'>Loading employees...</td></tr>");

            // Adding a random query parameter to prevent caching
            const response = await fetch(`/api/Employee?timestamp=${new Date().getTime()}`);
            if (response.ok) {
                const employees = await response.json();
                displayEmployees(employees);
            } else {
                $("#employeeTableBody").html("<tr><td colspan='3'>Failed to load employee data.</td></tr>");
            }
        } catch (error) {
            console.error("Error loading employees:", error);
            $("#employeeTableBody").html("<tr><td colspan='3'>An error occurred while loading employees.</td></tr>");
        }
    };


    // Function to populate the employee table
    const displayEmployees = (employees) => {
        $("#employeeTableBody").empty(); // Clear existing content
        console.log("Displaying employees:", employees); // Check if employees are updated

        if (employees.length === 0) {
            $("#employeeTableBody").html("<tr><td colspan='3'>No employees available.</td></tr>");
            return;
        }

        employees.forEach(emp => {
            const employeeRow = $(`
            <tr>
                <td>${emp.title}</td>
                <td>${emp.firstname}</td>
                <td>${emp.lastname}</td>
            </tr>
        `);

            // Attach click event to open update modal
            employeeRow.on("click", () => openUpdateModal(emp));
            $("#employeeTableBody").append(employeeRow);
        });
    };


    // Function to open the modal with employee data for updating
    const openUpdateModal = (employee) => {
        $("#employeeId").val(employee.id);
        $("#employeeTimer").val(employee.timerBase64);
        $("#employeeTitle").val(employee.title || "");  // Set the title value
        $("#employeeFirstName").val(employee.firstname || "");
        $("#employeeLastName").val(employee.lastname || "");
        $("#employeePhoneNo").val(employee.phoneno || "");
        $("#employeeEmail").val(employee.email || "");

        $("#employeeModal").modal("show");
    };

    // Function to save changes to an employee
    const updateEmployee = async () => {
        const employee = {
            id: $("#employeeId").val(),
            timerBase64: $("#employeeTimer").val(),
            title: $("#employeeTitle").val(),  // Include the title in the payload
            firstname: $("#employeeFirstName").val(),
            lastname: $("#employeeLastName").val(),
            phoneno: $("#employeePhoneNo").val(),
            email: $("#employeeEmail").val()
        };

        try {
            const response = await fetch('/api/Employee', {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(employee)
            });

            if (response.ok) {
                $("#updateMessage").text("Employee updated successfully!").css("color", "green").show();
                $("#employeeModal").modal("hide");
                await loadEmployees(); // Reload employee list after update
            } else {
                const errorMessage = response.status === 409
                    ? "Update conflict detected. Please reload and try again."
                    : "Failed to update employee. Please try again.";
                $("#updateMessage").text(errorMessage).css("color", "red").show();
            }
        } catch (error) {
            console.error("Error updating employee:", error);
            $("#updateMessage").text("An error occurred while updating. Please check the details and try again.").css("color", "red").show();
        }
    };


    // Bind the update function to the "Save Changes" button
    $("#saveEmployeeChanges").on("click", updateEmployee);

    // Load employees when the page loads
    loadEmployees();

    // Clear the message on page load
    $("#updateMessage").hide(); // Ensure it's hidden initially
});
