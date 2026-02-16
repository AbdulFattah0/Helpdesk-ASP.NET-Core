using HelpdeskDAL;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

public class EmployeeViewModel
{
    public string? Title { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public string? Phoneno { get; set; }
    public int? DepartmentId { get; set; }
    public int? Id { get; set; }
    public bool IsTech { get; set; }
    public string? StaffPicture64 { get; set; }

    [JsonIgnore]
    public byte[]? Timer { get; set; }

    [JsonPropertyName("timerBase64")]
    public string? TimerBase64
    {
        get => Timer != null ? Convert.ToBase64String(Timer) : null;
        set
        {
            try
            {
                Timer = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : null;
            }
            catch (FormatException)
            {
                Timer = null;
                Console.WriteLine("Invalid Base64 string provided for Timer.");
            }
        }
    }

    public async Task<List<EmployeeViewModel>> GetAll(HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        var employees = await dao.GetAll();
        var viewModelList = new List<EmployeeViewModel>();

        foreach (var emp in employees)
        {
            var viewModel = new EmployeeViewModel
            {
                Title = emp.Title ?? "No Title",
                Firstname = emp.FirstName ?? string.Empty,
                Lastname = emp.LastName ?? string.Empty,
                Email = emp.Email ?? string.Empty,
                Phoneno = emp.PhoneNo ?? string.Empty,
                DepartmentId = emp.DepartmentId,
                Id = emp.Id,
                IsTech = emp.IsTech ?? false, 
                Timer = emp.Timer,
                StaffPicture64 = emp.StaffPicture != null ? Convert.ToBase64String(emp.StaffPicture) : null
            };

            viewModelList.Add(viewModel);
        }

        return viewModelList;
    }


public async Task GetByLastname(string lastname, HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        var employee = await dao.GetByLastname(lastname);

        if (employee != null)
        {
            PopulateViewModel(employee);
        }
    }

    public async Task GetById(int id, HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        var employee = await dao.GetById(id);
        if (employee != null)
        {
            PopulateViewModel(employee);
        }
    }

    public async Task Add(HelpdeskContext context)
    {
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname))
        {
            throw new ArgumentException("First name and last name are required.");
        }

        var dao = new EmployeeDAO(context);

        try
        {
            var employee = new Employee
            {
                Title = this.Title ?? "No Title",
                FirstName = this.Firstname ?? string.Empty,
                LastName = this.Lastname ?? string.Empty,
                Email = this.Email ?? string.Empty,
                PhoneNo = this.Phoneno ?? string.Empty,
                DepartmentId = this.DepartmentId ?? 0,
                Timer = this.Timer,
                StaffPicture = string.IsNullOrEmpty(StaffPicture64) ? null : Convert.FromBase64String(StaffPicture64)
            };

            var result = await dao.Add(employee);

            if (result == UpdateStatus.Ok)
            {
                // Assign the new ID to the ViewModel
                this.Id = employee.Id;
            }
            else
            {
                throw new InvalidOperationException("Failed to add employee.");
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Invalid Base64 string in StaffPicture64: {ex.Message}");
            throw new ArgumentException("Staff picture must be a valid Base64 string.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding employee: {ex.Message}");
            throw;
        }
    }


    public async Task<UpdateStatus> Update(HelpdeskContext context)
    {
        if (Id == null) return UpdateStatus.Failed;

        var dao = new EmployeeDAO(context);
        var employee = await dao.GetById((int)Id);
        if (employee == null) return UpdateStatus.Failed;

        employee.Title = this.Title ?? employee.Title;
        employee.FirstName = this.Firstname ?? employee.FirstName;
        employee.LastName = this.Lastname ?? employee.LastName;
        employee.Email = this.Email ?? employee.Email;
        employee.PhoneNo = this.Phoneno ?? employee.PhoneNo;
        employee.DepartmentId = this.DepartmentId ?? employee.DepartmentId;
        // Preserve original Timer for concurrency check
        employee.Timer = this.Timer ?? employee.Timer;
        employee.StaffPicture = StaffPicture64 != null ? Convert.FromBase64String(StaffPicture64!) : null;

        return await dao.Update(employee);
    }

    public async Task GetByEmail(string email, HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        var employee = await dao.GetByEmail(email.ToLower());
        if (employee != null)
        {
            PopulateViewModel(employee);
        }
    }

    public async Task GetByPhoneNumber(string phoneNumber, HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        var employee = await dao.GetByPhoneNumber(phoneNumber);
        if (employee != null)
        {
            PopulateViewModel(employee);
        }
    }

    public async Task<int> Delete(int id, HelpdeskContext context)
    {
        var dao = new EmployeeDAO(context);
        return await dao.Delete(id);
    }

    private void PopulateViewModel(Employee employee)
    {
        Title = employee.Title ?? "No Title";
        Firstname = employee.FirstName ?? string.Empty;
        Lastname = employee.LastName ?? string.Empty;
        Email = employee.Email ?? string.Empty;
        Phoneno = employee.PhoneNo ?? string.Empty;
        DepartmentId = employee.DepartmentId;
        Timer = employee.Timer;
        Id = employee.Id;
        StaffPicture64 = employee.StaffPicture != null ? Convert.ToBase64String(employee.StaffPicture) : null;
    }
}