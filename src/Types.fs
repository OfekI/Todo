namespace Todo

open System

module Types =
  type Todo = { Id : Guid; Text : string }

  type Model =
    { AddTodoText : string
      Todos : seq<Todo>
      ShowDone : bool
      Done : seq<Todo> }

  type Msg =
    | AddTextChanged of string
    | TodoAdded of string
    | TodoChecked of Todo
    | TodoUnchecked of Todo
    | DoneOpened
    | DoneClosed
    | TodosFetched of seq<Todo> * seq<Todo>
    | FetchTodosFailed of exn
