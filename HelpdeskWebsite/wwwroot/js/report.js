$(() => {
    // Generate Employee Report
    $("#employeebutton").on("click", async () => {
        try {
            $("#lblstatus").text("Generating employee report on the server - please wait...");
            let response = await fetch(`/api/employeereport`);
            if (!response.ok) {
                throw new Error(`Status: ${response.status}, ${response.statusText}`);
            }
            let data = await response.json();
            if (data.msg === "Report Generated") {
                $("#lblstatus").text("Employee report successfully generated.");
                window.open("/pdfs/EmployeeReport.pdf");
            } else {
                $("#lblstatus").text("Problem generating employee report.");
            }
        } catch (error) {
            $("#lblstatus").text(`Error: ${error.message}`);
        }
    });

    // Generate Call Report
    $(() => {
        $("#callbutton").on("click", async (e) => {
            try {
                $("#lblstatus").text("Generating report on server - please wait...");
                let response = await fetch(`api/callreport`);

                if (!response.ok) {
                    throw new Error(`Status - ${response.status}, Text - ${response.statusText}`);
                }

                let data = await response.json(); // Wait for the response

                if (data.msg === "Call Report Generated") {
                    $("#lblstatus").text("Report generated successfully!");

                    // Open the generated PDF file in a new tab
                    window.open("/pdfs/CallReport.pdf", "_blank");
                } else {
                    $("#lblstatus").text("Problem generating report");
                }
            } catch (error) {
                $("#lblstatus").text(`Error: ${error.message}`);
            }
        });
    });

});
