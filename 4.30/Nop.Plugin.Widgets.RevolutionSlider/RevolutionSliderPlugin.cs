using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Data;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Services;
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

namespace Nop.Plugin.Widgets.RevolutionSlider
{
    public class RevolutionSliderPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        private static readonly string systemName = "ManageRevSlider";
        private static readonly string MenusystemName = "RevSlider";


        private IPermissionService _permissionService;
        private ISettingService _settings;
        private readonly ICustomerService _customerService;
        private ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private IRepository<RevSliderWidgetZone> _widgetZonesRepo;

        public bool HideInWidgetList => false;
        #region Ctor
        public RevolutionSliderPlugin( IPermissionService permissionService,
            ISettingService commonSettings, ICustomerService customerService, ILocalizationService localizationService,
            IWebHelper webHelper, IRepository<RevSliderWidgetZone> widgetZonesRepo)
        {
            _settings = commonSettings;
            _permissionService = permissionService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            _webHelper = webHelper;
            _widgetZonesRepo = widgetZonesRepo;
        }
        #endregion

        public IList<string> GetWidgetZones()
        {
            var sliders = _widgetZonesRepo.Table.Select(x => x.WidgetZone).ToArray<string>();

            return sliders.ToList<string>();
        }
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsRevolutionSlider";
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
            controllerName = "RevSlider";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.RevolutionSlider.Controllers" }, { "area", null } };
        }
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/RevSlider/List";
        }
        public override void Install()
        {
          
            InstallPermissions();
            #region RevSliderController
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Products.Fields.None", "No Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.All", "All");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.PublishedOnly", "Published only");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.UnPublishedOnly", "UnPublished only");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.AddNewRevSlider", "Added a new revolution slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Added", "The new slider has been added successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.EditRevSlider", "Edited a revolution slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.updated", "The revolution slider has been updated successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.DeleteRevSlider", "Deleted a revolution slider ('{0}')");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Deleted", "The Revolution slide has been deleted successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliderMenu", "Revolution Slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliderList", "Revolution Slider List");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.BackToList", "Back To Revolution Slider List");
            #endregion

            #region RevSliderListModel
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchRevSliderName", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchStore", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished", "Published");
            #endregion

            #region RevSliderModel
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name", "Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShortDescription", "Short description");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.IsExternal", "Open link in new window");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Link", "Link");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Link.hint", "Leave it empty to go to default product URL");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Preview", "Review");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.LimitedToStore", "Limited To Stores");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Price", "Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.OldPrice", "Old Price");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ProductId", "Select Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Published", "Published");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AvailableStartDateTime", "Available start date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AvailableEndDateTime", "Available end date");

            #endregion

            #region RevSliderValidator
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Values.Fields.Preview.Between0To5", "Review shoud be between 0 and 5");
            #endregion
            #region Views
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Info", "Revolution slider info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Products", "Products");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.UrlRecords", "SEOs");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Scheduling", "Scheduling");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.CustomStyle", "Custom Style");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.UrlRecord.AddNew","Add New SEO");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSliders.RevSlider.AddNew", "Add New Revolution Slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Product.AddNew", "Add New Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Settings", "Settings");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.StartFrom", "STARTING FROM");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Prices", "Prices");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.PictureThumbnailUrl", "Picture");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Published", "Published");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliders", "Revolution Slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.AddNew", "Add a new revolution slider");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.EditRevSliderDetails", "Edit revolution slider details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ProductInfo", "Product Info");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.CommonInfo", "General Information");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Basic", "Basic");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Advanced", "Advanced");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Product.Required", "Product is required");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.trialEnd", "Trial version has ended");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.trialWillEnd", "The trial version will end after {0} day(s).");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Name.Required", "Name is required");
            #endregion
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShowName", "Show name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AclCustomerRole", "Limited to customer roles");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.WidgetZones", "Widget Zones");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.SaveBeforeEdit", "You need to save the revolution slider before.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.AddNew", "Add new widget zone");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.WidgetZone", "Widget Zone");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.DisplayOrder", "Display order");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromDate", "From Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToDate", "To Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromTime", "From Time");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToTime", "To Time");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.SchedulePattern", "Schedule Pattern");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.SchedulePattern", "Schedule Pattern");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Schedule", "Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.CustomStyle", "Custom Style");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.HideOnMobile", "Hide on mobile");

            ////For trial Version
            //if (_settings.GetSetting("RevSliderdt4.3") == null)
            //    _settings.SetSetting<DateTime>("RevSliderdt4.3", DateTime.Now);
            base.Install();
        }

        public override void Uninstall()
        {

            UninstallPermissions();
            #region RevSliderController
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Products.Fields.None");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.All");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.PublishedOnly");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished.UnPublishedOnly");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.AddNewRevSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Added");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.EditRevSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.updated");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ActivityLog.DeleteRevSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliderMenu");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliderList");
            #endregion

            #region RevSliderListModel
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchRevSliderName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchStore");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.List.SearchPublished");
            #endregion

            #region RevSliderModel
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Name");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShortDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ProductId");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.IsExternal");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Link");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Link.hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Preview");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.LimitedToStore");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Price");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.OldPrice");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AvailableStartDateTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AvailableEndDateTime");

            #endregion

            #region RevSliderValidator
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Values.Fields.Preview.Between0To5");
            #endregion
            #region Views
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.StartFrom");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Info");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Products");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.UrlRecords");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Scheduling");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.CustomStyle");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.UrlRecord.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSliders.RevSlider.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Product.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Settings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Prices");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.PictureThumbnailUrl");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.Published");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSliders");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.EditRevSliderDetails");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.ProductInfo");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.CommonInfo");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Basic");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.Advanced");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.BackToList");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Product.Required");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.trialEnd");
            //_localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.trialWillEnd");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Name.Required");
            #endregion

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ShowName");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.AclCustomerRole");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.WidgetZones");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.SaveBeforeEdit");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.WidgetZone");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.WidgetZone.Fields.DisplayOrder");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.SchedulePattern");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Schedule");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.FromTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.ToTime");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.SchedulePattern");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.CustomStyle");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.RevolutionSlider.RevSlider.Fields.HideOnMobile");

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
            var RevSliderPluginNode = pluginNode.ChildNodes.FirstOrDefault(x => x.SystemName == MenusystemName);
            if (RevSliderPluginNode == null)
            {
                RevSliderPluginNode = new SiteMapNode()
                {
                    SystemName = MenusystemName,
                    Title = _localizationService.GetResource("Plugins.Widgets.RevolutionSlider.RevSliderMenu"),
                    Visible = true,
                    IconClass = "fa-gear",
                    Url = "~/Admin/RevSlider/List",
                };

               
                pluginNode.ChildNodes.Add(RevSliderPluginNode);
            }

        }




        public virtual void InstallPermissions()
        {
            //_permissionService.InsertPermissionRecord(RevolutionSliderPermissionProvider.ManageRevSliderPermission);
            //var permission1 = ManageRevSliderPermission;

            var permissionProviders = new List<Type> { typeof(RevolutionSliderPermissionProvider) };
            foreach (var providerType in permissionProviders)
            {
                var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
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
