using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Helpers;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Security.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Groups
{
    public class CreateSecurityGroupService : ICreateSecurityGroupService
    {
        #region Private Properties

        // Repos
        private IDataRepository<LoginDb> LoginRepo { get; }
        private IDataRepository<Permission> PermissionRepo { get; }
        private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }

        // Other
        private ICacheManager CacheManager { get; }
        private IDataContext Database { get; }

        #endregion

        #region Fields

        private SecurityGroup _group;

        private readonly HashSet<int> _loginIds = new HashSet<int>();
        private readonly HashSet<int> _permissionIds = new HashSet<int>();

        #endregion

        #region Constructor

        public CreateSecurityGroupService
        (
            IDataRepository<LoginDb> loginRepo,
            IDataRepository<Permission> permissionRepo,
            IDataRepository<SecurityGroup> securityGroupRepo,
            ICacheManager cacheManager,
            IDataContext database
        )
        {
            // Repos
            LoginRepo = loginRepo;
            PermissionRepo = permissionRepo;
            SecurityGroupRepo = securityGroupRepo;

            // Other
            CacheManager = cacheManager;
            Database = database;
        }

        #endregion

        #region ICreateSecurityGroupService

        public bool HasWarnings
        {
            get
            {
                Warnings.Clear();
                return !(ValidateEntity() && ValidateDatabase());
            }
        }

        public List<SecurityGroupWarning> Warnings { get; } = new List<SecurityGroupWarning>();

        public int SecurityGroupId { get; private set; }

        public ICreateSecurityGroupService New(string name, string description)
        {
            _loginIds.Clear();
            _group = new SecurityGroup
            {
                SecurityGroupName = name,
                SecurityGroupDesc = description,
                SecurityGroupLogins = new List<SecurityGroupLogin>(),
                Permissions = new List<Permission>()
            };

            return this;
        }

        public ICreateSecurityGroupService WithUser(int loginId)
        {
            if (_group == null) return this;

            if (loginId <= 0) return this;

            if (_loginIds.Contains(loginId)) return this;

            _loginIds.Add(loginId);

            return this;
        }

        public ICreateSecurityGroupService WithUsers(IEnumerable<int> loginIds)
        {
            if (_group == null) return this;

            loginIds = loginIds?.ToList();

            if (!loginIds?.Any() ?? true) return this;

            foreach (var loginId in loginIds) WithUser(loginId);

            return this;
        }

        public ICreateSecurityGroupService WithPermission(int permissionId)
        {
            if (_group == null) return this;

            if (permissionId <= 0) return this;

            if (_permissionIds.Contains(permissionId)) return this;

            _permissionIds.Add(permissionId);

            return this;
        }

        public ICreateSecurityGroupService WithPermissions(IEnumerable<int> permissionIds)
        {
            if (_group == null) return this;

            permissionIds = permissionIds?.ToList();

            if (!permissionIds?.Any() ?? true) return this;

            foreach (var permissionId in permissionIds) WithPermission(permissionId);

            return this;
        }

        public bool Create(bool saveChanges = true)
        {
            if (HasWarnings) return Clear(false);

            AddPermissions();
            AddUsers();

            SecurityGroupRepo.Add(_group);

            if (!saveChanges) return Clear(true);

            Database.SaveChanges();

            SecurityGroupId = _group.SecurityGroupId;

            return true;
        }

        #endregion

        #region Private Methods

        #region Validation

        private bool ValidateEntity()
        {
            var results = new SecurityGroupValidator(true).Validate(_group);

            if (results.IsValid) return true;

            Warnings.AddRange(results.Errors.Select(error => new SecurityGroupWarning
            (
                error.ErrorCode.EnumTryParse<SecurityGroupWarningCode>(out var @enum) ? @enum : SecurityGroupWarningCode.PropertyNotValid,
                $"[{error.PropertyName}]: {error.ErrorMessage}"
            )));

            return false;
        }

        private bool ValidateDatabase()
        {
            if (SecurityGroupRepo.Table.Any(sg => sg.SecurityGroupName.Equals(_group.SecurityGroupName, StringComparison.CurrentCulture)))
                Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.DuplicateGroupName, _group.SecurityGroupName));

            if (_permissionIds.Any())
            {
                var permissionsNotFound = _permissionIds.Except(PermissionRepo.Table.Select(p => p.PermissionId).AsEnumerable()).ToHashSet();

                if (permissionsNotFound.Any())
                    foreach (var id in permissionsNotFound)
                        Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.PermissionNotFound, id.ToString()));
            }

            if (!_loginIds.Any()) return !Warnings.Any();

            var usersNotFound = _loginIds.Except(LoginRepo.Table.Select(l => l.LoginId).AsEnumerable()).ToHashSet();

            if (!usersNotFound.Any()) return !Warnings.Any();

            foreach (var id in usersNotFound)
                Warnings.Add(new SecurityGroupWarning(SecurityGroupWarningCode.UserNotFound, id.ToString()));

            return !Warnings.Any();
        }

        #endregion

        private void AddPermissions()
        {
            if (!_permissionIds.Any()) return;

            var permissions = PermissionRepo.Table.Where(p => _permissionIds.Contains(p.PermissionId)).ToList();

            foreach (var permission in permissions)
            {
                if (_group.Permissions.Any(p => p.PermissionId == permission.PermissionId)) continue;

                _group.Permissions.Add(permission);
            }
        }

        private void AddUsers()
        {
            if (!_loginIds.Any()) return;

            foreach (var loginId in _loginIds)
            {
                _group.SecurityGroupLogins.Add(new SecurityGroupLogin
                {
                    LoginId = loginId
                });
            }
        }

        private bool Clear(bool success)
        {
            if (success) ClearCache();

            _group = null;
            _loginIds.Clear();
            _permissionIds.Clear();

            return success;
        }

        private void ClearCache()
        {
        }

        #endregion
    }
}
