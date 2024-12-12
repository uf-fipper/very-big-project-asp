using Asp.Models.Responses;
using Asp.Services.Attributes;
using Asp.Services.MemberServices;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Context;

namespace Asp.Services.ChatServices;

[Service]
public class ChatService(
    ILogger<MemberService> logger,
    DatabaseContext context,
    IDatabase redis
) { }
