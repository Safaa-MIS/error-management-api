
namespace ErrorManagement.ClinicalAttachment.Application.Interfaces;

public sealed record ClinicalUserDto(
    string UserId,
    string Eid,
    string FullName,
    string Email,
    string Mobile,
    string Username,
    string Facility,
    string Speciality,
    string Status);