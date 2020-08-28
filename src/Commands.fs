namespace Todo

open System
open Fable.SimpleHttp
open Fable.SimpleJson
open Types
open Utils

module Commands =
  let parseArray f =
    function
    | Some (JArray children) -> children |> List.map f |> List.choose id |> Some
    | _ -> None

  let parseObject keys f =
    function
    | JObject dict ->
        keys
        |> List.map (fun key -> Map.tryFind key dict)
        |> List.choose id
        |> f
    | _ -> None

  let parseTodo f =
    (function
    | [ JString text; JBool isDone ] when f isDone ->
        Some { Id = Guid.NewGuid(); Text = text }
    | _ -> None)
    |> parseObject [ "title"; "completed" ]
    |> parseArray

  let parseTodoItems = parseTodo not
  let parseDoneItems = parseTodo id

  let fetchTodos () =
    async {
      printfn "up in here"
      let! (_, resp) = Http.get "https://jsonplaceholder.typicode.com/todos"
      let json = resp |> SimpleJson.tryParse

      return (json, json)
             ||> tmap2 parseTodoItems parseDoneItems
             ||> tmap (Option.defaultValue [])
             ||> tmap List.toSeq
    }
