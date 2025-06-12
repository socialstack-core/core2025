using Api.Contexts;
using Api.Database;
using Api.Pages;
using Api.Startup;
using Api.Translate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Permissions;


[Collection("Global Setup Collection")]
public class FilterTests
{
	private Page PageOne()
	{
		return new Page { 
			Id = 1, 
			CreatedUtc = DateTime.UtcNow, 
			Title=new Localized<string>("Hello world"),
			BodyJson = new Localized<JsonString>(new JsonString("{\"s\":\"Hello world\"}"))
		};
	}

	private Page PageTwo()
	{
		return new Page { 
			Id = 2, 
			CreatedUtc = DateTime.UtcNow, 
			Title = new Localized<string>("Hello world")
		};
	}

	private Page PageThree()
	{
		return new Page { Id = 3, CreatedUtc = DateTime.UtcNow };
	}

	private Page PageFour()
	{
		return new Page { 
			Id = 4,
			CreatedUtc = DateTime.UtcNow, 
			Title = new Localized<string>("This is the 1!"),
			BodyJson = new Localized<JsonString>(new JsonString("{\"s\":\"This is the 1!\"}"))
		};
	}

	private Context AnonContext()
	{
		return new Context();
	}

	[Fact]
	public void BasicIdEqualsFilter_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		// Create and bind the filter:
		var filter1 = pgService.Where("Id=?").Bind((uint)1);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false));
		Assert.False(filter1.Match(AnonContext(), PageTwo(), false));

		// boxed:
		var filterB1 = pgService.Where("Id=?").Bind((object)((uint)1));

		Assert.True(filterB1.Match(AnonContext(), PageOne(), false));
		Assert.False(filterB1.Match(AnonContext(), PageTwo(), false));

		// long is outputted by the JSON serialiser so 
		// there exists a general boxed long conversion case:
		var filterB2 = pgService.Where("Id=?").Bind((object)((long)1));

		Assert.True(filterB2.Match(AnonContext(), PageOne(), false));
		Assert.False(filterB2.Match(AnonContext(), PageTwo(), false));

	}

	[Fact]
	public void NumericOperators_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		// Create and bind the filter:
		var filter1 = pgService.Where("Id > ?").Bind((uint)1);
		var filter2 = pgService.Where("Id < ?").Bind((uint)1);
		var filter3 = pgService.Where("Id >= ?").Bind((uint)1);
		var filter4 = pgService.Where("Id <= ?").Bind((uint)1);

		Assert.True(filter1.Match(AnonContext(), PageTwo(), false));
		Assert.False(filter1.Match(AnonContext(), PageOne(), false));

		Assert.False(filter2.Match(AnonContext(), PageTwo(), false));
		Assert.False(filter2.Match(AnonContext(), PageOne(), false));

		Assert.True(filter3.Match(AnonContext(), PageTwo(), false));
		Assert.True(filter3.Match(AnonContext(), PageOne(), false));

		Assert.False(filter4.Match(AnonContext(), PageTwo(), false));
		Assert.True(filter4.Match(AnonContext(), PageOne(), false));
	}

	[Fact]
	public void ConstCalcs_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		var filter1 = pgService.GetFilterFor("Id=1 or Id=3", DataOptions.Default, true);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false));
		Assert.False(filter1.Match(AnonContext(), PageTwo(), false));
		Assert.True(filter1.Match(AnonContext(), PageThree(), false));
		Assert.False(filter1.Match(AnonContext(), PageFour(), false));

		var filter2 = pgService.GetFilterFor("(Id=1 or Id=3) and Title=\"Hello world\"", DataOptions.Default, true);

		Assert.True(filter2.Match(AnonContext(), PageOne(), false));
		Assert.False(filter2.Match(AnonContext(), PageTwo(), false)); // Wrong ID but does have the title
		Assert.False(filter2.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter3 = pgService.GetFilterFor("Id=1 || Id=3", DataOptions.Default, true);

		Assert.True(filter3.Match(AnonContext(), PageOne(), false));
		Assert.False(filter3.Match(AnonContext(), PageTwo(), false));
		Assert.True(filter3.Match(AnonContext(), PageThree(), false));
		Assert.False(filter3.Match(AnonContext(), PageFour(), false));

		var filter4 = pgService.GetFilterFor("(Id=1 || Id=3) && Title=\"Hello world\"", DataOptions.Default, true);

		Assert.True(filter4.Match(AnonContext(), PageOne(), false));
		Assert.False(filter4.Match(AnonContext(), PageTwo(), false)); // Wrong ID but does have the title
		Assert.False(filter4.Match(AnonContext(), PageThree(), false)); // does not have the title


	}

	[Fact]
	public void StringOps_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		var filter1 = pgService.GetFilterFor("Title startsWith \"Hello\"", DataOptions.Default, true);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filter1.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter2 = pgService.GetFilterFor("Title endsWith \"world\"", DataOptions.Default, true);

		Assert.True(filter2.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filter2.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter3 = pgService.GetFilterFor("Title contains \"ello\"", DataOptions.Default, true);

		Assert.True(filter3.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filter3.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter4 = pgService.GetFilterFor("Title contains \"world\"", DataOptions.Default, true);

		Assert.True(filter4.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filter4.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter5 = pgService.GetFilterFor("Title contains \"ello\"", DataOptions.Default, true);

		Assert.True(filter5.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filter5.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter6 = pgService.GetFilterFor("Title contains 1", DataOptions.Default, true);

		Assert.False(filter6.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.True(filter6.Match(AnonContext(), PageFour(), false)); // "This is the 1!" title

		var filter7 = pgService.GetFilterFor("Title contains NULL", DataOptions.Default, true);

		Assert.True(filter7.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.True(filter7.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filter8 = pgService.GetFilterFor("Title contains null", DataOptions.Default, true);

		Assert.True(filter8.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.True(filter8.Match(AnonContext(), PageThree(), false)); // does not have the title

	}

	[Fact]
	public void JsonStringOps_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		var filter1 = pgService.GetFilterFor("BodyJson startsWith \"{\\\"s\\\":\\\"Hello", DataOptions.Default, true);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.False(filter1.Match(AnonContext(), PageThree(), false)); // does not have a body

		var filter2 = pgService.GetFilterFor("BodyJson endsWith \"world\\\"}\"", DataOptions.Default, true);

		Assert.True(filter2.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.False(filter2.Match(AnonContext(), PageThree(), false)); // does not have a body

		var filter3 = pgService.GetFilterFor("BodyJson contains \"ello\"", DataOptions.Default, true);

		Assert.True(filter3.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.False(filter3.Match(AnonContext(), PageThree(), false)); // does not have a body

		var filter4 = pgService.GetFilterFor("BodyJson contains \"world\"", DataOptions.Default, true);

		Assert.True(filter4.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.False(filter4.Match(AnonContext(), PageThree(), false)); // does not have a body

		var filter5 = pgService.GetFilterFor("BodyJson contains \"ello\"", DataOptions.Default, true);

		Assert.True(filter5.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.False(filter5.Match(AnonContext(), PageThree(), false)); // does not have a body

		var filter6 = pgService.GetFilterFor("BodyJson contains 1", DataOptions.Default, true);

		Assert.False(filter6.Match(AnonContext(), PageOne(), false)); // Has {"s":"Hello world"} body
		Assert.True(filter6.Match(AnonContext(), PageFour(), false)); // "This is the 1!" body
	}

	[Fact]
	public void StringOps_Args_ShouldBind()
	{
		// - Bound args -
		var pgService = Services.Get<PageService>();

		var filterA1 = pgService.Where("Title startsWith ?").Bind("Hello");

		Assert.True(filterA1.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA1.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA2 = pgService.Where("Title endsWith ?").Bind("world");

		Assert.True(filterA2.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA2.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA3 = pgService.Where("Title contains ?").Bind("ello");

		Assert.True(filterA3.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA3.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA4 = pgService.Where("Title contains ?").Bind("world");

		Assert.True(filterA4.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA4.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA5 = pgService.Where("Title containsAny ?").Bind("ello");

		Assert.True(filterA5.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA5.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA6 = pgService.Where("!(Title contains ?)").Bind("world");

		Assert.False(filterA6.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.True(filterA6.Match(AnonContext(), PageThree(), false)); // does not have the title

	}

	[Fact]
	public void StringOps_Arg_Arrays_ShouldBind()
	{
		// Bound array args
		var pgService = Services.Get<PageService>();

		var filterA1Ar = pgService.Where("Title startsWith [?]").Bind(new List<string>() { "Hello" });

		Assert.True(filterA1Ar.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA1Ar.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA2Ar = pgService.Where("Title endsWith [?]").Bind(new List<string>() { "world" });

		Assert.True(filterA2Ar.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA2Ar.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA3Ar = pgService.Where("Title contains [?]").Bind(new List<string>() { "ello" });

		Assert.True(filterA3Ar.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA3Ar.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA4Ar = pgService.Where("Title contains [?]").Bind(new List<string>() { "world" });

		Assert.True(filterA4Ar.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA4Ar.Match(AnonContext(), PageThree(), false)); // does not have the title

		var filterA5Ar = pgService.Where("Title containsAny [?]").Bind(new List<string>() { "ello" });

		Assert.True(filterA5Ar.Match(AnonContext(), PageOne(), false)); // Has "Hello world" title.
		Assert.False(filterA5Ar.Match(AnonContext(), PageThree(), false)); // does not have the title

	}

	[Fact]
	public void DateOperators_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		// Create and bind the filter:
		var tenAgo = DateTime.UtcNow.AddMinutes(-10);
		var filter1 = pgService.Where("CreatedUtc > ?").Bind(tenAgo);
		var filter2 = pgService.Where("CreatedUtc < ?").Bind(tenAgo);
		var filter3 = pgService.Where("CreatedUtc >= ?").Bind(tenAgo);
		var filter4 = pgService.Where("CreatedUtc <= ?").Bind(tenAgo);
		var filter5 = pgService.Where("CreatedUtc = ?").Bind(tenAgo);
		var filter6 = pgService.Where("CreatedUtc != ?").Bind(tenAgo);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false));
		Assert.False(filter2.Match(AnonContext(), PageOne(), false));
		Assert.True(filter3.Match(AnonContext(), PageOne(), false));
		Assert.False(filter4.Match(AnonContext(), PageOne(), false));
		Assert.False(filter5.Match(AnonContext(), PageOne(), false));
		Assert.True(filter6.Match(AnonContext(), PageOne(), false));

		var tenAgoPage = PageOne();
		tenAgoPage.CreatedUtc = tenAgo;

		Assert.True(filter5.Match(AnonContext(), tenAgoPage, false));
		Assert.False(filter6.Match(AnonContext(), tenAgoPage, false));

	}

	[Fact]
	public void DateRangedFilter_ShouldBindTwice()
	{
		var pgService = Services.Get<PageService>();

		// Create and bind the filter:
		var tenAgo = DateTime.UtcNow.AddMinutes(-10);
		var tenLater = DateTime.UtcNow.AddMinutes(10);
		var filter1 = pgService.Where("CreatedUtc > ? and CreatedUtc <= ?").Bind(tenAgo).Bind(tenLater);

		Assert.True(filter1.Match(AnonContext(), PageOne(), false));

		var oldPage = PageOne();
		oldPage.CreatedUtc = DateTime.UtcNow.AddMinutes(-20);
		
		var futurePage = PageOne();
		futurePage.CreatedUtc = DateTime.UtcNow.AddMinutes(20);

		var futureEqualPage = PageOne();
		futureEqualPage.CreatedUtc = tenLater;

		var oldEqualPage = PageOne();
		oldEqualPage.CreatedUtc = tenAgo;

		Assert.False(filter1.Match(AnonContext(), oldPage, false));
		Assert.False(filter1.Match(AnonContext(), futurePage, false));
		Assert.True(filter1.Match(AnonContext(), futureEqualPage, false));
		Assert.False(filter1.Match(AnonContext(), oldEqualPage, false));
	}

	[Fact]
	public void FilterWithBadName_ShouldFail()
	{
		var pgService = Services.Get<PageService>();

		Assert.Throws<PublicException>(() =>
		{
			var filter2 = pgService.Where("ThisIsntRealId=?").Bind((int)1);
		});

	}

	[Fact]
	public void FilterLowercaseFieldNames_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		var filter2 = pgService.Where("id=?").Bind((uint)1);
	}

	[Fact]
	public void SyntaxErrors_ShouldNotCompile()
	{
		var pgService = Services.Get<PageService>();

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id=");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id=? and");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id=? and ");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id=? or ");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("(Id=?");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id=?)");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id starts ?");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id >== ?");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("Id >= = ?");
		});

		Assert.Throws<PublicException>(() =>
		{
			pgService.Where("? = Id");
		});

	}

	[Fact]
	public void BasicIdEqualsFilterWithCoersion_ShouldBind()
	{
		var pgService = Services.Get<PageService>();

		var filter2 = pgService.Where("Id=?").Bind((int)1);

		var filter3 = pgService.Where("Id=?").Bind((long)1);

		var filter4 = pgService.Where("Id=?").Bind((ulong)1);

		var filterN1 = pgService.Where("Id=?").Bind((int?)1);

		var filterN2 = pgService.Where("Id=?").Bind((uint?)1);

		var filterN3 = pgService.Where("Id=?").Bind((long?)1);

		var filterN4 = pgService.Where("Id=?").Bind((ulong?)1);

		var filterN5 = pgService.Where("Id=?").Bind("1");

		// -boxed set-

		var filterB2 = pgService.Where("Id=?").Bind((object)((int)1));

		var filterB3 = pgService.Where("Id=?").Bind((object)(long)1);

		var filterB4 = pgService.Where("Id=?").Bind((object)((ulong)1));

		var filterBN1 = pgService.Where("Id=?").Bind((object)((int?)1));

		var filterBN2 = pgService.Where("Id=?").Bind((object)((uint?)1));

		var filterBN3 = pgService.Where("Id=?").Bind((object)((long?)1));

		var filterBN4 = pgService.Where("Id=?").Bind((object)((ulong?)1));

		var filterBN5 = pgService.Where("Id=?").Bind((object)"1");
	}

	[Fact]
	public void BasicIdEqualsFilter_ShouldBindArray()
	{
		var pgService = Services.Get<PageService>();

		var filter1 = pgService.Where("Id=[?]").Bind(new List<uint>() { 1 });

		var filter2 = pgService.Where("Id=[?]").Bind(new List<uint>() { 1 }.Select(id => id)); // IEnumerable<uint>

		var filter3 = pgService.Where("Id=[?]").Bind(new List<ulong>() { 1 }); // ulong -> uint binding *is* permitted. It is the only special coersion that happens.

		var filter4 = pgService.Where("Id=[?]").Bind(Newtonsoft.Json.JsonConvert.DeserializeObject("[1]"));

		var filter5 = pgService.Where("Id=[?]").Bind(Newtonsoft.Json.JsonConvert.DeserializeObject("[\"1\"]"));

		var collector = new LongIDCollector();
		collector.Add(1);

		pgService.Where("Id=[?]").Bind(collector);
	}

	[Fact]
	public void VirtualListFieldFilter_ShouldBindArray()
	{
		var pgService = Services.Get<PageService>();

		var filter1 = pgService.Where("Tags=[?]").Bind(new List<ulong>() { 1 });

		// Coersion is supported too
		var filter2 = pgService.Where("Tags=[?]").Bind(new List<ushort>() { 1 });
		var filter3 = pgService.Where("Tags=[?]").Bind(new List<short>() { 1 });
		var filter4 = pgService.Where("Tags=[?]").Bind(new List<uint>() { 1 });
		var filter5 = pgService.Where("Tags=[?]").Bind(new List<int>() { 1 });
		var filter6 = pgService.Where("Tags=[?]").Bind(new List<long>() { 1 });
	}

	[Fact]
	public void BasicIdEqualsFilter_ShouldNotBindArray()
	{
		var pgService = Services.Get<PageService>();

		Assert.Throws<PublicException>(() =>
		{
			var filter1 = pgService.Where("Id=[?]").Bind((uint)1); // uint != array
		});

		Assert.Throws<PublicException>(() =>
		{
			var filter3 = pgService.Where("Id=[?]").Bind(new List<int>() { 1 }); // uint != int
		});

		Assert.Throws<PublicException>(() =>
		{
			var filter4 = pgService.Where("Id=[?]").Bind(new List<long>() { 1 }); // long != uint
		});

		Assert.Throws<PublicException>(() =>
		{
			var filter5 = pgService.Where("Id=[?]").Bind(new List<uint?>() { 1 }); // uint? != uint
		});

	}

}