using Api.Contexts;
using Api.Eventing;
using Api.Pages;
using Api.Startup;
using Api.Startup.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Api.Pages.PageController;

namespace Api.Payments
{
    /// <summary>
    /// A base class for creating product related category trees
    /// </summary>
    /// <typeparam name="TCategory">The type of the category (e.g., ProductCategory).</typeparam>
    /// <typeparam name="TCategoryNode">The type of the category node (e.g., ProductCategoryNode).</typeparam>

    public abstract class BaseCategoryService<TCategory, TCategoryNode> : AutoService<TCategory>
            where TCategory : BaseCategory, new()
            where TCategoryNode : CategoryNode<TCategory, TCategoryNode>, new()
    {

        /// <summary>
        /// The label to use for the current category
        /// </summary>
        public abstract string CategoryLabel { get; }

        /// <summary>
        /// The include mapping to use for the current category
        /// </summary>
        public abstract string CategoryFieldName { get; }

        /// <summary>
        /// The url prefix to use for the current category
        /// </summary>
        public abstract string CategoryUrlPrefix { get; }

        protected readonly ProductService _productService;
        protected readonly PageService _pages;
        protected readonly PermalinkService _permalinks;
        protected CategoryTree<TCategoryNode>? _categoryTree;

        private bool _isPermalinkSyncRunning = false;

        /// <summary>
        ///  base class for creating product related category trees
        /// </summary>
        /// <param name="eventGroup"></param>
        /// <param name="productService"></param>
        /// <param name="pages"></param>
        /// <param name="permalinks"></param>
        protected BaseCategoryService(EventGroup<TCategory> eventGroup, ProductService productService, PageService pages, PermalinkService permalinks)
            : base(eventGroup)
        {
            _productService = productService;
            _pages = pages;
            _permalinks = permalinks;
            Cache();

            Events.Service.AfterStart.AddEventListener(async (ctx, sender) =>
            {
                await GetTree(ctx);
                return sender;
            });

            HashSet<string> excludeFields = new HashSet<string>() { "Categories", "Tags" };

            eventGroup.BeforeSettable.AddEventListener((Context ctx, JsonField<TCategory, uint> field) =>
            {
                if (field == null)
                {
                    return new ValueTask<JsonField<TCategory, uint>>(field);
                }

                // hide the core taxonomy fields as we have product specific ones
                if (excludeFields.Contains(field.Name))
                {
                    field.Writeable = false;
                    field.Hide = true;
                }

                if (field.Name == "ParentId")
                {
                    field.Module = "Admin/ContentSelect";
                    field.Data["contentType"] = typeof(TCategory).Name;
                    field.Data["label"] = $"Parent {CategoryLabel}";
                }

                return new ValueTask<JsonField<TCategory, uint>>(field);
            });


            eventGroup.BeforeCreate.AddEventListener(async (Context ctx, TCategory category) =>
            {
                if (category == null)
                {
                    return null;
                }

                // Ensure a slug is generated and is unique.
                if (string.IsNullOrEmpty(category.Slug))
                {
                    category.Slug = await SlugGenerator.GenerateUniqueSlug(this, ctx, category.Name.Get(ctx));
                }
                return category;
            });

            eventGroup.BeforeUpdate.AddEventListener(async (Context ctx, TCategory updated, TCategory original) =>
            {
                if (await HasCircularReference(ctx, updated.Id, updated.ParentId))
                {
                    return null;
                }
                return updated;
            });

            eventGroup.AfterCreate.AddEventListener(async (Context context, TCategory category) =>
            {

                // Permalink target which will be for whichever page wants to handle a  category as its primary content.
                // If a specific page for this category exists, it will ultimately pick that.
                var linkTarget = permalinks.CreatePrimaryTargetLocator(this, category);

                await permalinks.Create(
                    context,
                    new Permalink()
                    {
                        Url = GetInitialCategoryUrl(category),
                        Target = linkTarget
                    },
                    DataOptions.IgnorePermissions
                );

                // clear the cache
                _categoryTree = null;

                return category;
            });

            eventGroup.AfterUpdate.AddEventListener((Context ctx, TCategory category) =>
            {
                // clear the cache
                _categoryTree = null;

                return new ValueTask<TCategory>(category);
            });

            eventGroup.AfterDelete.AddEventListener(async (Context ctx, TCategory category) =>
            {
                var children = await Where("ParentId=?").Bind(category.Id).ListAll(ctx);
                foreach (var child in children)
                {
                    await Update(ctx, child, (c, toUpdate, orig) =>
                    {
                        toUpdate.ParentId = category.ParentId;
                    });
                }

                // clear the cache
                _categoryTree = null;

                return category;
            });
        }

        /// <summary>
        /// Get the cached lookup table (and rebuild if necessary)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<Dictionary<uint, TCategoryNode>> GetLookup(Context ctx)
        {
            var newTree = await GetTree(ctx);
            return newTree.IdLookup;
        }

        /// <summary>
        /// A convenience brute-force method for ensuring that all required permalinks exist.
        /// Best used after a major database edit (such as importing outside of SS).
        /// </summary>
        /// <returns></returns>
        public async ValueTask SyncPermalinks(Context context)
        {
            // This method is about to be exposed to an endpoint, in order to stop this from 
            // firing multiple times, let's add a blocker.
            if (_isPermalinkSyncRunning)
            {
                return;
            }

            _isPermalinkSyncRunning = true;

            Log.Warn(CategoryFieldName, $"Sync {typeof(TCategory).Name} permalinks");

            try
            {
                var all = await Where("", DataOptions.IgnorePermissions).ListAll(context);
                var links = new List<PermalinkUrlTarget>();

                foreach (var category in all)
                {
                    // Permalink target which will be for whichever page wants to handle a product category as its primary content.
                    // If a specific page for this category exists, it will ultimately pick that.
                    var linkTarget = _permalinks.CreatePrimaryTargetLocator(this, category);

                    var permalinkInfo = new PermalinkUrlTarget()
                    {
                        Url = GetInitialCategoryUrl(category),
                        Target = linkTarget
                    };

                    links.Add(permalinkInfo);
                }

                await _permalinks.BulkCreate(context, links);
            }
            finally
            {
                _isPermalinkSyncRunning = false;
            }
        }


        /// <summary>
        /// Use the primaryUrl system instead of calling this directly.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private string GetInitialCategoryUrl(TCategory category)
        {
            return $"/{CategoryUrlPrefix}/{category.Slug}";
        }

        public bool IsSyncRunning
        {
            get
            {
                return _isPermalinkSyncRunning;
            }
        }



        /// <summary>
        /// Gets the products for a given product category. The result is null if there are none.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<Product>> GetProducts(Context ctx, uint id)
        {
            var category = await Get(ctx, id);

            if (category == null)
            {
                return null;
            }

            return await _productService.Where($"{CategoryFieldName} contains ?").Bind(id).ListAll(ctx);
        }


        /// <summary>
        /// Gets the categories for a given product. The result is null if there are none.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<TCategory>> GetProductCategories(Context ctx, uint id)
        {
            var product = await _productService.Get(ctx, id);

            if (product == null)
            {
                return null;
            }

            return await ListBySource(ctx, product, CategoryFieldName);
        }


        /// <summary>
        /// Gets the product categories for a given product. The result is null if there are none.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<TCategoryNode>> GetChildren(Context ctx, uint id)
        {
            var category = await Get(ctx, id);

            if (category == null)
            {
                return null;
            }

            if (await GetLookup(ctx) != null && (await GetLookup(ctx)).TryGetValue(id, out var foundCategory))
            {
                return foundCategory.Children?.Count > 0 ? foundCategory.Children : null;
            }

            return null;
        }

        /// <summary>
        /// Gets all descendants of a category as a flat list.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="parentNodeId"></param>
        /// <returns></returns>
        public async ValueTask<List<TCategory>> GetChildrenAsFlatList(Context ctx, uint parentNodeId)
        {
            var tree = await GetTree(ctx);
            var flat = new List<TCategory>();

            if (!tree.IdLookup.TryGetValue(parentNodeId, out var root))
            {
                return flat;
            }

            void Traverse(TCategoryNode n)
            {
                foreach (var c in n.Children)
                {
                    flat.Add(c.Category);
                    Traverse(c);
                }
            }

            Traverse(root);
            return flat;
        }

        /// <summary>
        /// Gets a tree node for the admin panel at a given category slug path.
        /// As category slugs are globally unique, only actually the last one is used 
        /// (unless it is blank, in which case root categories are returned).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async ValueTask<TreeNodeDetail?> GetTreeNodeAtPath(Context context, string path)
        {
            var tree = await GetTree(context);

            if (string.IsNullOrEmpty(path))
            {
                // Root of the tree.
                var roots = tree.Roots;

                var rootSet = new List<RouterNodeMetadata>();

                if (roots != null)
                {
                    foreach (var root in roots)
                    {
                        rootSet.Add(ConvertNode(context, root, root.Category.Slug));
                    }
                }

                return new TreeNodeDetail()
                {
                    Self = null,
                    Children = rootSet
                };
            }

            if (path.EndsWith('/'))
            {
                path = path.Substring(0, path.Length - 1);
            }

            var lastSlash = path.LastIndexOf('/');

            string catSlug;

            if (lastSlash == -1)
            {
                catSlug = path;
            }
            else
            {
                catSlug = path.Substring(lastSlash + 1);
            }

            var lookup = tree.SlugLookup;

            if (!lookup.TryGetValue(catSlug, out TCategoryNode node))
            {
                // Not found.
                return null;
            }

            var kids = node.Children;

            var childSet = new List<RouterNodeMetadata>();

            if (kids != null)
            {
                foreach (var child in kids)
                {
                    childSet.Add(ConvertNode(context, child, path + "/" + child.Category.Slug));
                }
            }

            var products = node.Products;

            if (products != null)
            {
                foreach (var child in products)
                {
                    childSet.Add(ConvertNode(context, child));
                }
            }

            return new TreeNodeDetail()
            {
                Self = ConvertNode(context, node, path),
                Children = childSet
            };
        }


        /// <summary>
        /// Builds an admin tree view compatible struct of metadata for the given category node.
        /// </summary>
        /// <returns></returns>
        protected virtual RouterNodeMetadata ConvertNode(Context context, TCategoryNode node, string fullRoute)
        {
            return new RouterNodeMetadata
            {
                Type = typeof(TCategory).Name,
                EditUrl = $"/en-admin/{typeof(TCategory).Name.ToLowerInvariant()}/{node.Category.Id}",
                ContentId = node.Category.Id,
                Name = node.Category.Name.Get(context),
                FullRoute = fullRoute,
                ChildKey = node.Category.Slug,
                HasChildren = node.Children != null && node.Children.Count > 0
            };
        }

        /// <summary>
        /// Builds an admin tree view compatible struct of metadata for the given product node.
        /// </summary>
        /// <returns></returns>
        private RouterNodeMetadata ConvertNode(Context context, ProductNode node)
        {
            return new RouterNodeMetadata()
            {
                Type = "Product",
                EditUrl = "/en-admin/product/" + node.Product.Id,
                ContentId = node.Product.Id,
                Name = node.Product.Name.Get(context),
                FullRoute = node.Product.Slug,
                ChildKey = node.Product.Slug,
                HasChildren = false
            };
        }

        /// <summary>
        /// Get any parents for a category node
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TCategoryNode>> GetParents(Context ctx, uint id)
        {
            var productCategory = await Get(ctx, id);

            if (productCategory == null)
            {
                return null;
            }

            var lookup = await GetLookup(ctx);

            if (lookup != null && lookup.TryGetValue(id, out var foundCategory))
            {
                var parents = new List<TCategoryNode>();

                var current = foundCategory;
                while (current.Category.ParentId.HasValue)
                {
                    if (lookup.TryGetValue(current.Category.ParentId.Value, out var parent))
                    {
                        parents.Add(parent);
                        current = parent;
                    }
                    else
                    {
                        // Break if ParentId is set but not found in lookup (e.g., data issue)
                        break;
                    }
                }

                return parents;
            }

            return null;
        }



        /// <summary>
        /// Get the entire category tree (hopefully from cache)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<CategoryTree<TCategoryNode>> GetTree(Context ctx)
        {
            var tree = _categoryTree;

            if (tree != null)
            {
                return tree.Value;
            }

            //todo - remove products from tree

            // Build a new one:
            var newTree = await BuildCategoryTree(ctx, false);

            // Cache it:
            _categoryTree = newTree;

            return newTree;
        }


        /// <summary>
        /// Get the entire category tree with products (debug use only)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<CategoryTree<TCategoryNode>> GetProductTreeAndProducts(Context ctx)
        {
            // no caching so slooooww
            return await BuildCategoryTree(ctx, true);
        }

        /// <summary>
        /// Build the flat database list into a nested category structure
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="includeProducts"></param>
        /// <returns></returns>
        /// 
        protected virtual async Task<CategoryTree<TCategoryNode>> BuildCategoryTree(Context ctx, bool includeProducts = false)
        {
            Log.Info(CategoryFieldName, $"Building {typeof(TCategory).Name} structure");

            // get all categories 
            var categories = await Where("", DataOptions.IgnorePermissions).ListAll(ctx);

            // create lookup dictionaries
            var lookup = new Dictionary<uint, TCategoryNode>();
            var lookupBySlug = new Dictionary<string, TCategoryNode>();
            var roots = new List<TCategoryNode>();

            foreach (var category in categories)
            {
                var node = new TCategoryNode()
                {
                    Category = category
                };

                lookup[category.Id] = node;

                if (string.IsNullOrEmpty(category.Slug))
                {
                    continue;
                }
                lookupBySlug[category.Slug] = node;
            }

            foreach (var cat in categories)
            {
                var node = lookup[cat.Id];

                if (cat.ParentId.HasValue && lookup.TryGetValue(cat.ParentId.Value, out var parent))
                {
                    node.Parent = parent;
                    parent.Children.Add(node);
                }
                else
                {
                    roots.Add(node);
                }
            }

            // calculate all the node full paths
            foreach (var category in roots)
            {
                FinalizeCategoryNode(category);
            }

            if (includeProducts)
            {
                var products = await _productService.Where("", DataOptions.IgnorePermissions).ListAll(ctx);

                // Attach products to categories (products can be linked to multiple category nodes)
                foreach (var product in products)
                {

                    var listOfCategories = await ListBySource(ctx, product, CategoryFieldName, DataOptions.IgnorePermissions);
                    if (listOfCategories.Any())
                    {
                        foreach (var category in listOfCategories)
                        {
                            if (lookup.TryGetValue(category.Id, out var categoryNode))
                            {
                                var nodeProduct = new ProductNode
                                {
                                    Product = product,
                                    Slug = BuildNodeSlugPath(categoryNode, product)
                                };

                                categoryNode.Products.Add(nodeProduct);
                            }
                        }
                    }
                }
            }

            Log.Info(CategoryFieldName, $"Completed {typeof(TCategory).Name} structure");

            return new CategoryTree<TCategoryNode>()
            {
                IdLookup = lookup,
                SlugLookup = lookupBySlug,
                Roots = roots
            };
        }

        /// <summary>
        /// Extract the full path of a category node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="product"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        protected virtual string BuildNodeSlugPath(TCategoryNode node, Product product, string separator = "/")
        {
            var slugs = new Stack<string>();

            if (product != null)
            {
                if (!string.IsNullOrWhiteSpace(product.Slug))
                {
                    slugs.Push(product.Slug);
                }
                else
                {
                    var name = product.Name.GetFallback();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        slugs.Push(SlugGenerator.CreateSlug(name));
                    }
                }
            }

            while (node != null)
            {
                if (!string.IsNullOrWhiteSpace(node.Category.Slug))
                {
                    slugs.Push(node.Category.Slug);
                }
                else
                {
                    var cat = node.Category.Name.GetFallback();

                    if (string.IsNullOrWhiteSpace(cat))
                    {
                        slugs.Push(SlugGenerator.CreateSlug(cat));
                    }
                }
                node = node.Parent!;
            }

            return string.Join(separator, slugs.Select(SlugGenerator.CreateSlug));
        }

