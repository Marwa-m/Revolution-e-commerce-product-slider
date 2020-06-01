using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Services
{
    public partial class RevolutionSliderPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageRevolutionSliderPermission = new PermissionRecord { Name = "Admin area. Manage Revolution Slider", SystemName = "ManageProductSlider", Category = "Plugins" };

        HashSet<(string systemRoleName, PermissionRecord[] permissions)> IPermissionProvider.GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        ManageRevolutionSliderPermission
                    }
                ),
            };
        }


        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageRevolutionSliderPermission
            };
        }

    }
}
