using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Plugin.Widgets.ProductsSlider.Data;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider
{
    public class ProductsSliderPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin, IPermissionProvider
    {
        private static readonly string systemName = "ManageProductSlider";
        private static readonly string MenusystemName = "ProductSlider";

        public static readonly PermissionRecord ManageProductSliderPermission = new PermissionRecord { Name = "Admin area. Manage Product Slider", SystemName = systemName, Category = "Plugins" };

        private ProductSliderObjectContext _context;
        private IRepository<ProductSlider> _productSliderRepo;
        private IPermissionService _permissionService;
        private ISettingService _settings;
        private readonly ICustomerService _customerService;
        private ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        public bool HideInWidgetList => true;

        public ProductsSliderPlugin(ProductSliderObjectContext context, IRepository<ProductSlider> productSliderRepo, IPermissionService permissionService,
            ISettingService commonSettings, ICustomerService customerService, ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            _context = context;
            _productSliderRepo = productSliderRepo;
            _settings = commonSettings;
            _permissionService = permissionService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            _webHelper = webHelper;
        }
        public IList<string> GetWidgetZones()
        {
            return new List<string> { _settings.GetSettingByKey<string>("ProductSliderWidgetZone") };
        }
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsProductsSlider";
        }
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "List";
            controllerName = "ProductSlider";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.ProductsSlider.Controllers" }, { "area", null } };
        }
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/WidgetsProductSlider/List";
        }
        public override void Install()
        {
            _context.Install();
            InstallPermissions();
            #region ProductSliderController
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.Products.Fields.None", "No Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.All", "All");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.PublishedOnly", "Published only");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.UnPublishedOnly", "UnPublished only");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.AddNewProductSlider", "Added a new product slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Added", "The new slider has been added successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSlider", "Edited a slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.updated", "The slider has been updated successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.DeleteProductSlider", "Deleted a product slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Deleted", "The product slide has been deleted successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSliderSettings", "Edited a Slider settings.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.settingsupdated", "The slider settings has been updatedsuccessfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.settings", "Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliderMenu", "Product Slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliderList", "Product Slider List");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.BackToList", "Back To Product Slider List");
            #endregion

            #region ProductSliderListModel
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchProductSliderName", "Product Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchStore", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished", "Published");
            #endregion

            #region ProductSliderModel
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Name", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ShortDescription", "Short description");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.IsExternal", "Open link in new window");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Link", "Link");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Preview", "Review");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.LimitedToStore", "Limited To Stores");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Price", "Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.OldPrice", "Old Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ProductId", "Select Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Published", "Published");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableStartDateTime", "Available start date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableEndDateTime", "Available end date");

            #endregion

            #region ProductSliderValidator
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Values.Fields.Preview.Between0To5", "Preview shoud be between 0 and 5");
            #endregion
            #region Views
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.StartFrom", "STARTING FROM");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Info", "Product slider info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Prices", "Prices");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.PictureThumbnailUrl", "Picture");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Published", "Published");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders", "Product Slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.AddNew", "Add a new product slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.EditProductSliderDetails", "Edit product slider details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductInfo", "Product Info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Fields.WidgetZoneName", "Widget Zone Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Settings", "Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.CommonInfo", "General Information");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.Basic", "Basic");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.Advanced", "Advanced");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Product.Required", "Product is required");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.trialEnd", "Trial version has ended");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Name.Required", "Name is required");
            #endregion

            _settings.SetSetting<string>("ProductSliderWidgetZone", "home_page_top");
            ////For trial Version
            //if(_settings.GetSetting("ProductSliderdt")==null)
            // _settings.SetSetting<DateTime>("ProductSliderdt", DateTime.Now);
            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();

            UninstallPermissions();
            #region ProductSliderController
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.Products.Fields.None");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.All");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.PublishedOnly");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished.UnPublishedOnly");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.AddNewProductSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Added");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.updated");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.DeleteProductSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ActivityLog.EditProductSliderSettings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.settingsupdated");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliderMenu");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliderList");
            #endregion

            #region ProductSliderListModel
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchProductSliderName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchStore");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.List.SearchPublished");
            #endregion

            #region ProductSliderModel
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ShortDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.ProductId");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.IsExternal");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Link");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Preview");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.LimitedToStore");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Price");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.OldPrice");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableStartDateTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.AvailableEndDateTime");

            #endregion

            #region ProductSliderValidator
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Values.Fields.Preview.Between0To5");
            #endregion
            #region Views
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.StartFrom");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Info");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Prices");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.PictureThumbnailUrl");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Fields.Published");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.EditProductSliderDetails");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductInfo");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Fields.WidgetZoneName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSlidersSettings.Settings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.settings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.CommonInfo");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.Basic");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.Advanced");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.BackToList");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Product.Required");
           // _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.trialEnd");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.ProductsSlider.ProductSliders.Name.Required");
            #endregion

            var settingsToDelete = _settings
                     .GetSetting("ProductSliderWidgetZone");
            if (settingsToDelete !=null)
            _settings.DeleteSetting(settingsToDelete);

            base.Uninstall();
        }
        public bool Authenticate()
        {
            return true;
        }




        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Third party plugins",
                    Title = _localizationService.GetResource("Plugins"),
                    Visible = true,
                    IconClass = "fa-list"
                };

            }
            var ProductsSliderPluginNode = pluginNode.ChildNodes.FirstOrDefault(x => x.SystemName == MenusystemName);
            if (ProductsSliderPluginNode == null)
            {
                ProductsSliderPluginNode = new SiteMapNode()
                {
                    SystemName = MenusystemName,
                    Title = _localizationService.GetResource("Plugins.Widgets.ProductsSlider.ProductSliderMenu"),
                    Visible = true,
                    IconClass = "fa-gear",
                };

               
                ProductsSliderPluginNode.ChildNodes.Add(new SiteMapNode()

                {

                    Title = _localizationService.GetResource("List"),

                    Visible = true,

                    IconClass = "fa-list",

                    Url = "~/Admin/WidgetsProductSlider/List",
                    SystemName = MenusystemName+"List"

                });
                ProductsSliderPluginNode.ChildNodes.Add(new SiteMapNode()

                {
                    
                    Title = _localizationService.GetResource("Settings"),

                    Visible = true,

                    IconClass = "fa-dot-circle-o",

                    Url = "~/Admin/WidgetsProductSlider/Settings",
                    SystemName = "ProductsSliderSettings"

                });
                pluginNode.ChildNodes.Add(ProductsSliderPluginNode);
            }

        }



        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageProductSliderPermission
            };
        }

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = NopCustomerDefaults.AdministratorsRoleName,
                    PermissionRecords = new[]
                    {
                        ManageProductSliderPermission
                    }
                }
            };
        }
        public virtual void InstallPermissions()
        {
            _permissionService.InsertPermissionRecord(ManageProductSliderPermission);
            var permission1 = ManageProductSliderPermission;



            //default customer role mappings
            var defaultPermissions = GetDefaultPermissions();
            foreach (var defaultPermission in defaultPermissions)
            {
                var customerRole = _customerService.GetCustomerRoleBySystemName(defaultPermission.CustomerRoleSystemName);
                if (customerRole == null)
                {
                    //new role (save it)
                    customerRole = new CustomerRole
                    {
                        Name = defaultPermission.CustomerRoleSystemName,
                        Active = true,
                        SystemName = defaultPermission.CustomerRoleSystemName
                    };
                    _customerService.InsertCustomerRole(customerRole);
                }


                var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                              where p.SystemName == permission1.SystemName
                                              select p).Any();
                var mappingExists = (from mapping in customerRole.PermissionRecordCustomerRoleMappings
                                     where mapping.PermissionRecord.SystemName == permission1.SystemName
                                     select mapping).Any();
                if (defaultMappingProvided && !mappingExists)
                {
                    permission1.PermissionRecordCustomerRoleMappings.Add(new PermissionRecordCustomerRoleMapping { CustomerRole = customerRole });
                }
            }

        }
        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions()
        {
            var pm = _permissionService.GetPermissionRecordBySystemName(systemName);
            if (pm != null)
            {
                _permissionService.DeletePermissionRecord(pm);
            }

        }

        
    }
}
