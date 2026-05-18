using System.Collections.Generic;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Interfaces;

public interface IMemberService
{
	Member AddMember(Member member);
	List<Member> GetAllMembers();
	Member? GetMemberById(int memberId);
	Member? GetMemberByPhone(string phone);
	Member? GetMemberByEmail(string email);
	bool UpdateMembershipStatus(int memberId, AccountStatus accountStatus);
	bool DeactivateMember(int memberId);
}
