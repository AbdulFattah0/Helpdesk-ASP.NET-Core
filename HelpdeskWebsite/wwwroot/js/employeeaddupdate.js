const apiBaseUrl = "https://localhost:5001/api/employee";
const departmentsApiUrl = "https://localhost:5001/api/department";

$(() => {
    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            const response = await fetch(apiBaseUrl);
            if (response.ok) {
                const payload = await response.json();
                buildEmployeeList(payload);

                // Display the stored message, if any, or fallback to the new message
                $("#status").text(statusMessage || msg || "Employees Loaded").attr("class", "text-center alert alert-success").show();
            } else {
                $("#status").text("Error: Unable to retrieve employee data.").attr("class", "text-center alert alert-danger").show();
                console.error("Error:", response.statusText);
            }
        } catch (error) {
            $("#status").text(error.message).attr("class", "text-center alert alert-danger").show();
            console.error("Error in getAll:", error);
        }
    };


    const buildEmployeeList = (data) => {
        $("#employeeList").empty();
        const header = $(`
            <div class="list-group-item text-white bg-secondary row d-flex" id="status">Employee Info</div>
            <div class="list-group-item row d-flex text-center" id="heading">
                <div class="col-3 h4">Title</div>
                <div class="col-3 h4">First</div>
                <div class="col-3 h4">Last</div>
                <div class="col-3 h4">Phone Number</div>
            </div>
        `);
        header.appendTo($("#employeeList"));

        const addBtn = $(`<button class="list-group-item row d-flex" id="0">...click to add employee</button>`);
        addBtn.appendTo($("#employeeList"));

        if (data && data.length > 0) {
            sessionStorage.setItem("allemployees", JSON.stringify(data));
            data.forEach(emp => {
                if (!emp.id) {
                    console.warn("Invalid employee ID found:", emp);
                    return;
                }

                const title = emp.title || "N/A";
                const phone = emp.phoneno || "No Phone";

                const empBtn = $(`
                    <button class="list-group-item row d-flex" id="${emp.id}">
                        <div class="col-3">${title}</div>
                        <div class="col-3">${emp.firstname}</div>
                        <div class="col-3">${emp.lastname}</div>
                        <div class="col-3">${phone}</div>
                    </button>
                `);

                empBtn.appendTo($("#employeeList"));
            });
        } else {
            $("#employeeList").append("<div>No employees available</div>");
        }
    };

    $("#employeeList").on('click', (e) => {
        const id = e.target.closest('button')?.id;
        const data = JSON.parse(sessionStorage.getItem("allemployees"));
        if (id === "0") {
            setupForAdd();
        } else {
            setupForUpdate(id, data);
        }
    });

    const clearModalFields = () => {
        $("#TextBoxTitle").val("");
        $("#TextBoxFirstName").val("");
        $("#TextBoxSurname").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPhone").val("");
        $("#TextBoxDepartment").val("");
        sessionStorage.removeItem("employee");
        $("#theModal").modal("toggle");
    };

    const loadDepartments = async () => {
        try {
            const response = await fetch(departmentsApiUrl);
            if (response.ok) {
                const departments = await response.json();
                const departmentSelect = $("#TextBoxDepartment");
                departmentSelect.empty();
                departmentSelect.append('<option value="">Select Department</option>');

                departments.forEach(dept => {
                    departmentSelect.append(`<option value="${dept.id}">${dept.departmentName || 'Unnamed Department'}</option>`);
                });
            } else {
                console.error("Failed to load departments:", response.status, response.statusText);
            }
        } catch (error) {
            console.error("Error in loadDepartments:", error);
        }
    };

    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("Add Employee");
        clearModalFields();
        loadDepartments();
    };

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("Update Employee");
        clearModalFields();
        loadDepartments();

        data.forEach(employee => {
            if (employee.id === parseInt(id)) {
                $("#TextBoxTitle").val(employee.title);
                $("#TextBoxFirstName").val(employee.firstname);
                $("#TextBoxSurname").val(employee.lastname);
                $("#TextBoxEmail").val(employee.email);
                $("#TextBoxPhone").val(employee.phoneno);
                $("#TextBoxDepartment").val(employee.departmentId);
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#theModal").modal("toggle");
            }
        });
    };

    const update = async () => {
        try {
            let employee = JSON.parse(sessionStorage.getItem("employee"));
            if (!employee) {
                $("#status").text("No employee data available.");
                return;
            }

            // Update fields from the modal input
            employee.id = employee.id || parseInt($("#employeeId").val()); // Ensure Id is included and is an integer
            employee.title = $("#TextBoxTitle").val();
            employee.firstname = $("#TextBoxFirstName").val();
            employee.lastname = $("#TextBoxSurname").val();
            employee.email = $("#TextBoxEmail").val();
            employee.phoneno = $("#TextBoxPhone").val();
            employee.departmentId = $("#TextBoxDepartment").val() ? parseInt($("#TextBoxDepartment").val()) : null;

            console.log("Updating employee with data:", employee); // Log for debugging

            const response = await fetch(apiBaseUrl, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(employee),
            });

            if (response.ok) {
                const payload = await response.json();
                $("#status").text(payload.msg); // Show update message at the top
                getAll(payload.msg); // Refresh list to show updated data
            } else {
                const problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
        } catch (error) {
            $("#status").text(error.message);
            console.error("Error in updating employee:", error);
        }
        $("#theModal").modal("toggle");
    };


    const add = async () => {
        try {
            const emp = {
                title: $("#TextBoxTitle").val() || "No Title",
                firstname: $("#TextBoxFirstName").val(),
                lastname: $("#TextBoxSurname").val(),
                email: $("#TextBoxEmail").val(),
                phoneno: $("#TextBoxPhone").val(),
                departmentId: $("#TextBoxDepartment").val()
            };

            const response = await fetch(apiBaseUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(emp)
            });

            if (response.ok) {
                const data = await response.json();
                getAll(data.msg);
            } else {
                const problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
        } catch (error) {
            $("#status").text(error.message);
            console.error("Error in adding employee:", error);
        }
        $("#theModal").modal("toggle");
    };

    let statusMessage = ""; // Global variable to hold the persistent message

    const deleteEmployee = async (id) => {
        try {
            const response = await fetch(`${apiBaseUrl}/${id}`, {
                method: "DELETE"
            });

            if (response.ok) {
                const data = await response.json();
                statusMessage = data.msg; // Store the message globally for persistence
                getAll(""); // Refresh the list without updating the status message
            } else {
                const problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
        } catch (error) {
            statusMessage = error.message;
            console.error("Error in deleting employee:", error);
            getAll(""); // Refresh the list even if there's an error
        }
    };


 
    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "update" ? update() : add();
    });

    const errorRtn = (problemJson, status) => {
        if (status >= 500) {
            $("#status").text("Problem server side, see debug console");
        } else {
            $("#status").text(problemJson.msg || "Unknown error");
        }
    };

    getAll("");
});
