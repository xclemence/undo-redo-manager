# undo-redo-manager
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Ms Build](https://github.com/xclemence/undo-redo-manager/workflows/Ms%20Build/badge.svg?branch=master)
![.NET Core](https://github.com/xclemence/undo-redo-manager/workflows/.NET%20Core/badge.svg?branch=master)

Undo Redo, the best feature that every software should have. But, it is not simple to implenent a good undo redo management. You sould chose the good strategy to combine performance and maintanbility. Specialy about the greanularity of object persistancies.

This project is an analyse to compar three type of persistancies for undo redo mangement.

## Tacking manager
In fact when I said undo redo, I want track all update on entities to revet or apply change. In fact, it's like save action logs and use them to change le state of current item.

## Analyse Results

###  Persistancy Requirements
|           Property          |          Item              |     Data Set               |                 Fody                |
|:---------------------------:|:--------------------------:|:--------------------------:|:-----------------------------------:|
| Reflection for generic code | Save item state methods    | Save item state methodss   | Framework to generate code on build |
|                             | Restore item state methods | Restore item state methods |                                     |
|                             |                            | Entry point                |                                     |

###  Persistancy compare
|               |          Property          |          Item         |                              Data Set                              |             Fody             |
|---------------|:--------------------------:|:---------------------:|:------------------------------------------------------------------:|:----------------------------:|
| Advantages    | Simple, atomic             | More natural          | Save always global state, No logic between state                   | No code to manage undo redo  |
| Disadvantages | Performance of reflection  | DeepCopy methods      | Space to save all sates, No information about changes(global view) | Code of fody plug in complex |
| Optimization  | Cache of method infos      | Collection management | Entry point                                                        | N/A (specific code)          |
| Performance   | Good                       | depends of item size  | depends of entry point size                                        | Good                         |

## Analyse details

