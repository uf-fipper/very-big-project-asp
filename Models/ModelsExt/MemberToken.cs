using Microsoft.EntityFrameworkCore;
using Models.Context;

namespace Models.Models;

public partial class MemberToken
{
    public static async Task<MemberToken?> FromToken(
        DatabaseContext context,
        string token,
        Func<IQueryable<MemberToken>, IQueryable<MemberToken>>? extra = null
    )
    {
        var query = context.MemberTokens.Where(m => m.Token == token);
        query = extra?.Invoke(query) ?? query;
        var memberToken = await query.FirstOrDefaultAsync();
        return memberToken;
    }
}
