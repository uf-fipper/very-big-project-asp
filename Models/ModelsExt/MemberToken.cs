using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;

namespace Models.ModelsExt;

public static partial class MemberExt
{
    public static IQueryable<MemberToken> WhereMember(
        this IQueryable<MemberToken> query,
        int memberId
    ) => query.Where(m => m.MemberId == memberId);

    public static IQueryable<MemberToken> WhereToken(
        this IQueryable<MemberToken> query,
        string token
    ) => query.Where(m => m.Token == token);

    public static IQueryable<MemberToken> MustAvailable(this IQueryable<MemberToken> query) =>
        query.Where(m => m.Status == true && DateTime.Now < m.ExpireTime);
}
