[![Build Status](https://ci.appveyor.com/api/projects/status/github/ReactiveThings/merger?branch=master&svg=true)](https://ci.appveyor.com/project/ReactiveThings/merger) 
[![NuGet](https://img.shields.io/nuget/dt/ReactiveThings.Merger.svg)](https://www.nuget.org/packages/ReactiveThings.Merger) 

# Merger
Merge two collections in C#. Inspired by SQL MERGE statement.

## Examples:


```csharp
var source = new List<Model>() {
    new Model{Id = null},
    new Model{Id = 1}
};
var target = new List<Entity>() {
    new Entity{Id = 1},
    new Entity{Id = 2}
};

// you can also use extension method 
// await target.MergeUsing(source) 
await Merger.Merge(target).Using(source) 
    .On(sourceItem => sourceItem.Id, targetItem => targetItem.Id)
    .WhenMatched((sourceItem, targetItem) => targetItem.Property = sourceItem.Property)
    .WhenNotMatched(async (sourceItem) => { // there is also async overload
        await AsyncMethod();
        target.Add(new Entity { Property = sourceItem.Property });
    })
    .WhenNotMatchedBySource(targetItem => target.Remove(targetItem))
    .ExecuteAsync();