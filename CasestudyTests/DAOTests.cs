using HelpdeskDAL;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CasestudyTests
{
    public class DAOTests
    {
        private readonly ITestOutputHelper output;
        private readonly EmployeeDAO _employeeDAO;
        private readonly HelpdeskContext _context;

        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;

            var options = new DbContextOptionsBuilder<HelpdeskContext>()
                .UseInMemoryDatabase(databaseName: "HelpdeskDb")
                .Options;

            _context = new HelpdeskContext(options);
            _employeeDAO = new EmployeeDAO(_context);

            _context.Employees.Add(new Employee
            {
                FirstName = "Bill",
                LastName = "Smith",
                Email = "bs@abc.com",
                PhoneNo = "123-456-7890",
                DepartmentId = 100,
                Timer = new byte[] { 0x00, 0x01, 0x02, 0x03 }
            });

            _context.SaveChanges();
        }

        [Fact]
        public async Task Employee_GetByEmailTest()
        {
            var employee = await _employeeDAO.GetByEmail("bs@abc.com");
            Assert.NotNull(employee);
            Assert.Equal("bs@abc.com", employee?.Email);

            output.WriteLine($"Employee retrieved: {employee?.FirstName} {employee?.LastName}");
        }

        [Fact]
        public async Task Employee_GetByLastnameTest()
        {
            var employee = await _employeeDAO.GetByLastname("Smith");
            Assert.NotNull(employee);
            Assert.Equal("Smith", employee?.LastName);

            output.WriteLine($"Employee retrieved by last name: {employee?.FirstName} {employee?.LastName}");
        }

        [Fact]
        public async Task Employee_GetByIdTest()
        {
            var employee = await _employeeDAO.GetById(1);
            Assert.NotNull(employee);
            Assert.Equal("Bill", employee?.FirstName);

            output.WriteLine($"Employee retrieved by ID: {employee?.FirstName} {employee?.LastName}");
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            var employees = await _employeeDAO.GetAll();
            Assert.NotNull(employees);
            Assert.True(employees.Count > 0);

            output.WriteLine($"Number of employees retrieved: {employees.Count}");
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            var employee = await _employeeDAO.GetByPhoneNumber("123-456-7890");
            Assert.NotNull(employee);
            Assert.Equal("Bill", employee?.FirstName);

            output.WriteLine($"Employee retrieved by phone number: {employee?.FirstName} {employee?.LastName}");
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            var employee = await _employeeDAO.GetByLastname("Smith");
            Assert.NotNull(employee);
            employee.PhoneNo = "999-999-9999";

            var result = await _employeeDAO.Update(employee);
            Assert.Equal(UpdateStatus.Ok, result);

            var updatedEmployee = await _employeeDAO.GetById(employee.Id);
            Assert.Equal("999-999-9999", updatedEmployee?.PhoneNo);

            output.WriteLine($"Employee updated: {updatedEmployee?.FirstName} {updatedEmployee?.PhoneNo}");
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            var employee = await _employeeDAO.GetByLastname("Smith");
            Assert.NotNull(employee);
            var result = await _employeeDAO.Delete(employee.Id);
            Assert.True(result > 0);

            var deletedEmployee = await _employeeDAO.GetById(employee.Id);
            Assert.Null(deletedEmployee);

            output.WriteLine($"Employee deleted: {employee?.FirstName} {employee?.LastName}");
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            var newEmployee = new Employee
            {
                FirstName = "Abdul",
                LastName = "Marouf",
                Email = "a_marouf@fanshaweonline.ca",
                PhoneNo = "548-388-4360",
                DepartmentId = 500,
                Timer = new byte[] { 0x00, 0x01, 0x02, 0x03 }
            };

            var result = await _employeeDAO.Add(newEmployee);
            Assert.Equal(UpdateStatus.Ok, result);

            var addedEmployee = await _employeeDAO.GetByEmail("a_marouf@fanshaweonline.ca");
            Assert.NotNull(addedEmployee);
            Assert.Equal("Abdul", addedEmployee?.FirstName);

            output.WriteLine($"New employee added: {addedEmployee?.FirstName} {addedEmployee?.LastName}");
        }

        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }


        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            var context = new HelpdeskContext(new DbContextOptionsBuilder<HelpdeskContext>()
                .UseInMemoryDatabase("HelpdeskDb").Options);

            var dao = new EmployeeDAO(context);

            EmployeeViewModel newEmployee = new()
            {
                Firstname = "Joe",
                Lastname = "Smith",
                Phoneno = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com",
                TimerBase64 = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02, 0x03 })
            };

            await newEmployee.Add(context);
            Assert.NotNull(newEmployee.Id);
            output.WriteLine("New Employee Added - Id = " + newEmployee.Id);

            var retrievedEmployee = new EmployeeViewModel();
            await retrievedEmployee.GetById(newEmployee.Id.Value, context);
            Assert.NotNull(retrievedEmployee.Timer);
            output.WriteLine($"Employee {retrievedEmployee.Id} retrieved ");

            var originalTimer = retrievedEmployee.Timer;

            // Step 3: Update and verify Timer changes
            retrievedEmployee.Phoneno = "(555)555-5678";
            var updateStatus = await retrievedEmployee.Update(context);

            if (updateStatus == UpdateStatus.Ok)
            {
                output.WriteLine($"Employee {retrievedEmployee.Id} updated successfully with new phone: {retrievedEmployee.Phoneno}");
            }
            else
            {
                output.WriteLine($"Employee {retrievedEmployee.Id} update failed.");
            }

            Assert.Equal(UpdateStatus.Ok, updateStatus);

            var updatedEmployee = new EmployeeViewModel();
            await updatedEmployee.GetById(newEmployee.Id.Value, context);
            output.WriteLine($"Updated Employee {updatedEmployee.Id} retrieved ");

            Assert.NotEqual(originalTimer, updatedEmployee.Timer);

        }

        //[Fact]
        //public async Task Call_ComprehensiveTest()
        //{
        //    var options = new DbContextOptionsBuilder<HelpdeskContext>()
        //        .UseInMemoryDatabase("HelpdeskDb").Options;

        //    var context = new HelpdeskContext(options);


        //    var marouf = new Employee
        //    {
        //        FirstName = "Abdul",
        //        LastName = "Marouf",
        //        Email = "amarouf@helpdesk.com",
        //        PhoneNo = "123-456-7890",
        //        DepartmentId = 1,
        //        Timer = new byte[] { 0x00 }
        //    };
        //    var burner = new Employee
        //    {
        //        FirstName = "Burner",
        //        LastName = "Tech",
        //        Email = "burner@helpdesk.com",
        //        PhoneNo = "987-654-3210",
        //        DepartmentId = 2,
        //        Timer = new byte[] { 0x00 }
        //    };
        //    context.Employees.AddRange(marouf, burner);


        //    var hardDriveProblem = new Problem
        //    {
        //        Description = "Hard Drive Failure",
        //        Timer = new byte[] { 0x00 }
        //    };
        //    context.Problems.Add(hardDriveProblem);

        //    await context.SaveChangesAsync();

        //    var callDao = new CallDAO(context);


        //    Call newCall = new()
        //    {
        //        EmployeeId = marouf.Id,
        //        TechId = burner.Id,
        //        ProblemId = hardDriveProblem.Id,
        //        DateOpened = DateTime.Now,
        //        DateClosed = null,
        //        OpenStatus = true,
        //        Notes = "Marouf's drive is shot, Burner to fix it",
        //        Timer = new byte[] { 0x00 }
        //    };

        //    var addStatus = await callDao.Add(newCall);
        //    Assert.Equal(UpdateStatus.Ok, addStatus);
        //    output.WriteLine($"New Call Generated - Id = {newCall.Id}");


        //    var retrievedCall = await callDao.GetById(newCall.Id);
        //    Assert.NotNull(retrievedCall);
        //    Assert.NotNull(retrievedCall.Employee);
        //    Assert.NotNull(retrievedCall.Tech);
        //    Assert.NotNull(retrievedCall.Problem);
        //    output.WriteLine("New Call Retrieved");

        //    var originalTimer = retrievedCall.Timer;
        //    Assert.NotNull(originalTimer);



        //    retrievedCall.Notes += "\nOrdered new drive!";
        //    var updateStatus = await callDao.Update(retrievedCall);
        //    Assert.Equal(UpdateStatus.Ok, updateStatus);


        //    var updatedCall = await callDao.GetById(newCall.Id);
        //    Assert.NotNull(updatedCall);
        //    output.WriteLine($"Call was updated: {updatedCall.Notes}");

        //    Assert.NotEqual(originalTimer, updatedCall.Timer);



        //    retrievedCall.Timer = originalTimer;
        //    retrievedCall.Notes = "Another update that should fail due to stale data.";

        //    var staleUpdateStatus = await callDao.Update(retrievedCall);
        //    Assert.Equal(UpdateStatus.Stale, staleUpdateStatus);
        //    output.WriteLine("Call was not updated due to stale data");


        //    var deleteStatus = await callDao.Delete(newCall.Id);
        //    Assert.Equal(1, deleteStatus);
        //    output.WriteLine("Call was deleted!");


        //    var deletedCall = await callDao.GetById(newCall.Id);
        //    Assert.Null(deletedCall);
        //}




















    }
}