        protected virtual void FinalizeCategoryNode(TCategoryNode node)
        {
            if (node == null || node.Category == null)
            {
                return;
            }

            // Handle this node first
            if (node.Parent == null)
            {
                node.BreadcrumbCategories = new List<TCategory>() {
                    node.Category
                };
            }
            else
            {
                node.BreadcrumbCategories = new List<TCategory>(node.Parent.BreadcrumbCategories);

                // Add itself:
                node.BreadcrumbCategories.Add(node.Category);
            }

            // And handle any of its children next:
            if (node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    FinalizeCategoryNode(child);
                }
            }
        }


        /// <summary>
        /// Check for rescursive entries
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <param name="newParentId"></param>
        /// <returns></returns>
        public async Task<bool> HasCircularReference(Context ctx, uint id, uint? newParentId)
        {
            var currentParentId = newParentId;

            while (currentParentId.HasValue)
            {
                if (currentParentId.Value == id)
                {
                    // Found circular reference
                    return true;
                }

                // to be sure use the database version 
                var parentCategory = await Get(ctx, currentParentId.Value, DataOptions.NoCacheIgnorePermissions);
                if (parentCategory == null)
                {
                    // Parent not found
                    return true;
                }

                currentParentId = parentCategory.ParentId;
            }

            return false;
        }
    }
}
