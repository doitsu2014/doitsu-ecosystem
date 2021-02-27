using System;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using System.Collections.Generic;
using LanguageExt;
using System.Linq;
using System.Text.RegularExpressions;
using Shared.Abstraction.Models.Types;
using Shared.Extensions;
using static Shared.Validations.GenericValidator;

namespace FileConversion.Core.BeanMappers
{
    public class PackagingProductDocumentMapper : IBeanMapper
    {
        public Either<Error, IEnumerable<object>> Map(IEnumerable<object> data)
        {
            return ShouldNotNull(data)
                .ToEither()
                .MapLeft(errors => errors.Join()) 
                .Map(d => d.Cast<PackagingDocument>()
                    .Select(pd =>
                    {
                        var listProductInformations =
                            Regex.Split(pd.RawProductInformation, @"\[[0-9]+\]").Select(s => s.Trim());
                        var listProducts = listProductInformations.Select(pi =>
                            {
                                var product = new PackagingProductDocument();
                                var productProperties = pi.Split(";", StringSplitOptions.TrimEntries).ToList();
                                foreach (var productProperty in productProperties)
                                {
                                    var kv = productProperty.Split(":", StringSplitOptions.TrimEntries);
                                    try
                                    {
                                        if (kv[0].Equals("tên sản phẩm", StringComparison.CurrentCultureIgnoreCase))
                                            product.Name = kv[1];
                                        if (kv[0].Equals("giá", StringComparison.CurrentCultureIgnoreCase))
                                            product.Price = kv[1];
                                        if (kv[0].Equals("số lượng", StringComparison.CurrentCultureIgnoreCase))
                                            product.Quantity = int.Parse(kv[1]);
                                        if (kv[0].Equals("sku phân loại hàng", StringComparison.CurrentCultureIgnoreCase))
                                            product.CategorySku = kv[1];
                                        if (kv[0].Equals("sku sản phẩm", StringComparison.CurrentCultureIgnoreCase))
                                            product.Sku = kv[1];
                                    }
                                    catch
                                    {
                                        // by pass                                    
                                    }
                                }

                                return product.Name.IsNullOrEmpty() ? null : product;
                            })
                            .Where(p => p != null)
                            .ToList();

                        pd.Products = new List<PackagingProductDocument>();
                        pd.Products.AddRange(listProducts);

                        return pd;
                    })
                    .Cast<object>());
        }
    }
}