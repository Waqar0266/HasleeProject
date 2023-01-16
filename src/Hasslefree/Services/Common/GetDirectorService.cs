using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Linq;

namespace Hasslefree.Services.Common
{
    public class GetDirectorService : IGetDirectorService, IInstancePerRequest
    {
        //Repos
        private IReadOnlyRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }
        private IReadOnlyRepository<SecurityGroup> SecurityGroupRepo { get; }
        private IReadOnlyRepository<Login> LoginRepo { get; }
        private IReadOnlyRepository<Person> PersonRepo { get; }

        public GetDirectorService
        (
            //Repos
            IReadOnlyRepository<SecurityGroupLogin> securityGroupLoginRepo,
            IReadOnlyRepository<SecurityGroup> securityGroupRepo,
            IReadOnlyRepository<Login> loginRepo,
            IReadOnlyRepository<Person> personRepo
        )
        {
            //Repos
            SecurityGroupLoginRepo = securityGroupLoginRepo;
            SecurityGroupRepo = securityGroupRepo;
            LoginRepo = loginRepo;
            PersonRepo = personRepo;
        }

        public Person Get()
        {
            return (from person in PersonRepo.Table
                    join login in LoginRepo.Table on person.PersonId equals login.PersonId
                    join securityGroupLogin in SecurityGroupLoginRepo.Table on login.LoginId equals securityGroupLogin.LoginId
                    join securityGroup in SecurityGroupRepo.Table on securityGroupLogin.SecurityGroupId equals securityGroup.SecurityGroupId
                    where securityGroup.SecurityGroupName == "Director"
                    select person).FirstOrDefault();
        }
    }
}
