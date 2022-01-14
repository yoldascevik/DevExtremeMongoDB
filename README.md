## DevExtremeMongoDB

**DxDatagrid MongoDB (MongoDriver) Support For .Net**

### Sample Use
```csharp
[HttpGet]  
public ActionResult Movies(DataSourceLoadOptions loadOptions)  
{  
     var movieList = _movieCollection.Find(FilterDefinition<Movie>.Empty); // native mongodb query  
     var loadResult = MongoDataSourceLoader.Load(movieList, loadOptions); // apply grid options   
     var jsonResult = JsonConvert.SerializeObject(loadResult); // create json response  
  
     return Content(jsonResult, "application/json");  
}
```
> *This repository also contains a sample ASP .NET 5 MVC Project. (mongodb required)*

### :white_check_mark: Supported Operations
- [x] Sorting
- [x] Row Filtering
 - [x] All Filter Operations
	 - [x] Equals
	 - [x] Does not equals
	 - [x] Less than
	 - [x] Greater than
	 - [x] Less than or equal to
	 - [x] Greater than or equal to
	 - [x] Between
	 - [x] Contains
	 - [x] Does not contains
	 - [x] Starts with
	 - [x] Ends with
- [x] All Paging Operations
	- [x] Skip
	- [x] Take
	- [x] Count

### :negative_squared_cross_mark: Unsupported Operations (yet)

 - [ ] Header Filtering
 - [ ] Summary

## Contributing

Please browse the [CONTRIBUTING.md](https://github.com/yoldascevik/DevExtremeMongoDB/blob/master/CONTRIBUTING.md) file to contribute.
