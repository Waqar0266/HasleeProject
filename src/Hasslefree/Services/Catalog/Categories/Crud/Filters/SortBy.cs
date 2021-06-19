namespace Hasslefree.Services.Catalog.Categories.Crud.Filters
{
	public enum SortBy
	{
		None,
		Created,
		CreatedDesc,
		Name,
		NameDesc,
		Path, // sort by NestedLevel & Display Order
		PathDesc, // sort by NestedLevel & Display Order
		TotalProducts,
		TotalProductsDesc
	}
}
