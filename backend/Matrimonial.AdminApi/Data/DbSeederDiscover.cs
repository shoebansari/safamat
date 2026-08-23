using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Data;

public static class DbSeederDiscover
{
    private const int TargetDiscoverProfiles = 52;

    private static readonly (string First, string Last)[] MaleNames =
    [
        ("Aarav", "Sharma"), ("Vikram", "Singh"), ("Rohan", "Mehta"), ("Arjun", "Reddy"),
        ("Karan", "Joshi"), ("Aditya", "Kapoor"), ("Nikhil", "Verma"), ("Siddharth", "Iyer"),
        ("Harsh", "Patel"), ("Manish", "Gupta"), ("Ravi", "Kumar"), ("Suresh", "Nair"),
        ("Deepak", "Chopra"), ("Ankit", "Malhotra"), ("Pranav", "Desai"), ("Yash", "Agarwal"),
        ("Rahul", "Bansal"), ("Amit", "Saxena"), ("Varun", "Khanna"), ("Gaurav", "Pillai"),
        ("Sanjay", "Rao"), ("Kunal", "Shah"), ("Vivek", "Mishra"), ("Abhishek", "Dubey"),
        ("Tarun", "Bhatia"), ("Rajesh", "Menon"), ("Pankaj", "Tiwari"), ("Mohit", "Sethi")
    ];

    private static readonly (string First, string Last)[] FemaleNames =
    [
        ("Ananya", "Sharma"), ("Priya", "Patel"), ("Neha", "Singh"), ("Kavya", "Reddy"),
        ("Isha", "Mehta"), ("Pooja", "Joshi"), ("Sneha", "Kapoor"), ("Divya", "Verma"),
        ("Riya", "Iyer"), ("Shreya", "Gupta"), ("Aisha", "Khan"), ("Meera", "Nair"),
        ("Tanvi", "Chopra"), ("Nidhi", "Malhotra"), ("Swati", "Desai"), ("Anjali", "Agarwal"),
        ("Kritika", "Bansal"), ("Sakshi", "Saxena"), ("Aditi", "Khanna"), ("Pallavi", "Pillai"),
        ("Rashmi", "Rao"), ("Bhavna", "Shah"), ("Lakshmi", "Mishra"), ("Geeta", "Dubey"),
        ("Sunita", "Bhatia"), ("Rekha", "Menon"), ("Jyoti", "Tiwari"), ("Nisha", "Sethi")
    ];

    private static readonly (string City, string State)[] Locations =
    [
        ("Mumbai", "Maharashtra"), ("Delhi", "Delhi"), ("Bangalore", "Karnataka"),
        ("Chennai", "Tamil Nadu"), ("Hyderabad", "Telangana"), ("Pune", "Maharashtra"),
        ("Ahmedabad", "Gujarat"), ("Kolkata", "West Bengal"), ("Jaipur", "Rajasthan"),
        ("Lucknow", "Uttar Pradesh"), ("Surat", "Gujarat"), ("Indore", "Madhya Pradesh"),
        ("Chandigarh", "Punjab"), ("Kochi", "Kerala"), ("Nagpur", "Maharashtra")
    ];

    private static readonly string[] Religions = ["Hindu", "Muslim", "Christian", "Sikh", "Jain"];
    private static readonly string[] MotherTongues = ["Hindi", "Gujarati", "Marathi", "Tamil", "Telugu", "Bengali", "Punjabi", "Kannada", "Malayalam", "English"];
    private static readonly string[] MaritalStatuses = ["Never Married", "Divorced", "Widowed"];
    private static readonly string[] Occupations = ["Software Engineer", "Doctor", "Teacher", "Business Analyst", "Chartered Accountant", "Architect", "Lawyer", "Marketing Manager", "Data Scientist", "Civil Engineer"];
    private static readonly string[] Educations = ["B.Tech", "MBA", "M.D.", "B.Com", "M.Sc", "B.A.", "M.Tech", "CA", "B.Arch", "LLB"];
    private static readonly string[] Castes = ["General", "OBC", "SC", "ST"];

    public static async Task SeedBulkDiscoverUsersAsync(ApplicationDbContext context)
    {
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.IsActive);
        if (tenant == null) return;

