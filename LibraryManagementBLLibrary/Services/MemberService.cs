using LibraryManagementBLLibrary.Interfaces;
using LibraryManagementDALLibrary.Repositories;
using LibraryManagementModelLibrary.Enums;
using LibraryManagementModelLibrary.Exceptions;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementBLLibrary.Services
{
    public class MemberService : IMemberService
    {
        private readonly MemberRepository _memberRepository;

        public MemberService()
        {
            _memberRepository = new MemberRepository();
        }

        public Member AddMember(Member member)
        {
            try
            {
                if (member == null)
                {
                    throw new LibraryValidationException("Member details are required.");
                }

                if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.Phone) || string.IsNullOrWhiteSpace(member.Email))
                {
                    throw new LibraryValidationException("Member name, phone, and email are required.");
                }

                return _memberRepository.Create(member)!;
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("AddMember", ex);
            }
        }

        public List<Member> GetAllMembers()
        {
            try
            {
                return _memberRepository.GetAllMembers();
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetAllMembers", ex);
            }
        }

        public Member? GetMemberById(int memberId)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _memberRepository.GetMemberById(memberId);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMemberById", ex);
            }
        }

        public Member? GetMemberByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    throw new LibraryValidationException("Phone is required.");
                }

                return _memberRepository.GetMemberByPhone(phone);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMemberByPhone", ex);
            }
        }

        public Member? GetMemberByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new LibraryValidationException("Email is required.");
                }

                return _memberRepository.GetMemberByEmail(email);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("GetMemberByEmail", ex);
            }
        }

        public bool UpdateMembershipStatus(int memberId, AccountStatus accountStatus)
        {
            try
            {
                if (memberId <= 0)
                {
                    throw new LibraryValidationException("Member id must be greater than zero.");
                }

                return _memberRepository.UpdateMembershipStatus(memberId, accountStatus);
            }
            catch (LibraryManagementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceOperationException("UpdateMembershipStatus", ex);
            }
        }

        public bool DeactivateMember(int memberId)
        {
            return UpdateMembershipStatus(memberId, AccountStatus.Deactivated);
        }
    }
}