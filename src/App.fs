namespace Todo

open System
open Fable.MaterialUI.Props
open Fable.MaterialUI.Core
open Fable.React.Props
open Fable.React
open Elmish
open Elmish.React
open Types
open Commands
open Components

module MainModule =
  let init () =
    let model =
      { AddTodoText = ""
        Todos = []
        ShowDone = false
        Done = [] }

    let cmd =
      Cmd.OfAsync.either fetchTodos () TodosFetched FetchTodosFailed

    model, cmd

  let update msg model =
    match msg with
    | AddTextChanged text -> { model with AddTodoText = text }, Cmd.none
    | TodoAdded text ->
        { model with
            AddTodoText = ""
            Todos =
              { Id = Guid.NewGuid(); Text = text }
              :: (model.Todos |> Seq.toList) },
        Cmd.none
    | TodoChecked todo ->
        { model with
            Todos =
              model.Todos
              |> Seq.filter (fun t -> t.Id <> todo.Id)
            Done = todo :: (model.Done |> Seq.toList) },
        Cmd.none
    | TodoUnchecked todo ->
        { model with
            Todos = todo :: (model.Todos |> Seq.toList)
            Done =
              model.Done
              |> Seq.filter (fun t -> t.Id <> todo.Id) },
        Cmd.none
    | DoneOpened -> { model with ShowDone = true }, Cmd.none
    | DoneClosed -> { model with ShowDone = false }, Cmd.none
    | TodosFetched (todoItems, doneItems) ->
        { model with
            Todos = todoItems
            Done = doneItems },
        Cmd.none
    | FetchTodosFailed _ -> model, Cmd.none

  let view model dispatch =
    div [] [
      div [ Id "header" ] [
        h1 [ Id "title" ] [ str "ToDoer" ]
        textField [ Id "add-todo"
                    MaterialProp.Label(str "Add a Todo Item")
                    FullWidth true
                    MaterialProp.Value model.AddTodoText
                    OnChange(fun e -> dispatch (AddTextChanged e.Value))
                    OnKeyPress(fun e ->
                      if e.charCode = 13.0 then
                        dispatch (TodoAdded model.AddTodoText)
                      else
                        ()) ] []
      ]
      ul
        [ Id "todos" ]
        ((TodoItemList
            {| key = Guid.NewGuid()
               containerProps = [ Id "todo" ]
               todos = model.Todos
               isChecked = false
               onChecked = fun todo _ -> dispatch (TodoChecked todo) |})
         :: if model.ShowDone then
              [ TodoDivider
                  {| key = Guid.NewGuid()
                     isOpen = true
                     onClick = fun _ -> dispatch DoneClosed |}
                TodoItemList
                  {| key = Guid.NewGuid()
                     containerProps = [ Id "done" ]
                     todos = model.Done
                     isChecked = true
                     onChecked = fun todo _ -> dispatch (TodoUnchecked todo) |} ]
            else
              [ TodoDivider
                  {| key = Guid.NewGuid()
                     isOpen = false
                     onClick = fun _ -> dispatch DoneOpened |} ])
    ]

  Program.mkProgram init update view
  |> Program.withReactBatched "app"
  |> Program.run
