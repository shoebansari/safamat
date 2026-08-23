using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetMyProfileAsync(Guid userId);
    Task<UserProfileDto?> GetPublicProfileAsync(Guid viewerId, Guid targetUserId);
    Task<UserProfileDto?> SaveBasicProfileAsync(Guid userId, SaveProfileRequest request);
    Task<UserProfileDto?> SaveEducationAsync(Guid userId, SaveEducationRequest request);
    Task<UserProfileDto?> SaveOccupationAsync(Guid userId, SaveOccupationRequest request);
    Task<UserProfileDto?> SaveFamilyAsync(Guid userId, SaveFamilyRequest request);
    Task<UserProfileDto?> SaveLifestyleAsync(Guid userId, SaveLifestyleRequest request);
    Task<UserProfileDto?> SaveLocationAsync(Guid userId, SaveLocationRequest request);
    Task<UserPreferenceDto?> GetPreferencesAsync(Guid userId);
    Task<UserPreferenceDto?> SavePreferencesAsync(Guid userId, SavePreferenceRequest request);
    Task<UserPhotoDto?> AddPhotoAsync(Guid userId, AddPhotoRequest request);
    Task<UserPhotoDto?> UploadPhotoAsync(Guid userId, IFormFile file, bool isPrimary);
    Task<bool> DeletePhotoAsync(Guid userId, Guid photoId);
    Task<UserProfileDto?> GetForTenantReviewAsync(Guid tenantId, Guid userId);
}

public class UserProfileService : IUserProfileService
{
    private readonly ApplicationDbContext _context;
    private readonly IPhotoStorageService _photoStorage;

    public UserProfileService(ApplicationDbContext context, IPhotoStorageService photoStorage)
    {
        _context = context;
        _photoStorage = photoStorage;
    }

    public async Task<UserProfileDto?> GetMyProfileAsync(Guid userId) =>
        await LoadProfileAsync(userId, includeUnapprovedPhotos: true);

    public async Task<UserProfileDto?> GetPublicProfileAsync(Guid viewerId, Guid targetUserId)
    {
        await RecordViewAsync(viewerId, targetUserId);
        return await LoadProfileAsync(targetUserId, includeUnapprovedPhotos: false);
    }

