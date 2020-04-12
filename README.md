# undo-redo-manager
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Ms Build](https://github.com/xclemence/undo-redo-manager/workflows/Ms%20Build/badge.svg?branch=master)
![.NET Core](https://github.com/xclemence/undo-redo-manager/workflows/.NET%20Core/badge.svg?branch=master)

Undo Redo, the best feature that every software should have. But, it is not simple to implement a good undo redo management. You should chose the good strategy to combine performance and maintainability. Specially about the granularity of object persistences.

This project is an analyze to compare three type of persistences for undo redo management.

## Tacking manager
Undo redo concept need mechanism to revert and apply changes on simple object. It's very similar to track all actions (with all informations mandatory to revert/apply changes) and save them in list. All tracking item should be standalone to managed revert/apply.

With this design, the type of undo redo is managed by implementation of ITrackinAction, we can make a PropertyChangedTrackingAction to track action of properties or make an ItemTracikingAction to track action on item or something else.

### Tracking Scope

Sometime we need group some changes inside one action. For that, the tracking scope provide a way to save all actions inside the scope. At this end, new MultiTractionAction was create with all changes (list of ITrackingAction).

Tracking Manager handle a base scope to managed action on top level and provide a method to generate new scope (to start new scope tracking)?

### Tracking by property

Track changes by property is the more atomic mechanism. the idea is keep a reference to updated item and setter method info to managed revert/apply.
To avoid lot of code without business logic, we use reflection to define found setter method. This point is the more important point for good performances.
Some try are implemented in test app to find the best solution.

To improve performance and avoid make refection analyses each call, we can easy make a cache with all method info used.

### Tracking by Data set

Tracking by data set consist to save a snapshot of an Entry Point (world with all items to include in undo redo) ans apply the snapshot on current world. For that, we need method to duplication the world and save it and another method reset the world state.
The cache with this method is embedded. Indeed, snapshot is the cache.

To be honest, this method is not very compatible with standalone tracking item, but the compatibility is enough to compare with other solutions.

Due to general state, it's not possible to keep the change who generate state.

### Tracking by Item

Item Tracking is keep a snapshot of entity when an action occluded on it. and save it in tracking action. This method is the middle between data set and property. For this method we need methods to duplicate and restore entities states. 
Save Entities instead of World, need less memory and is more efficient.

To enhance performance, we can save items copies in cache, but the items cache is very complicate to manage specially to know when invalidate cache.

### Tracking Collection
Property and item tracking managed only changes on internal states, they not track collection changed.

### global diagram 

<img src="doc/ClassDiagram.svg"/>

## Fody Usage
[Fody](https://github.com/Fody/Fody) is a great extensible tool for weaving .net assemblies.
With this tool, it is possible to develop a plug in to avoid reflection usage in property tracking. We can generate in compilation time some method to replace setter method info

Base code
```cs
[Tracking]
public class TestModel
{
  public int Value { get; set; }
}
```
Generated code
```cs
[Tracking]
public class BaseModel
{
  private TrackingManager trackingManager;

  public int Value
  {
    [CompilerGenerated]
    get
    {
      return Value;
    }
    [CompilerGenerated]
    set
    {
      if (Value != value)
      {
        trackingManager.AddAction(this.GetTrackingPropertyUpdateFunc(Value, value, new Action<BaseModel, int>(TrackingItemSetter_Value)));
      }
      Value = value;
    }
  }  
  public BaseModel()
  {
      trackingManager = TrackingManagerProvider.GetDefault();
  }  
  private static void TrackingItemSetter_Value(BaseModel P_0, int P_1)
  {
      P_0.Value = P_1;
  }
}

```

## Analyze Results

###  Persistence Requirements
|           Property          |          Item                    |     Data Set                     |                 Fody                |
|:---------------------------:|:--------------------------------:|:--------------------------------:|:-----------------------------------:|
| Reflection for generic code | Keep one item reference per item | Keep one item reference per item | Framework to generate code on build |
|                             | Save item state methods          | Save item state methods          |                                     |
|                             | Restore item state methods       | Restore item state methods       |                                     |
|                             |                                  | Entry point                      |                                     |

###  Persistence compare
|               |          Property          |          Item                                              |                              Data Set                              |             Fody             |
|---------------|:--------------------------:|:----------------------------------------------------------:|:------------------------------------------------------------------:|:----------------------------:|
| Advantages    | Simple, atomic             |                                                            | Save always global state, No logic between state                   | No code to manage undo redo  |
| Disadvantages | Performance of reflection  | Cache is very difficult to managed specially invalidation  | Space to save all sates, No information about changes(global view) | Code of Fody plug in complex |
| Optimization  | Cache of method infos      | Item cache                                                 | Entry point                                                        | N/A (specific code)          |
| Performance   | Good                       | depends of item size                                       | depends of entry point size                                        | Good                         |
| Complexity    | Simple                     | very complex                                               | Simple                                                             | Medium                       |

## Analyse details

To test performance of methods, an application has been develop to make actions on simple model (driver, address, car). Test include only changes on properties.

### performance results
Test parameters:
* Seed: 3
* Driver number: 40
* Car number: 40
* Address number: 40

| Operation | Number | Property   | Item       | DataSet    | Fody       |
|-----------|--------|------------|------------|------------|------------|
| Creation  | 19272  | 00.1454487 | 00.2377482 | 39.9245249 | **00.0538056** |
| Revert    | 19272  | 00.0316663 | 00.0428675 | 16.2472283 | **00.0138809** |
| Apply     | 19272  | 00.0315690 | 00.0434608 | 16.3403993 | **00.0212801** |


### Interface of Test application 
<img src="doc/TestApp.png"/>