$(function () {
    // Show delete confirmation dialog when Delete button is clicked
    $('#deletebutton').on('click', () => {
        $('#dialog').slideDown(); // Slide down for a smooth appearance
    });

    // Handle "Yes" button in confirmation dialog
    $('#yesbutton').on('click', async () => {
        const employeeId = $('#employeeId').val();
        if (employeeId) {
            await deleteEmployee(employeeId); // Delete employee
            $('#dialog').hide(); // Hide confirmation dialog
            showTopStatusMessage(`Employee ${employeeId} deleted successfully!`, 'success');
            $('#employeeModal').modal('hide'); // Close modal after deletion
            getAllEmployees(); // Refresh list
        }
    });

    // Handle "No" button in confirmation dialog
    $('#nobutton').on('click', () => {
        $('#dialog').slideUp(); // Slide up to hide without action
        showModalStatusMessage("Delete cancelled", 'info');
    });

    // Handle add/update employee if employee ID exists, otherwise handle add
    $('#actionbutton').on('click', async () => {
        const employeeId = $('#employeeId').val();
        if (employeeId) {
            console.log("Updating employee with ID:", employeeId);
            await updateEmployee(employeeId); // Update employee if ID exists
        } else {
            console.log("Adding new employee");
            await addEmployee(); // Add new employee if no ID
        }
        getAllEmployees(); // Refresh list after add/update
    });

    // Open the Add Employee modal (clearing ID field)
    $('#addEmployeeButton').on('click', () => {
        clearAddModalFields(); // Clear all modal input fields for add modal
        $('#addEmployeeModal').modal('show'); // Show the Add Employee modal
        $('#employeeModal').modal('hide'); // Ensure the Update/Delete modal is hidden
    });

    // Function to fetch and display all employees
    async function getAllEmployees() {
        try {
            const response = await fetch('/api/Employee');
            if (response.ok) {
                const employees = await response.json();
                console.log("Employee list after update:", employees);
                displayEmployeeList(employees);
            } else {
                console.error("Failed to load employees.");
            }
        } catch (error) {
            console.error("Error loading employees:", error);
        }
    }

    // Function to display employees in the table
    function displayEmployeeList(employees) {
        const employeeList = $('#employeeList');
        employeeList.empty(); // Clear existing rows

        employees.forEach(emp => {
            const employeeRow = $(`
                <tr>
                    <td>${emp.title}</td>
                    <td>${emp.firstname}</td>
                    <td>${emp.lastname}</td>
                </tr>
            `);

            // Attach a click event to the row to open the modal for editing
            employeeRow.on('click', () => {
                openEmployeeModal(emp); // Pass the employee object to openEmployeeModal
            });

            employeeList.append(employeeRow);
        });
    }

    // Function to open the modal with employee data for editing
    function openEmployeeModal(employee) {
        console.log("Opening modal for Employee ID:", employee.id);

        $('#TextBoxTitle').val(employee.title);
        $('#TextBoxFirstName').val(employee.firstname);
        $('#TextBoxSurname').val(employee.lastname);
        $('#TextBoxEmail').val(employee.email);
        $('#TextBoxPhone').val(employee.phoneno);

        if (employee.department && employee.department.id) {
            $('#ddlDepartments').val(employee.department.id);
        } else {
            $('#ddlDepartments').val("");
        }

        $('#employeeId').val(employee.id || "");
        $('#employeeModal').modal('show');
        $('#addEmployeeModal').modal('hide'); // Ensure the Add Employee modal is hidden
    }

    // Function to clear all input fields in the Update/Delete modal
    function clearModalFields() {
        $('#employeeId').val('');
        $('#TextBoxTitle').val('');
        $('#TextBoxFirstName').val('');
        $('#TextBoxSurname').val('');
        $('#TextBoxEmail').val('');
        $('#TextBoxPhone').val('');
        $('#ddlDepartments').val('');
    }

    // Function to clear all input fields in the Add Employee modal
    function clearAddModalFields() {
        $('#AddTextBoxTitle').val('');
        $('#AddTextBoxFirstName').val('');
        $('#AddTextBoxSurname').val('');
        $('#AddTextBoxEmail').val('');
        $('#AddTextBoxPhone').val('');
        $('#AddDdlDepartments').val('');
    }

    // Function to populate departments dropdowns in both modals
    async function populateDepartments() {
        try {
            const response = await fetch('/api/Department');
            if (response.ok) {
                const departments = await response.json();
                $('#ddlDepartments, #AddDdlDepartments').empty();

                departments.forEach(dept => {
                    const option = `<option value="${dept.id}">${dept.departmentName}</option>`;
                    $('#ddlDepartments').append(option);
                    $('#AddDdlDepartments').append(option);
                });
            } else {
                console.error("Failed to load departments.");
            }
        } catch (error) {
            console.error("Error loading departments:", error);
        }
    }

    // Function to add an employee (called by Add Employee modal)
    $('#addNewEmployeeButton').on('click', async () => {
        const newEmployee = {
            title: $('#AddTextBoxTitle').val(),
            firstname: $('#AddTextBoxFirstName').val(),
            lastname: $('#AddTextBoxSurname').val(),
            email: $('#AddTextBoxEmail').val(),
            phoneno: $('#AddTextBoxPhone').val(),
            departmentId: $('#AddDdlDepartments').val(),
        };
        try {
            const response = await fetch('/api/Employee', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newEmployee),
            });
            if (response.ok) {
                showTopStatusMessage("Employee added successfully!", 'success');
                $('#addEmployeeModal').modal('hide'); // Close the Add Employee modal on success
                getAllEmployees(); // Refresh the employee list
            } else {
                showModalStatusMessage("Failed to add employee.", 'danger');
            }
        } catch (error) {
            console.error("Error adding employee:", error);
            showModalStatusMessage("Error adding employee.", 'danger');
        }
    });

    // Function to update an employee
    async function updateEmployee(id) {
        const updatedEmployee = {
            id,
            title: $('#TextBoxTitle').val(),
            firstname: $('#TextBoxFirstName').val(),
            lastname: $('#TextBoxSurname').val(),
            email: $('#TextBoxEmail').val(),
            phoneno: $('#TextBoxPhone').val(),
            departmentId: $('#ddlDepartments').val(),
        };

        console.log("Updating employee with data:", updatedEmployee);

        try {
            const response = await fetch('/api/Employee', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedEmployee),
            });

            if (response.ok) {
                showTopStatusMessage("Employee updated successfully!", 'success');
                $('#employeeModal').modal('hide'); // Close the Update/Delete modal on success
                getAllEmployees(); // Refresh list to show updated data
            } else {
                const errorText = await response.json();
                showModalStatusMessage(errorText.msg || "Failed to update employee.", 'danger');
            }
        } catch (error) {
            console.error("Error updating employee:", error);
            showModalStatusMessage("Error updating employee.", 'danger');
        }
    }

    // Function to delete an employee
    async function deleteEmployee(id) {
        try {
            const response = await fetch(`/api/Employee/${id}`, {
                method: 'DELETE',
            });
            if (response.ok) {
                const deletedEmployee = await response.json(); // Assuming API returns employee info
                showTopStatusMessage(`Employee ${deletedEmployee.lastname} deleted successfully!`, 'success');
                getAllEmployees(); // Refresh list to show the updated data
            } else {
                showTopStatusMessage("Failed to delete employee.", 'danger');
            }
        } catch (error) {
            console.error("Error deleting employee:", error);
            showTopStatusMessage("Error deleting employee.", 'danger');
        }
    }

    // Initializing the message box once when the page loads
    document.addEventListener('DOMContentLoaded', function () {
        $('#topStatus').show().text('');
    });

    // Utility function to show status messages with different styles at the top of the employee list
    function showTopStatusMessage(message, type) {
        const topStatus = $('#topStatus');
        topStatus.text(message).attr('class', `alert alert-${type} text-center`).show();
    }

    // Utility function to show status messages within the modal
    function showModalStatusMessage(message, type) {
        $('#modalstatus').text(message).attr('class', `text-center alert alert-${type}`).show();
    }

    // Initial load of departments and employees
    populateDepartments();
    getAllEmployees();
});
