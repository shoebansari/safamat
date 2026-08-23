using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IUserSocialService
{
    Task<List<DiscoverProfileDto>> DiscoverAsync(Guid userId, Guid tenantId, DiscoverFilterRequest? filters = null);
    Task<DiscoverFilterOptionsDto> GetDiscoverFilterOptionsAsync(Guid userId, Guid tenantId);
    Task<List<InterestRequestDto>> GetInterestsAsync(Guid userId, string type);
    Task<InterestRequestDto?> SendInterestAsync(Guid senderId, Guid receiverId);
    Task<InterestRequestDto?> RespondInterestAsync(Guid userId, Guid interestId, string status);
    Task<List<MatchDto>> GetMatchesAsync(Guid userId);
    Task<List<DiscoverProfileDto>> GetFavoritesAsync(Guid userId);
    Task<bool> ToggleFavoriteAsync(Guid userId, Guid favoriteUserId);
    Task<bool> BlockUserAsync(Guid userId, Guid blockedUserId);
    Task<bool> ReportUserAsync(Guid reporterId, ReportUserRequest request);
}

public class UserSocialService : IUserSocialService
{
    private readonly ApplicationDbContext _context;

    public UserSocialService(ApplicationDbContext context) => _context = context;

    public async Task<List<DiscoverProfileDto>> DiscoverAsync(Guid userId, Guid tenantId, DiscoverFilterRequest? filters = null)
    {
        var blocked = await GetBlockedIdsAsync(userId);
        var me = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserId == userId);
        var myGender = me?.Profile?.Gender;

        var users = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .Include(u => u.Occupation)
            .Include(u => u.Educations)
            .Include(u => u.Location)
            .Include(u => u.Preference)
            .Where(u => u.TenantId == tenantId && u.UserId != userId && u.IsActive &&
                        u.Profile != null && u.Profile.ProfileStatus == "Approved" &&
                        u.Photos.Any(p => p.IsApproved) &&
                        !blocked.Contains(u.UserId))
            .ToListAsync();