    public async Task<UserProfileDto?> SaveBasicProfileAsync(Guid userId, SaveProfileRequest request)
    {
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return null;

        if (user.Profile == null)
        {
            user.Profile = new Entities.UserProfile { ProfileId = Guid.NewGuid(), UserId = userId, CreatedOn = DateTime.UtcNow };
            _context.UserProfiles.Add(user.Profile);
        }

        var p = user.Profile;
        p.Gender = request.Gender;
        p.DateOfBirth = request.DateOfBirth;
        p.Height = request.Height;
        p.Weight = request.Weight;
        p.MaritalStatus = request.MaritalStatus;
        p.Religion = request.Religion;
        p.Caste = request.Caste;
        p.SubCaste = request.SubCaste;
        p.MotherTongue = request.MotherTongue;
        p.BloodGroup = request.BloodGroup;
        p.AboutMe = request.AboutMe;
        p.IsProfileCompleted = !string.IsNullOrWhiteSpace(request.Gender) &&
                               request.DateOfBirth.HasValue &&
                               !string.IsNullOrWhiteSpace(request.AboutMe);
        p.ProfileStatus = p.IsProfileCompleted ? "Pending" : p.ProfileStatus;
        p.UpdatedOn = DateTime.UtcNow;
        user.UpdatedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserProfileDto?> SaveEducationAsync(Guid userId, SaveEducationRequest request)
    {
        var edu = await _context.UserEducations.FirstOrDefaultAsync(e => e.UserId == userId);
        if (edu == null)
        {
            edu = new Entities.UserEducation { EducationId = Guid.NewGuid(), UserId = userId };
            _context.UserEducations.Add(edu);
        }
        edu.Qualification = request.Qualification;
        edu.College = request.College;
        edu.University = request.University;
        edu.PassingYear = request.PassingYear;
        edu.EducationType = request.EducationType;
        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserProfileDto?> SaveOccupationAsync(Guid userId, SaveOccupationRequest request)
    {
        var occ = await _context.UserOccupations.FirstOrDefaultAsync(o => o.UserId == userId);
        if (occ == null)
        {
            occ = new Entities.UserOccupation { OccupationId = Guid.NewGuid(), UserId = userId };
            _context.UserOccupations.Add(occ);
        }
        occ.Occupation = request.Occupation;
        occ.CompanyName = request.CompanyName;
        occ.Designation = request.Designation;
        occ.AnnualIncome = request.AnnualIncome;
        occ.WorkLocation = request.WorkLocation;
        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserProfileDto?> SaveFamilyAsync(Guid userId, SaveFamilyRequest request)
    {
        var fam = await _context.UserFamilyDetails.FirstOrDefaultAsync(f => f.UserId == userId);
        if (fam == null)
        {
            fam = new Entities.UserFamilyDetail { FamilyId = Guid.NewGuid(), UserId = userId };
            _context.UserFamilyDetails.Add(fam);
        }
        fam.FamilyType = request.FamilyType;
        fam.FamilyStatus = request.FamilyStatus;
        fam.FatherName = request.FatherName;
        fam.FatherOccupation = request.FatherOccupation;
        fam.MotherName = request.MotherName;
        fam.MotherOccupation = request.MotherOccupation;
        fam.Brothers = request.Brothers;
        fam.Sisters = request.Sisters;
        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserProfileDto?> SaveLifestyleAsync(Guid userId, SaveLifestyleRequest request)
    {
        var life = await _context.UserLifestyles.FirstOrDefaultAsync(l => l.UserId == userId);
        if (life == null)
        {
            life = new Entities.UserLifestyle { LifestyleId = Guid.NewGuid(), UserId = userId };
            _context.UserLifestyles.Add(life);
        }
        life.Diet = request.Diet;
        life.Smoking = request.Smoking;
        life.Drinking = request.Drinking;
        life.Hobbies = request.Hobbies;
        life.LanguagesKnown = request.LanguagesKnown;
        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserProfileDto?> SaveLocationAsync(Guid userId, SaveLocationRequest request)
    {
        var loc = await _context.UserLocations.FirstOrDefaultAsync(l => l.UserId == userId);
        if (loc == null)
        {
            loc = new Entities.UserLocation { LocationId = Guid.NewGuid(), UserId = userId };
            _context.UserLocations.Add(loc);
        }
        loc.Country = request.Country;
        loc.State = request.State;
        loc.City = request.City;
        loc.Address = request.Address;
        loc.Pincode = request.Pincode;
        await _context.SaveChangesAsync();
        return await GetMyProfileAsync(userId);
    }

    public async Task<UserPreferenceDto?> GetPreferencesAsync(Guid userId)
    {
        var pref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
        if (pref == null) return null;
        return MapPreference(pref);
    }

    public async Task<UserPreferenceDto?> SavePreferencesAsync(Guid userId, SavePreferenceRequest request)
    {
        var pref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
        if (pref == null)
        {
            pref = new Entities.UserPreference { PreferenceId = Guid.NewGuid(), UserId = userId };
            _context.UserPreferences.Add(pref);
        }
        pref.MinAge = request.MinAge;
        pref.MaxAge = request.MaxAge;
        pref.MinHeight = request.MinHeight;
        pref.MaxHeight = request.MaxHeight;
        pref.Religion = request.Religion;
        pref.Caste = request.Caste;
        pref.Education = request.Education;
        pref.Occupation = request.Occupation;
        pref.Country = request.Country;
        pref.State = request.State;
        pref.City = request.City;
        await _context.SaveChangesAsync();
        return MapPreference(pref);
    }

    public async Task<UserPhotoDto?> AddPhotoAsync(Guid userId, AddPhotoRequest request)
    {
        var count = await _context.UserPhotos.CountAsync(p => p.UserId == userId);
        if (count >= 3)
            throw new InvalidOperationException("Maximum 3 photos allowed per profile.");

        if (request.IsPrimary)
        {
            var existing = await _context.UserPhotos.Where(p => p.UserId == userId && p.IsPrimary).ToListAsync();
            foreach (var p in existing) p.IsPrimary = false;
        }

        var order = await _context.UserPhotos.CountAsync(p => p.UserId == userId);
        var photo = new Entities.UserPhoto
        {
            PhotoId = Guid.NewGuid(),
            UserId = userId,
            PhotoUrl = request.PhotoUrl,
            IsPrimary = request.IsPrimary || order == 0,
            DisplayOrder = order + 1,
            IsApproved = false,
            UploadedOn = DateTime.UtcNow
        };
        _context.UserPhotos.Add(photo);
        await _context.SaveChangesAsync();
        return MapPhoto(photo);
    }

    public async Task<UserPhotoDto?> UploadPhotoAsync(Guid userId, IFormFile file, bool isPrimary)
    {
        const long maxBytes = 2 * 1024 * 1024;
        if (file.Length == 0) throw new InvalidOperationException("No file uploaded.");
        if (file.Length > maxBytes) throw new InvalidOperationException("Could not upload: image must not be more than 2 MB.");

        var allowed = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType.ToLower()))
            throw new InvalidOperationException("Only JPG, PNG, or WEBP images are allowed.");

        var count = await _context.UserPhotos.CountAsync(p => p.UserId == userId);
        if (count >= 3)
            throw new InvalidOperationException("Maximum 3 photos allowed per profile.");

        if (isPrimary)
        {
            var existing = await _context.UserPhotos.Where(p => p.UserId == userId && p.IsPrimary).ToListAsync();
            foreach (var p in existing) p.IsPrimary = false;
        }

        var order = await _context.UserPhotos.CountAsync(p => p.UserId == userId);
        var photoId = Guid.NewGuid();
        var stored = await _photoStorage.StoreUserPhotoAsync(photoId, userId, file);

        var photo = new Entities.UserPhoto
        {
            PhotoId = photoId,
            UserId = userId,
            PhotoUrl = stored.PhotoUrl,
            ImageData = stored.ImageData,
            ContentType = stored.ContentType,
            IsPrimary = isPrimary || order == 0,
            DisplayOrder = order + 1,
            IsApproved = false,
            UploadedOn = DateTime.UtcNow
        };
        _context.UserPhotos.Add(photo);
        await _context.SaveChangesAsync();
        return MapPhoto(photo);
    }

    public async Task<bool> DeletePhotoAsync(Guid userId, Guid photoId)
    {
        var photo = await _context.UserPhotos.FirstOrDefaultAsync(p => p.PhotoId == photoId && p.UserId == userId);
        if (photo == null) return false;

        if (photo.PhotoUrl.StartsWith("/uploads/"))
        {
            var relative = photo.PhotoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            // Caller may delete file separately; best-effort skip here
        }

        _context.UserPhotos.Remove(photo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserProfileDto?> GetForTenantReviewAsync(Guid tenantId, Guid userId)
    {
        var belongs = await _context.Users.AnyAsync(u => u.UserId == userId && u.TenantId == tenantId);
        if (!belongs) return null;
        return await LoadProfileAsync(userId, includeUnapprovedPhotos: true);
    }

    private async Task RecordViewAsync(Guid viewerId, Guid targetUserId)
    {
        if (viewerId == targetUserId) return;
        _context.ProfileViews.Add(new Entities.ProfileView
        {
            ViewId = Guid.NewGuid(),
            ViewerUserId = viewerId,
            ViewedUserId = targetUserId,
            ViewedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    private async Task<UserProfileDto?> LoadProfileAsync(Guid userId, bool includeUnapprovedPhotos)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .Include(u => u.Educations)
            .Include(u => u.Occupation)
            .Include(u => u.Family)
            .Include(u => u.Lifestyle)
            .Include(u => u.Location)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return null;

        var isApproved = user.Profile?.ProfileStatus == "Approved";
        if (!includeUnapprovedPhotos && !isApproved) return null;

        var photos = user.Photos.AsEnumerable();
        if (!includeUnapprovedPhotos)
            photos = photos.Where(p => p.IsApproved);
        else if (!photos.Any())
            photos = user.Photos.AsEnumerable();

        var edu = user.Educations.FirstOrDefault();
        return new UserProfileDto
        {
            ProfileId = user.Profile?.ProfileId ?? Guid.Empty,
            UserId = user.UserId,
            UserCode = user.UserCode,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Gender = user.Profile?.Gender,
            DateOfBirth = user.Profile?.DateOfBirth,
            Age = CalcAge(user.Profile?.DateOfBirth),
            Height = user.Profile?.Height,
            Weight = user.Profile?.Weight,
            MaritalStatus = user.Profile?.MaritalStatus,
            Religion = user.Profile?.Religion,
            Caste = user.Profile?.Caste,
            SubCaste = user.Profile?.SubCaste,
            MotherTongue = user.Profile?.MotherTongue,
            BloodGroup = user.Profile?.BloodGroup,
            AboutMe = user.Profile?.AboutMe,
            IsProfileCompleted = user.Profile?.IsProfileCompleted ?? false,
            ProfileStatus = user.Profile?.ProfileStatus ?? "Pending",
            PrimaryPhotoUrl = photos.FirstOrDefault(p => p.IsPrimary)?.PhotoUrl ?? photos.FirstOrDefault()?.PhotoUrl,
            Education = edu == null ? null : new UserEducationDto
            {
                EducationId = edu.EducationId, Qualification = edu.Qualification, College = edu.College,
                University = edu.University, PassingYear = edu.PassingYear, EducationType = edu.EducationType
            },
            Occupation = user.Occupation == null ? null : new UserOccupationDto
            {
                OccupationId = user.Occupation.OccupationId, Occupation = user.Occupation.Occupation,
                CompanyName = user.Occupation.CompanyName, Designation = user.Occupation.Designation,
                AnnualIncome = user.Occupation.AnnualIncome, WorkLocation = user.Occupation.WorkLocation
            },
            Family = user.Family == null ? null : new UserFamilyDto
            {
                FamilyId = user.Family.FamilyId, FamilyType = user.Family.FamilyType,
                FamilyStatus = user.Family.FamilyStatus, FatherName = user.Family.FatherName,
                FatherOccupation = user.Family.FatherOccupation, MotherName = user.Family.MotherName,
                MotherOccupation = user.Family.MotherOccupation, Brothers = user.Family.Brothers, Sisters = user.Family.Sisters
            },
            Lifestyle = user.Lifestyle == null ? null : new UserLifestyleDto
            {
                LifestyleId = user.Lifestyle.LifestyleId, Diet = user.Lifestyle.Diet,
                Smoking = user.Lifestyle.Smoking, Drinking = user.Lifestyle.Drinking,
                Hobbies = user.Lifestyle.Hobbies, LanguagesKnown = user.Lifestyle.LanguagesKnown
            },
            Location = user.Location == null ? null : new UserLocationDto
            {
                LocationId = user.Location.LocationId, Country = user.Location.Country,
                State = user.Location.State, City = user.Location.City,
                Address = user.Location.Address, Pincode = user.Location.Pincode
            },
            Photos = photos.OrderBy(p => p.DisplayOrder).Select(MapPhoto).ToList()
        };
    }

    private static int? CalcAge(DateOnly? dob)
    {
        if (!dob.HasValue) return null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Value.Year;
        if (dob.Value > today.AddYears(-age)) age--;
        return age;
    }

    private static UserPhotoDto MapPhoto(Entities.UserPhoto p) => new()
    {
        PhotoId = p.PhotoId, PhotoUrl = p.PhotoUrl, IsPrimary = p.IsPrimary,
        DisplayOrder = p.DisplayOrder, IsApproved = p.IsApproved, UploadedOn = p.UploadedOn
    };

    private static UserPreferenceDto MapPreference(Entities.UserPreference p) => new()
    {
        PreferenceId = p.PreferenceId, MinAge = p.MinAge, MaxAge = p.MaxAge,
        MinHeight = p.MinHeight, MaxHeight = p.MaxHeight, Religion = p.Religion,
        Caste = p.Caste, Education = p.Education, Occupation = p.Occupation,
        Country = p.Country, State = p.State, City = p.City
    };
}
