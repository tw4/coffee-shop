using coffee_shop_backend.Business.Abstracts;
using coffee_shop_backend.Contexs;
using coffee_shop_backend.Dto.User;
using coffee_shop_backend.Entitys.Concreates;
using Microsoft.AspNetCore.Mvc;

namespace coffee_shop_backend.Business.Concreates;

public class UserManager : IUserServices
{
    private readonly CoffeeShopContex _coffeeShopContex;
    private readonly IJwtServices _jwtServices;

    public UserManager(CoffeeShopContex coffeeShopContex, IJwtServices jwtServices)
    {
        _coffeeShopContex = coffeeShopContex;
        _jwtServices = jwtServices;
    }

    public IActionResult AddUser(AddUserRequest request)
    {
        User user = new User() { };
        user.Name = request.Name;
        user.Surname = request.Surname;
        user.Email = request.Email;
        user.Password = request.Password;
        user.Role = EnumRole.USER;
        _coffeeShopContex.Users.Add(user);
        try
        {
            _coffeeShopContex.SaveChanges();
            return new OkObjectResult(new { message = "User added successfully", success = true });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult( new { message = e.Message});
        }
    }

    public IActionResult GetUserById(string token)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedResult();
        }

        long Id = _jwtServices.GetUserIdFromToken(token);
        User? user = (from u in _coffeeShopContex.Users where u.Id == Id select u).FirstOrDefault();

        if (user == null )
        {
            return new BadRequestObjectResult(new { message = "User not found", success = false });
        }
        return new OkObjectResult(new { message = "User found", success = true, user = user });
    }

    public IActionResult DeleteUserById(string token)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedResult();
        }

        long Id = _jwtServices.GetUserIdFromToken(token);
        User? user = (from u in _coffeeShopContex.Users where u.Id == Id select u).FirstOrDefault();

        if (user == null)
        {
            return new BadRequestObjectResult(new { message = "User not found", success = false });
        }

        _coffeeShopContex.Users.Remove(user);

        try
        {
            _coffeeShopContex.SaveChanges();
            return new OkObjectResult(new { message = "User deleted successfully", success = true });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { message = e.Message });
        }
    }

    public IActionResult UpdateUserPassword(string token, UpdateUserPasswordRequest request)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedResult();
        }

        long Id = _jwtServices.GetUserIdFromToken(token);

        User? user = (from u in _coffeeShopContex.Users where u.Id == Id select u).FirstOrDefault();

        if (user == null)
        {
            return new BadRequestObjectResult(new { message = "User not found", success = false });
        }

        user.Password = request.Password;
        _coffeeShopContex.Users.Update(user);

        try
        {
            _coffeeShopContex.SaveChanges();
            return new OkObjectResult(new { message = "User password updated successfully", success = true });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { message = e.Message });
        }
    }

    public IActionResult UpdateBasicUserInformation(string token, UpdateBasicUserInformationRequest request)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedResult();
        }

        long Id = _jwtServices.GetUserIdFromToken(token);
        User? user = (from u in _coffeeShopContex.Users where u.Id == Id select u).FirstOrDefault();

        if (user == null)
        {
            return new BadRequestObjectResult(new { message = "User not found", success = false });
        }

        user.Name = request.Name;
        user.Surname = request.Surname;
        user.Email = request.Email;
        _coffeeShopContex.Users.Update(user);

        try
        {
            _coffeeShopContex.SaveChanges();
            return new OkObjectResult(new { message = "User information updated successfully", success = true });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { message = e.Message });
        }
    }
}