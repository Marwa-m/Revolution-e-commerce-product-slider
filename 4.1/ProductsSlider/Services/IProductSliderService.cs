using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.ProductsSlider.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.ProductsSlider.Services
{
    
    public partial interface IProductSliderService
    {
        IPagedList<ProductSlider> SearchProductSliders(string Name = "", int storeId = 0,
    int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false,bool? overridePublished=false);

        /// Gets all products
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        IPagedList<Product> GetAllProducts(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        /// <summary>
        /// Delete productSlider
        /// </summary>
        /// <param name="productSlider">productSlider</param>
        void DeleteProductSlider(ProductSlider productSlider);

        void DeleteProductSliders(IList<ProductSlider> productSliders);


        ProductSlider GetProductSliderById(int productSliderId);

        void InsertProductSlider(ProductSlider productSlider);

        void UpdateProductSlider(ProductSlider productSlider);

        IList<ProductSlider> GetProductSlidersByIds(int[] productSliderIds);

        List<ProductSlider> GetProductSliderForShowing(int storeId);
    }

}
