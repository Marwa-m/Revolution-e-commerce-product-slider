using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Plugin.Widgets.RevolutionSlider.Domain;
using Nop.Plugin.Widgets.RevolutionSlider.Models;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Services
{
    public partial interface IRevSliderService
    {
        #region RevSliderWidgetZone
        /// <summary>
        /// Delete RevSliderWidgetZone
        /// </summary>
        /// <param name="RevSliderWidgetZone">RevSliderWidgetZone</param>
        void DeleteRevSliderWidgetZone(RevSliderWidgetZone RevSliderWidgetZone);



        RevSliderWidgetZone GetRevSliderWidgetZoneById(int RevSliderWidgetZoneId);

        List<string> GetRevSliderWidgetZones();

        void InsertRevSliderWidgetZone(RevSliderWidgetZone RevSliderWidgetZone);

        void UpdateRevSliderWidgetZone(RevSliderWidgetZone RevSliderWidgetZone);

        IPagedList<RevSliderWidgetZone> GetRevSliderWidgetZonesByRevSliderId(int revSliderId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        #endregion

        #region RevSliderProduct
        IPagedList<Product> GetAllProducts(string name = "",
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        IPagedList<RevSliderProduct> GetRevSliderProductsByRevSliderId(int revSliderId,
    int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        RevSliderProduct GetRevSliderProductById(int revSliderProductId);
        void UpdateRevSliderProduct(RevSliderProduct revSliderProduct);
        void DeleteRevSliderProduct(RevSliderProduct revSliderProduct);
        void InsertRevSliderProduct(RevSliderProduct revSliderProduct);

        List<ProductDetailsModel> GetProductDetailsByRevSliderId(int revSliderId);
        #endregion

        #region RevSlider
        RevSlider GetRevSliderById(int revSliderId);

        IPagedList<RevSlider> GetAllRevSliders(string revSliderName = "",
           int storeId = 0,
           int pageIndex = 0,
           int pageSize = int.MaxValue,
           bool showHidden = false,
           bool? overridePublished = null);
        IList<RevSlider> GetRevSlidersByWidgetZoneName(string WidgetName = "");

        void InsertRevSlider(RevSlider RevSlider);

        void UpdateRevSlider(RevSlider RevSlider);
        void DeleteRevSlider(RevSlider revSlider);
        #endregion

        #region RevSliderUrlRecord
        /// <summary>
        /// Delete RevSliderWidgetZone
        /// </summary>
        /// <param name="RevSliderWidgetZone">RevSliderWidgetZone</param>
        void DeleteRevSliderUrlRecord(RevSliderUrlRecord RevSliderUrlRecord);
        RevSliderUrlRecord GetRevSliderUrlRecordById(int RevSliderUrlRecordId);
        List<UrlRecord> GetRevSliderUrlRecords();

        void InsertRevSliderUrlRecord(RevSliderUrlRecord RevSliderUrlRecord);

        void UpdateRevSliderUrlRecord(RevSliderUrlRecord RevSliderUrlRecord);

        IPagedList<RevSliderModel.RevSliderUrlRecordModel> GetRevSliderUrlRecordsByRevSliderId(int revSliderId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        #endregion
        IList<RevSliderProductPresentaionModel> PrepareRevSliderProductPresentaionModel(int revSliderId);

    }
}
