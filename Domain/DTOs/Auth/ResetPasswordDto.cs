using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Auth;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string? ConfirmPassword { get; set; }
}
