namespace Project1

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating


 [<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>
 
    [<NoComparison>]
    type Task = { Name: string; Done: Var<bool> }
 
    let tasks = 
        ListModel.Create (fun task -> task.Name)
              [ { Name = "Have breakfast"; Done = Var.Create true }
                { Name = "Have lunch"; Done = Var.Create false } ]
 
    let newTaskName = Var.Create ""
 
    let createTaskView task =
        IndexTemplate.ListItem()
            .Task(task.Name)
            .Clear(fun _ -> tasks.RemoveByKey task.Name)
            .Done(task.Done)
            .ShowDone(Attr.ClassPred "checked" task.Done.V)
            .Doc()
 
    let addTask () =
        if newTaskName.Value.Trim().Length > 0 then
            let newTask = { Name = newTaskName.Value.Trim(); Done = Var.Create false }
            tasks.Add newTask
            Var.Set newTaskName ""
 
    let clearCompletedTasks () =
        tasks.RemoveBy (fun task -> task.Done.Value)
 
    [<SPAEntryPoint>]
    let main =
        IndexTemplate.Main()
            .ListContainer(
                ListModel.View tasks |> Doc.BindSeqCached createTaskView
            )
            .NewTaskName(newTaskName)
            .Add(fun _ -> addTask())
            .ClearCompleted(fun _ -> clearCompletedTasks())
            .Doc()
        |> Doc.RunById "tasks"

