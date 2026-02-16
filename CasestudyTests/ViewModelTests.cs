using HelpdeskDAL;
using HelpdeskViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CasestudyTests
{
    public class ViewModelTests
    {
        private readonly ITestOutputHelper output;

        public ViewModelTests(ITestOutputHelper output) // Inject the output helper
        {
            this.output = output;
        }

        private HelpdeskContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<HelpdeskContext>()
                .UseInMemoryDatabase(databaseName: "HelpdeskTestDb")
                .Options;

            var context = new HelpdeskContext(options);

            if (!context.Employees.AnyAsync().Result)
            {
                context.Employees.Add(new Employee
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    PhoneNo = "(555) 555-5555",
                    DepartmentId = 100,
                    Timer = new byte[] { 0x00, 0x01, 0x02, 0x03 }
                });
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Employee_GetByLastnameTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel { Lastname = "Smith" };
            await vm.GetByLastname(vm.Lastname!, context);
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByIDTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel { Id = 1 };
            await vm.GetById((int)vm.Id!, context);
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel { Phoneno = "(555) 555-5555" };
            await vm.GetByPhoneNumber(vm.Phoneno!, context);
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetAllTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel();
            List<EmployeeViewModel> allEmployees = await vm.GetAll(context);
            Assert.True(allEmployees.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel
            {
                Firstname = "Jane",
                Lastname = "Doe",
                Phoneno = "(888)888-8888",
                Title = "Ms.",
                DepartmentId = 100,
                Email = "jane.doe@someemail.com",
                Timer = new byte[] { 0x00, 0x01, 0x02, 0x03 }
            };

            await vm.Add(context);
            Assert.True(vm.Id > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel { Lastname = "Smith" };
            await vm.GetByLastname(vm.Lastname!, context);
            Assert.NotNull(vm.Firstname);

            vm.Email = vm.Email == "john.smith@example.com" ? "new.email@example.com" : "john.smith@example.com";
            var updateResult = await vm.Update(context);

            Assert.Equal(UpdateStatus.Ok, updateResult);
        }

        [Fact]
        public async Task Employee_DeleteTest()
        {
            var context = GetInMemoryContext();
            var vm = new EmployeeViewModel { Phoneno = "(555) 555-5555" };
            await vm.GetByPhoneNumber(vm.Phoneno!, context);
            Assert.NotNull(vm.Firstname);
            Assert.True(await vm.Delete(vm.Id!.Value, context) == 1, "Expected the delete to affect 1 row.");
        }


        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            var context = new HelpdeskContext(new DbContextOptionsBuilder<HelpdeskContext>()
                .UseInMemoryDatabase("HelpdeskDb").Options);

            var dao = new EmployeeDAO(context);

            EmployeeViewModel newEmployee = new()
            {
                Firstname = "Some",
                Lastname = "Employee",
                Phoneno = "(777)777-7777",
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
        //public async Task Call_ComprehensiveVMTest()
        //{
        //    var options = new DbContextOptionsBuilder<HelpdeskContext>()
        //        .UseInMemoryDatabase("HelpdeskDb").Options;

        //    var context = new HelpdeskContext(options);

        //    // Seed employees
        //    var employee = new Employee
        //    {
        //        FirstName = "Abdul",
        //        LastName = "Marouf",
        //        Email = "amarouf@helpdesk.com",
        //        PhoneNo = "(123) 456-7890",
        //        DepartmentId = 100,
        //        Timer = new byte[] { 0x00 }
        //    };

        //    var tech = new Employee
        //    {
        //        FirstName = "Burner",
        //        LastName = "Tech",
        //        Email = "burner@helpdesk.com",
        //        PhoneNo = "(987) 654-3210",
        //        DepartmentId = 200,
        //        Timer = new byte[] { 0x00 }
        //    };

        //    context.Employees.AddRange(employee, tech);


        //    var problem = new Problem
        //    {
        //        Description = "Memory Upgrade",
        //        Timer = new byte[] { 0x00 }
        //    };

        //    context.Problems.Add(problem);
        //    await context.SaveChangesAsync();


        //    var newCall = new CallViewModel(options)
        //    {
        //        EmployeeId = employee.Id,
        //        TechId = tech.Id,
        //        ProblemId = problem.Id,
        //        DateOpened = DateTime.Now,
        //        DateClosed = null,
        //        OpenStatus = true,
        //        Notes = "Marouf has bad RAM, Burner to fix it"
        //    };

        //    await newCall.Add();
        //    Assert.True(newCall.Id > 0);
        //    output.WriteLine($"New Call Generated - Id = {newCall.Id}");


        //    var retrievedCall = new CallViewModel(options);
        //    await retrievedCall.GetById(newCall.Id);
        //    Assert.NotNull(retrievedCall);
        //    output.WriteLine($"Call was updated {retrievedCall.Notes}");

        //    var originalTimer = retrievedCall.Timer;

        //    var updateStatus = await retrievedCall.Update();
        //    Assert.Equal(UpdateStatus.Ok, updateStatus);
        //    output.WriteLine($"Ordered new RAM!");

        //    retrievedCall.Timer = originalTimer;
        //    retrievedCall.Notes = "Another update attempt";

        //    var staleUpdateStatus = await retrievedCall.Update();
        //    Assert.Equal(UpdateStatus.Stale, staleUpdateStatus);
        //    output.WriteLine("Call was not updated, data was stale");

        //    var deleteStatus = await retrievedCall.Delete();
        //    Assert.Equal(1, deleteStatus);
        //    output.WriteLine("Call was deleted!");

        //    var deletedCall = new CallViewModel(options);
        //    var found = await deletedCall.GetById(newCall.Id);

        //    Assert.False(found, "Expected the call to be deleted, but it was found.");
        //}

    }
    
}
