$(() => {
    $("#getbutton").on('click', async (e) => {
        try {
            let email = $("#TextBoxEmail").val();
            $("#status").text("Please wait...");

            console.log("Fetching employee with email:", email); // Debug email value

            // Adjust the endpoint to ensure it's correctly configured to use email
            let response = await fetch(`/api/Employee/${email}`);

            console.log("Response status:", response.status); // Debug status

            if (response.ok) {
                let data = await response.json();
                console.log("Data received:", data); // Debug received data

                // Display retrieved data or "N/A" if any field is undefined
                $("#title").text(data.title || "N/A");
                $("#firstname").text(data.firstname || "N/A");
                $("#lastname").text(data.lastname || "N/A");
                $("#phone").text(data.phoneno || "N/A");
                $("#email").text(data.email || "N/A");
                $("#status").text("Employee found");
            }
            else if (response.status === 404) {
                // Handle not found specifically
                $("#title").text("not found");
                $("#firstname").text("not found");
                $("#lastname").text("not found");
                $("#phone").text("not found");
                $("#email").text("not found");
                $("#status").text("No such employee");
            }
            else {
                // Handle other error statuses by calling the error handler
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            }
        } catch (error) {
            console.error("Error occurred:", error); // Log any errors
            $("#status").text("Failed to fetch: " + error.message);
        }
    });
});

// Error handler
const errorRtn = (problemJson, status) => {
    console.log("Problem with request:", problemJson, status);
    if (status >= 500) {
        $("#status").text("Server-side error, see debug console");
    } else if (status >= 400) {
        let keys = Object.keys(problemJson.errors || {});
        let problemText = keys.length > 0 ? problemJson.errors[keys[0]][0] : "Unknown client error";
        $("#status").text("Client-side error, see browser console");
        console.log({
            status: status,
            statusText: problemText, // Display the first error message
        });
    } else {
        $("#status").text("An unexpected error occurred.");
    }
};