        var discoverableCount = await context.Users.CountAsync(u =>
            u.TenantId == tenant.TenantId && u.IsActive &&
            u.Profile != null && u.Profile.ProfileStatus == "Approved" &&
            u.Photos.Any(p => p.IsApproved));

        if (discoverableCount >= TargetDiscoverProfiles) return;

        var existingCodes = await context.Users
            .Where(u => u.TenantId == tenant.TenantId)
            .Select(u => u.UserCode)
            .ToListAsync();
        var codeSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toCreate = TargetDiscoverProfiles - discoverableCount;
        var rng = new Random(42);
        var userIndex = 3;

        for (var i = 0; i < toCreate; i++)
        {
            string userCode;
            do
            {
                userCode = $"USR{userIndex:D3}";
                userIndex++;
            } while (codeSet.Contains(userCode));
            codeSet.Add(userCode);

            var isMale = i % 2 == 0;
            var name = isMale ? MaleNames[i % MaleNames.Length] : FemaleNames[i % FemaleNames.Length];
            var gender = isMale ? "Male" : "Female";
            var loc = Locations[i % Locations.Length];
            var age = rng.Next(24, 38);
            var dob = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age).AddDays(-rng.Next(0, 365)));
            var height = isMale ? rng.Next(165, 188) : rng.Next(152, 175);
            var userId = Guid.NewGuid();
            var portraitNum = (i % 70) + 1;
            var photoFolder = isMale ? "men" : "women";
            var photoUrl = $"https://randomuser.me/api/portraits/{photoFolder}/{portraitNum}.jpg";
            var username = $"{name.First.ToLower()}{userIndex}";

            context.Users.Add(new User
            {
                UserId = userId,
                TenantId = tenant.TenantId,
                UserCode = userCode,
                UserName = username,
                FirstName = name.First,
                LastName = name.Last,
                Email = $"{username}@discover.demo",
                Phone = $"9{rng.Next(100000000, 999999999)}",
                Password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                Profile = new UserProfile
                {
                    ProfileId = Guid.NewGuid(),
                    UserId = userId,
                    Gender = gender,
                    DateOfBirth = dob,
                    Height = height,
                    Weight = rng.Next(50, 85),
                    MaritalStatus = MaritalStatuses[i % 3 == 2 ? 0 : i % 3],
                    Religion = Religions[i % Religions.Length],
                    Caste = Castes[i % Castes.Length],
                    MotherTongue = MotherTongues[i % MotherTongues.Length],
                    AboutMe = $"{name.First} is a {Occupations[i % Occupations.Length].ToLower()} from {loc.City}, looking for a compatible life partner.",
                    IsProfileCompleted = true,
                    ProfileStatus = "Approved",
                    CreatedOn = DateTime.UtcNow
                }
            });

            context.UserPhotos.Add(new UserPhoto
            {
                PhotoId = Guid.NewGuid(),
                UserId = userId,
                PhotoUrl = photoUrl,
                IsPrimary = true,
                DisplayOrder = 1,
                IsApproved = true
            });

            if (i % 3 == 0)
            {
                var extraNum = ((i + 10) % 70) + 1;
                context.UserPhotos.Add(new UserPhoto
                {
                    PhotoId = Guid.NewGuid(),
                    UserId = userId,
                    PhotoUrl = $"https://randomuser.me/api/portraits/{photoFolder}/{extraNum}.jpg",
                    IsPrimary = false,
                    DisplayOrder = 2,
                    IsApproved = true
                });
            }

            context.UserLocations.Add(new UserLocation
            {
                LocationId = Guid.NewGuid(),
                UserId = userId,
                Country = "India",
                State = loc.State,
                City = loc.City
            });

            context.UserOccupations.Add(new UserOccupation
            {
                OccupationId = Guid.NewGuid(),
                UserId = userId,
                Occupation = Occupations[i % Occupations.Length],
                WorkLocation = loc.City
            });

            context.UserEducations.Add(new UserEducation
            {
                EducationId = Guid.NewGuid(),
                UserId = userId,
                Qualification = Educations[i % Educations.Length],
                EducationType = "Full Time"
            });
        }

        await context.SaveChangesAsync();
    }
}
