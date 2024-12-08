using AuthService.BLL.Interfaces.Services;
using MassTransit;

namespace AuthService.BLL.Messaging.Consumers;

public class GetUserConsumer : IConsumer<GetUserMessage>
{
    private readonly IUserService _userService;

    public GetUserConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<GetUserMessage> context)
    {
        var userId = context.Message.UserId;
        var user = await _userService.GetByIdAsync(userId);
        await context.RespondAsync(new UserResponseMessage()
        {
            Id = user.Id,
            Login = user.Login,
            FirstName = user.FirstName,
            LastName = user.LastName
        });
    }
}