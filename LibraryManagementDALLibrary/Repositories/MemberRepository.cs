using LibraryManagementDALLibrary.Contexts;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementDALLibrary.Repositories
{
    public class MemberRepository : AbstractRepository<int, Member>
    {
        public MemberRepository()
        {
            _context = new LibraryContext();
        }

        public Member? GetMemberById(int memberId)
        {
            return _context.Members
                           .Include(m => m.Membershiptype)
                           .FirstOrDefault(m => m.Id == memberId);
        }

        public Member? GetMemberByPhone(string phone)
        {
            return _context.Members
                           .Include(m => m.Membershiptype)
                           .FirstOrDefault(m => m.Phone == phone);
        }

        public Member? GetMemberByEmail(string email)
        {
            return _context.Members
                           .Include(m => m.Membershiptype)
                           .FirstOrDefault(m => m.Email == email);
        }

        public List<Member> GetAllMembers()
        {
            return _context.Members
                           .Include(m => m.Membershiptype)
                           .ToList();
        }

        public Member UpdateMember(Member member)
        {
            var existingMember = _context.Members.Find(member.Id)
                ?? throw new KeyNotFoundException($"No member found with ID: {member.Id}");

            _context.Entry(existingMember).CurrentValues.SetValues(member);
            _context.SaveChanges();
            return existingMember;
        }

        public bool UpdateMembershipStatus(int memberId, AccountStatus accountStatus)
        {
            var member = _context.Members.Find(memberId)
                ?? throw new KeyNotFoundException($"No member found with ID: {memberId}");

            member.Accountstatus = accountStatus;
            _context.SaveChanges();
            return true;
        }

        public bool DeactivateMember(int memberId)
        {
            return UpdateMembershipStatus(memberId, AccountStatus.Deactivated);
        }
    }
}