        var genderFilter = filters?.Gender;
        if (string.IsNullOrWhiteSpace(genderFilter) && !string.IsNullOrWhiteSpace(myGender))
        {
            var opposite = myGender.Equals("Male", StringComparison.OrdinalIgnoreCase) ? "Female" :
                myGender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? "Male" : null;
            if (opposite != null)
            {
                var filtered = users.Where(u =>
                    u.Profile?.Gender != null &&
                    u.Profile.Gender.Equals(opposite, StringComparison.OrdinalIgnoreCase)).ToList();
                if (filtered.Count > 0) users = filtered;
            }
        }
        else if (!string.IsNullOrWhiteSpace(genderFilter))
        {
            users = users.Where(u =>
                u.Profile?.Gender != null &&
                u.Profile.Gender.Equals(genderFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        users = ApplyDiscoverFilters(users, filters);

        var myPref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
        return users.Select(u => MapDiscover(u, myPref)).OrderByDescending(p => p.MatchPercentage).ToList();
    }

    public async Task<DiscoverFilterOptionsDto> GetDiscoverFilterOptionsAsync(Guid userId, Guid tenantId)
    {
        var blocked = await GetBlockedIdsAsync(userId);
        var users = await _context.Users
            .Include(u => u.Profile)
            .Include(u => u.Photos)
            .Include(u => u.Occupation)
            .Include(u => u.Educations)
            .Include(u => u.Location)
            .Where(u => u.TenantId == tenantId && u.UserId != userId && u.IsActive &&
                        u.Profile != null && u.Profile.ProfileStatus == "Approved" &&
                        u.Photos.Any(p => p.IsApproved) &&
                        !blocked.Contains(u.UserId))
            .ToListAsync();

        return new DiscoverFilterOptionsDto
        {
            Cities = Distinct(users.Select(u => u.Location?.City)),
            States = Distinct(users.Select(u => u.Location?.State)),
            Religions = Distinct(users.Select(u => u.Profile?.Religion)),
            MotherTongues = Distinct(users.Select(u => u.Profile?.MotherTongue)),
            MaritalStatuses = Distinct(users.Select(u => u.Profile?.MaritalStatus)),
            Occupations = Distinct(users.Select(u => u.Occupation?.Occupation)),
            Educations = Distinct(users.Select(u => u.Educations.FirstOrDefault()?.Qualification)),
            Genders = Distinct(users.Select(u => u.Profile?.Gender))
        };
    }

    private static List<string> Distinct(IEnumerable<string?> values) =>
        values.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v!.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(v => v).ToList();

    private static List<Entities.User> ApplyDiscoverFilters(List<Entities.User> users, DiscoverFilterRequest? filters)
    {
        if (filters == null) return users;

        if (filters.MinAge.HasValue)
            users = users.Where(u => GetAge(u.Profile?.DateOfBirth) >= filters.MinAge).ToList();
        if (filters.MaxAge.HasValue)
            users = users.Where(u => GetAge(u.Profile?.DateOfBirth) <= filters.MaxAge).ToList();
        if (!string.IsNullOrWhiteSpace(filters.City))
            users = users.Where(u => Eq(u.Location?.City, filters.City)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.State))
            users = users.Where(u => Eq(u.Location?.State, filters.State)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.Religion))
            users = users.Where(u => Eq(u.Profile?.Religion, filters.Religion)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.Caste))
            users = users.Where(u => Eq(u.Profile?.Caste, filters.Caste)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.MotherTongue))
            users = users.Where(u => Eq(u.Profile?.MotherTongue, filters.MotherTongue)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.MaritalStatus))
            users = users.Where(u => Eq(u.Profile?.MaritalStatus, filters.MaritalStatus)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.Occupation))
            users = users.Where(u => Eq(u.Occupation?.Occupation, filters.Occupation)).ToList();
        if (!string.IsNullOrWhiteSpace(filters.Education))
            users = users.Where(u => Eq(u.Educations.FirstOrDefault()?.Qualification, filters.Education)).ToList();
        if (filters.MinHeight.HasValue)
            users = users.Where(u => u.Profile?.Height >= filters.MinHeight).ToList();
        if (filters.MaxHeight.HasValue)
            users = users.Where(u => u.Profile?.Height <= filters.MaxHeight).ToList();

        return users;
    }

    private static bool Eq(string? value, string? filter) =>
        value != null && filter != null &&
        value.Equals(filter, StringComparison.OrdinalIgnoreCase);

    private static int? GetAge(DateOnly? dob)
    {
        if (!dob.HasValue) return null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Value.Year;
        if (dob.Value > today.AddYears(-age)) age--;
        return age;
    }

    public async Task<List<InterestRequestDto>> GetInterestsAsync(Guid userId, string type)
    {
        IQueryable<Entities.InterestRequest> query = _context.InterestRequests
            .Include(i => i.Sender).ThenInclude(s => s.Photos)
            .Include(i => i.Receiver).ThenInclude(r => r.Photos);

        query = type.ToLower() == "sent"
            ? query.Where(i => i.SenderUserId == userId)
            : query.Where(i => i.ReceiverUserId == userId);

        var items = await query.OrderByDescending(i => i.SentOn).ToListAsync();
        return items.Select(MapInterest).ToList();
    }

    public async Task<InterestRequestDto?> SendInterestAsync(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId) return null;
        if (await _context.InterestRequests.AnyAsync(i =>
            i.SenderUserId == senderId && i.ReceiverUserId == receiverId))
            throw new InvalidOperationException("Interest already sent.");

        var interest = new Entities.InterestRequest
        {
            InterestId = Guid.NewGuid(),
            SenderUserId = senderId,
            ReceiverUserId = receiverId,
            Status = "Pending",
            SentOn = DateTime.UtcNow
        };
        _context.InterestRequests.Add(interest);

        var sender = await _context.Users.FindAsync(senderId);
        _context.Notifications.Add(new Entities.Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = receiverId,
            Title = "New Interest",
            MessageText = sender != null
                ? $"{sender.FirstName} {sender.LastName} sent you an interest request."
                : "Someone sent you an interest request.",
            RelatedUserId = senderId,
            CreatedOn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return await GetInterestByIdAsync(interest.InterestId);
    }

    public async Task<InterestRequestDto?> RespondInterestAsync(Guid userId, Guid interestId, string status)
    {
        var interest = await _context.InterestRequests
            .Include(i => i.Sender).ThenInclude(s => s.Photos)
            .Include(i => i.Receiver).ThenInclude(r => r.Photos)
            .FirstOrDefaultAsync(i => i.InterestId == interestId && i.ReceiverUserId == userId);
        if (interest == null) return null;

        interest.Status = status is "Accepted" or "Rejected" ? status : "Pending";
        interest.RespondedOn = DateTime.UtcNow;

        if (interest.Status == "Accepted")
        {
            var exists = await _context.Matches.AnyAsync(m =>
                (m.UserId1 == interest.SenderUserId && m.UserId2 == interest.ReceiverUserId) ||
                (m.UserId1 == interest.ReceiverUserId && m.UserId2 == interest.SenderUserId));
            if (!exists)
            {
                _context.Matches.Add(new Entities.Match
                {
                    MatchId = Guid.NewGuid(),
                    UserId1 = interest.SenderUserId,
                    UserId2 = interest.ReceiverUserId,
                    MatchPercentage = 85,
                    MatchedOn = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
        return MapInterest(interest);
    }

    public async Task<List<MatchDto>> GetMatchesAsync(Guid userId)
    {
        var matches = await _context.Matches
            .Include(m => m.User1).ThenInclude(u => u.Photos)
            .Include(m => m.User2).ThenInclude(u => u.Photos)
            .Where(m => m.UserId1 == userId || m.UserId2 == userId)
            .OrderByDescending(m => m.MatchedOn)
            .ToListAsync();

        return matches.Select(m =>
        {
            var other = m.UserId1 == userId ? m.User2 : m.User1;
            return new MatchDto
            {
                MatchId = m.MatchId,
                UserId = other.UserId,
                Name = $"{other.FirstName} {other.LastName}",
                PhotoUrl = GetApprovedPhoto(other),
                MatchPercentage = m.MatchPercentage,
                MatchedOn = m.MatchedOn
            };
        }).ToList();
    }

    public async Task<List<DiscoverProfileDto>> GetFavoritesAsync(Guid userId)
    {
        var favIds = await _context.Favorites.Where(f => f.UserId == userId).Select(f => f.FavoriteUserId).ToListAsync();
        var users = await _context.Users
            .Include(u => u.Profile).Include(u => u.Photos).Include(u => u.Occupation).Include(u => u.Location)
            .Where(u => favIds.Contains(u.UserId))
            .ToListAsync();
        return users.Select(u => MapDiscover(u, null)).ToList();
    }

    public async Task<bool> ToggleFavoriteAsync(Guid userId, Guid favoriteUserId)
    {
        var existing = await _context.Favorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.FavoriteUserId == favoriteUserId);
        if (existing != null)
        {
            _context.Favorites.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }
        _context.Favorites.Add(new Entities.Favorite
        {
            FavoriteId = Guid.NewGuid(),
            UserId = userId,
            FavoriteUserId = favoriteUserId,
            CreatedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BlockUserAsync(Guid userId, Guid blockedUserId)
    {
        if (userId == blockedUserId) return false;
        if (await _context.BlockedUsers.AnyAsync(b => b.UserId == userId && b.BlockedUserId == blockedUserId))
            return true;
        _context.BlockedUsers.Add(new Entities.BlockedUser
        {
            BlockedId = Guid.NewGuid(),
            UserId = userId,
            BlockedUserId = blockedUserId,
            CreatedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReportUserAsync(Guid reporterId, ReportUserRequest request)
    {
        _context.Reports.Add(new Entities.Report
        {
            ReportId = Guid.NewGuid(),
            ReporterUserId = reporterId,
            ReportedUserId = request.ReportedUserId,
            Reason = request.Reason,
            Details = request.Details,
            Status = "Pending",
            CreatedOn = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<HashSet<Guid>> GetBlockedIdsAsync(Guid userId)
    {
        var blocked = await _context.BlockedUsers
            .Where(b => b.UserId == userId || b.BlockedUserId == userId)
            .Select(b => b.UserId == userId ? b.BlockedUserId : b.UserId)
            .ToListAsync();
        return blocked.ToHashSet();
    }

    private async Task<InterestRequestDto?> GetInterestByIdAsync(Guid id)
    {
        var interest = await _context.InterestRequests
            .Include(i => i.Sender).ThenInclude(s => s.Photos)
            .Include(i => i.Receiver).ThenInclude(r => r.Photos)
            .FirstOrDefaultAsync(i => i.InterestId == id);
        return interest == null ? null : MapInterest(interest);
    }

    private static DiscoverProfileDto MapDiscover(Entities.User u, Entities.UserPreference? myPref)
    {
        var photos = GetApprovedPhotos(u);
        return new DiscoverProfileDto
        {
            UserId = u.UserId,
            UserCode = u.UserCode,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Age = GetAge(u.Profile?.DateOfBirth),
            Gender = u.Profile?.Gender,
            City = u.Location?.City,
            State = u.Location?.State,
            Religion = u.Profile?.Religion,
            Caste = u.Profile?.Caste,
            MotherTongue = u.Profile?.MotherTongue,
            MaritalStatus = u.Profile?.MaritalStatus,
            Occupation = u.Occupation?.Occupation,
            Education = u.Educations.FirstOrDefault()?.Qualification,
            Height = u.Profile?.Height,
            PrimaryPhotoUrl = photos.FirstOrDefault(),
            PhotoUrls = photos,
            MatchPercentage = CalcMatch(u, myPref)
        };
    }

    private static List<string> GetApprovedPhotos(Entities.User u) =>
        u.Photos.Where(p => p.IsApproved).OrderBy(p => p.IsPrimary ? 0 : 1).ThenBy(p => p.DisplayOrder)
            .Select(p => p.PhotoUrl).ToList();

    private static decimal CalcMatch(Entities.User u, Entities.UserPreference? pref)
    {
        if (pref == null) return 50;
        var score = 50m;
        if (!string.IsNullOrEmpty(pref.Religion) && pref.Religion == u.Profile?.Religion) score += 15;
        if (!string.IsNullOrEmpty(pref.City) && pref.City == u.Location?.City) score += 15;
        if (!string.IsNullOrEmpty(pref.Occupation) && pref.Occupation == u.Occupation?.Occupation) score += 10;
        if (pref.MinAge.HasValue && pref.MaxAge.HasValue && u.Profile?.DateOfBirth.HasValue == true)
        {
            var age = DateOnly.FromDateTime(DateTime.UtcNow).Year - u.Profile.DateOfBirth!.Value.Year;
            if (age >= pref.MinAge && age <= pref.MaxAge) score += 10;
        }
        return Math.Min(score, 99);
    }

    private static string? GetApprovedPhoto(Entities.User u) =>
        u.Photos.FirstOrDefault(p => p.IsPrimary && p.IsApproved)?.PhotoUrl
        ?? u.Photos.FirstOrDefault(p => p.IsApproved)?.PhotoUrl;

    private static InterestRequestDto MapInterest(Entities.InterestRequest i) => new()
    {
        InterestId = i.InterestId,
        SenderUserId = i.SenderUserId,
        ReceiverUserId = i.ReceiverUserId,
        SenderName = $"{i.Sender.FirstName} {i.Sender.LastName}",
        ReceiverName = $"{i.Receiver.FirstName} {i.Receiver.LastName}",
        SenderPhotoUrl = GetApprovedPhoto(i.Sender),
        ReceiverPhotoUrl = GetApprovedPhoto(i.Receiver),
        Status = i.Status,
        SentOn = i.SentOn
    };
}
