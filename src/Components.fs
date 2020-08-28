namespace Todo

open System
open Fable.React.Props
open Fable.React
open Fable.MaterialUI.Core
open Types

module Components =
  let IconTextLi =
    FunctionComponent.Of<{| key : Guid
                            liProps : seq<IHTMLProp>
                            icon : ReactElement
                            text : string |}>(fun props ->
      li props.liProps [ props.icon; str props.text ])

  let TodoItem =
    FunctionComponent.Of<{| key : Guid
                            todo : Todo
                            isChecked : bool
                            onCheck : (unit -> unit) |}>(fun props ->
      IconTextLi
        {| key = Guid.NewGuid()
           liProps =
             [ Id(props.todo.Id.ToString())
               Class
                 (sprintf
                   "todo-row todo-item %s"
                    (if props.isChecked then "checked" else ""))
               OnClick(fun _ -> props.onCheck ()) ]
           icon = checkbox [ Checked props.isChecked ]
           text = props.todo.Text |})

  let TodoDivider =
    FunctionComponent.Of<{| key : Guid
                            isOpen : bool
                            onClick : unit -> unit |}>(fun props ->
      IconTextLi
        {| key = Guid.NewGuid()
           liProps =
             [ Id "todo-divider"
               Class "todo-row"
               OnClick(fun _ -> props.onClick ()) ]
           icon =
             icon [ Class "show-more" ] [
               str
                 (if props.isOpen then
                   "keyboard_arrow_up"
                  else
                    "keyboard_arrow_down")
             ]
           text = "" |})

  let todoItem isChecked onCheck todo =
    TodoItem
      {| key = todo.Id
         todo = todo
         isChecked = isChecked
         onCheck = onCheck todo |}

  let TodoItemList =
    FunctionComponent.Of<{| key : Guid
                            containerProps : seq<IHTMLProp>
                            todos : seq<Todo>
                            isChecked : bool
                            onChecked : Todo -> unit -> unit |}>(fun props ->
      props.todos
      |> Seq.map (todoItem props.isChecked props.onChecked)
      |> div props.containerProps)
