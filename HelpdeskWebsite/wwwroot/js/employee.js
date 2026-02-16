$(function () {
    // Show delete confirmation dialog when Delete button is clicked
    $('#deletebutton').on('click', () => {
        $('#dialog').slideDown(); // Slide down for a smooth appearance
    });

    $('#yesbutton').on('click', async () => {
        const employeeId = $('#employeeId').val();
        const employeeLastName = $('#TextBoxSurname').val();

        if (employeeId) {
            await deleteEmployee(employeeId);
            $('#dialog').hide();
            showTopStatusMessage(`Employee ${employeeLastName} deleted successfully!`, 'success');
            $('#employeeModal').modal('hide');
            getAllEmployees();
        }
    });

    $('#nobutton').on('click', () => {
        $('#dialog').slideUp();
        showModalStatusMessage("Delete cancelled", 'info');
    });

    $('#actionbutton').on('click', async () => {
        if ($('#employeeId').val() && !validateEmployeeForm()) {
            return; // Do not proceed if validation fails for update
        }

        const employeeId = $('#employeeId').val();
        if (employeeId) {
            console.log("Updating employee with ID:", employeeId);
            await updateEmployee(employeeId);
        }
        if ($('#uploader')[0].files.length > 0) {
            await updateEmployeePicture(employeeId);
        } else {
            console.log("Adding new employee");
            await addEmployee();
        }
        getAllEmployees();
    });

    $('#addEmployeeButton').on('click', () => {
        clearAddModalFields();
        $('#addEmployeeModal').modal('show');
        $('#employeeModal').modal('hide');
    });



    // Real-time validation for Update Modal
    $('#TextBoxTitle').on('input', function () {
        validateTitle($(this));
        toggleUpdateButton();
    });

    $('#TextBoxFirstName').on('input', function () {
        validateFirstName($(this));
        toggleUpdateButton();
    });

    $('#TextBoxEmail').on('input', function () {
        validateEmail($(this));
        toggleUpdateButton();
    });

    // Function to validate Title
    function validateTitle(inputField) {
        const value = inputField.val();
        if (!['Mr.', 'Ms.', 'Mrs.', 'Dr.'].includes(value)) {
            showValidationMessage(inputField, "Title must be Mr., Ms., Mrs., or Dr.");
            return false;
        } else {
            clearValidationMessage(inputField);
            return true;
        }
    }

    // Function to validate First Name
    function validateFirstName(inputField) {
        const value = inputField.val();
        if (!value || value.length < 1 || value.length > 25) {
            showValidationMessage(inputField, "First Name must be 1-25 characters long.");
            return false;
        } else {
            clearValidationMessage(inputField);
            return true;
        }
    }

    // Function to validate Email
    function validateEmail(inputField) {
        const value = inputField.val();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            showValidationMessage(inputField, "Email must be in a valid format (e.g., name@example.com).");
            return false;
        } else {
            clearValidationMessage(inputField);
            return true;
        }
    }

    // Function to show validation message under the input field
    function showValidationMessage(inputField, message) {
        if (inputField.next('.validation-message').length === 0) {
            inputField.after(`<div class="validation-message text-danger">${message}</div>`);
        }
    }

    // Function to clear validation message
    function clearValidationMessage(inputField) {
        inputField.next('.validation-message').remove();
    }

    // Validation function for Update Modal
    function validateEmployeeForm() {
        const isTitleValid = validateTitle($('#TextBoxTitle'));
        const isFirstNameValid = validateFirstName($('#TextBoxFirstName'));
        const isEmailValid = validateEmail($('#TextBoxEmail'));
        return isTitleValid && isFirstNameValid && isEmailValid;
    }

    // Toggle the Update button visibility based on validation
    function toggleUpdateButton() {
        const isFormValid = validateEmployeeForm();
        if (isFormValid) {
            $('#actionbutton').show(); // Show the button if valid
        } else {
            $('#actionbutton').hide(); // Hide the button if invalid
        }
    }

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
        employeeList.empty();

        employees.forEach(emp => {
            const employeeRow = $(`
                <tr>
                    <td>${emp.title}</td>
                    <td>${emp.firstname}</td>
                    <td>${emp.lastname}</td>
                </tr>
            `);

            employeeRow.on('click', () => {
                openEmployeeModal(emp);
            });

            employeeList.append(employeeRow);
        });
    }


    function openEmployeeModal(employee) {
        console.log("Opening modal for Employee ID:", employee.id);

        sessionStorage.setItem("employee", JSON.stringify(employee));


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
        if (employee.staffPictureBase64) {
            $("#imageHolder").html(`<img height="120" width="110" src="data:image/png;base64,${employee.staffPictureBase64}" />`);
        } else {
            $("#imageHolder").html('<p>No picture available</p>');
        }


        $('#employeeModal').modal('show');
        $('#addEmployeeModal').modal('hide');
        toggleUpdateButton();
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
        $("#imageHolder").html('');
        sessionStorage.removeItem("picture");
        $("#uploadstatus").text("");
        $("#imageHolder").html("");
        $("#uploader").val("");
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
                $('#addEmployeeModal').modal('hide');
                getAllEmployees();
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
                $('#employeeModal').modal('hide');
                getAllEmployees();
            } else {
                const errorText = await response.json();
                showModalStatusMessage(errorText.msg || "Failed to update employee.", 'danger');
            }
        } catch (error) {
            console.error("Error updating employee:", error);
            showModalStatusMessage("Error updating employee.", 'danger');
        }
    }

    async function deleteEmployee(id) {
        try {
            const response = await fetch(`/api/Employee/${id}`, {
                method: 'DELETE',
            });
            if (response.ok) {
                const deletedEmployee = await response.json();
                showTopStatusMessage(`Employee ${deletedEmployee.lastname} deleted successfully!`, 'success');
                getAllEmployees();
            } else {
                showTopStatusMessage("Failed to delete employee.", 'danger');
            }
        } catch (error) {
            console.error("Error deleting employee:", error);
            showTopStatusMessage("Error deleting employee.", 'danger');
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        $('#topStatus').show().text('');
        $('#actionbutton').hide(); // Initially hide the update button
    });

    function showTopStatusMessage(message, type) {
        const topStatus = $('#topStatus');
        topStatus.text(message).attr('class', `alert alert-${type} text-center`).show();
    }

    function showModalStatusMessage(message, type) {
        $('#modalstatus').text(message).attr('class', `text-center alert alert-${type}`).show();
    }

    $('#srch').on('keyup', function () {
        const query = $(this).val().toLowerCase();


        $('#employeeList tr').filter(function () {
            const lastName = $(this).find('td:nth-child(3)').text().toLowerCase();
            $(this).toggle(lastName.indexOf(query) > -1);
        });
    });


    // Listen for file selection and show preview
    $('#uploader').on('change', function (event) {
        const file = event.target.files[0];

        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                // Show the uploaded image in the image holder
                $('#imageHolder').html(`<img src="${e.target.result}" height="120" width="110" alt="Employee Picture" style="border: 1px solid #ddd; border-radius: 5px;" />`);
            };
            reader.readAsDataURL(file);
        }
    });

    // Function to update employee picture along with details
    async function updateEmployeePicture(employeeId) {
        const file = $('#uploader')[0].files[0];
        if (!file) {
            showModalStatusMessage("Please choose a picture to upload.", 'info');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);
        formData.append('employeeId', employeeId);

        try {
            const response = await fetch(`/api/Employee/`, {
                method: 'PUT',
                body: formData,
            });

            if (response.ok) {
                showTopStatusMessage("Employee picture updated successfully!", 'success');
                $('#employeeModal').modal('hide');
                getAllEmployees();
            } else {
                showModalStatusMessage("Failed to upload picture.", 'danger');
            }
        } catch (error) {
            console.error("Error uploading picture:", error);
            showModalStatusMessage("Error uploading picture.", 'danger');
        }
    }

    // Add Employee Validation and Event Handlers
    // Add Employee Validation and Event Handlers
    $('#AddTextBoxTitle').off('input').on('input', function () {
        validateTitle($(this));
        toggleAddButton();
    });

    $('#AddTextBoxFirstName').off('input').on('input', function () {
        validateFirstName($(this));
        toggleAddButton();
    });

    $('#AddTextBoxEmail').off('input').on('input', function () {
        validateEmail($(this));
        toggleAddButton();
    });


    function validateAddEmployeeForm() {
        const isTitleValid = validateTitle($('#AddTextBoxTitle'));
        const isFirstNameValid = validateFirstName($('#AddTextBoxFirstName'));
        const isEmailValid = validateEmail($('#AddTextBoxEmail'));
        return isTitleValid && isFirstNameValid && isEmailValid;
    }

    function toggleAddButton() {
        const isFormValid = validateAddEmployeeForm();
        $('#addNewEmployeeButton').prop('disabled', !isFormValid); 
    }


    $('#addNewEmployeeButton').off('click').on('click', async function () {
        if (!validateAddEmployeeForm()) {
            showModalStatusMessage("Please fix the errors before adding the employee.", "danger");
            return;
        }

        $(this).prop('disabled', true); // Temporarily disable the button to avoid duplicates

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
                $('#addEmployeeModal').modal('hide');
                getAllEmployees();
            } else {
                showModalStatusMessage("Failed to add employee.", 'danger');
            }
        } catch (error) {
            console.error("Error adding employee:", error);
            showModalStatusMessage("Error adding employee.", 'danger');
        } finally {
            $(this).prop('disabled', false); 
        }
    });


    toggleAddButton(); 

    populateDepartments();
    getAllEmployees();
});
